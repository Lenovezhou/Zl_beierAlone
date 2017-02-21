using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Datas;

public class SenceGameObject : MonoBehaviour
{
    public const string NONE_TAG = "Untagged";
    public const string PLAYER_TAG = "Player";
    public const string AI_TAG = "AI";
    public const string AI_STONE = "Stone";
    public const string AI_BULLET_TAG = "Bullet";
    public const string PLAYER_BULLET_TAG = "PlayerBullet";
    public const string UI_TAG = "UI";
    public const string HURT_TAG = "OnHurt";
    public const int MIN_DISTANS = 10;

    //所有Ai生成距离
    public const float AI_PALYER_MAX_GAP = 150;

    public const string Weapon_TAG = "Weapon";
    //  左手柄
    public GameObject LeftRein;
    //  右手柄
    public GameObject RightRein;
    //  左边炮台
    public LookAt LeftBattery;
    //  右边炮台
    public LookAt RightBattery;
    //  玩家
    public GameObject Player;

    public bool isSevice = false;

    public GameObject LeftFirePoint;
    public GameObject RightFirePoint;
    public GameObject EnemyEffect;  //  爆炸特效
    public int LastHP;
    public AI _pc_man;
    public Role _player;
    public Bullet _bullet;     //敌人与自己的子弹都返回该变量
    public Prop _weapon;
    public UIMenu _uiMenu;  //  UI
    public GameMenu _game_menu;  //  UI控制脚本
    public Ranking _ranking;
    public GuidedMissile _missile;  //  导弹
    public ObjectType type;

    public Vector3 ai_start_position;
    public Vector3 ai_target_point;
    public Vector3 ai_end_point;
    public float ai_move_speed;
    public Rigidbody rig;
    public float luach_time;
    public float ai_life_time;
    public float ai_range;  //  ai跑的距离
    public bool ai_in_end;
    public Follo follow;
    public StoneWalk Stone; 
    public bool ClientIsDestroy = false;   //  客户端是否销毁

    public GameObject LeftFlower;
    public GameObject RightFlower;

    public bool _canDestroy = false;
    private float _canDestroyTime = 0;

    private float left_time;
    private float right_time;

    private float des_inv_time;
    public float destroy_time = 10;

    private Vector3 last_position;
    private Vector3 last_rotation;

    private Vector3 last_camera_position;
    private Vector3 last_camera_rotation;
    private Vector3 last_rein_left_ratation;
    private Vector3 last_rein_left_position;
    private Vector3 last_rein_right_ratation;
    private Vector3 last_rein_right_position;
    private bool last_rein_left_key;
    private bool last_rein_right_key;

    private string lase_player_name;
    private int last_page_tab;

    private Ray ray;
    private RaycastHit hit;
    //RootObject为AI，Role，Bullet的基类
    public RootObject SenceObject
    {
        set
        {
            switch (value.type)     //根据设置的value的type类型枚举来赋值
            {
                case ObjectType.Player:
                    _player = value as Role;
                    break;
                case ObjectType.AI:
                    _pc_man = value as AI;

                    break;
                case ObjectType.Bullet:
                    _bullet = value as Bullet;

                    break;
                case ObjectType.PlayerBullet:
                    _bullet = value as Bullet;
                    break;
                case ObjectType.Weapon:
                    _weapon = value as Prop;
                    break;
                case ObjectType.UI:
                    _uiMenu = value as UIMenu;
                    break;
                case ObjectType.Ranking:
                    _ranking = value as Ranking;
                    break;
                case ObjectType.GuidedMissile:
                    _missile = value as GuidedMissile;
                    break;
            }
            type = value.type;
        }
        get
        {
            switch (type)
            {
                case ObjectType.Player:
                    return _player;
                case ObjectType.AI:
                    return _pc_man;
                case ObjectType.Bullet:
                    return _bullet;
                case ObjectType.PlayerBullet:
                    return _bullet;
                case ObjectType.Weapon:
                    return _weapon;
                case ObjectType.UI:
                    return _uiMenu;
                case ObjectType.Ranking:
                    return _ranking;
                case ObjectType.GuidedMissile:
                    return _missile;
                default:
                    return null;
            }
        }
    }

    public virtual void OnTriggerStay(Collider otherColl)
    {
        if (isSevice &&
            type == ObjectType.PlayerBullet &&
            MsgCenter.Instance.player.useWeapon.Name.Equals("weapon2"))
        {
            MsgCenter.Instance.OnHit(this, otherColl);
        }
    }

    //public virtual void OnTriggerExit(Collider otherColl)
    //{
    //    if (isSevice &&
    //        type == ObjectType.Weapon &&
    //        MsgCenter.Instance.player.useWeapon.Name.Equals("weapon2"))
    //    {
    //        _canDestroy = true;
    //    }
    //}

    public virtual void OnTriggerEnter(Collider otherColl)
    {
        //Debug.Log(" OnTriggerEnter:" + gameObject.name);
        if (isSevice)
        {
            MsgCenter.Instance.OnHit(this, otherColl);
            if (type == ObjectType.GuidedMissile && otherColl.tag == AI_TAG)
            {
                Destroy(gameObject);
                Audiocontroller.instans.PlayBossAudio("Explosion");/*weapon3是击中BOSS添加声音*/

            }
            if (MsgCenter.Instance.player.useWeapon.Name == "weapon3" && _pc_man != null && _pc_man.ai_type == AIType.LifeAndAttack)
            {
             //   Destroy(otherColl.gameObject);
             //   DestroyObject(otherColl.gameObject, 0.5f);
            }
        }
        else
        {
            //导弹打到大怪时播放声音
            if (otherColl.tag == PLAYER_BULLET_TAG && _pc_man != null && _pc_man.ai_type == AIType.LifeAndAttack)
            {
                if (Recever.rInstance != null && Recever.rInstance.player.useWeapon.Name == "weapon3")
                {
                 //   Audiocontroller.instans.PlayBossAudio("Explosion");
                    DestroyObject(otherColl.gameObject, 0.5f);
                }
            }

            //石头破碎声音
            if (otherColl.gameObject.CompareTag(PLAYER_TAG) && _pc_man != null && _pc_man.ai_type == AIType.Stone)
            {
                Debug.Log("ontriggerenter:::::"+this.name);
                //播放石头爆炸声音
                istrigger = true;
                transform.FindChild("Enemy Explode").gameObject.SetActive(true);
            }
            if (otherColl.gameObject.CompareTag(PLAYER_BULLET_TAG) && this.CompareTag(AI_TAG))
            {
                if (_pc_man.HP <= 0 && _pc_man.ai_type == AIType.LifeAndAttack)
                {
                   // Audiocontroller.instans.PlayGameAudio("dabaozha");
                  //  Recever.rInstance.DestroySenceObject(this);
                }
                if (_pc_man.ai_type == AIType.LifeAndAttack)
                {
                     GameObject obj=null;
                     if (!Recever.rInstance.player.useWeapon.Name.Equals("weapon2"))
                     {
                         if (Recever.rInstance.player.useWeapon.Name.Equals("weapon1"))
                         {
                             //Debug.Log("ififiififififififi");
                             obj = Instantiate(Recever.rInstance.HitEffectPrefab,
                       otherColl.transform.position, Quaternion.identity) as GameObject;
                         }
                         else
                         {
                            obj = Instantiate(Recever.rInstance.HitBigEffectPrefab,
                            otherColl.transform.position, Quaternion.identity) as GameObject;
                         }
                     }
                     else 
                     {
                         obj = Instantiate(Recever.rInstance.HitBigEffectPrefab,
                           transform.position, Quaternion.identity) as GameObject;
                     }
                    Destroy(obj, MsgCenter.EffectLifeTime);
                }
                else if (_pc_man.ai_type != AIType.LifeAndAttack && _pc_man.ai_type != AIType.Stone)
                {
                    GameObject obj = null;
                    if (!Recever.rInstance.player.useWeapon.Name.Equals("weapon2"))
                    {
                        obj = Instantiate(Recever.rInstance.HitEffectPrefab,
                       otherColl.transform.position, Quaternion.identity) as GameObject;
                    }
                    else
                    {
                        obj = Instantiate(Recever.rInstance.HitEffectPrefab,
                         transform.position, Quaternion.identity) as GameObject;
                    }
                   
                        Destroy(obj, MsgCenter.EffectLifeTime);
                }
               
            }
        }
        // Recever.rInstance.cover_prefab.SetActive(true);
        
    }

    public void InitStartPos() 
    {
        ai_in_end = false;
        ai_start_position = Random.onUnitSphere * AI_PALYER_MAX_GAP +
            MsgCenter.Instance.PlayerData.gameObject.transform.position;
        //Debug.Log(ai_start_position + "onunitsphere" + Random.onUnitSphere * AI_PALYER_MAX_GAP);
        if (ai_start_position.z < 0)
        {
            ai_start_position.z = -ai_start_position.z;
        }
        //ai_start_position = new Vector3(Random.Range(-3*ai_start_position.x,3*ai_start_position.x),
        //    Random.Range(-ai_start_position.y,ai_start_position.y),Random.Range(50, ai_start_position.z));
        transform.position = ai_start_position;
        if (gameObject.transform.position.y <= 0)
        {
            gameObject.transform.position = new Vector3(gameObject.transform.position.x,
                -gameObject.transform.position.y,
                transform.position.z);
        }
        if (gameObject.transform.position.z <= 0)
        {
            gameObject.transform.position = new Vector3(transform.position.x,
            gameObject.transform.position.y, -gameObject.transform.position.z);
        }
        rig = GetComponent<Rigidbody>();
        transform.LookAt(Random.insideUnitSphere * AI_PALYER_MAX_GAP +
            transform.position);
        ai_move_speed = MsgCenter.Instance.AIMoveSpeed;
        follow = this.gameObject.AddComponent<Follo>();
        //Debug.Log(_pc_man.ai_type.ToString());
        switch (_pc_man.ai_type)
        {
            case AIType.Lithe:

                Debug.Log("444444444444444444444AIType.Lithe");
                ai_target_point = (MsgCenter.Instance.PlayerData.gameObject.transform.position - transform.position).normalized
                    * AI_PALYER_MAX_GAP * 0.95f + transform.position;
                if (ai_target_point.z < 0)
                {
                    ai_target_point.z = -ai_target_point.z;
                }
                ai_target_point.y = Random.Range(-1.1f, 1.5f);
                follow.Target = ai_target_point;
                ai_end_point = Random.insideUnitSphere * AI_PALYER_MAX_GAP * 0.95f + ai_target_point;
                if (ai_target_point.x > 0 && ai_end_point.x < 0 ||
                    ai_target_point.x < 0 && ai_end_point.x > 0)
                {
                    ai_end_point.x = -ai_end_point.x;
                }
                ai_end_point.y = Random.Range(5, 10);
                //rig.velocity = ai_move_speed * (MsgCenter.Instance.PlayerData.transform.position - transform.position).normalized;
                break;
            case AIType.Attack:
                Debug.Log("333333333333333333AIType.Attack");
                ai_target_point = (MsgCenter.Instance.PlayerData.gameObject.transform.position - transform.position).normalized
                    * AI_PALYER_MAX_GAP * 0.95f + transform.position;
                if (ai_target_point.z < 0)
                {
                    ai_target_point.z = -ai_target_point.z;
                }
                ai_target_point.y = Random.Range(-2.1f, 2);
                follow.Target = ai_target_point;
                ai_end_point = Random.insideUnitSphere * AI_PALYER_MAX_GAP * 0.9f + ai_target_point;
                if (ai_target_point.x > 0 && ai_end_point.x < 0 ||
                    ai_target_point.x < 0 && ai_end_point.x > 0)
                {
                    ai_end_point.x = -ai_end_point.x;
                }
                ai_end_point.y = Random.Range(5, 10);
                break;
		case AIType.LifeAndAttack:
                //follow.enabled = false;

                //Debug.Log("222222222222222222222222222222222222222222222AIType.LifeAndAttack");
                //Vector3 StartPos = MsgCenter.Instance.StartPosition.position;
                //Stone = gameObject.AddComponent<StoneWalk>();
                //Stone.Tartget = MsgCenter.Instance.EndPosition.position;
                //Stone.RunSpeed = 3f;
                //if (Random.Range(-1, 1) > 0)
                //{
                //    Debug.Log("boss>>>>>>" + transform.position);
                //    StartPos.x = -StartPos.x;
                //    Stone.Tartget.x = -Stone.Tartget.x;
                //}
			//transform.position = StartPos;\
			follow.RunSpeed = 0.5f;
			follow.RotateSpeed = 0.01f;
			transform.position = MsgCenter.Instance.StartPosition.position;
			follow.EndPos = MsgCenter.Instance.TargetPosition.position;
			follow.UPPos = MsgCenter.Instance.EndPosition.position;
                
                break;
            case AIType.Stone: 
                //Debug.Log("name:" + gameObject.name + "; " + transform.position);
                InitStoneAI();
                break;
            case AIType.SmartAI:
                ai_start_position.x = Random.Range(-10, 10);
                ai_start_position.y = Random.Range(-10, 20);
                ai_start_position.z = Random.Range(100, 200);
                transform.position = ai_start_position;
                break;
        }

        transform.forward = rig.velocity.normalized;
        ai_life_time = 0;
    }


    public virtual void Start()
    {
        ray = new Ray(Vector3.zero,Vector3.zero);
        last_position = Vector3.zero;
        last_rotation = Vector3.zero;
        last_rein_left_position = Vector3.zero;
        last_rein_right_position = Vector3.zero;
        last_rein_left_ratation = Vector3.zero;
        last_rein_right_ratation = Vector3.zero;
        last_camera_position = Vector3.zero;
        last_camera_rotation = Vector3.zero;
        last_rein_left_key = false;
        last_rein_left_key = false;
        if (isSevice)
        {
            switch (type)
            {
                case ObjectType.AI:
                    break;
                case ObjectType.Bullet:
                    des_inv_time = 0;
                    destroy_time = 4;
                    break;
                case ObjectType.PlayerBullet:
                    des_inv_time = 0;
                    destroy_time = 4;
                    break;
            }
        }
        else 
        {
            
        }
    }

    private bool isHit = false;
    
    void InitStoneAI()
    {
        //Debug.Log("InitStoneAI ：： name:" + gameObject.name + "; " + transform.position);
        follow.enabled = false;
        StoneWalk stone = gameObject.AddComponent<StoneWalk>();

        ai_start_position = new Vector3(MsgCenter.Instance.PlayerData.gameObject.transform.position.x,
            MsgCenter.Instance.PlayerData.gameObject.transform.position.y,
            AI_PALYER_MAX_GAP);

        //if (ai_start_position.z <= 300)
        //{
        //    ai_start_position.z += 300;
        //}

        transform.position = ai_start_position;

        ai_target_point = MsgCenter.Instance.PlayerData.gameObject.transform.position;
        transform.LookAt(ai_target_point);
        ai_target_point.y = ai_target_point.y + Random.Range(-15f, 10f);
        ai_target_point.x = ai_target_point.x + Random.Range(-15f, 20f);
        follow.Target = ai_target_point;
        ai_end_point = (-(ai_start_position - ai_target_point).normalized) * 2 * AI_PALYER_MAX_GAP +
            ai_target_point;
        stone.Tartget = ai_end_point;
    }
    private float des_wait_time;
    private Vector2 _tmp_V2;
    private float _weapon_time = 0;
	private float timer;
    private float rotatelimit;
    /// <summary>
    /// 在Update内收集信息，刷新信息
    /// </summary>
    public virtual void Update()
    {
        if (SenceObject != null)
        {
            _tmp_V2 = Camera.main.WorldToScreenPoint(transform.position);
            if (_tmp_V2.x >= 0 && _tmp_V2.x <= Screen.width ||
                _tmp_V2.y >= 0 && _tmp_V2.y <= Screen.height)
            {
                isVisible = true;
            }
            else
            {
                isVisible = false;
            }
            if (isSevice)// 服务器
            {
                if (SenceObject.type == ObjectType.AI ||
                    SenceObject.type == ObjectType.Weapon)
                {
                    //  根据位置采集需要发送的对象
                    if ((last_position - transform.position).magnitude >= MsgCenter.Instance.SosoPositionSetup)
                    {
                        last_position = transform.position;
                        MsgCenter.Instance.AddToSendList(this);
                    }
                    //  根据旋转采集需要发送的对象
                    if (last_rotation.x - transform.eulerAngles.x >= MsgCenter.Instance.SosoRotationSetup ||
                        last_rotation.y - transform.eulerAngles.y >= MsgCenter.Instance.SosoRotationSetup ||
                        last_rotation.z - transform.eulerAngles.z >= MsgCenter.Instance.SosoRotationSetup)
                    {
                        last_rotation = transform.eulerAngles;
                        MsgCenter.Instance.AddToSendList(this);
                    }
                }
                if (SenceObject.type == ObjectType.Player && !MsgCenter.Instance.IsShaking)
                {
                    transform.localPosition = Vector3.Lerp(transform.localPosition,
    new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y, Camera.main.transform.position.z + 0.1f), 0.05f);
                    Vector3 templook = new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y + 2f, 
                        Camera.main.transform.position.z + 0.1f) -
                   transform.position;
                    // 让飞机始终看向主角头上2f的距离
                    transform.up = templook;

                    //transform.position = Vector3.Lerp(transform.position, Camera.main.transform.position, 0.05f);
                    //if (Camera.main.transform.localEulerAngles.z >= 340
                    //    || Camera.main.transform.localEulerAngles.z <= 20)
                    //{
                    //    rotatelimit = Camera.main.transform.localEulerAngles.z;
                    //   // transform.up = Vector3.Lerp(transform.up,Camera.main.transform.up,0.25f);
                    //}
                    //else if(Camera.main.transform.localEulerAngles.z > 20)
                    //{
                    //  //  rotatelimit = 20;
                    //}
                    //else if (Camera.main.transform.localEulerAngles.z < 340)
                    //{
                    //  //  rotatelimit = 340;
                    //}
                    //transform.rotation = Quaternion.Slerp(transform.localRotation, Quaternion.Euler(0, 0, rotatelimit), 0.05f);
                }
                //  
                switch (MsgCenter.Instance.GameState)
                {
                    case GameState.Start:
                        if (SenceObject.type == ObjectType.Player)
                        {
                           

                            //if (_player.LeftRein != null)
                            //{
                            //    LeftRein.transform.position = _player.LeftRein.Position;
                            //    LeftRein.transform.rotation = _player.LeftRein.Rotation;
                            //}
                            //if (_player.RightRein != null)
                            //{
                            //    RightRein.transform.position = _player.RightRein.Position;
                            //    RightRein.transform.rotation = _player.RightRein.Rotation;
                            //}
                            //Camera.main.transform.position = _player.CameraPosition;
                            //Camera.main.transform.eulerAngles = _player.CameraRotation;
                            //Player.transform.position = _player.Position;
                            //Player.transform.rotation = _player.Rotation;
                        }
                        if(SenceObject.type == ObjectType.UI)
                        {
                            switch (_game_menu.muenu_state)
                            {
                                case MenuState.None:
                                    if (lase_player_name != _game_menu.PlayerName)
                                    {
                                        lase_player_name = _game_menu.PlayerName;
                                        _uiMenu.Name = _game_menu.PlayerName;
                                        MsgCenter.Instance.AddToSendList(this);
                                        //Debug.Log(">>>>>>>>>>" + _uiMenu.Name);
                                    }
                                    break;
                                case MenuState.CreatePlayer:
                                    break;
                                case MenuState.Display:
                                    if (last_page_tab != _game_menu.PageTab)
                                    {
                                        last_page_tab = _game_menu.PageTab;
                                        _uiMenu.PageTab = _game_menu.PageTab;
                                        MsgCenter.Instance.AddToSendList(this);
                                    }
                                    break;
                                case MenuState.StartGame:
                                    Debug.Log(">>>>>>>StartGameStartGameStartGameStartGameStartGame");
                                    MsgCenter.Instance.DestroySenceObject(this, 0.1f);
                                    MsgCenter.Instance.GameState = GameState.Playing;
                                    break;
                                case MenuState.Continue:
                                    break;
                                case MenuState.ExitGame:
                                    break;
                                default:
                                    break;
                            }
                        }
                        break;
                    case GameState.Playing:
                        switch (SenceObject.type)
                        {
                            case ObjectType.Player:
                                CrateBullet(_player, true, null);
                                
                                //if (_player.LeftRein != null)
                                //{
                                //    //Debug.Log("UpdatePlayer");
                                //    LeftRein.transform.position = _player.LeftRein.Position;
                                //    LeftRein.transform.rotation = _player.LeftRein.Rotation;
                                //    RightRein.transform.position = _player.RightRein.Position;
                                //    RightRein.transform.rotation = _player.RightRein.Rotation;
                                //    Camera.main.transform.position = _player.CameraPosition;
                                //    Camera.main.transform.eulerAngles = _player.CameraRotation;
                                //    CrateBullet(_player, true, null);
                                //}
                                //else
                                //{
                                //    //Debug.Log("else");
                                //}
                                //Player.transform.position = _player.Position;
                                //Player.transform.rotation = _player.Rotation;

                                break;
					case ObjectType.AI:
								
                                if(_pc_man.ai_type == AIType.Stone)
                                {
								//	transform.RotateAround (transform.forward,100*Time.deltaTime);

                                    Debug.DrawLine(ai_start_position, ai_target_point, Color.green);
                                    Debug.DrawLine(ai_target_point, ai_end_point, Color.red);
                                }
                                
                                //if (!MsgCenter.Instance.HurtPoints.ContainsKey(SenceObject.ID) &&
                                //    (MsgCenter.Instance.PlayerData.transform.position - transform.position).magnitude <= 5)
                                //{
                                //    ray = new Ray(transform.position, MsgCenter.Instance.PlayerData.transform.position - transform.position);
                                //    if (Physics.Raycast(ray, out hit))
                                //    {
                                //        if (hit.transform.gameObject.CompareTag(SenceGameObject.PLAYER_TAG))
                                //        {
                                //            MsgCenter.Instance.HurtPoints.Add(SenceObject.ID, hit.point);
                                //        }
                                //    }
                                
                                
                                MsgCenter.Instance.AILogic(this, LeftFirePoint, RightFirePoint);
                                if (LastHP != null &&
                                    LastHP != _pc_man.HP)
                                {
                                    LastHP = _pc_man.HP;
                                    MsgCenter.Instance.AddToSendList(this);
                                }
                                else if (LastHP == null) 
                                {
                                    LastHP = _pc_man.HP;
                                }

                                if (_pc_man.Rotation != transform.rotation)
                                {
                                    MsgCenter.Instance.AddToSendList(this);
                                }
                                _pc_man.Position = transform.position;
                                _pc_man.Rotation = transform.rotation;
                                //rig.WakeUp();
                                break;
                            case ObjectType.Bullet:
                                des_inv_time += Time.deltaTime;

                                if (des_inv_time >= destroy_time)
                                {
                                    //Debug.LogError("Destroy");
                                    MsgCenter.Instance.DestroySenceObject(this);
                                }
                                break;
                            case ObjectType.PlayerBullet:
                                //Debug.Log(">>>>>>>>>>>>>>>>>>" + SenceObject.Name);
                                if (!SenceObject.Name.Equals("weapon2"))
                                {
                                    des_inv_time += Time.deltaTime;
                                    if (des_inv_time >= destroy_time)
                                    {
                                        MsgCenter.Instance.SenceObjects.Remove(SenceObject.ID);
                                        Destroy(gameObject);
                                    }
                                }
                                
                                //_bullet.Position = transform.position;
                                //_bullet.Rotation = transform.rotation;
                                break;
                            case ObjectType.Weapon:
                                if (!_canDestroy)
                                {
                                    _canDestroyTime += Time.deltaTime;
                                    if (_canDestroyTime >= 1f)
                                    {
                                        _canDestroy = true;
                                    }
                                    
                                }
                                _weapon_time += Time.deltaTime;
                                _weapon.Position = transform.position;
                                _weapon.Rotation = transform.rotation;
                                if (_weapon_time >= 0.2f)
                                {
                                    _weapon_time = 0;
                                    MsgCenter.Instance.AddToSendList(this);
                                }
                                break;
                        }
                        break;
                    case GameState.End:
                        if (SenceObject.type == ObjectType.Player)
                        {
                            if (left_text.gameObject.activeSelf)
                            {
                                MsgCenter.Instance.LeftSlider.gameObject.SetActive(false);
                                left_text.gameObject.SetActive(false);
                            }
                            if (right_text.gameObject.activeSelf)
                            {
                                MsgCenter.Instance.RightSlider.gameObject.SetActive(false);
                                right_text.gameObject.SetActive(false);
                            }
                            if (bullet_data_1 != null && bullet_data_1.gameObject.activeSelf)
                            {
                                bullet_data_1.gameObject.SetActive(false);
                            }
                            //if (_player.LeftRein != null)
                            //{
                            //    LeftRein.transform.position = _player.LeftRein.Position;
                            //    LeftRein.transform.rotation = _player.LeftRein.Rotation;
                            //}
                            //if (_player.RightRein != null)
                            //{
                            //    RightRein.transform.position = _player.RightRein.Position;
                            //    RightRein.transform.rotation = _player.RightRein.Rotation;
                            //}
                            //Camera.main.transform.position = _player.CameraPosition;
                            //Camera.main.transform.eulerAngles = _player.CameraRotation;
                            //Player.transform.position = _player.Position;
                            //Player.transform.rotation = _player.Rotation;

                        }
                        if (SenceObject.type == ObjectType.UI)
                        {
                            switch (_game_menu.muenu_state)
                            {
                                case MenuState.Continue:
                                    Application.LoadLevel(MsgCenter.Instance.SenceName);
                                    break;
                                case MenuState.ExitGame:

                                    break;
                            }
                        }
                        break;
                }
            }
            else //客户端，采集主角数据，刷新ai，子弹数据
            {
                // Debug.Log(SenceObject.Name+"************************");
                switch (SenceObject.type)
                {
                    case ObjectType.Player:
                        //Debug.Log("Player");

                        SenceObject.Position = Player.transform.position;
                        SenceObject.Rotation = Player.transform.rotation;
                        _player.CameraPosition = Camera.main.transform.position;
                        _player.CameraRotation = Camera.main.transform.eulerAngles;
                        ////Debug.Log(_player.CameraRotation);
                        if (_player.RightRein == null)
                        {
                            _player.RightRein = new Rein(RightRein.transform.position, RightRein.transform.rotation, false);
                        }
                        if (_player.LeftRein == null)
                        {
                            _player.LeftRein = new Rein(LeftRein.transform.position, LeftRein.transform.rotation, false);
                        }
                        _player.RightRein.Position = RightRein.transform.position;
                        _player.RightRein.Rotation = RightRein.transform.rotation;
                        _player.LeftRein.Position = LeftRein.transform.position;
                        _player.LeftRein.Rotation = LeftRein.transform.rotation;

                        if ((RightRein.transform.position - last_rein_right_position).magnitude >= Recever.rInstance.SosoPositionSetup ||
                            (RightRein.transform.eulerAngles - last_rein_right_ratation).magnitude >= Recever.rInstance.SosoRotationSetup ||
                            (LeftRein.transform.position - last_rein_left_position).magnitude >= Recever.rInstance.SosoPositionSetup ||
                            (LeftRein.transform.eulerAngles - last_rein_left_ratation).magnitude >= Recever.rInstance.SosoRotationSetup ||
                            (Camera.main.transform.eulerAngles - last_camera_rotation).magnitude >= Recever.rInstance.SosoRotationSetup ||
                            (transform.position - last_position).magnitude >= Recever.rInstance.SosoPositionSetup ||
                            (transform.eulerAngles - last_rotation).magnitude >= Recever.rInstance.SosoRotationSetup ||
                            last_rein_left_key != _player.LeftRein.Key ||
                            last_rein_right_key != _player.RightRein.Key)
                        {
                            last_position = transform.position;
                            last_rotation = transform.eulerAngles;
                            last_rein_right_position = RightRein.transform.position;
                            last_rein_right_ratation = RightRein.transform.eulerAngles;
                            last_rein_left_position = LeftRein.transform.position;
                            last_rein_left_ratation = LeftRein.transform.eulerAngles;
                            last_camera_rotation = Camera.main.transform.eulerAngles;
                            last_position = transform.position;
                            last_rotation = transform.eulerAngles;
                            last_rein_left_key = _player.LeftRein.Key;
                            last_rein_right_key = _player.RightRein.Key;
                            Recever.rInstance.AddToSendList(this);
                            //MsgCenter.Instance.AddToSendList(this);
                        }

                        
                        break;
                    case ObjectType.AI:
                        //接收石头数据时有位置突然变换，在生成时将其置为false
                        if (!this.gameObject.activeSelf && _pc_man.ai_type == AIType.Stone)
                        {
                            this.gameObject.SetActive(true);
                        }
                       
                          //  Debug.Log(_pc_man.Name+_pc_man.Rotation); 
                        transform.position = Vector3.Lerp(transform.position, _pc_man.Position, 0.3f);
                        transform.rotation = Quaternion.Lerp(transform.rotation, _pc_man.Rotation, 0.3f);
                 
                        if (last_position == Vector3.zero)
                        {
                            last_position = transform.position;
                        }
                        else
                        {
                            if (last_position == transform.position)
                            {
                                //如果敌机不动的话0.5f后销毁，除了Boss之外
                                des_wait_time += Time.deltaTime;
                                if (des_wait_time >= 1f && _pc_man.ai_type != AIType.LifeAndAttack)
                                {
                                 //  gameObject.SetActive(false);
                                }
                            }
                            else 
                            {
                                if (!gameObject.activeSelf)
                                {
                                    gameObject.SetActive(true);
                                }
                                last_position = transform.position;
                                des_wait_time = 0;
                            }
                        }
                        if (!Recever.rInstance.HurtPoints.ContainsKey(SenceObject.ID) &&
                            (Recever.rInstance.PlayerData.transform.position - transform.position).magnitude <= MIN_DISTANS)
                        {
                            ray = new Ray(transform.position, Recever.rInstance.PlayerData.transform.position - transform.position);
                            if (Physics.Raycast(ray, out hit))
                            {
                                if (hit.transform.gameObject.CompareTag(SenceGameObject.PLAYER_TAG))
                                {
                                    Recever.rInstance.HurtPoints.Add(SenceObject.ID, hit.point);
                                }
                            }
                        }
                       
                        break;
                    case ObjectType.Bullet:
                        ai_life_time += Time.deltaTime;
                        if (ai_life_time >= 20f)
                        {
                            Recever.rInstance.DestroySenceObject(this, 0);
                        }
                        break;
                    case ObjectType.PlayerBullet:
                        //transform.position = _bullet.Position;
                        //transform.rotation = _bullet.Rotation;
                        break;
                    case ObjectType.Weapon:
                        transform.position = _weapon.Position;
                        transform.rotation = _weapon.Rotation;

                        break;
                    case ObjectType.UI:
                        Recever.rInstance.Menu.Name = _uiMenu.Name;
                        
                        break;
                    default:
                        break;
                }
            }
        }
        
    }

    public bool isVisible;
    //void OnBecameVisible() 
    //{
    //    isVisible = false;
    //    if(SenceObject.type == ObjectType.AI)
    //    {
    //        Debug.Log(gameObject.name + ":OnBecameVisible");
    //    }
        
    //}

    //void OnBecameInvisible() 
    //{
    //    isVisible = true;
    //    if (SenceObject.type == ObjectType.AI)
    //    {
    //        Debug.Log(gameObject.name + ":OnBecameInvisible");
    //    }
    //}

    int luach_inv_time;
    /// <summary>
    /// 生成子弹
    /// </summary>
    /// <param name="role"></param>
    /// <param name="isSevice"></param>
    /// <param name="obj"></param>
    SenceGameObject bullet_data_1 = null;
    SenceGameObject bullet_data_2 = null;
    TextMesh left_text,right_text;
    public void CrateBullet(Role role, bool isSevice, RootObject obj)
    {
        //Debug.Log("weapon = " + role.useWeapon.Name);
        //Debug.Log("LaunchSpeed = " + role.LaunchSpeed);
       
        left_time += Time.deltaTime;
        luach_inv_time = Mathf.RoundToInt(Mathf.Min(left_time / role.LaunchSpeed * 100, 100));
        if (left_text == null)
        {
            left_text = MsgCenter.Instance.LeftHub.transform.FindChild("succes").GetComponent<TextMesh>();
        }
        left_text.text = "<color=green>"+luach_inv_time.ToString()+"%</color>";


        if (_player.useWeapon.Name.Equals("weapon3"))
        {
            if (MsgCenter.Instance.GameState.Equals(GameState.End))
            {
                Debug.Log("GameMenu");
                left_text.gameObject.SetActive(false);
            }
            else if (MsgCenter.Instance.GameState.Equals(GameState.Playing))
            {
                left_text.gameObject.SetActive(true);
            }
            if (luach_inv_time >= 100)
            {
                //Debug.Log("left_luach_inv_time >= 100");
                MsgCenter.Instance.LeftSlider.fillAmount = 1;
                Messenger.Broadcast<string>(GameEvent.DisplayWeapon, "FireLL");
            }
            else
            {
                MsgCenter.Instance.LeftSlider.fillAmount = luach_inv_time / 100f;
                Messenger.Broadcast<string>(GameEvent.HideWeapon, "FireLL");
            }
        }
        else
        {
            //Debug.Log("elselfjslkefjlskjefkl");
            MsgCenter.Instance.LeftSlider.fillAmount = 0;
            left_text.gameObject.SetActive(false);
        }
        if (role.LeftRein.Key)
        {
            
            if (left_time >= role.LaunchSpeed)
            {
                left_time = 0;
                Bullet bullet;
                if (isSevice)
                {
                    bullet = new Bullet(0, role.useWeapon.Name,
                        role.Attack, Vector3.zero, Quaternion.identity,
                        ObjectState.None, ObjectType.PlayerBullet, false);
                }
                else
                {
                    bullet = obj as Bullet;
                }
                bullet_data_1 = MsgCenter.Instance.CreateBullet(bullet, isSevice);
                if (bullet.Name.Equals("weapon2"))
                {
                    //Debug.LogError("执行？？");
                    bullet_data_1.gameObject.transform.SetParent(LeftFirePoint.transform, false);
                    bullet_data_1.transform.localPosition = Vector3.zero;
                    bullet_data_1.transform.localEulerAngles = Vector3.zero;
                    LeftHasSend = true;
                }
                //bullet_data.gameObject.transform.parent = LeftRein.transform;
                bullet_data_1.gameObject.transform.position = LeftFirePoint.transform.position;
                bullet_data_1.gameObject.transform.forward = LeftFirePoint.transform.forward;
                bullet_data_1.gameObject.tag = PLAYER_BULLET_TAG;

                

                Rigidbody rig = bullet_data_1.GetComponent<Rigidbody>();
                bullet_data_1.SenceObject.Name = "left";
                //Debug.Log(bullet.Name + ">>>>> " + bullet.ID);
                //bullet_data.SenceObject = bullet;
                //Debug.Log("---=====>>>>>" + bullet_data.SenceObject.Name);
                bullet_data_1.gameObject.SetActive(true);
                if (bullet.Name.Equals("weapon3"))
                {
                    //GuidedMissile missile = bullet_data_1.gameObject.AddComponent<GuidedMissile>();
                    Guided guided = bullet_data_1.gameObject.AddComponent<Guided>();
                    if (MsgCenter.Instance.left_fire_target != null)
                    {
                        guided.Target = MsgCenter.Instance.left_fire_target.transform;
                        bullet_data_1._missile.target_id = MsgCenter.Instance.left_fire_target.SenceObject.ID;
                        MsgCenter.Instance.left_fire_target = null;
                    }
                    bullet_data_1.SenceObject.Position = bullet_data_1.gameObject.transform.position;
                    bullet_data_1.SenceObject.Rotation = bullet_data_1.gameObject.transform.rotation;
                    MsgCenter.Instance.AddToSendList(bullet_data_1);
                    rig.AddForce(LeftBattery.transform.forward * MsgCenter.LUACH_FORCE);
                }
                else if (rig != null)
                {
                    rig.AddForce(LeftBattery.transform.forward * MsgCenter.LUACH_FORCE);
                }
            }
        }

        if (role.LeftRein.Key == false 
            && LeftHasSend
            && MsgCenter.Instance.player.useWeapon.Name.Equals("weapon2")
            && bullet_data_1 != null)
        {
            bullet_data_1.gameObject.SetActive(false);
            LeftHasSend = false;
        }
        right_time += Time.deltaTime;
        luach_inv_time = Mathf.RoundToInt(Mathf.Min(right_time / role.LaunchSpeed * 100, 100));

        
        if (right_text == null) 
        {
            right_text = MsgCenter.Instance.RightHub.transform.FindChild("succes").GetComponent<TextMesh>();
        }
        right_text.text = "<color=green>"+luach_inv_time.ToString()+"%</color>";
        if (_player.useWeapon.Name.Equals("weapon3"))
        {
            right_text.gameObject.SetActive(true);
            if (luach_inv_time >= 100)
            {
                //Debug.Log("right_luach_inv_time >= 100");
                MsgCenter.Instance.RightSlider.fillAmount = 1;
                Messenger.Broadcast<string>(GameEvent.DisplayWeapon, "FireRR");
            }
            else
            {
                MsgCenter.Instance.RightSlider.fillAmount = luach_inv_time / 100f;
                Messenger.Broadcast<string>(GameEvent.HideWeapon, "FireRR");
            }
        }
        else
        {
            MsgCenter.Instance.RightSlider.fillAmount = 0;
            right_text.gameObject.SetActive(false);
        }
        if (role.RightRein.Key)
        {
            if (right_time >= role.LaunchSpeed)
            {
                right_time = 0;
                Bullet bullet;
                if (isSevice)
                {
                    bullet = new Bullet(0, role.useWeapon.Name,
                        role.Attack, Vector3.zero, Quaternion.identity,
                        ObjectState.None, ObjectType.PlayerBullet, true);
                }
                else
                {
                    bullet = obj as Bullet;
                }
                bullet_data_2 = MsgCenter.Instance.CreateBullet(bullet, isSevice, true);
                //Debug.LogError("bullet_data" + bullet_data.gameObject.name);
                if (bullet.Name.Equals("weapon2"))
                {
                    //Debug.LogError("执行？？");
                    bullet_data_2.gameObject.transform.SetParent(RightFirePoint.transform, false);
                    bullet_data_2.transform.localPosition = Vector3.zero;
                    bullet_data_2.transform.localEulerAngles = Vector3.zero;
                    RightHasSend = true;
                }
                //bullet_data.gameObject.transform.parent = RightRein.transform;
                bullet_data_2.gameObject.transform.position = RightFirePoint.transform.position;
                bullet_data_2.gameObject.transform.forward = RightFirePoint.transform.forward;
                bullet_data_2.gameObject.tag = PLAYER_BULLET_TAG;

                Rigidbody rig = bullet_data_2.gameObject.GetComponent<Rigidbody>();
                bullet_data_2.SenceObject.Name = "right";
                //Debug.Log(bullet.Name + ">>>>> " + bullet.ID);
                //bullet_data.SenceObject = bullet;
                //Debug.Log("---=====>>>>>" + bullet_data.SenceObject.Name);
                bullet_data_2.gameObject.SetActive(true);
                
                if (bullet.Name.Equals("weapon3"))
                {
                    //GuidedMissile missile = bullet_data_1.gameObject.AddComponent<GuidedMissile>();
                    Guided guided = bullet_data_2.gameObject.AddComponent<Guided>();
                    if (MsgCenter.Instance.right_fire_target != null)
                    {
                        guided.Target = MsgCenter.Instance.right_fire_target.transform;
                        bullet_data_2._missile.target_id = MsgCenter.Instance.right_fire_target.SenceObject.ID;
                        MsgCenter.Instance.right_fire_target = null;
                    }
                    bullet_data_2.SenceObject.Position = bullet_data_2.gameObject.transform.position;
                    bullet_data_2.SenceObject.Rotation = bullet_data_2.gameObject.transform.rotation;
                    MsgCenter.Instance.AddToSendList(bullet_data_2);
                    rig.AddForce(RightBattery.transform.forward * MsgCenter.LUACH_FORCE);
                }
                else if (rig != null)
                {
                    rig.AddForce(RightBattery.transform.forward * MsgCenter.LUACH_FORCE);
                }
            }
        }
        if (role.RightRein.Key == false 
            && RightHasSend &&
            MsgCenter.Instance.player.useWeapon.Name.Equals("weapon2")
            && bullet_data_2 != null)
        {
            bullet_data_2.gameObject.SetActive(false);
            RightHasSend = false;
        }
    }

   // bool auto_walke = false;

    //void LateUpdate() 
    //{
    //    if (auto_walke)
    //    {
    //        transform.Translate(transform.forward);
    //    }
    //    if (auto_walke && transform.position.z < -1)
    //    {
            
    //    }
    //}


    void OnBecameVisible()
    {
        if (type == ObjectType.GuidedMissile &&
            _missile.target_id == null)
        {
            Debug.Log("destroy:>>>");
            timer += Time.deltaTime;
            if (timer >= 5 &&
                MsgCenter.Instance != null)
            {
                MsgCenter.Instance.DestroySenceObject(this,10);
            }
        }
    }

    /// <summary>
    /// 特效播放的声音
    /// </summary>
    void PlayMusic()
    {
        if (_pc_man != null)
        {

            switch (_pc_man.ai_type)
            {
                case AIType.Lithe:
                case AIType.Attack:
                case AIType.SmartAI:
                    Audiocontroller.instans.PlayEffectAudio("Explosion");/*敌人爆炸声音*/
                    break;
                case AIType.LifeAndAttack:
                    Audiocontroller.instans.PlayGameAudio("dabaozha");/*Boss爆炸声音*/
                    break;
                case AIType.Stone:
                        Audiocontroller.instans.PlayEffectAudio("shitou");/*石头爆炸声音*/
                    break;
                default:
                    break;
            }
        }
    }
    private bool LeftHasSend;
    private bool RightHasSend;
    private bool istrigger=false;
    void OnDestroy()
    {
        if ( _pc_man != null && _pc_man.ai_type == AIType.LifeAndAttack )
        {
            Debug.Log("ondestroy+ EnemyEffect == null" + EnemyEffect == null);
        }
        //Debug.Log("ondestroy"+EnemyEffect == null);
        if (EnemyEffect != null)
        {
            //Debug.Log("IFIIFIFIFIIFIIFIFI");
            EnemyEffect.transform.parent = null;
            EnemyEffect.SetActive(true);
            Destroy(EnemyEffect, 2);
            PlayMusic();
        }
        if(type == ObjectType.AI &&
            _pc_man.ai_type != AIType.Stone)
        {
            if (Recever.rInstance != null && Recever.rInstance.GameState == GameState.Playing)
            {
                Recever.rInstance.Text3D("<color=green>+"+_pc_man.Grade.ToString()+"</color>",transform.position);
            }
        }
        if (MsgCenter.Instance.HurtPoints.ContainsKey(SenceObject.ID))
        {
       //     Debug.Log("<color=yelow>从击中点的列表中删除"+SenceObject.Name+"</color>");
        //    MsgCenter.Instance.HurtPoints.Remove(SenceObject.ID);
        }
        
    }

}
