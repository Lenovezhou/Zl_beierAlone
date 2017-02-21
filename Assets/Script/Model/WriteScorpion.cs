using UnityEngine;
using System.Collections;
using System.Text;
using System.IO;
//using Newtonsoft.Json;
using System.Xml;
using System.Collections.Generic;
using UnityEngine.UI;

public class WriteScorpion : MonoBehaviour {

    public static Dictionary<string, int> sortdic = new Dictionary<string, int>();
    private static int nowstate;
    GameObject targetL;
    // Use this for initialization
    void Start()
    {

        CreatXML("this", 15);
        CreatXML("day", 30);
        CreatXML("tt", 20);
        CreatXML("aa", 10);
        CreatXML("cc", 150);
        CreatXML("mm", 50);

        //List<int> temp = ReadXml(Application.dataPath + "/XML/scort.xml", "this", 500);
        //Bubble(temp);
       // CreatBubblelist(sortdic);
       // Debug.Log("<color=yellow><size=20>" + (temp.IndexOf(500)+1) + "</size>"+sortdic.Count+"</color>");
    }
    /// <summary>
    /// 生成排行榜
    /// </summary>
    public void CreatBubblelist(Dictionary<string,int> dic,GameObject parent) 
    {
        List<string> tempname = new List<string>();
        List<int> tempsort = new List<int>();
         foreach (string  item in dic.Keys)
        {
            tempname.Add(item);
            Debug.Log("<color=yellow><size=20>" +item + "</size></color>");
        }
        foreach (int item in dic.Values)
        {
            tempsort.Add(item);
            
        }
        //取值
       // int s = dic["this"];
        for (int i = 0; i < parent.transform.childCount; i++)
        {
            if (i % 2 == 0)
            {
                parent.transform.GetChild(i).GetComponent<Text>().text = tempname[i / 2];
            }
            else {
                parent.transform.GetChild(i).GetComponent<Text>().text = tempname[(i - 1)/2];
            }
        }
       
       
    }





    /// <summary>
    /// 冒泡排序
    /// </summary>
    static void Bubble(List<int> list)
    {
        int temp = 0;
        for (int i = list.Count; i > 0; i--)
        {
            for (int j = 0; j < i - 1; j++)
            {
                if (list[j] < list[j + 1])
                {
                    temp = list[j];
                    list[j] = list[j + 1];
                    list[j + 1] = temp;
                }
            }
        }
    }

    
    public static void CreatXMLs(float a)
    {

        XmlDocument doc = new XmlDocument();
        XmlNode declare = doc.CreateXmlDeclaration("1.0", "utf-8", "");
        doc.AppendChild(declare);
        XmlElement root = doc.CreateElement("Program");
        doc.AppendChild(root);

        //for (int i = 0; i < roomid.Count; i++)
        //{

        XmlElement id = doc.CreateElement("id");

        id.SetAttribute("layoutID", a.ToString());

        //    id.SetAttribute("type", sceneindex[i].ToString());
            root.AppendChild(id);

        //}
        doc.Save(Application.dataPath + "/XML/config.xml");

    }
    /// <summary>
    /// 创建xml的方法
    /// </summary>
    public static void CreatXML(string name,int scort)
    {
       XmlDocument doc = new XmlDocument();
            XmlElement books;
            if (File.Exists(Application.dataPath + "/XML/scort.xml"))
            {
                //如果文件存在 加载XML
                doc.Load(Application.dataPath + "/XML/scort.xml");
                //获得文件的根节点
                books = doc.DocumentElement;
            }
            else
            {
                Debug.Log("保存成功");
                //如果文件不存在
                //创建第一行
                XmlDeclaration dec = doc.CreateXmlDeclaration("1.0", "utf-8", null);
                doc.AppendChild(dec);
                //创建跟节点
                books = doc.CreateElement("program");
                doc.AppendChild(books);
            }
            //5、给根节点Books创建子节点
            XmlElement book1 = doc.CreateElement("Player");
            //将book添加到根节点
            books.AppendChild(book1);
 
 
            //6、给Book1添加子节点
            XmlElement name1 = doc.CreateElement(name);
            name1.InnerText = scort.ToString();
            book1.AppendChild(name1);
 
            //XmlElement price1 = doc.CreateElement("Price");
            //price1.InnerText = "110";
            //book1.AppendChild(price1);
 
            //XmlElement des1 = doc.CreateElement("Des");
            //des1.InnerText = "看不懂";
            //book1.AppendChild(des1);


            doc.Save(Application.dataPath + "/XML/scort.xml");
           
          
 


    }
  

    /// <summary>
    /// 读取XML的方法
    /// </summary>
    /// <param name="path">路径</param>
   static List<int> ReadXml(string path,string name,int sort)
    {
        List<int> templist = new List<int>();
        XmlDocument xml = new XmlDocument();
        xml.Load(path);
        XmlElement xmlroot = xml.DocumentElement;           //得到根节点
        XmlNode str1 = xml.SelectSingleNode("program");     //获取单个节点
        int a = 0;
        bool isfind = false; 
       foreach (XmlElement item in str1)
       {
           //如果找到节点名字相同时
           if (item.ChildNodes[0].Name == name)
           {
               // 如果分数大于xml的值
               if (sort > int.Parse(item.ChildNodes[0].InnerText))
               {
                   // 修改innertext
                   item.ChildNodes[0].InnerText = sort.ToString();
                 
               }
               else   // 如果分数小于xml的值
               { 
                    // 不存储进xml，只做比较
                   templist.Add(sort);
               }
               isfind = true;
           }    
           
            templist.Add(int.Parse(item.ChildNodes[0].InnerText));
            sortdic.Add(item.ChildNodes[0].Name,int.Parse(item.ChildNodes[0].InnerText));
        }
       //如果找到节点名字不相同时：存储节点
       if (!isfind)
       {
           CreatXML(name,sort);
       }
       xml.Save(Application.dataPath + "/XML/scort.xml");
       //List<int> temp = new List<int>();
       //Bubble(templist);
       return templist;
    }



    // Update is called once per frame
    void Update()
    {

    }
//    //1.使用JsonReader读 Json字符串：
//   static void ReadJson(TextAsset text)
//    {
//        string jsonText = @"{""input"" : ""value"", ""output"" : ""result""}";
//        JsonReader reader=new JsonTextReader(new StringReader(jsonText));
//        while(reader.Read())
//        {
//             Debug.Log(reader.TokenType + "\t\t" + reader.ValueType + "\t\t" + reader.Value);
//        }
//    }
////2.使用JsonWriter写字符串：
//   static void CreatJson()
//    {
//        StringWriter sw=new StringWriter();
//        JsonWriter writer = new JsonTextWriter(sw);

//        writer.WriteStartObject();
//        writer.WritePropertyName("input");
//        writer.WriteValue("value");
//        writer.WritePropertyName("output");
//        writer.WriteValue("result");
//        writer.WriteEndObject();
//        writer.Flush();

//        string jsonText = sw.GetStringBuilder().ToString();
//        Debug.Log(jsonText);
//    }
   
}

public class JsonPaserWeb
{
    
}
