using UnityEngine;
using System.Collections;

public class Hide : MonoBehaviour {
    public float Timer = 0;
    public float VisibleTime = 0.5f;

    private bool isrotate = false;

	void Update () 
    {
        Timer += Time.deltaTime;
        if(Timer >= VisibleTime)
        {
            Timer = 0;
            gameObject.SetActive(false);
        }

        if (gameObject.activeSelf && !isrotate)
        {
            transform.Rotate(Random.Range(10f,360f),0,0);
            isrotate = true;
        }
        else {
           isrotate = false;
        }
	}
}
