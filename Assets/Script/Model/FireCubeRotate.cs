using UnityEngine;
using System.Collections;

public class FireCubeRotate : MonoBehaviour {

	public float roundspeed;

    Transform around;
    private GameObject child;
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		transform.RotateAround( transform.forward,-Time.deltaTime*roundspeed);
       // child.transform.RotateAround(child.transform.up,-Time.deltaTime);
	}
}
