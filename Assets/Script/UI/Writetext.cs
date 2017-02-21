using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using UnityEngine.UI;
using System;

public class Writetext : MonoBehaviour {

    /// <summary>
    /// 此脚本是能够将文本字符串随着时间打字或褪色显示。
    /// </summary>

    public UnityEvent myEvent;//回调某个函数
    public int charsPerSecond = 5;
    // public AudioClip mAudioClip;             // 打字的声音，不是每打一个字播放一下，开始的时候播放结束就停止播放
    private bool isActive = false;

    private float timer;
    private string words;
    private Text mText;
  //  DataManage dataMan;

    void Start()
    {

        if (myEvent == null)
            myEvent = new UnityEvent();

       // words = "《太空历险记》是仿造日本的《超时空要塞》来制作的二十六集国产游戏，讲述在遥远的未来，随着人类赖以生存的各种能源的日渐减少，人们必须对宇宙的各种资源进行探索与开发。在这个过程中，人们遇到了宇宙能量强大的掠夺者--伊克斯星人。他们恐怖的攻击力及侵略性足以威胁整个太阳系。人们马上成立以神州号为首的太空防卫联盟军，和伊克斯星人展开了保护宇宙资源的太空之战，并研发出随舰的羿式高机动可变战斗机，成为防御外敌的主力战机。在长久的战争中，太空防卫联盟军被逼迫到宇宙各处，伊克斯星人的进攻越来越疯狂了。";
        words = GetComponent<Text>().text;
        GetComponent<Text>().text = string.Empty;
        timer = 0;
        isActive = true;
        charsPerSecond = Mathf.Max(1, charsPerSecond);
        mText = GetComponent<Text>();
    }

    //void ReloadText()
    //{
    //    Debug.Log("调用啦");
    //    words = GetComponent<Text>().text;
    //    mText = GetComponent<Text>();
    //}

    //public void OnStart()
    //{
    //    ReloadText();
    //    isActive = true;
    //}

    void OnStartWriter()
    {
        if (isActive)
        {
            try
            {
                mText.text = words.Substring(0, (int)(charsPerSecond * timer));
                timer += Time.deltaTime;
            }
            catch (System.Exception)
            {
                OnFinish();
            }
        }
    }

    void OnFinish()
    {
        isActive = false;
        timer = 0;
        GetComponent<Text>().text = words;
        try
        {

            myEvent.Invoke();
            Debug.Log("没有问题");
        }
        catch (Exception)
        {

            Debug.Log("问题");
        }
    }

    void Update()
    {
        OnStartWriter();
    }
}
