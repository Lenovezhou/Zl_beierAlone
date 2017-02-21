using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Datas;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using UnityEngine.UI;

public class Recever : MsgCenter {
    //public delegate void audio
    public static Recever rInstance;
    // 本地监听端口
    public string ListenHost;
    public int ListenPort;
    //存储主角色信息，以待发送
    public Dictionary<int, SenceGameObject> send_player;

    [HideInInspector]
    public GameObject hitParticle,firePointL,firePointR;

    public GameObject targetL,targetR;

    //开始连接服务器
    public bool isstartconnect=false;
    //动态添加脚本
    public PlayerController playerData;
   
    // 开枪
    public bool isfire;
    //分数和名字的数据
    public List<int> sort = new List<int>();
    public List<string> rankingname = new List<string>();

    // sorket 数据
    Dictionary<int, RootObject> sorketdic;
    Dictionary<int, RootObject> tempdic;

    Dictionary<int, RootObject> objct_dic = new Dictionary<int, RootObject>();

    private Ray ray;
    private RaycastHit hit;
    private Dictionary<int, GameObject> guideddestroy = new Dictionary<int, GameObject>();
    public override void OnAwake()
    {
        //Instance = this;
        //hitParticle = Instantiate(hit_particle_prefab)as GameObject;
        //hitParticle.SetActive(false);

        send_player = new Dictionary<int, SenceGameObject>();
        SenceObjects = new Dictionary<int, SenceGameObject>();
        rInstance = this;
        //base.OnAwake();
    }
   
	public override void Start () 
    {
        sorketdic = new Dictionary<int, RootObject>();
        //InitNetwork();
	}

    //public override void InitNetwork()
    //{
    //    network_server = gameObject.AddComponent<NetworkClient>();
    //    network_server.isScreening = false;
    //}

    public override void AddToSendList(SenceGameObject obj_data)
    {
        if (!send_player.ContainsKey(obj_data.SenceObject.ID))
        {
            send_player.Add(obj_data.SenceObject.ID, obj_data);
        }
    }

    int Aiattack_min = 0, Aiattack_more = 0, Aiattack_max = 0;
    string[] strs;
    int id;
    // 刷新场景内所有物体
    public void Refresh(List<RootObject> temp_dic)
    {
        //服务器里没有本地有
        foreach (RootObject value in temp_dic)
        {
            if (SenceObjects.ContainsKey(value.ID) &&
                value.state == ObjectState.Destroy)
            {
                //Debug.Log(SenceObjects.ContainsKey(temptest) + "::：value.contains过滤后"+value.Name + "：::value.name");

                //Ai销毁时，销毁对应的导弹
                if (value.type == ObjectType.AI && guideddestroy.ContainsKey(value.ID))
                {
                    Destroy(guideddestroy[value.ID],1f);
                }
                //不销毁Boss，只隐藏
                if (value.type == ObjectType.AI)
                {
                    AI ai = value as AI;
                    if (ai.ai_type == AIType.LifeAndAttack && SenceObjects.ContainsKey(value.ID) && SenceObjects[value.ID]!= null)
                    {
                        SenceObjects[value.ID].gameObject.SetActive(false);
                        Destroy(SenceObjects[value.ID].GetComponent<SenceGameObject>());
                    }
                    else
                    {
                        DestroySenceObject(SenceObjects[value.ID]);
                    }
                }
                else
                {
                    DestroySenceObject(SenceObjects[value.ID]);
                }
            }else if(value.type == ObjectType.AI && SenceObjects.ContainsKey(value.ID) &&
                value.state != ObjectState.Destroy)
            {
                
            }
            
        }

        foreach (RootObject value in temp_dic)
        {
            //本地包含服务器物体
            if (Recever.rInstance.SenceObjects.ContainsKey(value.ID) && value != null)
            {
                RefreshObject(value);
            }
            //本地不包含服务器物体
            else if (!roleViews.ContainsKey(value.ID) && value.state != ObjectState.Destroy)
            {
                GameObject obj ;
                Role player;
                AI ai;
                Bullet bullet;
                SenceGameObject obj_data;
                HP_View hpview;
                switch (value.type)
                {
                    case ObjectType.Player:
                        player = value as Role;
                     //Debug.Log("<color.red>"+player.Index+"</color>");
                        obj = Instantiate(RolePrefabs[player.Index]) as GameObject;
                        obj_data = obj.AddComponent<SenceGameObject>();
                        playerData = obj.AddComponent<PlayerController>();
                        playerData.prefabs = this.BulletPrefabs;
                        obj_data.Player = obj.transform.FindChild("_player").gameObject;
                        GradeTx = obj_data.Player.transform.FindChild("Grade").GetComponent<TextMesh>();
                        PlayerHPText = obj_data.Player.transform.FindChild("PlayerHPText").GetComponent<TextMesh>();
                        topworming = obj_data.Player.transform.FindChild("Canvas").FindChild("Image_info").GetComponentInChildren<TextMesh>();
                        rightrange = obj_data.Player.transform.FindChild("RightRange").GetComponent<TextMesh>();
                        leftrange = obj_data.Player.transform.FindChild("LeftRange").GetComponent<TextMesh>();
                        RefreshPlayerGrad(player.HP, player.Grade);
                        obj_data.RightBattery = obj_data.Player.transform.FindChild("FireRR").gameObject.AddComponent<LookAt>();
                        obj_data.LeftBattery = obj_data.Player.transform.FindChild("FireLL").gameObject.AddComponent<LookAt>();

                        //obj_data.RightBattery.transform.FindChild("PaoL").gameObject.AddComponent<Gunrotate>();
                        //obj_data.LeftBattery.transform.FindChild("PaoR").gameObject.AddComponent<Gunrotate>();

                        obj_data.LeftFirePoint = obj_data.LeftBattery.transform.FindChild("FirePoint").gameObject;
                        obj_data.RightFirePoint = obj_data.RightBattery.transform.FindChild("FirePoint").gameObject;

                        FirePointcontroller temp1 = obj_data.LeftBattery.transform.FindChild("FirePoint").gameObject.AddComponent<FirePointcontroller>();
                        FirePointcontroller temp2 = obj_data.RightBattery.transform.FindChild("FirePoint").gameObject.AddComponent<FirePointcontroller>();
                        temp1.Init(false,true);
                        temp2.Init(false,true);



                        obj_data.RightRein = obj.transform.FindChild("Controller (right)").gameObject;
                        obj_data.LeftRein = obj.transform.FindChild("Controller (left)").gameObject;
                        //obj_data.Camera = obj.transform.FindChild("Camera (head)").gameObject;
                        hpview = obj.AddComponent<HP_View>();
                        roleViews.Add(value.ID, hpview);
                        hpview.role = value as Role;
                      //  Debug.Log("<color.red>obj_data._weapon " + obj_data._weapon+"******************</color>");
                        hpview.Init();

                        obj_data.SenceObject = value;
                        DefaultWeapon = obj_data._player.useWeapon.Name;
                        PlayerData = obj_data;
                        SenceObjects.Add(value.ID, obj_data);
                        send_player.Add(value.ID, obj_data);
                        obj.transform.parent = this.transform;
                        obj.name = "id_" + value.ID.ToString() + "_" + value.Name;
                        //network_server.isScreening = false;
                        break;
                    case ObjectType.AI:
                        ai = value as AI;
                        //Debug.Log("<color=yellow>"+ai.Position+"</color>");
                        obj = null;
                        switch (ai.ai_type)
                        {
                            case AIType.Lithe:
                                obj = Instantiate(AiPrefabs[ai.Index], ai.Position, ai.Rotation) as GameObject;
                                break;
                            case AIType.Attack:
                                obj = Instantiate(AiPrefabs[ai.Index], ai.Position, ai.Rotation) as GameObject;
                                Aiattack_more = ai.Index;
                                break;
                            case AIType.LifeAndAttack:

                                if (Boss == null)
                                {
                                    obj = Instantiate(AiPrefabs[ai.Index], ai.Position, ai.Rotation) as GameObject;
                                }
                                else
                                {
                                    obj = Boss;
                                }
                                Aiattack_max = ai.Index;
                                //  初始化 AI只为大怪添加血条

                                hpview = obj.AddComponent<HP_View>();
                                roleViews.Add(value.ID, hpview);
                                hpview.role = value as AI;
                                hpview.Init();

                                break;
                            case AIType.SmartAI:
                                obj = Instantiate(AiPrefabs[ai.Index], ai.Position, ai.Rotation) as GameObject;
                                Aiattack_min = ai.Index;
                                break;
                            case AIType.Stone:
                                obj = Instantiate(StonePrefabs[ai.Index], ai.Position, ai.Rotation) as GameObject;
                                break;
                            default:
                                Debug.LogError("没有对应的aitype");
                                break;
                        }


                        obj.SetActive(true);
                        obj_data = obj.AddComponent<SenceGameObject>();
                        
                        obj_data.EnemyEffect = obj.transform.FindChild("Enemy Explode").gameObject;
 
                        obj_data.gameObject.tag = SenceGameObject.AI_TAG;
                        if ( ai.ai_type != AIType.Stone)
                        {
                            obj.AddComponent<Follo>();
                            obj.GetComponent<BoxCollider>().size = new Vector3(5f,2.5f,5f);
                        }
                       
                        obj_data.SenceObject = value;
                        SenceObjects.Add(value.ID, obj_data);
                        obj.transform.parent = this.transform;
                        obj.name = "id_" + value.ID.ToString() + "_" + value.Name;
                        
                        break;
                    case ObjectType.Bullet:
                        bullet = value as Bullet;
                        strs = bullet.Name.Split('_');
                        id = int.Parse(strs[0]);
                        bullet.Name = strs[1];
                        obj_data = CreateBullet(bullet,false);
                        //Debug.Log(SenceObjects[id]._pc_man.Index+">>>>>"+Aiattack_min+">>>>>"+Aiattack_more+">>>>>>"+Aiattack_max);
                        /*生成敌机子弹时播放音效*/
                        if (SenceObjects.ContainsKey(id) && SenceObjects[id]._pc_man.Index == Aiattack_more)
                        {
                      //      Audiocontroller.instans.PlayGameAudio("Ss01");
                        }
                        if (SenceObjects.ContainsKey(id) && SenceObjects[id]._pc_man.Index == Aiattack_max)
                        {
                      //      Audiocontroller.instans.PlayGameAudio("kehuan");
                        }
                        if (SenceObjects.ContainsKey(id) && SenceObjects[id]._pc_man.Index == Aiattack_min)
                        {
                       //     Audiocontroller.instans.PlayGameAudio("Ss02");
                        }

                       
                        if (SenceObjects.ContainsKey(id) )
                        {
                            //SenceObjects[id].transform.FindChild("Enemy").FindChild("FireFlower").gameObject.SetActive(true);
                            if (SenceObjects[id]._pc_man.ai_type != AIType.LifeAndAttack) 
                            {
                                if (SenceObjects[id].GetComponentInChildren<LookAtPlayer>())
                                {
                                    obj_data.gameObject.transform.position = SenceObjects[id].GetComponentInChildren<LookAtPlayer>().gameObject.transform.position;
                                }
                                else {
                                    Debug.LogError("没有找到lookatplayer");
                                    obj_data.gameObject.transform.position = SenceObjects[id].transform.position;
                                }
                            }
                            else
                            {
                                Debug.LogError("生成子弹时找不到对应的发射点");
                                obj_data.gameObject.transform.position = value.Position;
                            }

                            //控制开火特效
                            if (SenceObjects[id] != null && SenceObjects[id].GetComponentInChildren<LookAtPlayer>() && SenceObjects[id]._pc_man.ai_type != AIType.Stone)
                            {
                                //Debug.Log("<color=yellow>"+SenceObjects[id].name+"</color>");
                                SenceObjects[id].GetComponentInChildren<LookAtPlayer>().isFire = true;
                            }
                            else
                            {
                                Debug.LogError("没有找到；LookAtPlayer;");
                            }
                        }
                        else
                        {
                            Debug.LogError("生成子弹时找不到对应的发射点");
                            obj_data.gameObject.transform.position = value.Position;
                        }

                        obj_data.gameObject.transform.rotation = value.Rotation;
                        ray = new Ray(obj_data.transform.position,obj_data.transform.forward);
                        if(Physics.Raycast(ray,out hit) &&
                            hit.transform.gameObject.CompareTag(SenceGameObject.PLAYER_TAG)
                            && !HurtPoints.ContainsKey(obj_data.SenceObject.ID))
                        {
                            HurtPoints.Add( obj_data.SenceObject.ID,hit.point);
                        }
                        obj_data.gameObject.GetComponent<Rigidbody>().AddForce(obj_data.transform.forward * MsgCenter.LUACH_FORCE*2);
                        obj_data.gameObject.tag = SenceGameObject.AI_BULLET_TAG;
                        Destroy(obj_data.gameObject,10f);
                        //SenceObjects.Add(value.ID, obj_data);
                        break;
                    case ObjectType.PlayerBullet:
                     
                        //bullet = value as Bullet;
                        //obj_data = CreateBullet(bullet,false);
                        //obj_data.gameObject.transform.position = value.Position;
                        //obj_data.gameObject.transform.rotation = value.Rotation;
                        //SenceObjects.Add(value.ID, obj_data);
                        break;
                    case ObjectType.Weapon:
                     //   if (value.state != ObjectState.Destroy)
                        {
                            string temppath = dropOut.ReadXML(value.Name);
                            GameObject tempobj = Resources.Load(temppath) as GameObject;
                           // Debug.Log("<color=yellow>生成掉；落物" + value.Name +value.Position+ "</color>");

                            GameObject OBJ = Instantiate(tempobj, value.Position, value.Rotation) as GameObject;
                            OBJ.AddComponent<BOX_co>();
                            BoxCollider box = OBJ.GetComponent<BoxCollider>();
                            box.center = Vector3.zero;
                            box.size = Vector3.one;
                            SenceGameObject objdate = OBJ.AddComponent<SenceGameObject>();
                            objdate.SenceObject = value;
                            SenceObjects.Add(value.ID, objdate);
                            //OBJ.transform.position = value.Position;
                            //OBJ.transform.rotation = value.Rotation;
                            OBJ.SetActive(true);
                        }
                        
                        break;
                    case ObjectType.UI:
                        UIMenu ui = value as UIMenu;
                        SenceGameObject mune_data = Recever.rInstance.DisplayMenu(ui.IsStart);
                        if (ui.IsStart)
                        {
                            GameState = Datas.GameState.Start;
                        }
                        else {
                            GameState = Datas.GameState.End;
                        }
                        mune_data.SenceObject = ui;
                        SenceObjects.Add(mune_data.SenceObject.ID, mune_data);
                        if (ui.PageTab != null && sort.Count <= 10)
                        {
                            sort.Add(ui.PageTab);
                        }
                        else
                        {
                            Debug.Log("<color=red>分数数组长度"+sort.Count+"</color>");
                        }
                        if (ui.Name != null && rankingname.Count <= 10)
                        {
                            rankingname.Add(ui.Name);
                        }
                        else
                        {
                            Debug.Log("<color=red>名字数组长度" + rankingname.Count + "</color>");
                        }
                        break;
                    case ObjectType.Ranking:
                        ranking = value as Ranking;
                        obj = new GameObject();
                        obj_data = obj.AddComponent<SenceGameObject>();
                        obj_data.SenceObject = ranking;
                        SenceObjects.Add(ranking.ID, obj_data);
                        break;
                    case ObjectType.GuidedMissile:
                        
                        GuidedMissile gu = value as GuidedMissile;
                        Debug.Log(gu.type);
                        GameObject guided = Instantiate(Bullets["weapon3"]);
                       // guided.AddComponent<Collider_items>();
                        obj_data = guided.AddComponent<SenceGameObject>();
                        obj_data.SenceObject = value;
                        guided.tag = SenceGameObject.PLAYER_BULLET_TAG;
                        //if(gu.Name == "right")
                        //{
                        //    guided.GetComponent<Rigidbody>().velocity = PlayerData.RightRein.transform.forward;
                        //}else{
                        //    guided.GetComponent<Rigidbody>().velocity = PlayerData.LeftRein.transform.forward;    
                        //}
                        guided.transform.position = gu.Position;
                        Guided guid = guided.AddComponent<Guided>();
                        if (gu.target_id == null || 
                            !SenceObjects.ContainsKey(gu.target_id))
                        {
                            guid.Target = null;
                        }
                        else 
                        {
                            //导弹目标为掉落物时没有爆炸效果
                            if (SenceObjects[gu.target_id].type != ObjectType.Weapon)
                            {
                                obj_data.EnemyEffect = guided.transform.FindChild("Enemy Explode").gameObject;
                            }

                            //设置导弹的目标，及与目标对应，一起消失
                            guid.Target = SenceObjects[gu.target_id].transform;
                            if (!guideddestroy.ContainsKey(gu.target_id))
                            {
                                guideddestroy.Add(gu.target_id, guided);
                            }
                        }
                        SenceObjects.Add(gu.ID, obj_data);
                        break;
                }
            }
        }
    }




    public override void DestroySenceObject(SenceGameObject obj_data)
    {
        SenceObjects.Remove(obj_data.SenceObject.ID);
        if (obj_data != null)
        Destroy(obj_data.gameObject);
    }


    public GameObject CreateBullet(GameObject prefab,Transform fier_point,bool isdestroy) 
    {
        GameObject temp = null;
        if (BulletPool.Count > 0)
        {
            temp = BulletPool[0];
            BulletPool.Remove(temp);
            temp.SetActive(true);
        }
        else
        {
            temp = Instantiate(prefab);
        }
        temp.transform.position = fier_point.position;
        temp.transform.rotation = fier_point.rotation;

        temp.tag = SenceGameObject.PLAYER_BULLET_TAG;

        Collider_items co= temp.AddComponent<Collider_items>();
        co.recever = this;
        if (temp.GetComponent<Rigidbody>())
        {
            Rigidbody rb = temp.GetComponent<Rigidbody>();
            rb.useGravity = false;
            rb.isKinematic = false;
            rb.AddForce(fier_point.forward * LUACH_FORCE*2);
        }
        if (isdestroy)
        {
        //    Destroy(temp, 5f);
        }
        return temp;
    }




    public override void ReadConfig(NetConfig config)
    {
        //testtext.text = config.ClientHost + config.ClientPort;
        base.ReadConfig(config);
        ListenHost = config.ClientHost;
        ListenPort = int.Parse(config.ClientPort);
    }
    void _Refreshranger()
    {
        _rightend = UnityEngine.Random.Range(10.2f, 100.5f);
        _leftend = UnityEngine.Random.Range(100.2f, 1000.5f);
    }
    float _leftend, _rightend;
    public override void Update() 
    {
        
        //刷新左右随机数
        if (leftrange != null && rightrange != null)
        {
            if (_leftend-float.Parse(leftrange.text)  <= 0.1 && -float.Parse(rightrange.text) + _rightend <= 0.1)
            {
                _Refreshranger();
            }
            else
            {
                leftrange.text = Mathf.Lerp(float.Parse(leftrange.text), _leftend, 0.2f).ToString("f2") ;
                rightrange.text = Mathf.Lerp(float.Parse(rightrange.text), _rightend, 0.2f).ToString("f2");
            }
        }
        
        if (PlayerData!=null && isstartconnect)
        {

            Getfire(Bullets[player.useWeapon.Name], left_key, right_key);
           
        }
    }
    float timerL, timerR;
    bool ishaveright = false;
    bool ishabeleft = false;
    GameObject parent2 = null;
    GameObject tempL = null;
    GameObject tempR = null;
    public void Getfire(GameObject prefab, bool leftkey, bool rightkey)
    {
       // Debug.Log("<color=yellow> LaunchSpeed = " + player.LaunchSpeed + "</color>");
        if (player.useWeapon.Name.Equals("weapon1"))
        {
            if (tempL)
            {
                tempL.SetActive(false);
            }
            if (tempR)
            {
                tempR.SetActive(false);
            }
            timerL += Time.deltaTime;
            timerR += Time.deltaTime;
            if (left_key || right_key)
            {
                
                if (rightkey&&timerR >= player.LaunchSpeed)
                {
                    timerR = 0f;
                    GameObject parent = player_data.RightBattery.gameObject;
                    GameObject temp = CreateBullet(prefab, parent.transform.FindChild("FirePoint").transform,true);
                //    Audiocontroller.instans.ChangeSounds(7);
                }
                if (leftkey && timerL >= player.LaunchSpeed)
                {
                    timerL = 0f;
                    GameObject parent = player_data.LeftBattery.gameObject;
                    GameObject temp = CreateBullet(prefab, parent.transform.FindChild("FirePoint").transform,true);
                 //   Audiocontroller.instans.ChangeSounds(7);

                }
            
            }
        }
        else if (player.useWeapon.Name.Equals("weapon2"))
        {
          //  Debug.Log("weapon2Right_key"+right_key+"Left_key"+left_key);
            if (right_key)
            {
                if (ishaveright)
                {

                    tempR.SetActive(true);
                }
                else 
                {
           //         Debug.Log(ishaveright + ">>>>>>ishaverightishaveright");
                    parent2 = player_data.RightBattery.gameObject;
                    tempR = Instantiate(prefab);
                    tempR.transform.position = parent2.transform.FindChild("FirePoint").transform.position;
                    tempR.transform.rotation = parent2.transform.FindChild("FirePoint").transform.rotation;
                    tempR.transform.SetParent(parent2.transform.FindChild("FirePoint").transform);
                    tempR.tag = SenceGameObject.PLAYER_BULLET_TAG;
                    ishaveright = true;
                }
                
            }
            else if (tempR != null)
            {
                tempR.SetActive(false);
            }
      //      Debug.Log(left_key + " -> left_key");
       //     Debug.Log(right_key + " -> right_key");
            if (left_key)
            {

                if (ishabeleft)
                {
                    tempL.SetActive(true);
                }
                else 
                {
                    Debug.Log(ishabeleft + ">>>>>>ishabeleft");
                    parent2 = player_data.LeftBattery.gameObject;
                    tempL = Instantiate(prefab);
                    tempL.transform.position = parent2.transform.FindChild("FirePoint").transform.position;
                    tempL.transform.rotation = parent2.transform.FindChild("FirePoint").transform.rotation;
                    tempL.transform.SetParent(parent2.transform.FindChild("FirePoint").transform);
                    Debug.Log(parent2.transform.FindChild("FirePoint") + "parent.transform.FindChild(FirePoint)" + left_key);
                    tempL.tag = SenceGameObject.PLAYER_BULLET_TAG;
                    ishabeleft = true;
                }
                
            }
            else if (tempL != null)
            {
                tempL.SetActive(false);
            }
        }
        else if (player.useWeapon.Name.Equals("weapon3"))
        {
            if (tempL)
            {
                tempL.SetActive(false);
            }
            if (tempR)
            {
                tempR.SetActive(false);
            }
        }
        //第三种武器的子弹

       
    }
}
