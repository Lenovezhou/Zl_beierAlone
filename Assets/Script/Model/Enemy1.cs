using UnityEngine;
using System.Collections;

public class Enemy1 : MonoBehaviour
{
    public float MoveSpeed=1;
    public float TrunSpeed=1;
    public Transform LookatPoint;
    public Transform Target;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update ()
    {
        LookatPoint.LookAt(Target);
        LookatPoint.position = transform.position;
        transform.Translate(Vector3.forward * MoveSpeed);
        transform.rotation = Quaternion.Lerp(transform.rotation, LookatPoint.rotation,TrunSpeed*0.01f);
	}
}
