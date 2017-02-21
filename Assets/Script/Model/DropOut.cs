using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using Datas;
using System.IO;
using System.Text;
public class DropOut : MonoBehaviour
{
    //private static DropOut _instance;
    //private DropOut()
    //{
    //    weaponList = new Dictionary<string, string>();
    //    ReadXML();
    //}

    //public static DropOut GetInstance
    //{
    //    get
    //    {
    //        if (_instance == null)
    //        {
    //            _instance = new DropOut();
    //        }
    //        return _instance;
    //    }
    //}

    void Start()
    {
        weaponList = new Dictionary<string, string>();
        ReadXML();
    }
   public void ReadXML()
    {
        TextAsset text = Resources.Load<TextAsset>("weapon/weapons");
        XmlDocument myXML = new XmlDocument();
        myXML.LoadXml(text.text);
        foreach (XmlElement item in myXML.DocumentElement.ChildNodes)
        {
            string name = string.Empty;
            if (item.Attributes["Name"] != null)
            {
                name = item.Attributes["Name"].Value;
            }

            string path = string.Empty;
            if (item.Attributes["path"] != null)
            {
                path = item.Attributes["path"].Value;
            }
            weaponList.Add(name, path);
        }
        Init();
    }

   public void Init() 
   {
       GameObject res_obj;
       GameObject obj;
       weaponPool = new Dictionary<string, GameObject>();
       foreach (string name in weaponList.Keys)
       {
           res_obj = Resources.Load<GameObject>(weaponList[name]);
           obj = (GameObject)Instantiate(res_obj, Vector3.zero, Quaternion.identity);
           weaponPool.Add(name,obj);
       }
   }
   public string ReadXML(string name)
   {
       TextAsset text = Resources.Load<TextAsset>("weapon/weapons");
       XmlDocument myXML = new XmlDocument();
       myXML.LoadXml(text.text);
        string path=string.Empty;
       foreach (XmlElement item in myXML.DocumentElement.ChildNodes)
       {
           //string name = string.Empty;
           if (item.Attributes["Name"].Value.Equals(name))
           {
               if (item.Attributes["path"] != null)
               {
               //    Debug.Log("item.Attributes[path]"+item.Attributes["path"]);
                   path = item.Attributes["path"].Value;
               }
           }
       }
       return path;
   }
    //struct DropData
    //{
    //    public string name;
    //    public string path;

    //    public DropData(string name, string path)
    //    {
    //        this.name = name;
    //        this.path = path;
    //    }
    //}
   
    private Dictionary<string, string> weaponList;
    private Dictionary<string, GameObject> weaponPool;
    private bool test_bl = false;
    public void CreateWeapon(string weapon_name, Vector3 position)
    {
        
        //Debug.Log("weapon_nameweapon_name::::::   " + weapon_name);
        if (!string.IsNullOrEmpty(weapon_name))
        {
            //Debug.Log(weaponPool.ContainsKey(weapon_name) + "  是否存在");
            if (weaponPool.ContainsKey(weapon_name))
            {
                //Debug.Log("weaponPool.ContainsKey(weapon_name)");

                GameObject go = weaponPool[weapon_name];
                if(go == null)
                {
                    Init();
                    go = weaponPool[weapon_name];
                }
                if (go.activeSelf)
                {
                    StartCoroutine(CreateWp(weapon_name,go,position));
                }
                else 
                {
                    //Debug.Log(go.name + " go.name");
                    SenceGameObject temp = go.GetComponent<SenceGameObject>();
                    if (temp == null)
                    {
                        temp = go.AddComponent<SenceGameObject>();
                    }
                    temp.SenceObject = new Prop(MsgCenter.Instance.GetID(), weapon_name, 0, 0, 0,
                        position, Quaternion.identity, ObjectState.None, ObjectType.Weapon);
                    if (position.z > 80)
                    {
                        Vector3 tmp_V3 = Camera.main.transform.position - position;
                        temp.transform.position = tmp_V3.normalized * (tmp_V3.magnitude - 80) + position;
                    }
                    else
                    {
                        temp.transform.position = position;
                        temp.SenceObject.Position = position;
                        temp.SenceObject.Rotation = Quaternion.identity;
                    }
                    temp.isSevice = true;

                    //if(test_bl)
                    //{
                    //    go.tag = SenceGameObject.NONE_TAG;
                    //}
                    test_bl = true;
                    temp.enabled = true;
                    go.SetActive(true);

                    MsgCenter.Instance.AddToSendList(temp);
                }
                
            }
        }
    }

    public IEnumerator CreateWp(string weapon_name,GameObject go, Vector3 position) 
    {
        //Debug.Log(go.name + " go.name");
        SenceGameObject temp = go.GetComponent<SenceGameObject>();
        MsgCenter.Instance.DestroySenceObject(temp,0.1f);
        while (!temp.ClientIsDestroy)
        {
            yield return null;
        }

        temp.SenceObject = new Prop(MsgCenter.Instance.GetID(), weapon_name, 0, 0, 0,
            position, Quaternion.identity, ObjectState.None, ObjectType.Weapon);
        if (position.z > 80)
        {
            Vector3 tmp_V3 = Camera.main.transform.position - position;
            temp.transform.position = tmp_V3.normalized * (tmp_V3.magnitude - 80) + position;
        }
        else
        {
            temp.transform.position = position;
            temp.SenceObject.Position = position;
            temp.SenceObject.Rotation = Quaternion.identity;
        }
        temp.isSevice = true;

        //if(test_bl)
        //{
        //    go.tag = SenceGameObject.NONE_TAG;
        //}
        test_bl = true;
        temp.enabled = true;
        go.SetActive(true);

        MsgCenter.Instance.AddToSendList(temp);
    }

    public void DestroyWeapon(string name) 
    {
        if(weaponPool.ContainsKey(name))
        {
            weaponPool[name].SetActive(false);
        }
    }

    public void Don() { }
}
