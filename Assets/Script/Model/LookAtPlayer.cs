using UnityEngine;
using System.Collections;

public class LookAtPlayer : MonoBehaviour {

    public bool isFire = false;

    public GameObject fire_flower;



    private float timer;
	void Start () {
        if (transform.FindChild("FireFlower") != null) 
        {
            fire_flower = transform.FindChild("FireFlower").gameObject;
        }
	}
	
	// Update is called once per frame
	void Update () 
    {
        if (MsgCenter.Instance != null)
        {
            transform.LookAt(MsgCenter.Instance.PlayerData.transform);
        }
        else 
        {
            if (Recever.rInstance.PlayerData != null && fire_flower != null)
            {
               // fire_flower.transform.SetParent(transform.parent);
                transform.LookAt(Recever.rInstance.PlayerData.transform);

                //开火特效不改变
                //fire_flower.transform.forward = Vector3.forward;
               // timer += Time.deltaTime;
                //控制开火
                //timer += Time.deltaTime;

                if (isFire)
                {
                    for (int i = 0; i < fire_flower.transform.childCount ; i++)
                    {
                        fire_flower.transform.GetChild(i).gameObject.SetActive(true);
                    }
                    isFire = false;
                }
               
                //if (!isFire && timer >= 0.5f)
                //{
                //    if (fire_flower.activeSelf)
                //    {
                //        fire_flower.SetActive(false);
                //    }

                //}
            }
        }
	}
}
