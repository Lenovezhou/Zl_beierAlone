using UnityEngine;
using System.Collections;

public class Moveplayer : MonoBehaviour {

    //电影纹理
    public MovieTexture movTexture;
    private MeshRenderer renderer;
    private float timer;

    void Start()
    {
        renderer = GetComponent<MeshRenderer>();
        //设置当前对象的主纹理为电影纹理
        renderer.material.mainTexture = movTexture;
        //设置电影纹理播放模式为循环
        movTexture.loop = false;
        movTexture.Play();
        audio.Play();
    }

    //void OnGUI()
    //{
    //    if (GUILayout.Button("播放/继续"))
    //    {
    //        //播放/继续播放视频
    //        if (!movTexture.isPlaying)
    //        {
    //            movTexture.Play();
    //            audio.Play();
    //        }

    //    }

    //    if (GUILayout.Button("暂停播放"))
    //    {
    //        //暂停播放
    //        movTexture.Pause();
    //        audio.Pause();
    //    }

    //    if (GUILayout.Button("停止播放"))
    //    {
    //        //停止播放
    //        movTexture.Stop();
    //    }
    //}

    void Update() 
    {
        timer += Time.deltaTime;
        if (timer > 5f)
        {
            movTexture.Stop();
            Debug.Log("停止或者暂停");
            transform.localScale = Vector3.Lerp(transform.localScale,Vector3.zero,0.5f);
            if (Vector3.Distance(transform.localScale,Vector3.zero) < 0.1f)
            {
                gameObject.SetActive(false);
            }
        }
        if (!movTexture.isPlaying)
        {
        }
    }
    //电影纹理
    //public MovieTexture movTexture;
    public AudioClip clip;
    public AudioSource audio;

    //void Start()
    //{
    //    //设置电影纹理播放模式为循环
    //    movTexture.loop = true;
    //}

    //void OnGUI()
    //{
    //    //绘制电影纹理
    //    GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), movTexture, ScaleMode.StretchToFill);

    //    if (GUILayout.Button("播放/继续"))
    //    {
    //        //播放/继续播放视频
    //        if (!movTexture.isPlaying)
    //        {
    //            movTexture.Play();
    //            AudioSource.PlayClipAtPoint(clip,Vector3.zero);
    //        }

    //    }

    //    if (GUILayout.Button("暂停播放"))
    //    {
    //        //暂停播放
    //        movTexture.Pause();
    //    }

    //    if (GUILayout.Button("停止播放"))
    //    {
    //        //停止播放
    //        movTexture.Stop();
    //    }
    //}

}
