using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityStandardAssets;

public class Bosslightsshane : MonoBehaviour {

   

    private float Timer, VisibleTime;
	void Start () {
        VisibleTime = 2f;
	}
	
	void Update () {
        Timer += Time.deltaTime;
        if (Timer >= 5f)
        {
            Timer = 0;
        }
        if (Timer >= 1.5f && Timer <=VisibleTime)
        {
            for (int i = 0; i < transform.childCount - 1; i++)
            {
                transform.GetChild(i).gameObject.SetActive(false);
            }
        }
        else
        {
            for (int i = 0; i < transform.childCount - 1; i++)
            {
                transform.GetChild(i).gameObject.SetActive(true);
                VisibleTime = Random.Range(3f,4f);
            }

        }
	}
}
