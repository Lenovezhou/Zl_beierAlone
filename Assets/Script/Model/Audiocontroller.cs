using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Audiocontroller : MonoBehaviour {
    public AudioClip[] clips;
    public List<string> ClipNames;
    public static Audiocontroller instans;
    private AudioSource[] audios;

    private AudioSource BackgroundAudio;
    private AudioSource PlayingGameAudio;
    private AudioSource PlayingEffectAudio;
    private AudioSource BossSoundsAudio;
    private AudioSource PlayerSoundsAudio;

    public Dictionary<string, AudioClip> AudioClips;
    private string tempClipname;

    void Awake() 
    {
        instans = this;
    }

	void Start () 
    {
        audios = GetComponents<AudioSource>();
        audios[0].playOnAwake = true;
        audios[0].loop = true;
        BackgroundAudio = audios[0];
       // audios[0].Stop();
        audios[1].playOnAwake = false;
        audios[1].loop = false;
        audios[2].playOnAwake = false;
        audios[2].loop = false;
        audios[3].playOnAwake = false;
        audios[3].loop = false;
        PlayingGameAudio = audios[1];
        PlayingEffectAudio = audios[2];
        BossSoundsAudio = audios[3];
        PlayerSoundsAudio = audios[4];
        InitClips();
	}

    public void InitClips() 
    {
        AudioClips = new Dictionary<string, AudioClip>();
        for (int i = 0; i < clips.Length; i++)
        {
            //Debug.Log(ClipNames[i]);
            AudioClips.Add(ClipNames[i],clips[i]);
        }
    }


    bool iscoverSounds = false;
    float timer=1f;
   
	void Update () 
    {
        if (MsgCenter.Instance != null)
        {
            switch (MsgCenter.Instance.GameState)
            {
                case Datas.GameState.None:
                    break;
                case Datas.GameState.Start:
                    if (tempClipname != "denglu")
                    {
                       // ChangeBGM(4);
                        PlayfashioneAudio("denglu");/*登录界面声音*/
                        tempClipname = "denglu";
                    }
                    break;
                case Datas.GameState.Playing:
                    if (tempClipname != "BGM")
                    {
                       // ChangeBGM(0);
                        PlayfashioneAudio("BGM");/*游戏进行声音*/
                        tempClipname = "BGM";
                    }
                    break;
                case Datas.GameState.End:
                    break;
                case Datas.GameState.Exit:
                    break;
                default:
                    break;
            }

        }
	}
    /// <summary>
    /// 背景音乐专用
    /// </summary>
    /// <param name="audio_name"></param>
    public void PlayfashioneAudio(string audio_name) 
    {
        if (AudioClips.ContainsKey(audio_name))
        {
            BackgroundAudio.Stop();
            BackgroundAudio.clip = AudioClips[audio_name];
            BackgroundAudio.Play();
        }
        else
        {
            Debug.LogError("不存在这首歌" + audio_name);
        }
    }

    public void PlayGameAudio(string audio_name)
    {

        if (AudioClips.ContainsKey(audio_name))
        {
          //  Debug.Log("BOSS爆炸");
            //PlayingGameAudio.Stop();
            //PlayingGameAudio.clip = AudioClips[audio_name];
            //PlayingGameAudio.Play();
            PlayingGameAudio.PlayOneShot(AudioClips[audio_name],0.5f);
        }
        else
        {
            Debug.LogError("不存在这首歌" + audio_name);
        } 
    }

    public void PlayEffectAudio(string audio_name)
    {
        if (AudioClips.ContainsKey(audio_name))
        {
            //PlayingEffectAudio.Stop();
            //PlayingEffectAudio.clip = AudioClips[audio_name];
            //PlayingEffectAudio.Play();
            PlayingGameAudio.PlayOneShot(AudioClips[audio_name], 0.5f);
        }
        else {
            Debug.LogError("不存在这首歌"+audio_name);
        }
    }


    public void PlayBossAudio(string audio_name)
    {
        
        if (AudioClips.ContainsKey(audio_name))
        {
            //Debug.Log(audio_name);
            BossSoundsAudio.Stop();
            BossSoundsAudio.clip = AudioClips[audio_name];
            BossSoundsAudio.Play();
        }
        else
        {
            Debug.LogError("不存在这首歌" + audio_name);
        }
    }



    /// <summary>
    /// 专为Player设置
    /// </summary>
    /// <param name="audio_name"></param>
    public void PlayerAudio(string audio_name)
    {
        if (AudioClips.ContainsKey(audio_name))
        {
            PlayerSoundsAudio.Stop();
            PlayerSoundsAudio.clip = AudioClips[audio_name];
            PlayerSoundsAudio.Play();
        }
        else
        {
            Debug.LogError("不存在这首歌" + audio_name);
        }
    }
}
