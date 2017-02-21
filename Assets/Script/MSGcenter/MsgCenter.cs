using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using Datas;
using System;
using System.IO;
using Newtonsoft.Json.Linq;
using System.Text;

public class MsgCenter : MonoBehaviour
{
    public string globalname;
    public static MsgCenter Instance;
    
    public GameObject GameMenu;

    public Text HPTx;

    public string Host;
    public int Port;

    public TextMesh GradeTx,PlayerHPText,leftrange,rightrange,topworming;
    //  关卡数据
    public Dictionary<int, Role> RoleLevelData;

    public Dictionary<int, List<AI>> AILevelData;

    //  >>>>>>>>>>>>>>>>>  场景数据<id,对象> <<<<<<<<<<<<<
    public Dictionary<int, SenceGameObject> SenceObjects;
    //<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<
    //场景内现在的主角色
    public SenceGameObject player_data;

    public GameObject HitEffectPrefab;
    public GameObject HitBigEffectPrefab;

    public Dictionary<string, GameObject> onhit_particle = new Dictionary<string, GameObject>();
    public List<string> hit_names = new List<string>();
    public List<GameObject> hit_prefabes = new List<GameObject>();

    public string DefaultWeapon;

    public Role player;
    public GameObject Boss;
    public SenceGameObject PlayerData
    {
        set
        {
            player_data = value;
            player = player_data.SenceObject as Role;
        }
        get
        {
            return player_data;
        }
    }

    // 选择左右手
    public bool left_key 
    {
        set 
        {
            //Debug.Log("SetSET");
            if (PlayerData._player.LeftRein == null)
            {
                PlayerData._player.LeftRein = new Rein(player_data.LeftRein.transform.position, player_data.LeftRein.transform.rotation, value);
            }
            else
            {
                PlayerData._player.LeftRein.Key = value;
            }
            
        }
        get
        {
            if (PlayerData._player.LeftRein!=null)
            {
                return PlayerData._player.LeftRein.Key;
            }
            else 
            {
                return false;
            }
        }
    }

    public bool right_key
    {
        set
        {
            if (PlayerData._player.RightRein == null)
            {
                PlayerData._player.RightRein = new Rein(player_data.RightRein.transform.position, player_data.RightRein.transform.rotation, value);
            }
            else 
            {
                PlayerData._player.RightRein.Key = value;
            }
            
        }
        get
        {
            if (PlayerData._player.RightRein != null)
            {
                return PlayerData._player.RightRein.Key;
            }
            else
            {
                return false;
            }
        }
    }

    public const float LUACH_FORCE = 17000;
    // 玩家预设
    public List<GameObject> RolePrefabs;
    // AI预设
    public List<GameObject> AiPrefabs;
    // 陨石预设
    public List<GameObject> StonePrefabs;
    // 左侧武器预设
    public List<GameObject> LeftWeaponPrefabs;
    // 武器名称
    public List<string> WeaponsName;
    // 右侧武器预设
    public List<GameObject> RightWeaponPrefabs;
    //  销毁物体列表
    public Dictionary<int,SenceGameObject> DestroyGameObject;
    //  武器预设
    public Dictionary<string, GameObject> LeftWeapons;
    public Dictionary<string, GameObject> RightWeapons;
    //  武器数据
    public Dictionary<string, Weapon> WeaponData;
    //  排行榜
    public Ranking ranking = null;
    //  网络管理脚本
    //public NetworkServer network_server;
    //  发送数据的间隔时间
    public float SendInvTime;
    //  子弹预设
    public List<GameObject> BulletPrefabs;
    public GameObject ranking_prefab;
    public GameObject cover_prefab;

    //玩家名字预设
    public Sprite[] PlayerHP;
    public Sprite[] playerGrade;
    public GameObject grade_prefab;
    public GameObject flytext;
    public List<string> WeaponBulletName;
    //  武器名称和子弹物体相对应
    public Dictionary<string, GameObject> Bullets;

    public GameState GameState = GameState.None;

    public float AIMoveSpeed;

    public Dictionary<int, HP_View> roleViews;

    public bool IsServe = false;
    public bool CanLaunchBullet = false;

    public float SosoPositionSetup;
    public float SosoRotationSetup;

    public float QuickPositionSetup;
    public float QuickRotationSetup;

    public Transform StartPosition;
    public Transform TargetPosition;
    public Transform EndPosition;

    public SenceGameObject left_fire_target = null;
    public SenceGameObject right_fire_target = null;

    public string SenceName;

    public bool IsShaking = false;

    public DropOut dropOut;
    public AI_Logic ai_logic;

    public GameObject EmptyObject;
    public string RankingDirName;

    public Dictionary<int, Vector3> HurtPoints;

    public GameObject HubObj;
    public GameObject LockHubObj;

    public GameObject LeftHub;
    public Image LeftSlider;
    public GameObject RightHub;
    public Image RightSlider;

    public GameObject LeftLockHub;
    public GameObject RightLockHub;

    public GameMenu Menu;
    private string _last_weapon;
    private int id_index;

    private bool _started = false;

	private int alive_AI;

    //敌人被打击时播放的粒子效果,枪口效果
    public GameObject hit_particle_prefab, fire_point_particle;
    public List<GameObject> BulletPool = new List<GameObject>();
    void Awake()
    {
        ReadConfigFile();
        HurtPoints = new Dictionary<int, Vector3>();
        OnAwake();
        roleViews = new Dictionary<int, HP_View>();
        InitWeaponData();
        LoadLevelData();


        Messenger.Init();
        //InitNetwork();
    }

    public virtual void OnAwake()
    {
        ReadRanking();
        Instance = this;
        DestroyGameObject = new Dictionary<int,SenceGameObject>();
        StartCoroutine(DestroyObject());
        ai_logic = gameObject.AddComponent<AI_Logic>();
     //   coverchildren = cover_prefab.GetComponentsInChildren<Image>();
    }

    // Use this for initialization
    public virtual void Start()
    {
        //测试
        GameState = Datas.GameState.Start;

        Init(1);
        LeftHub = Instantiate(HubObj);
        LeftHub.transform.FindChild("succes").gameObject.SetActive(false);
        LeftHub.AddComponent<LookAtCamera>();
        LeftSlider = LeftHub.transform.FindChild("Wait").gameObject.GetComponent<Image>();
        RightHub = Instantiate(HubObj);
        RightHub.transform.FindChild("succes").gameObject.SetActive(false);
        RightHub.AddComponent<LookAtCamera>();
        RightSlider = RightHub.transform.FindChild("Wait").gameObject.GetComponent<Image>();
        RightLockHub = Instantiate(LockHubObj);
        RightLockHub.AddComponent<LookAtCamera>();
        RightLockHub.SetActive(false);
        LeftLockHub = Instantiate(LockHubObj);
        LeftLockHub.AddComponent<LookAtCamera>();
        LeftLockHub.SetActive(false);
    }

    private Role _role;
    float leftend=0, rightend=0;
    protected void Refreshranger()
    {
        rightend = UnityEngine.Random.Range(10.2f, 100.5f);
        leftend = UnityEngine.Random.Range(100.2f, 1000.5f);
    }
    // Update is called once per frame
    public virtual void Update()
    {
        if (leftrange != null && rightrange != null)
        {
            if (leftend - float.Parse(leftrange.text) <= 0.1 && rightend - float.Parse(rightrange.text) <= 0.1)
            {
                Refreshranger();
                //leftrange.text = Mathf.Lerp(float.Parse(leftrange.text), leftend, 0.2f).ToString();
                //rightrange.text = Mathf.Lerp(float.Parse(rightrange.text), rightend, 0.2f).ToString();   
            }
            else
            {
                leftrange.text = Mathf.Lerp(float.Parse(leftrange.text), leftend, 0.2f).ToString("f2");
                rightrange.text = Mathf.Lerp(float.Parse(rightrange.text), rightend, 0.2f).ToString("f2");          
            }
        }
        RefreshPlayerGrad(player.HP,player.Grade);

        //Debug.Log();
        switch (GameState)
        {
            case GameState.Start:
                if(!_started)
                {
//                    string[] visiablename = ranking.GetenableName(all_names);
//                    Debug.Log(visiablename.Length);
                    DisplayMenu(true);
                }
                break;
            case Datas.GameState.Playing:
                _started = false;
                break;
            case GameState.End:
                if (!_started)
                {
                    ranking.AddToRanking(new Person(player.Name,player.Grade));
                    ranking.SaveTo(Application.dataPath + "/" + 
                        RankingDirName);
                    DisplayMenu(false);
                    //Debug.Log("gamestate.end,gamestate.end,gamestate.endgamestate.end,");
                    EmptyObject = new GameObject();
                    SenceGameObject obj_data = EmptyObject.AddComponent<SenceGameObject>();
                    obj_data.SenceObject = ranking;
                    AddToSendList(obj_data);
                    ClearScene(SenceObjects);
                }
                break;
            case GameState.Exit:
                Application.Quit();
                break;
            default:
                break;
        }
    }

    public static bool IsServece() 
    {
        if(Instance != null)
        {
            return true;
        }
        return false;
    }
    private int temp_HP;
    private int priviousHP;
    private bool isassigne = false;
    private int temp_grade = -1;
    public void RefreshPlayerGrad(int _Hp,int _Grade) 
    {
        if (!isassigne)
        {
            priviousHP = player.HP;
            isassigne = true;
        }
            //避免重复赋值
            if (temp_grade != _Grade)
            {
                GradeTx.text = "                "+ _Grade.ToString();
                temp_grade = _Grade;
            }
            if (temp_HP != _Hp)
            {
                if (_Hp / priviousHP <= 1)
                {
                    PlayerHPText.text = "HP:" + Mathf.Round((((float)_Hp / (float)priviousHP)) * 100) + "%";
                }
                else {
                    PlayerHPText.text = "HP:"+100 + "%";
                }
                temp_HP = _Hp;
            }
        
    }
    public SenceGameObject DisplayMenu(bool isStart) 
    {
        GameObject obj = Instantiate(GameMenu) as GameObject;
        SenceGameObject obj_data = obj.AddComponent<SenceGameObject>();
        _started = true;
        obj_data.SenceObject = new UIMenu(GetID(), "", 0, ObjectType.UI, isStart);
        obj_data._game_menu = obj.AddComponent<GameMenu>();
        obj_data._game_menu.isStart = isStart;
        obj_data.isSevice = IsServe;
        if(isStart)
        {
            obj_data._game_menu.muenu_state = MenuState.None;
        }
        else
        {
            obj_data._game_menu.muenu_state = MenuState.Display;
            //Debug.Log(" obj_data._game_menu.muenu_state" + obj_data._game_menu.muenu_state);
        }
       
        Menu = obj_data._game_menu;
        if(IsServe)
        {
            AddToSendList(obj_data);
            AddToSendList(PlayerData);
        }
        obj.SetActive(true);
        return obj_data;
    }

    public int GetID()
    {
        return ++id_index;
    }

    public void AILogic(SenceGameObject obj_data, GameObject left_point, GameObject right_point)
    {
        ai_logic.OnLogic(obj_data, left_point, right_point);
    }

    public void ReadRanking() 
    {
        string path = Application.dataPath + "/" + RankingDirName;
        ranking = Ranking.LoadFrom(path,GetID());
        if(ranking == null)
        {
            DirectoryInfo dir = new DirectoryInfo(Path.GetDirectoryName(path));
            if (!dir.Exists)
            {
                dir.Create();
            }
            if(!File.Exists(path))
            {
                FileStream fs = new FileStream(path, FileMode.Create);
                fs.Close();
            }
            Debug.Log("ranking == null");
            ranking = new Ranking(GetID(),"ranking");
        }
    }

   
    private void ReadConfigFile() 
    {
        string path = Application.dataPath + "/Config/config.txt";
        //Debug.LogError(path);
        if (File.Exists(path))
        {
            FileStream fs = new FileStream(path, FileMode.Open, FileAccess.ReadWrite);

            NetConfig config = null;
            try
            {
                byte[] bt1 = new byte[256];
                int n = fs.Read(bt1, 0, (int)fs.Length);
                string data = Encoding.UTF8.GetString(bt1);
                JObject jobj = JObject.Parse(data);
                config = new NetConfig();
                config.ServerHost = jobj["ServerHost"].Value<string>();
                config.ServerPort = jobj["ServerPort"].Value<string>();
                config.ClientHost = jobj["ClientHost"].Value<string>();
                config.ClientPort = jobj["ClientPort"].Value<string>();
            }
            catch (System.Exception exc)
            {
                Debug.LogError(exc.Message);
            }
            finally
            {
                if (config != null)
                {
                    ReadConfig(config);
                }
                fs.Close();
            }
        }
    }

    public virtual void ReadConfig(NetConfig config) 
    {
        Host = config.ServerHost;
        Port = int.Parse(config.ServerPort);
    }


    public void AddToRanking(Person person) 
    {
        ranking.AddToRanking(person);
    }

    public void SaveRanking() 
    {
        string path = Application.dataPath + "/" + RankingDirName;
        ranking.SaveTo(path);
    }

    public void InitWeapon()
    {
        LeftWeapons = new Dictionary<string, GameObject>();
        RightWeapons = new Dictionary<string, GameObject>();
        //  将武器、名字两列表对应生成武器字典
        for (int i = 0; i < LeftWeaponPrefabs.Count; i++)
        {
            //Debug.Log(i + "ssssssssssssssssssssssssssss");
            LeftWeapons.Add(WeaponsName[i], LeftWeaponPrefabs[i]);
            RightWeapons.Add(WeaponsName[i], RightWeaponPrefabs[i]);
        }
        //  将武器名字和对应的子弹对应
        Bullets = new Dictionary<string, GameObject>();
        for (int i = 0; i < BulletPrefabs.Count; i++)
        {
            Bullets.Add(WeaponBulletName[i], BulletPrefabs[i]);
        }
    }



    /// <summary>
    /// 初始化武器表
    /// </summary>
    public void InitWeaponData()
    {
        InitWeapon();
        WeaponData = new Dictionary<string, Weapon>();
        //  从xml读取武器的参数
        TextAsset[] levels = Resources.LoadAll<TextAsset>("weapons");
        XmlDocument xml;
        XmlElement xmlRoot;
        foreach (TextAsset txt in levels)
        {
            xml = new XmlDocument();
            xml.LoadXml(txt.ToString());
            xmlRoot = xml.DocumentElement;
            foreach (XmlNode node in xmlRoot["weapon_list"].ChildNodes)
            {
                WeaponData.Add(node.Attributes["Name"].Value,
                        new Weapon(node.Attributes["Name"].Value,
                            int.Parse(node.Attributes["HP"].Value),
                            int.Parse(node.Attributes["Attack"].Value),
                            float.Parse(node.Attributes["LaunchSpeed"].Value),
                            float.Parse(node.Attributes["LifeTime"].Value)
                            )
                    );
            }
        }
        GameObject obj1, obj2;
        PlayerWeapon weapon1;
        //  将武器参数与预设还有对应要添加的脚本合在一起
        //////Debug.Log(Weapons.Count);
        List<string> keys = new List<string>(LeftWeapons.Keys);
        foreach (string key in keys)
        {
            obj1 = Instantiate(LeftWeapons[key]) as GameObject;
            obj2 = Instantiate(RightWeapons[key]) as GameObject;
            weapon1 = obj1.AddComponent<PlayerWeapon>();
            weapon1.weapon_data = WeaponData[key];
            //Destroy(Weapons[key]);
            LeftWeapons[key] = obj1;
            RightWeapons[key] = obj2;
            obj1.SetActive(false);
            obj2.SetActive(false);
        }

        dropOut = gameObject.AddComponent<DropOut>();
    }

    public void ResetGame()
    {
        id_index = 0;
        if (SenceObjects != null)
        {
            if (SenceObjects.Count > 0)
            {
                List<int> keys = new List<int>(SenceObjects.Keys);
                for (int i = 0; i < keys.Count; i++)
                {
                    Destroy(SenceObjects[i].gameObject);
                }
                SenceObjects.Clear();
            }
        }
        else
        {
            SenceObjects = new Dictionary<int, SenceGameObject>();
        }
        GameState = Datas.GameState.None;
        InitWeaponData();
        LoadLevelData();
        //InitNetwork();
        Init(1);
        Debug.Log("ResetGame");
    }

    public void LoadLevel(int n)
    {
        id_index = 0;
        if (SenceObjects != null)
        {
            if (SenceObjects.Count > 0)
            {
                List<int> keys = new List<int>(SenceObjects.Keys);
                for (int i = 0; i < keys.Count; i++)
                {
                    Destroy(SenceObjects[i].gameObject);
                }
                SenceObjects.Clear();
            }
        }
        else
        {
            SenceObjects = new Dictionary<int, SenceGameObject>();
        }
        InitWeaponData();
        LoadLevelData();
        //InitNetwork();
        Init(n);
    }


    private void DestroyBoss() 
    {
    
    }


    GameObject obj = null;
    public const float EffectLifeTime = 0.35f;
   protected bool issmallenmycome = false;
    public virtual void OnHit(SenceGameObject self, Collider otherColl)
    {
        SenceGameObject obj_data;
        switch (self.SenceObject.type)
        {
            case ObjectType.Player:
                //Debug.Log("Player OnHit");
                if (otherColl.gameObject.tag == SenceGameObject.AI_BULLET_TAG ||
                    //otherColl.gameObject.tag == SenceGameObject.AI_TAG ||   /*>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>*/
                    otherColl.gameObject.CompareTag(SenceGameObject.AI_STONE))
                {
                    otherColl.gameObject.SetActive(false);
                    //GameObject obj = Instantiate(HitEffectPrefab, otherColl.gameObject.transform.position,
                    //    Quaternion.identity) as GameObject;
                    Destroy(obj, EffectLifeTime);
                    obj_data = otherColl.gameObject.GetComponent<SenceGameObject>();
                    self._player.HP -= obj_data.SenceObject.Attack;
                    //Debug.Log("OnHit>>>>>(" + obj_data.SenceObject.Attack + ")self._player.HP" + self._player.HP);
                    self._player.HP = Mathf.Max(0, self._player.HP);
                    if (self._player.HP <= 0)
                    {
                        self.SenceObject.state = ObjectState.Unused;
                        GameState = Datas.GameState.End;
                        //cover_prefab.SetActive(false);
                    }
                    AddToSendList(self);
                    DestroySenceObject(obj_data, 1.0f);
                }
                break;
            case ObjectType.PlayerBullet:
                //Debug.Log(">>>>>>>>>>>>>>>>>>>>>");

                if (otherColl.gameObject.tag == SenceGameObject.AI_TAG)
                {
                    obj_data = otherColl.gameObject.GetComponent<SenceGameObject>();
                    
                    //Debug.Log(self.gameObject.name + ":-=>>" + otherColl.gameObject.name);
                    AI ai = obj_data.SenceObject as AI;
                    
                    ai.HP -= self._bullet.Attack;
                    ai.HP = Mathf.Max(ai.HP, 0);

                    obj = Instantiate(HitEffectPrefab, otherColl.gameObject.transform.position,
                            Quaternion.identity) as GameObject;
                    Destroy(obj, EffectLifeTime);
                    
                    if (ai.HP <= 0)
                    {
                        Debug.Log(">>>>>>>>>>>>>>>>>>>>>");
                        otherColl.gameObject.tag = SenceGameObject.NONE_TAG;

                        //if (ai.ai_type == AIType.LifeAndAttack)
                        //{
                        //    Debug.Log(">>>>>>>>>>>>>>>>>>>>>");
                        //    obj_data.gameObject.SetActive(false);
                        //    Destroy(obj_data.gameObject,1f);
                        //}
                        //else {
                           Destroy(obj_data.gameObject); 
                        //}
                        //Debug.Log("PlayerBullet -> destroy -> AI : " + obj_data.gameObject.name);

                        dropOut.CreateWeapon(ai.RewardWeapon, otherColl.gameObject.transform.position);


                        //服务器飘字
                        if (ai.ai_type != AIType.Stone)
                        {
                            player.Grade += ai.Grade;
                            Text3D("<color=green>+" + ai.Grade.ToString() + "</color>", otherColl.transform.position);
                        }

                        if (CanEndGame && obj_data._pc_man.ai_type == AIType.LifeAndAttack)
                        {
                            StartCoroutine(MsgCenter.Instance.EndGame());
                        }                     
                        AddToSendList(PlayerData);
                    }
                    //Debug.Log("end.");
                    if (!player.useWeapon.Name.Equals("weapon2"))
                    {
                        DestroySenceObject(self);
                    }
                }
                break;
            case ObjectType.GuidedMissile:
                if (otherColl.gameObject.tag == SenceGameObject.AI_TAG)
                {
                    obj_data = otherColl.gameObject.GetComponent<SenceGameObject>();
                    //Debug.Log(self.gameObject.name + ":-=>>" + otherColl.gameObject.name);
                    AI ai = obj_data.SenceObject as AI;
                    ai.HP -= self._missile.Attack;
                    ai.HP = Mathf.Max(ai.HP, 0);

                    if (ai.HP <= 0)
                    {
                        otherColl.gameObject.SetActive(false);
                        //Debug.Log(player.Grade);
                        //Debug.Log(ai.Grade);
                        dropOut.CreateWeapon(ai.RewardWeapon, otherColl.gameObject.transform.position);
                        //服务器飘字
                        if (ai.ai_type != AIType.Stone)
                        {
                            player.Grade += ai.Grade;
                            Text3D("<color=green>+" + ai.Grade.ToString() + "</color>", otherColl.transform.position);
                        }
                        //DestroyBoss();
                        obj_data.gameObject.SetActive(false);
                        Destroy(obj_data.gameObject,0.5f);

                        //DestroySenceObject(obj_data, 0.3f);
                        //AddToSendList(PlayerData);
                    }
                    //AddToSendList(obj_data);
                    DestroySenceObject(self,0);
                }
                break;
            case ObjectType.Weapon:
                
                //Debug.Log("Weapon onHit:" + otherColl.gameObject.tag);
                if (otherColl.gameObject.tag == SenceGameObject.PLAYER_BULLET_TAG)
                {
                    
                    obj_data = otherColl.gameObject.GetComponent<SenceGameObject>();
                    // PlayerData._player.useWeapon = new Weapon();
                    if (self._canDestroy)
                    {
                        ChangeWeapon(WeaponData[self.SenceObject.Name]);
                        if (//!self.SenceObject.Name.Equals("HP") && 
                            LeftWeapons.ContainsKey(self.SenceObject.Name))
                        {
                            Debug.Log("执行了？切换为" + player.useWeapon.Name + "武器");
                            Animatorcontroller.instance.PlayeMove();/*切换武器动画*/
                       //     Audiocontroller.instans.PlayEffectAudio("huanwuqi");/*切换武器声音*/
                            Messenger.Broadcast<string>(GameEvent.ChangeWeapon, self.SenceObject.Name);
                            HideBullet();
                        }
                        Destroy(self.gameObject);
                        //AddToSendList(PlayerData);

                        //DestroySenceObject(obj_data, 0);

                        DestroySenceObject(self, 0);
                    }
                }
                break;
            default:
                //Debug.Log(">>>>>>>>>>>>>>>>>>>>>><<<<<<<<<<<<<<<<<<<<<<<<<<<<");
                break;
        }
    }

    public void ChangeWeapon(Weapon weapon) 
    {
        if (!weapon.Name.Equals("HP"))
        {
            PlayerData._player.useWeapon = new Weapon(weapon);
            PlayerData._player.Attack = weapon.Attack;
            PlayerData._player.HP += weapon.HP;
            PlayerData._player.LaunchSpeed = weapon.LaunchSpeed;
        }
        else 
        {
            PlayerData._player.HP += weapon.HP;
        }
    }

    public IEnumerator DestroyObject() 
    {
        List<int> keys = new List<int>();
        while (true)
        {
            keys.Clear();
            keys.AddRange(DestroyGameObject.Keys);
            foreach (int i in keys)
            {
                if (DestroyGameObject[i] != null &&
                    DestroyGameObject[i].SenceObject.state == ObjectState.Destroy &&
                    DestroyGameObject[i].ClientIsDestroy)
                {
                    try
                    {
                        if (DestroyGameObject[i].type == ObjectType.Weapon)
                        {
                            DestroyGameObject[i].enabled = false;
                            DestroyGameObject[i].SenceObject.state = ObjectState.None;
                            dropOut.DestroyWeapon(DestroyGameObject[i].SenceObject.Name);
                            DestroyGameObject.Remove(i);
                        }
                        else 
                        {
                            if (DestroyGameObject[i].type == ObjectType.AI &&
                                DestroyGameObject[i]._pc_man.ai_type == AIType.LifeAndAttack)
                            {
                                DestroyGameObject[i].gameObject.SetActive(false);
                                Destroy(DestroyGameObject[i].gameObject);
                                DestroyGameObject.Remove(i);
                            }
                            else 
                            {
                                Destroy(DestroyGameObject[i].gameObject);
                                DestroyGameObject.Remove(i);
                            }
                        }
                    }
                    catch (Exception exc)
                    {
                        if (i < DestroyGameObject.Count)
                        {
                            DestroyGameObject.Remove(i);
                        }
                        
                        Debug.LogError(exc.Message);
                        continue;
                    }
                    
                }
                else if (DestroyGameObject[i] == null)
                {
                    DestroyGameObject.Remove(i);
                }
                yield return new WaitForSeconds(0.001f);
            }
            yield return new WaitForSeconds(0.02f);
        }
    }

    /// <summary>
    /// 从数据列表移除
    /// </summary>
    /// <param name="obj_data"></param>
    public virtual void DestroySenceObject(SenceGameObject obj_data, float time)
    {
        //    Debug.Log("DestroySenceObject");
        if (IsServe)
        {
            //obj_data.enabled = false;
//            Debug.Log("destroy:" + obj_data.gameObject.name);
            
            if (obj_data.SenceObject.type == ObjectType.Weapon)
            {
                obj_data.enabled = false;
                obj_data.ClientIsDestroy = false;
                obj_data.SenceObject.state = ObjectState.Destroy;
                AddToSendList(obj_data);
            }
            else if (obj_data.SenceObject.type == ObjectType.UI)
            {
                _started = false;
            }
            else 
            {
                //if (obj_data.SenceObject.type == ObjectType.AI)
                //{
                //    AI ai = obj_data.SenceObject as AI;
                //    if (ai.ai_type == AIType.LifeAndAttack)
                //    {
                        obj_data.gameObject.SetActive(false);
                //    }
                //    else
                //    {
                //        Destroy(obj_data.gameObject);
                //    }
                //}
                //else 
                //{
                //    Destroy(obj_data.gameObject);
                //}
                return;
            }

            if (!DestroyGameObject.ContainsKey(obj_data.SenceObject.ID))
            {
                DestroyGameObject.Add(obj_data.SenceObject.ID,obj_data);
            }
            
            //Destroy(obj_data.gameObject, time);
        }
        else
        {
            
            if (SenceObjects.ContainsKey(obj_data.SenceObject.ID))
            {
               
                SenceObjects.Remove(obj_data.SenceObject.ID);
               
                if (obj_data != null)
                {
                    Debug.Log("销毁掉落物");
                    Destroy(obj_data.gameObject);
                }
            }
        }
    }


    /// <summary>
    /// 飘字
    /// </summary>
    /// <param name="sort"></param>
    /// <param name="pos"></param>
    public void Text3D(string sort,Vector3 pos) 
    {
		alive_AI--;    /*实时监测ai数量*/
        //Debug.Log ("alive_AI-------------"+alive_AI);

        GameObject a = Instantiate(flytext);
        a.GetComponentInChildren<TextMesh>().text =sort;
        a.transform.position = pos;
        Vector3 postemp = Camera.main.transform.position;
        a.transform.LookAt(Camera.main.transform.position);
        a.transform.localEulerAngles = new Vector3(0,a.transform.localEulerAngles.y,0);
        a.transform.eulerAngles = new Vector3(0,a.transform.eulerAngles.y,0);
        //a.transform.Translate(Vector3.up);
        Destroy(a, 1f);
        Audiocontroller.instans.PlayEffectAudio("Explosion");/*敌人爆炸声音*/
    }


    float temptimer;
    bool isWarning = false;
    /// <summary>
    /// 警报
    /// </summary>
    public void Warning(string msg)
    {
        if (!isWarning)
        {
            topworming.text = msg;
            isWarning = true;
            StartCoroutine(DisWarning());
        }
        //temptimer = systime;
    }

    public IEnumerator DisWarning() 
    {
        int  i = 0;
        while (i < 4)
        {
            topworming.transform.parent.gameObject.SetActive(true);
            yield return new WaitForSeconds(1);
            topworming.transform.parent.gameObject.SetActive(false);
            yield return new WaitForSeconds(1);
            i++;
        }
        isWarning = false;
    }

    /// <summary>
    /// 立即销毁
    /// </summary>
    /// <param name="obj_data"></param>
    public virtual void DestroySenceObject(SenceGameObject obj_data)
    {
        Destroy(obj_data.gameObject);
    }

    /// <summary>
    /// 添加到发送列表
    /// </summary>
    /// <param name="obj_data"></param>
    public virtual void AddToSendList(SenceGameObject obj_data)
    {
        if (!SenceObjects.ContainsKey(obj_data.SenceObject.ID))
        {
            //if (obj_data.SenceObject.type == ObjectType.Weapon)
            //{
            //    Debug.Log("create Weapon");
            //}
            //if (obj_data.SenceObject.type == ObjectType.AI &&
            //    obj_data._pc_man.ai_type == AIType.LifeAndAttack &&
            //    obj_data.SenceObject.state == ObjectState.Destroy)
            //{
            //    Debug.LogError(">>>>AddToSendList : Boss.");
            //}
            SenceObjects.Add(obj_data.SenceObject.ID, obj_data);
        }
        else 
        {
            //if (obj_data.SenceObject.type == ObjectType.AI &&
            //    obj_data._pc_man.ai_type == AIType.LifeAndAttack &&
            //    obj_data.SenceObject.state == ObjectState.Destroy)
            //{
            //    Debug.LogError(">>>>AddToSendList is Lose : Boss.");
            //}
            //Debug.LogError("send to msg is exits! obj_name = " + obj_data.gameObject.name);
        }
    }

    /// <summary>
    /// 加载关卡数据
    /// </summary>
    public void LoadLevelData()
    {
        RoleLevelData = new Dictionary<int, Role>();
        TextAsset[] levels = Resources.LoadAll<TextAsset>("levels_role");
        XmlDocument xml;
        XmlElement xmlRoot;
        Role role;
        foreach (TextAsset txt in levels)
        {
            xml = new XmlDocument();
            xml.LoadXml(txt.ToString());
            xmlRoot = xml.DocumentElement;
            role = new Role(-1, xmlRoot["Role"].Attributes["Name"].Value,
                Vector3.zero, Quaternion.identity,
                int.Parse(xmlRoot["Role"].Attributes["HP"].Value),
                float.Parse(xmlRoot["Role"].Attributes["LaunchSpeed"].Value),
                int.Parse(xmlRoot["Role"].Attributes["BulletLimit"].Value),
                int.Parse(xmlRoot["Role"].Attributes["BulletCount"].Value),
                ObjectState.None,
                ObjectType.Player,
                int.Parse(xmlRoot["Role"].Attributes["Grade"].Value),
                new Weapon(WeaponData[xmlRoot["Role"].Attributes["useWeapon"].Value]),
                null,
                int.Parse(xmlRoot["Role"].Attributes["Attack"].Value),
                int.Parse(xmlRoot["Role"].Attributes["Gold"].Value),
                int.Parse(xmlRoot["Role"].Attributes["Index"].Value)
                );
            RoleLevelData.Add(int.Parse(xmlRoot["Role"].Attributes["Level"].Value), role);
        }
        AILevelData = new Dictionary<int, List<AI>>();
        levels = Resources.LoadAll<TextAsset>("levels_ai");
        //Debug.Log(levels);
        AI ai;
        List<AI> ls_ai;
        int level = -1;
        foreach (TextAsset txt in levels)
        {
            xml = new XmlDocument();
            xml.LoadXml(txt.ToString());
            //Debug.Log(txt.ToString());
            //Debug.Log("for:" + xml.InnerXml);
            xmlRoot = xml.DocumentElement;
            ls_ai = new List<AI>();
            level = int.Parse(xmlRoot["ai_list"].Attributes["Level"].Value);
            foreach (XmlNode node in xmlRoot["ai_list"].ChildNodes)
            {
                //Debug.Log(node.Attributes["Name"].Value);
                //Debug.Log(node.Attributes["HP"].Value);
                //Debug.Log(node.Attributes["LaunchSpeed"].Value);
                //Debug.Log(node.Attributes["Grade"].Value);
                //Debug.Log(node.Attributes["Weapon"].Value);
                //Debug.Log(node.Attributes["Attack"].Value);
                //Debug.Log(node.Attributes["next_create_time"].Value);
                //Debug.Log(node.Attributes["RewardWeapon"].Value);
                ai = new AI(-1, node.Attributes["Name"].Value, Vector3.zero, Quaternion.identity,
                    int.Parse(node.Attributes["HP"].Value),
                    float.Parse(node.Attributes["LaunchSpeed"].Value),
                    0,
                    0,
                    ObjectState.None, ObjectType.AI,
                    int.Parse(node.Attributes["Grade"].Value),
                    WeaponData[node.Attributes["Weapon"].Value],
                    null,
                    int.Parse(node.Attributes["Attack"].Value),
                    0,
                    float.Parse(node.Attributes["next_create_time"].Value),
                    0,
                    int.Parse(node.Attributes["Index"].Value),
                    node.Attributes["RewardWeapon"].Value
                    );

                ai.ai_type = (AIType)int.Parse(node.Attributes["type"].Value);
                ai.life_time = float.Parse(node.Attributes["life_time"].Value);
                ls_ai.Add(ai);
                //Debug.Log(ai.ai_type + ":" + ai.Name);
            }
            AILevelData.Add(level, ls_ai);
        }
    }

    public IEnumerator EndGame()
    {
        yield return new WaitForSeconds(3f);
        GameState = Datas.GameState.End;
    }

    //public virtual void InitNetwork()
    //{
    //    if (network_server == null)
    //    {
    //        network_server = gameObject.AddComponent<NetworkServer>();
    //        network_server.isScreening = true;
    //    }
    //    else 
    //    {
    //        Destroy(network_server);
    //        network_server = gameObject.AddComponent<NetworkServer>();
    //        network_server.isScreening = true;
    //    }
    //}


    public void ClearScene(Dictionary<int,SenceGameObject> sence_dic) 
    {
        List<SenceGameObject> sencelist = new List<SenceGameObject>(sence_dic.Values);
        for (int i = 0; i < sencelist.Count; i++)
        {
            if (sencelist[i].SenceObject.type != ObjectType.UI 
                && sencelist[i].SenceObject.type != ObjectType.Player
                && sencelist[i] != null)
            {
                //Debug.LogError("ClearScene:" + sencelist[i].gameObject.name);
              //  Destroy(sencelist[i].gameObject);
                sencelist[i].gameObject.SetActive(false);
            }
        }
    }



    /// <summary>
    /// 初始化场景关卡
    /// </summary>
    /// <param name="level"></param>
    public virtual void Init(int level)
    {
        //Debug.Log("Init");
        SenceObjects = new Dictionary<int, SenceGameObject>();
        if (RoleLevelData.ContainsKey(level))
        {
            //PlayerData;
            GameObject obj = Instantiate(RolePrefabs[RoleLevelData[level].Index]) as GameObject;
            SenceGameObject obj_data = obj.transform.FindChild("_player").gameObject.AddComponent<SenceGameObject>();
            RoleLevelData[level].Position = obj.transform.position;
            RoleLevelData[level].Rotation = obj.transform.rotation;
            obj_data.SenceObject = RoleLevelData[level];
            DefaultWeapon = obj_data._player.useWeapon.Name;
            obj_data.SenceObject.ID = GetID();
            _role = obj_data.SenceObject as Role;
            //SenceObjects.Add(obj_data.SenceObject.ID, obj_data);
            obj.transform.parent = this.transform;
            obj.name = "id_" + id_index + "_" + RoleLevelData[level].Name;
            obj_data.Player = obj.transform.FindChild("_player").gameObject;
            topworming = obj_data.Player.transform.FindChild("Canvas").FindChild("Image_info").GetComponentInChildren<TextMesh>();
            GradeTx = obj_data.Player.transform.FindChild("Grade").GetComponent<TextMesh>();
            PlayerHPText = obj_data.Player.transform.FindChild("PlayerHPText").GetComponent<TextMesh>();
            rightrange = obj_data.Player.transform.FindChild("RightRange").GetComponent<TextMesh>();
            leftrange = obj_data.Player.transform.FindChild("LeftRange").GetComponent<TextMesh>();

            obj_data.RightBattery = obj_data.Player.transform.FindChild("FireRR").gameObject.AddComponent<LookAt>();
            obj_data.RightBattery.Used = true;

            obj_data.LeftBattery = obj_data.Player.transform.FindChild("FireLL").gameObject.AddComponent<LookAt>();
            obj_data.LeftBattery.Used = true;


            HP_View hpview = obj.AddComponent<HP_View>();
            roleViews.Add(_role.ID, hpview);
            hpview.role = _role;
            hpview.Init();

            obj_data.LeftFirePoint = obj_data.LeftBattery.transform.FindChild("FirePoint").gameObject;
            obj_data.RightFirePoint = obj_data.RightBattery.transform.FindChild("FirePoint").gameObject;

            FirePointcontroller temp1 = obj_data.LeftBattery.transform.FindChild("FirePoint").gameObject.AddComponent<FirePointcontroller>();
            FirePointcontroller temp2 = obj_data.RightBattery.transform.FindChild("FirePoint").gameObject.AddComponent<FirePointcontroller>();
            temp1.Init(true,true);
            temp2.Init(true,true);
            
            obj_data.RightRein = obj.transform.FindChild("Controller (left)").gameObject;
            obj_data.RightRein.AddComponent<HandControl>();
            //obj_data.RightRein.transform.GetChild(0).GetComponent<SteamVR_RenderModel>().enabled = false;
          //  obj_data.RightRein.GetComponent<SteamVR_TrackedObject>().enabled = false;
            //obj_data.RightRein.GetComponent<HandControl>().enabled = false;
            obj_data.RightBattery.ReinObj = obj_data.RightRein.gameObject;
            obj_data.LeftRein = obj.transform.FindChild("Controller (right)").gameObject;
            obj_data.LeftRein.AddComponent<HandControl>();
            //obj_data.LeftRein.transform.GetChild(0).GetComponent<SteamVR_RenderModel>().enabled = false;
          // obj_data.LeftRein.GetComponent<SteamVR_TrackedObject>().enabled = false;
            //obj_data.LeftRein.GetComponent<HandControl>().enabled = false;
            obj_data.LeftBattery.ReinObj = obj_data.LeftRein.gameObject;
            obj.transform.FindChild("Camera (head)").GetComponent<SteamVR_GameView>().enabled = true;
            obj_data.LeftRein.SetActive(true);
            obj_data.RightRein.SetActive(true);
            obj_data.isSevice = true;
            SenceObjects.Add(_role.ID, obj_data);
            id_index++;
            PlayerData = obj_data;
        }
        StartCoroutine(CreateAI(level));
    }
    /// <summary>
    /// 根据客户端消息刷新玩家属性
    /// </summary>
    /// <param name="sence_object"></param>
    public void RefreshObject(RootObject sence_object)
    {
        if (IsServe)
        {       //非主角时，根据Id记录信息赋值
            Role role = sence_object as Role;
            if (sence_object.ID == player_data.SenceObject.ID)
            {
                player.Position = role.Position;
                player.Rotation = role.Rotation;
                player.LeftRein = role.LeftRein;
                player.RightRein = role.RightRein;
                player.CameraPosition = role.CameraPosition;
                player.CameraRotation = role.CameraRotation;
            }
        }
        else
        {
            if (roleViews.ContainsKey(sence_object.ID))
            {
                roleViews[sence_object.ID].role = sence_object as Role;
            }
            //Debug.Log(sence_object==null);
            //Debug.Log(PlayerData.SenceObject == null);
            if (SenceObjects.ContainsKey(sence_object.ID) && sence_object.ID != PlayerData.SenceObject.ID)
            {
            //    if (sence_object.type == ObjectType.AI)
            //    {
            //        AI ai = sence_object as AI;
            //        if (ai.ai_type == AIType.LifeAndAttack && ai.HP <= 0)
            //        {
            //            Debug.Log("<color=yellow>"+SenceObjects[sence_object.ID].name+"</color>");
            //            Destroy(SenceObjects[sence_object.ID].gameObject);
            //        }
            //    }

                SenceObjects[sence_object.ID].SenceObject = sence_object;
            }
            else if (sence_object.ID == PlayerData.SenceObject.ID)
            {
                Role role = (sence_object as Role);
               // 刷新主角血量和得分
                player.HP = role.HP;
                player.Grade = role.Grade;
                RefreshPlayerGrad(role.HP,role.Grade);
            //    Debug.Log("<color=yellow>RefirshObject>>>muthod</color>" + player.HP);
                ChangeWeapon(role.useWeapon);
                if (_last_weapon != role.useWeapon.Name)
                {
                    _last_weapon = role.useWeapon.Name;
                    Debug.Log("执行了？切换为" + role.useWeapon.Name + "武器");
                    Messenger.Broadcast<string>(GameEvent.ClientChangeWeapon, role.useWeapon.Name);
                   // Audiocontroller.instans.PlayBossAudio("huanwuqi");/*切换武器声音*/
                    Animatorcontroller.instance.PlayeMove();/*切换武器动画*/
                }
                //Debug.Log(role1.useWeapon.Name);
               
                //Debug.Log(sence_object.Name + "         senceobj_name_________________________________");
            }
        }
    }

    public void HideBullet() 
    {
        if (HasCachaLeft != null)
        {
            Debug.Log(HasCachaLeft.name);
            HasCachaLeft.SetActive(false);
        }
        if (HasCachaRight != null)
        {
            //Debug.Log(HasCachaRight.name);
            HasCachaRight.SetActive(false);
        }
    }


    /// <summary>
    /// 根据ai的type来设置抬头提示
    /// </summary>
    /// <param name="type"></param>

    public void ShowWarming(AIType type) 
    {
        //Debug.Log(type.ToString()+">>>>>>>>>>"+issmallenmycome.ToString());
        switch (type)
        {
            case AIType.Lithe:
                if (!issmallenmycome)
                {
                    Warning("敌机来袭！！！");
                    issmallenmycome = true;
                }
                break;
            case AIType.Attack:
                if (!issmallenmycome)
                {
                    Warning("敌机来袭！！！");
                    issmallenmycome = true; 
                }
                break;
            case AIType.LifeAndAttack:
                Warning("Boss来袭！！！");
                break;
            case AIType.SmartAI:
                if (!issmallenmycome)
                {
                    Warning("敌机来袭！！！");
                    issmallenmycome = true;
                }
                break;
            case AIType.Stone:
                 Warning("陨石来袭！！！");
                break;
            default:
                break;   
        }
    }



    /// <summary>
    /// 根据子弹数据生成子弹
    /// </summary>
    /// <param name="weaponName"></param>
    /// <param name="bullet"></param>
    /// <returns></returns>
    public SenceGameObject CreateBullet(Bullet bullet, bool isSevice, bool isRight = false)
    {
        GameObject obj;
        SenceGameObject res = null;
        if (Bullets.ContainsKey(bullet.Name))
        {
     //       Debug.Log(bullet.Name);
            if (bullet.Name.Equals("weapon2"))
            {
                if (isRight)
                {
                    if (HasCachaRight == null)
                    {
                        obj = Instantiate(Bullets[bullet.Name]) as GameObject;
                        // res = obj.AddComponent<SenceGameObject>();
                        HasCachaRight = obj;
                    }
                    else
                    {
                        obj = HasCachaRight;
                        //res
                    }
                }
                else
                {
                    if (HasCachaLeft == null)
                    {
                        obj = Instantiate(Bullets[bullet.Name]) as GameObject;

                        // res = obj.AddComponent<SenceGameObject>();
                        HasCachaLeft = obj;
                    }
                    else
                    {
                        obj = HasCachaLeft;
                    }
                }
            }
            else
            {
                obj = Instantiate(Bullets[bullet.Name]) as GameObject;
            }
            res = obj.GetComponent<SenceGameObject>();
            if (res == null)
            {
                res = obj.AddComponent<SenceGameObject>();
            }
            if (bullet.Name.Equals("weapon3"))
            {
                res.SenceObject = new GuidedMissile(GetID(), "GuidedMissile",
                    bullet.Attack,Vector3.zero,Quaternion.identity,-1);
                res.EnemyEffect = obj.transform.FindChild("Enemy Explode").gameObject;
            }
            else 
            {
                if (bullet.Name.Equals("weapon1"))
                {
                    Audiocontroller.instans.PlayerAudio("xieshizidan");
                }
                res.SenceObject = bullet;
            }
            
            res.isSevice = isSevice;
            if (isSevice)
            {
                
                bullet.ID = GetID();
                switch (bullet.type)
                {
                    case ObjectType.Bullet:
                        obj.tag = SenceGameObject.AI_BULLET_TAG;
                        obj.name = "ai_bullet_" + bullet.ID.ToString();
                        if (IsServe)
                        {
                            Debug.Log(obj.name);
                            obj.transform.position = bullet.Position;
                            obj.transform.rotation = bullet.Rotation;
                        }
                        SenceObjects.Add(bullet.ID, res);
                        break;
                    case ObjectType.PlayerBullet:
                        obj.name = "role_bullet_" + bullet.ID.ToString();
                        obj.tag = SenceGameObject.PLAYER_BULLET_TAG;
                        break;
                }
            }
        }
        else
        {
            Debug.LogError("err");
        }
        return res;
    }

    public SenceGameObject CreateBullet(Bullet bullet, int id, bool isRight = false)
    {
        GameObject obj;
        SenceGameObject res = null;
        if (Bullets.ContainsKey(bullet.Name))
        {
            //Debug.Log("ai发射子弹"+bullet.d);
            obj = Instantiate(Bullets[bullet.Name]) as GameObject;
            res = obj.GetComponent<SenceGameObject>();
            if (res == null)
            {
                res = obj.AddComponent<SenceGameObject>();
            }
            res.SenceObject = bullet;

            res.isSevice = true;
            obj.tag = SenceGameObject.AI_BULLET_TAG;
            res.SenceObject.Name = id.ToString() + "_" + res.SenceObject.Name;
            obj.transform.position = bullet.Position;
            obj.transform.rotation = bullet.Rotation;
            AddToSendList(res);
        }
        else
        {
            Debug.LogError("err");
        }
        return res;
    }

    public GameObject HasCachaRight = null;
    public GameObject HasCachaLeft = null;
    /// <summary>
    /// 创建AI
    /// </summary>
    /// <param name="level"></param>
    /// <returns></returns>
    private bool CanEndGame = false;
    private SenceGameObject boss = null; 
    public IEnumerator CreateAI(int level)
    {
        while (GameState != Datas.GameState.Playing)
        {
            yield return null;
            //Debug.Log("GameState.Start");
        }
        //GameState = Datas.GameState.Playing;
        if (AILevelData.ContainsKey(level))
        {
            List<AI> ls_ai = AILevelData[level];
            GameObject obj;
            GameObject fire_point;
            SenceGameObject senceObj;
            float f_level = 0;
            while (true)
            {
                for (int i = 0; i < ls_ai.Count; i++)
                {
					while (alive_AI > 2) 
					{
						yield return null;	
					}
					
                    if (GameState == Datas.GameState.Playing)
                    {
                        if (ls_ai[i].ai_type != AIType.LifeAndAttack ||
                            ls_ai[i].ai_type == AIType.LifeAndAttack &&
                            boss == null)
                        {
                            //Debug.Log(ls_ai[i].ai_type + ":" + ls_ai[i].Name);
                            if (ls_ai[i].ai_type != AIType.Stone)
                            {
                                obj = Instantiate(AiPrefabs[ls_ai[i].Index],
                                    UnityEngine.Random.insideUnitSphere * SenceGameObject.AI_PALYER_MAX_GAP + Vector3.zero,
                                    Quaternion.identity) as GameObject;
                                //Debug.Log("ls_ai[i].ai_type != AIType.Stone" + obj.name);
								alive_AI++;/*AI实时数量*/
                                //Debug.Log ("alive_AI++++++++++"+alive_AI);
                            }
                            else
                            {
                                obj = Instantiate(StonePrefabs[ls_ai[i].Index],
                                        UnityEngine.Random.insideUnitSphere * SenceGameObject.AI_PALYER_MAX_GAP + Vector3.zero,
                                        Quaternion.identity) as GameObject;
                            }

                            senceObj = obj.AddComponent<SenceGameObject>();
                            if (ls_ai[i].ai_type == AIType.LifeAndAttack)
                            {
                                boss = senceObj;
                            }
                            ls_ai[i].ID = GetID();
                            senceObj.SenceObject = AI.GetAI(ls_ai[i]);
                            senceObj._pc_man.ai_type = ls_ai[i].ai_type;
                            if (senceObj.transform.childCount > 0)
                            {
                                senceObj.EnemyEffect = obj.transform.FindChild("Enemy Explode").gameObject;
                                if (ls_ai[i].ai_type != AIType.Stone)
                                {
                                    //只为Boss修改查找子物体方式：
                                    if (ls_ai[i].ai_type == AIType.LifeAndAttack)
                                    {
                                        fire_point = obj.transform.FindChild("Enemy").FindChild("FirePoint").gameObject;
                                    }
                                    else {
                                        fire_point = obj.transform.FindChild("FirePoint").gameObject;
                                    }
                                    senceObj.LeftFlower = fire_point.transform.FindChild("FireFlower").FindChild("leftFlower").gameObject;
                                    senceObj.RightFlower = fire_point.transform.FindChild("FireFlower").FindChild("rightFlower").gameObject;
                                    senceObj.LeftFlower.GetComponent<Hide>().enabled = true;
                                    senceObj.RightFlower.GetComponent<Hide>().enabled = true;
                                    if (fire_point.transform.childCount > 1)
                                    {
                                        senceObj.LeftFirePoint = fire_point.transform.GetChild(0).gameObject;
                                        senceObj.RightFirePoint = fire_point.transform.GetChild(1).gameObject;
                                    }
                                    else if (fire_point.transform.childCount > 0)
                                    {
                                        senceObj.LeftFirePoint = fire_point.transform.GetChild(0).gameObject;
                                    }
                                    HP_View hpview = obj.AddComponent<HP_View>();
                                    hpview.role = senceObj._pc_man;
                                    hpview.Init();
                                    roleViews.Add(ls_ai[i].ID, hpview);
                                }
                            }

                            obj.transform.parent = this.transform;
                            obj.name = "id_" + id_index.ToString() + "_" + ls_ai[i].Name;
                            if (ls_ai[i].ai_type == AIType.Stone)
                            {
                                obj.tag = SenceGameObject.AI_STONE;

                            }
                            else
                            {
                                obj.tag = SenceGameObject.AI_TAG;
                            }
                            //ShowWarming(ls_ai[i].ai_type);

                            senceObj.InitStartPos();
//                            Debug.Log(obj.transform.position);
                            ls_ai[i].Position = obj.transform.position;
                            ls_ai[i].Rotation = obj.transform.rotation;
                            senceObj.isSevice = true;
                            /*引起客户端初始AI位置不准确*/
                          //  AddToSendList(senceObj);
                            obj.SetActive(true);
                            yield return new WaitForSeconds(ls_ai[i].next_create_time - f_level);
                        }

                    }

                }
                yield return null;
                //f_level += 0.1f;
            }
            
        }
        CanEndGame = true;
    }
}
