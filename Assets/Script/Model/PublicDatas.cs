using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Text;

namespace Datas
{
    public enum ObjectState
    {
        None,
        Unused,
        Hurt,
        Destroy
    }

    public enum ObjectType 
    {
        Player,  // 玩家
        AI,   //  
        Bullet,   //  电脑的子弹
        PlayerBullet,  // 玩家的子弹
        Weapon,
        UI,
        Ranking,
        GuidedMissile   //  导弹
    }

    public enum AIType 
    {
        Lithe,   //  轻盈型
        Attack,  //  高攻击型
        LifeAndAttack,    //  高攻击力、高生命型
        SmartAI,
        Stone
    }

    public enum GameState 
    {
        None,
        Start,
        Playing,
        End,
        Exit
    }

    public enum MenuState
    {
        None,   //  无状态
        CreatePlayer,  //  创建角色名字
        Display,   //  查看教程 或 正常显示
        StartGame,  //  开始游戏
        Continue,   //  继续游戏
        ExitGame    //  结束游戏
    }

    public class Connection
    {
        public string Host;
        public int Port;
        public IPEndPoint ip;
        public Connection(string p_Host, int p_Port) 
        {
            Host = p_Host;
            Port = p_Port;
            ip = new IPEndPoint(IPAddress.Parse(Host), Port);
        }
    }

    public class Vector4 
    {
        public float x;
        public float y;
        public float z;
        public float w;

        [JsonConstructor]
        public Vector4(float p_x,float p_y,float p_z,float p_w) 
        {
            x = p_x;
            y = p_y;
            z = p_z;
            w = p_w;
        }

        public Vector4(float p_x, float p_y, float p_z)
        {
            x = p_x;
            y = p_y;
            z = p_z;
        }
    }

    public class RootObject 
    {
        //  物体的ID
        public int ID;
        //  物体名称
        public string Name;
        //  位置
        public Vector4 v4_position;
        public Vector4 position 
        {
            set 
            {
                v4_position = value;
                if (v4_position != null)
                {
                    _position = new Vector3(v4_position.x, v4_position.y, v4_position.z);
                }
                
            }
            get 
            {
                return v4_position;
            }
        }
        
        //  物体状态
        public ObjectState state;
        //  物体的类型
        public ObjectType type;

        //  物体能够造成的伤害
        public int Attack;
        [JsonIgnore]
        private Vector3 _position;
        [JsonIgnore]
        public Vector3 Position 
        {
            set 
            {
                _position = value;
                if(_position != Vector3.zero)
                {
                    position = new Vector4(_position.x, _position.y, _position.z, 0);
                }
            }
            get 
            {
                return _position;
            }
        }

        //  旋转角
        private Vector4 v4_rotation;
        public Vector4 rotation 
        {
            set 
            {
                v4_rotation = value;
                if (value != null)
                    _rotation = new Quaternion(v4_rotation.x, v4_rotation.y, v4_rotation.z, v4_rotation.w);
                else
                {
                }
            }
            get 
            {
                return v4_rotation;
            }
        }
        [JsonIgnore]
        private Quaternion _rotation;
        [JsonIgnore]
        public Quaternion Rotation 
        {
            set 
            {
                _rotation = value;

                if (_rotation != null && _rotation != Quaternion.identity)
                {
                    //Debug.Log(_rotation.x + "," + _rotation.y + "," + _rotation.z);
                    rotation = new Vector4(_rotation.x, _rotation.y, _rotation.z, _rotation.w);
                }
                else 
                {
                    rotation = new Vector4(0f,0f,0f,1f);
                }
            }
            get 
            {
                return _rotation;
            }
        }

        public RootObject(int p_ID, string p_Name, Vector3 position,int p_attack,
            Quaternion rotation, ObjectState p_state, ObjectType p_type) 
        {
            ID = p_ID;
            Name = p_Name;
            Position = position;
            Rotation = rotation;
            state = p_state;
            type = p_type;
            Attack = p_attack;
        }

        public RootObject(int p_ID, string p_Name,int p_attack, ObjectState p_state, ObjectType p_type) 
        {
            ID = p_ID;
            Name = p_Name;
            Attack = p_attack;
            state = p_state;
            type = p_type;
        }

        [JsonConstructor]
        public RootObject(int p_ID, string p_Name,Vector4 p_position,Vector4 p_rotation, 
            ObjectState p_state, ObjectType p_type,
            int p_attack)
        {
            ID = p_ID;
            Name = p_Name;
            Attack = p_attack;
            state = p_state;
            type = p_type;
            if(p_position != null)
            {
                Position = new Vector3(p_position.x, p_position.y, p_position.z);
            }
            if(p_rotation != null)
            {
                Rotation = new Quaternion(p_rotation.x, p_rotation.y, p_rotation.z, p_rotation.w);
            }
            
        }
    }

    public class Role : RootObject 
    {
        // 血量
        public int HP
        {
            set 
            {
                hp = value;
                //Debug.Log("value[name]="+ Name + "  [value] = " + value);
            }
            get { return hp; }
        }
        private int hp;
        
        
        // 子弹发射速度(颗/秒)
        public float LaunchSpeed;
        
        //  子弹夹上限
        public int BulletLimit;
        //  当前拥有的子弹数
        public int BulletCount;

        //  消灭敌人数，即分数
        public int Grade;

        //  角色当前使用的武器
        public Weapon useWeapon;

        //  角色拥有的金币数
        public int Gold;

        //  玩家在预设列表中的索引
        public int Index = -1;

        //  手柄信息
        public Rein LeftRein;
        public Rein RightRein;
        

        private Vector4 _camera_position;
        public Vector4 camera_position
        {
            set
            {
                _camera_position = value;
                if (_camera_position != null)
                {
                    cameraPosition = new Vector3(_camera_position.x, _camera_position.y, _camera_position.z);
                }
            }
            get
            {
                return _camera_position;
            }
        }

        private Vector3 cameraPosition;
        [JsonIgnore]
        public Vector3 CameraPosition
        {
            set
            {
                cameraPosition = value;
                _camera_position = new Vector4(cameraPosition.x, cameraPosition.y, cameraPosition.z, 0);
            }
            get
            {
                return cameraPosition;
            }
        }

        private Vector4 _camera_rotation;
        public Vector4 camera_rotation
        {
            set
            {
                _camera_rotation = value;
                if (_camera_rotation != null)
                {
                    cameraRotation = new Vector3(_camera_rotation.x, _camera_rotation.y, _camera_rotation.z);
                }
            }
            get
            {
                return _camera_rotation;
            }
        }

        private Vector3 cameraRotation;
        [JsonIgnore]
        public Vector3 CameraRotation
        {
            set
            {
                cameraRotation = value;
                _camera_rotation = new Vector4(cameraRotation.x, cameraRotation.y, cameraRotation.z);
               // Debug.Log(cameraRotation);
            }
            get
            {
                return cameraRotation;
            }
        }

        /// <summary>
        /// 玩家初始化
        /// </summary>
        /// <param name="p_Name">玩家名字</param>
        /// <param name="p_Position">实例玩家地点</param>
        /// <param name="p_Rotation">实例玩家角度</param>
        /// <param name="p_HP">玩家初始最大血量</param>
        /// <param name="p_LaunchSpeed">子弹速度</param>
        /// <param name="p_BulletLimit">子弹数量</param>
        /// <param name="p_BulletCount">单价数量</param>

        public Role(int id,string p_Name, Vector3 p_Position, Quaternion p_Rotation,
            int p_HP, float p_LaunchSpeed, int p_BulletLimit, int p_BulletCount, ObjectState p_State,ObjectType p_type,
            int p_Grade, Weapon p_Weapon, object weapon_obj, int p_Attack, int p_Gold,int p_index)
            : base(id, p_Name, p_Position, p_Attack,p_Rotation, p_State, p_type)
        {
            HP = p_HP;
            LaunchSpeed = p_LaunchSpeed;
            BulletCount = p_BulletCount;
            BulletLimit = p_BulletLimit;
            Grade = p_Grade;
            useWeapon = p_Weapon;
            Gold = p_Gold;
            Index = p_index;
        }

        [JsonConstructor]
        public Role(int p_HP, float p_LaunchSpeed, int p_BulletLimit, int p_BulletCount,
            int p_Grade, Weapon p_Weapon, int p_Gold, int p_index, int p_id, string p_Name, 
            Vector4 p_position, Vector4 p_rotation,
             ObjectState p_State, ObjectType p_type, int p_Attack, Rein p_leftRein, Rein p_rightRein,
            Vector4 p_camera_position,Vector4 p_camera_rotation)
            : base(p_id, p_Name, p_position, p_rotation,p_State,p_type, p_Attack)
        {
            HP = p_HP;
            LaunchSpeed = p_LaunchSpeed;
            BulletCount = p_BulletCount;
            BulletLimit = p_BulletLimit;
            Grade = p_Grade;
            useWeapon = p_Weapon;
            Gold = p_Gold;
            Index = p_index;
            LeftRein = p_leftRein;
            RightRein = p_rightRein;
            camera_position = p_camera_position;
            camera_rotation = p_camera_rotation;
        }
    }

    public class AI:Role
    {
        public float next_create_time = -1;
        public float launch_offset;
        public string RewardWeapon;
        public AIType ai_type;
        public float life_time;
        public AI(int id, string p_Name, Vector3 p_Position, Quaternion p_Rotation,
            int p_HP, float p_LaunchSpeed, int p_BulletLimit, int p_BulletCount, ObjectState p_State, ObjectType p_type,
            int p_Grade, Weapon p_Weapon, object weapon_obj, int p_Attack, int p_Gold, float p_next_create_time,
            float p_launch_offset, int p_Index ,string p_RewardsWeapon)
            :base(id,p_Name,p_Position,p_Rotation,p_HP,p_LaunchSpeed,p_BulletLimit,p_BulletCount,
            p_State, p_type, p_Grade, p_Weapon, weapon_obj, p_Attack, p_Gold, p_Index)
        {
            next_create_time = p_next_create_time;
            launch_offset = p_launch_offset;
            RewardWeapon = p_RewardsWeapon;
        }
        [JsonConstructor]
        public AI(float p_next_create_time, float p_launch_offset,string p_RewardsWeapon,
            int p_HP, float p_LaunchSpeed, int p_BulletLimit, int p_BulletCount,
            int p_Grade, Weapon p_Weapon, int p_Gold, 
             int p_Index,int p_id, string p_Name , Vector4 p_position,
            Vector4 p_rotation,ObjectState p_State, ObjectType p_type, int p_Attack)
            : base(p_HP,p_LaunchSpeed,p_BulletLimit,p_BulletCount,p_Grade,
            p_Weapon, p_Gold, p_Index, p_id, p_Name, p_position,p_rotation,  p_State, p_type, p_Attack,null,null,
            null,null)
        {
            next_create_time = p_next_create_time;
            launch_offset = p_launch_offset;
            RewardWeapon = p_RewardsWeapon;
        }

        public static AI GetAI(AI ai) 
        {
            return new AI(ai.ID,ai.Name,ai.Position,ai.Rotation,ai.HP,ai.LaunchSpeed,ai.BulletLimit,
                ai.BulletCount,ai.state,ai.type,ai.Grade,ai.useWeapon,null,ai.Attack,ai.Gold,ai.next_create_time,
                ai.launch_offset,ai.Index,ai.RewardWeapon);
        }
    }

    public class Bullet : RootObject 
    {
        public bool IsRight;

        public Bullet(int p_ID, string p_Name, int p_Attack,Vector3 position,
            Quaternion rotation, ObjectState p_state, ObjectType p_type,bool isRight)
            : base(p_ID, p_Name, position,p_Attack, rotation, p_state, p_type)
        {
            IsRight = isRight;
        }

        [JsonConstructor]
        public Bullet(int p_ID, string p_Name, int p_Attack, 
            Vector4 p_position,Vector4 p_rotation, 
            ObjectState p_state, ObjectType p_type,bool isRight)
            : base(p_ID, p_Name, p_position,p_rotation,p_state,p_type, p_Attack)
        {
            IsRight = isRight;
        }
    }

    public class Prop:RootObject
    {
        //  武器的生命值
        public int HP;
        //  发射速度
        public float LaunchSpeed;

        public Prop(int p_id, string p_Name, int p_HP, int p_Attack, float p_luach_speed,
            Vector3 position,Quaternion rotation,
            ObjectState p_state,ObjectType type):
            base(p_id, p_Name, position,p_Attack,rotation,ObjectState.None,ObjectType.Weapon)
        {
            HP = p_HP;
            LaunchSpeed = p_luach_speed;
        }

        [JsonConstructor]
        public Prop(int p_id, string p_Name, int p_HP, int p_Attack, float p_luach_speed,
            Vector4 position, Vector4 rotation,
            ObjectState p_state, ObjectType type) :
            base(p_id,p_Name,position, rotation, p_state, type,p_Attack)
        {
            HP = p_HP;
            LaunchSpeed = p_luach_speed;
        }
    }

    public class Weapon
    {
        public string Name;

        public int Attack;
        //  武器添加的生命值
        public int HP;
        //  发射速度
        public float LaunchSpeed;

        public float LifeTime;
        [JsonConstructor]
        public Weapon(string p_Name, int p_HP, int p_Attack, float p_luach_speed,float life_tiem)
        {
            LifeTime = life_tiem;
            Name = p_Name;
            Attack = p_Attack;
            HP = p_HP;
            LaunchSpeed = p_luach_speed;
        }

        public Weapon(Weapon weapon)
        {
            Name = weapon.Name;
            Attack = weapon.Attack;
            HP = weapon.HP;
            LaunchSpeed = weapon.LaunchSpeed;
            LifeTime = weapon.LifeTime;
        }
    }

    /// <summary>
    /// 手柄信息
    /// </summary>
    public class Rein
    {
        public bool Key;
        //  位置
        private Vector4 v4_position;
        public Vector4 position 
        {
            set
            {
                v4_position = value;
                if (v4_position != null)
                {
                    _position = new Vector3(v4_position.x, v4_position.y, v4_position.z);
                }
                else
                {
                    //Debug.LogError("v4_position == null");
                }
            }
            get 
            {
                return v4_position;
            }
        }
        [JsonIgnore]
        private Vector3 _position;
        [JsonIgnore]
        public Vector3 Position
        {
            set
            {
                _position = value;
                position = new Vector4(_position.x,_position.y,_position.z,0);
            }
            get
            {
                return _position;
            }
        }
        //  旋转角
        private Vector4 v4_rotation;
        public Vector4 rotation 
        {
            set 
            {
                v4_rotation = value;
                if (v4_rotation != null)
                {
                    _rotation = new Quaternion(v4_rotation.x, v4_rotation.y, v4_rotation.z, v4_rotation.w);
                }
                else 
                {
                    //Debug.LogError("v4_rotation == null");
                }
            }
            get 
            {
                return v4_rotation;
            }
        }

        [JsonIgnore]
        private Quaternion _rotation;
        [JsonIgnore]
        public Quaternion Rotation
        {
            set
            {
                _rotation = value;
                rotation = new Vector4(_rotation.x,_rotation.y,_rotation.z,_rotation.w);
            }
            get
            {
                return _rotation;
            }
        }

        public Rein(Vector3 p_position,Quaternion p_rotation,bool p_key) 
        {
            Position = p_position;
            Rotation = p_rotation;
            Key = p_key;
        }

        [JsonConstructor]
        public Rein(Vector4 p_position,Vector4 p_rotation,bool p_key) 
        {
            position = p_position;
            rotation = p_rotation;
            Key = p_key;
        }
    }

    public class UIMenu : RootObject
    {
        public int PageTab;
        public bool IsStart;

        [JsonConstructor]
        public UIMenu(int p_ID, string p_Name, int p_PageTab,ObjectType type,bool isStart)
            : base(p_ID, p_Name, null, null, ObjectState.None, type, 0)
        {
            PageTab = p_PageTab;
            IsStart = isStart;
        }
    }

    public class Person 
    {
        public string Name;
        public int Score;
        [JsonConstructor]
        public Person(string p_Name, int p_Score) 
        {
            Name = p_Name;
            Score = p_Score;
        }
    }

    public class NetConfig 
    {
        public string ServerHost;
        public string ServerPort;
        public string ClientHost;
        public string ClientPort;
        [JsonConstructor]
        public NetConfig(string pServerHost, string pServerPort, string pClientHost, string pClientPort) 
        {
            ServerHost = pServerHost;
            ServerPort = pServerPort;
            ClientHost = pClientHost;
            ClientPort = pClientPort;
        }

        public NetConfig() { }
    }

    public class Ranking : RootObject
    {
        public const int RANK_COUNT = 10;
        public int Count;
        public Person MinScore;   //  最低分

        //[JsonIgnore]
        public List<Person> RankPersonList;

        [JsonConstructor]
        public Ranking(int p_id, string p_name, List<Person> p_RankPerson) :
            base(p_id,p_name,0,ObjectState.None,ObjectType.Ranking)
        {
            //RankPerson = p_RankPerson;
            RankPersonList = p_RankPerson;
            
            //Debug.Log("3.RankPersonList.count = " + RankPersonList.Count);
        }

        public Ranking(int p_id, string p_name)
            : base(p_id, p_name, 0, ObjectState.None, ObjectType.Ranking) 
        {
            //RankPerson = new Person[RANK_COUNT];
            RankPersonList = new List<Person>();
        }

        public static Ranking LoadFrom(string file_name,int id)
        {
            Ranking res = null;
            if(File.Exists(file_name))
            {
                FileStream fs = new FileStream(file_name, FileMode.Open, FileAccess.Read);
                try
                {  
                    byte[] bt = new byte[256];
                    fs.Read(bt,0,(int)fs.Length);
                    if (bt[0] != 0)
                    {

                        string data = Encoding.UTF8.GetString(bt);
                        //Debug.Log(data);
                        res = new Ranking(id, "");
                        JArray array = JArray.Parse(data);
                        foreach (JToken token in array)
                        {
                            //Debug.Log(token.ToString());
                            res.RankPersonList.Add(token.ToObject<Person>());

                        }
                        //res.RankPersonList = JsonConvert.DeserializeObject<List<Person>>(data);
                    
                    }
			
                }
                catch (System.Exception exc)
                {
                    Debug.LogError(exc.Message);
                    res = null;
                }
                finally 
                {
                    fs.Close();
                }
            }
            return res;
        }

        public void SaveTo(string file_name) 
        {
            string data = JsonConvert.SerializeObject(this.RankPersonList);
            FileStream fs = new FileStream(file_name, FileMode.Create, FileAccess.Write);
            byte[] bt = Encoding.UTF8.GetBytes(data);
            fs.Write(bt,0,bt.Length);
            fs.Close();
        }

        public void Sort() 
        {
            Person person1;
            for (int i = 0; i < RankPersonList.Count - 1; i++)
            {
                for (int j = i; j < RankPersonList.Count; j++)
                {
                    if (RankPersonList[j].Score > RankPersonList[i].Score)
                    {
                        person1 = RankPersonList[i];
                        RankPersonList[i] = RankPersonList[j];
                        RankPersonList[j] = person1;
                    }
                }
            }
            string test = "";
            for (int i = 0; i < RankPersonList.Count; i++)
            {
                if (i >= RANK_COUNT)
                {
                    while (RankPersonList.Count > RANK_COUNT)
                    {
                        RankPersonList.RemoveAt(i);
                    }
                    break;
                }
                else 
                {
                    test += "\n " + i.ToString() + " :->>" + RankPersonList[i].Name + ":" + RankPersonList[i].Score;
                }
            }
          //  Debug.Log(test);
            //Debug.Log("2.RankPersonList.count = " + RankPersonList.Count);
        }

        public void AddToRanking(Person person) 
        {
            //Debug.Log("AddToRanking");
            Count++;
            //Debug.Log("1.RankPersonList.count = " + RankPersonList.Count);
            RankPersonList.Add(person);
            Sort();
            string test = "";
            foreach (Person item in RankPersonList)
            {
                test += "\n" + item.Name + ":" + item.Score;
            }
            //Debug.Log(test);
        }

        public List<string> GetenableName(string[] nameall)
        {
            List<string> ALL_names = new List<string>(nameall);
            List<string> unused_names=new List<string>();
            bool is_used = false;
            for (int i = 0; i < ALL_names.Count; i++)
            {
                is_used = false;
                for (int j = 0; j < RankPersonList.Count; j++)
                {
                    if (ALL_names[i] == RankPersonList[j].Name)
                    {
                        is_used = true;
                        break;
                    }
                }
                if (!is_used) 
                {
                    unused_names.Add(ALL_names[i]);
                }
                
            }

            return unused_names;
        }
    }

   

    public class GuidedMissile : RootObject 
    {
        public int target_id;
        [JsonConstructor]
        public GuidedMissile(int p_ID, string p_Name, Vector4 p_position, Vector4 p_rotation,int t_id) 
            :base(p_ID,p_Name,p_position,p_rotation,ObjectState.None,ObjectType.GuidedMissile,0)
        {
            target_id = t_id;
        }

        public GuidedMissile(int p_ID, string p_Name, int  attack,Vector3 p_position, Quaternion p_rotation, int t_id)
            : base(p_ID, p_Name, p_position, attack, p_rotation, ObjectState.None, ObjectType.GuidedMissile)
        {
            target_id = t_id;
        }
    }
}


