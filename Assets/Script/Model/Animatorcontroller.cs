using UnityEngine;
using System.Collections;


/// <summary>
/// 武器切换动画脚本，包括移动和升级特效
/// </summary>
public class Animatorcontroller : MonoBehaviour {
    public static Animatorcontroller instance;
    public GameObject FireLeft;
    public GameObject FireRight;
    public Animation L_partical, R_partical;
    public Vector3 startPostion;
    public Vector3 endPosition;
    public Vector3 defaultPosition1;
    public Vector3 defaultPosition2;
    public bool isPlaying;
    public float lerp_setup;
    public Vector3 tar_position1;
    public Vector3 tar_position2;

    private GameObject L_object,R_object;
    void Awake() 
    {
        instance = this;
        FireLeft = transform.FindChild("FireLL").gameObject;
        FireRight = transform.FindChild("FireRR").gameObject;
        //记录炮台初始位置
        defaultPosition1 = FireLeft.transform.localPosition;
        defaultPosition2 = FireRight.transform.localPosition;
        L_object = FireLeft.transform.FindChild("BlueClinderFX").gameObject;
        R_object = FireRight.transform.FindChild("BlueClinderFX").gameObject;
        L_object.SetActive(true);
        R_object.SetActive(true);
        L_partical = L_object.GetComponent<Animation>();
        R_partical = R_object.GetComponent<Animation>();
    }
	void Inite () {
        //播放武器升级动画：
        L_partical.Play();
        R_partical.Play();
        L_object.GetComponent<AudioSource>().Play();
	}

   public void PlayeMove() 
    {
       Inite();
       isPlaying = true;
       FireLeft.transform.localPosition = new Vector3(
           defaultPosition1.x + startPostion.x,
           defaultPosition1.y + startPostion.y,
           defaultPosition1.z + startPostion.z);

       FireRight.transform.localPosition = new Vector3(
           defaultPosition2.x + startPostion.x,
           defaultPosition2.y + startPostion.y,
           defaultPosition2.z + startPostion.z);
    }

   //public IEnumerator StopPlay() 
   //{
   //    isPlaying = false;
   //    FireLeft.SetActive(false);
   //    FireRight.SetActive(false);
   //    Debug.Log("结束动画");
   //}


	// Update is called once per frame
	void Update () 
    {
        if (isPlaying)
        {
            FireLeft.transform.localPosition = Vector3.Lerp(FireLeft.transform.localPosition, defaultPosition1, lerp_setup);
            FireRight.transform.localPosition = Vector3.Lerp(FireRight.transform.localPosition, defaultPosition2, lerp_setup);
        }
	}
}
