using UnityEngine;
using System.Collections;

public class HP_lookat : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        if (Camera.main != null)
        {
            transform.LookAt(Camera.main.transform.position);
        }
	}
}
