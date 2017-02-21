using UnityEngine;
using System.Collections;

public class StoneWalk : MonoBehaviour {
    public Vector3 Tartget;
    public float RunSpeed = 20f;

    private Rigidbody rig;
	// Use this for initialization
	void Start () 
    {
	    rig = gameObject.GetComponent<Rigidbody>();
	}
	
	// Update is called once per frame
	void Update () 
    {
        
            transform.LookAt(Tartget);
            rig.velocity = (Tartget - transform.position).normalized * RunSpeed;
            Debug.DrawLine(transform.position, Tartget, Color.yellow);
        
	}
}
