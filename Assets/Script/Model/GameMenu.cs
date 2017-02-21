using UnityEngine;
using System.Collections;
using Datas;
using System.Collections.Generic;
using UnityEngine.UI;
using System;
using UnityEngine.SceneManagement;

public class GameMenu : MonoBehaviour
{
    public static GameMenu instance;
    public int PageTab;  //  如果是浏览教程状态，则指示当前显示的页码
    public bool isStart;   //  是否是进入游戏时的菜单

    public string PlayerName;   //  创建的角色名字
    private MenuState _muenu_state;
    public MenuState muenu_state
    {
        set
        {
            //Debug.Log("set::::"+_muenu_state.ToString());
            OnAwake();
            _muenu_state = value;
            if (isStart)
            {
                //   Debug.Log("isStart = true");
                switch (_muenu_state)
                {
                    //开始界面
                    case MenuState.None:
                        ChangePanel(0);
                        break;
                    case MenuState.CreatePlayer:

                        break;
                    //点击创建
                    case MenuState.Display:
                        ChangePanel(1);
                        break;
                    case MenuState.StartGame:
                        for (int i = 0; i < items.Count; i++)
                        {
                            items[i].SetActive(false);
                        }
                        this.gameObject.SetActive(false);
                        if (MsgCenter.Instance != null)
                        {
                            MsgCenter.Instance.globalname = PlayerName;
                            MsgCenter.Instance.player.Name = MsgCenter.Instance.Menu.PlayerName;
                            MsgCenter.Instance.CanLaunchBullet = true;
                            MsgCenter.Instance.GameState = GameState.Playing;
                            MsgCenter.Instance.Menu = null;
                        }
                        else
                        {
                            //Recever.rInstance.globalname = PlayerName;
                            //Recever.rInstance.isstartconnect = true;
                            //Recever.rInstance.Menu = null;
                            //Recever.rInstance.GameState = GameState.Playing;
                        }

                        break;
                }
            }
            else
            {
                //Debug.Log("isStart = false"+_muenu_state.ToString());
                switch (_muenu_state)
                {
                    //点击创建
                    case MenuState.Display:
                        // this.gameObject.SetActive(true);
                        ChangePanel(2);
                        if (Recever.rInstance!= null)
                        {
                            Recever.rInstance.GameState = GameState.End;
                            Recever.rInstance.ClearScene(Recever.rInstance.SenceObjects);
                        }
                        StartCoroutine(Writerecoding());
                        break;
                    case MenuState.Continue:
                        break;
                    case MenuState.ExitGame:
                        break;
                    default:
                        break;
                }
            }
        }
        get
        {
            return _muenu_state;
        }
    }

    public string buttonName;

    public GameObject RankingPrefab;

    public List<GameObject> items = new List<GameObject>();

    private GameObject current;

    private InputField nameinput;

    private GameObject textparent, RankingLeft, first, second, third;

    private string[] playernameprfab;
    private bool ishasname = false;
    private int name_index = 0;
    private bool init_ranking = false;

    public string Name
    {
        get
        {
            Debug.Log("getvalue:::"+PlayerName);
            return PlayerName;
        }
        set
        {
            PlayerName = value;
            nameinput.text = PlayerName;
        }
    }

    void OnAwake()
    {
        instance = this;
        playernameprfab = new string[] { "Jack", "Rose", "Micheal", "John", "Tom", "Jernny", "King", "Jane", "Tony", "Magic", "Angela" 
        ,"Lily","Jim","StanSen","Json","Cool"};
        first = transform.FindChild("first").gameObject;
        second = transform.FindChild("second").gameObject;
        third = transform.FindChild("Third").gameObject;
        RankingLeft = third.transform.FindChild("RankingLeft").gameObject;
        nameinput = first.transform.FindChild("InputField").GetComponent<InputField>();
        Button[] buttons = transform.GetComponentsInChildren<Button>();
        textparent = third.transform.FindChild("Content").gameObject;
        items.Add(first);
        items.Add(second);
        items.Add(third);
        RankingPrefab = transform.FindChild("Third").FindChild("Content").FindChild("Text").gameObject;
    }

    public void RandomName()
    {
        if (!ishasname &&
            MsgCenter.Instance != null)
        {
            List<string> name_strs =  MsgCenter.Instance.ranking.GetenableName(playernameprfab);
            Name = name_strs[(int)UnityEngine.Random.Range(0, name_strs.Count)];
            ishasname = true;
        }
    }
    public void CreatPlayer()
    {
        //   Debug.Log("CreatPlayer");
    }
    public void StartGame()
    {
        //    Debug.Log("StartGame");
        if(!MsgCenter.IsServece())
        {
            Recever.rInstance.GameState = GameState.Playing;
        }
    }
    private bool isreloadscene = false;
    int a=0;
    public void RedoGame()
    {
        //   Debug.Log("RedoGame");
        if (MsgCenter.Instance != null)
        {
            SceneManager.LoadScene(MsgCenter.Instance.SenceName);
        }
        else if(Recever.rInstance != null && !isreloadscene)
        {
            Resources.UnloadUnusedAssets();
            GC.Collect();
            GC.WaitForPendingFinalizers();//挂起当前线程，直到处理终结器队列的线程清空该队列为止
            GC.Collect();
            SceneManager.LoadScene(Recever.rInstance.SenceName);
            isreloadscene = true;
        }
        
    }
    public void ExitGame()
    {
       // Debug.Log("ExitGame");
        Application.Quit();
    }

    public void ChangePanel(int temp)
    {
        if (current)
        {
            current.SetActive(false);
        }
        items[temp].SetActive(true);
        current = items[temp];
    }
    bool isrange_name = false;
    public void Click(string btnName)
    {
        //if (btnName.Equals(string.Empty))
        //{
        //Debug.Log("BBBBBBBBBBBBBBBBBBBBB"+btnName);
        //}

        Audiocontroller.instans.PlayerAudio("dianjiUI");/*UI界面时，点击按钮声音*/
        
        switch (btnName)
        {
            case "NameButton":
                RandomName();
                isrange_name = true;
                break;
            case "CreatPlayerButton":
                if (isrange_name)
                {
                    CreatPlayer();
                    muenu_state = MenuState.Display;
                }
                break;
            case "StartGameButton":
                muenu_state = MenuState.StartGame;
                StartGame();
                break;
            case "RedoButton":
                RedoGame();
                break;
            case "ExitButton":
                ExitGame();
                break;
            default:
                break;
        }

    }

    //public void Writerecoding(int[] namesort, GameObject parent)
    //{
    //    if (namesort.Length == 10)
    //    {
    //        Debug.Log("<color=yellow>开始写入排行榜</color>");
    //        for (int i = 0; i < namesort.Length; i++)
    //        {
    //            parent.transform.GetChild(i).GetComponent<Text>().text = name[i].ToString();
    //            parent.transform.GetChild(i).GetChild(0).GetComponent<Text>().text = num[i].ToString();
    //        }
    //    }
    //    else
    //    {
    //        Debug.Log("<color=yellow>接收数据，名称，分数，数量不同只有" + namesort.Length + "个元素</color>");
    //    }

    //}


    private bool is_ranking = false;
    bool iswrite = false;
    /// <summary>
    /// 两个列表必须一样的长度，parent的结构不能改变
    /// </summary>
    public IEnumerator Writerecoding()
    {
        //Debug.Log("outoutoutoutout");
        if (!is_ranking)
        {
            //Debug.Log("ififiififiifififiifififiif");
            is_ranking = true;
            while ((MsgCenter.Instance != null && MsgCenter.Instance.ranking == null) ||
            (Recever.rInstance != null && Recever.rInstance.ranking == null))
            {
                yield return null;
            }
            Ranking ranking;
            if (MsgCenter.Instance != null)
            {
                ranking = MsgCenter.Instance.ranking;
            }
            else
            {
                ranking = Recever.rInstance.ranking;
            }
            ranking.Sort();
            Debug.Log("<color=yellow>开始写入排行榜  count = " + ranking.RankPersonList.Count + "</color>");
            for (int i = 0; i < ranking.RankPersonList.Count; i++)
            {
                //yield return new WaitForSeconds(0.1f);
                if (ranking.RankPersonList[i] != null)
                {
                    GameObject temp_A = Instantiate(RankingPrefab) as GameObject;
                    temp_A.name = i.ToString();
                    temp_A.transform.SetParent(textparent.transform);
                    temp_A.transform.SetAsLastSibling();
                    temp_A.SetActive(true);
                    temp_A.transform.localPosition = new Vector3(temp_A.transform.localPosition.x,
                        temp_A.transform.localPosition.y,
                        0);
                 //   Debug.Log("playername" + PlayerName + "rececer.rinstance.playername++++++=====" +Recever.rInstance.globalname);
                    if (MsgCenter.Instance != null && ranking.RankPersonList[i].Name.Equals(MsgCenter.Instance.globalname))
                    {
                        temp_A.GetComponent<Text>().text = "<color=yellow>" + ranking.RankPersonList[i].Name.ToString() + "</color>";
                        temp_A.transform.GetChild(0).GetComponent<Text>().text = "<color=yellow>" + ranking.RankPersonList[i].Score.ToString() + "</color>";
                        /*使第几名变成黄色，再添加箭头*/
                        third.transform.GetChild(i).GetComponentInChildren<Text>().color = Color.yellow;
                        RankingLeft.transform.position = new Vector3(third.transform.GetChild(i).position.x - 10,
                            third.transform.GetChild(i).position.y, third.transform.GetChild(i).position.z);
                    }
                    else if (Recever.rInstance != null && ranking.RankPersonList[i].Name.Equals(Recever.rInstance.globalname))
                    {
                        /*使第几名变成黄色，再添加箭头*/
                        third.transform.GetChild(i).GetComponentInChildren<Text>().color = Color.yellow;
                        RankingLeft.transform.position = new Vector3(third.transform.GetChild(i).position.x - 10,
                            third.transform.GetChild(i).position.y, third.transform.GetChild(i).position.z);

                        temp_A.GetComponent<Text>().text = "<color=yellow>" + ranking.RankPersonList[i].Name.ToString() + "</color>";
                        temp_A.transform.GetChild(0).GetComponent<Text>().text = "<color=yellow>" + ranking.RankPersonList[i].Score.ToString() + "</color>";
                    }
                    else {
                        temp_A.GetComponent<Text>().text = ranking.RankPersonList[i].Name.ToString();
                        temp_A.transform.GetChild(0).GetComponent<Text>().text = ranking.RankPersonList[i].Score.ToString();
                    }

                }

            }
        }


    }
   

    void Update()
    {
        if (MsgCenter.Instance != null)
        {
            if (MsgCenter.Instance.left_key || MsgCenter.Instance.right_key)
            {
                Click(buttonName);
            }
            if (!init_ranking && MsgCenter.Instance.ranking != null)
            {
                init_ranking = true;
                if (!isStart)
                {
                    //Debug.Log("isstart" + isStart);
                   // muenu_state = MenuState.Display;
                }

            }
        }
        else
        {
            if (Recever.rInstance.left_key || Recever.rInstance.right_key)
            {
                Click(buttonName);
            }
            if (!init_ranking &&
            Recever.rInstance.ranking != null)
            {
                init_ranking = true;
                if (!isStart )
                {
                    Debug.Log("isstart" + isStart );
                //    muenu_state = MenuState.Display;
                }

            }
        }

    }
}
