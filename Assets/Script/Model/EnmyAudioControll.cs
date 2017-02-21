using UnityEngine;
using System.Collections;
using Datas;

public class EnmyAudioControll : MonoBehaviour {
    private SenceGameObject sencegame;
    private AudioSource audio;
    public AudioClip cliptemp;
    private AI pc_man;
    private float timer;
	void Start () {
        audio = gameObject.GetComponent<AudioSource>();
        //sencegame = this.GetComponent<SenceGameObject>();
        //if (sencegame.type == Datas.ObjectType.AI)
        //{
        //    pc_man = (sencegame.SenceObject)as AI;
        //    switch (pc_man.Index)
        //    {
        //        case 0:
        //            audio.clip=Audiocontroller.instans.clips[8];
        //            break;
        //        case 1:
        //            audio.clip = Audiocontroller.instans.clips[8];
        //            break;
        //        case 2:
        //            audio.clip = Audiocontroller.instans.clips[8];
        //            break;
        //    }
        //}
	}
    bool isplayed=false;

	void Update ()
    {
        if (gameObject.activeSelf && audio != null && !isplayed)
        {
            Debug.Log("播放音乐");
            audio.Play();
            isplayed = true;
        }
	    
	}
}
