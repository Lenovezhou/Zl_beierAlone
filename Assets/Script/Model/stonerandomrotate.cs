using UnityEngine;
using System.Collections;

public class stonerandomrotate : MonoBehaviour {

    public GameObject target;

	// Use this for initialization
	void Start () {
        target = transform.parent.parent.FindChild("target").gameObject;
        target.transform.Rotate(Random.Range(10f,180f),Random.Range(20f,180f),Random.Range(30f,180f));
	}
	
	// Update is called once per frame
	void Update () {
        transform.RotateAround(target.transform.up,Time.deltaTime*2);
	}
}
