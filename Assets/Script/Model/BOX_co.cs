using UnityEngine;
using System.Collections;

public class BOX_co : MonoBehaviour {

    float timer;

	void Start () {
	
	}
    public void OnTriggerEnter(Collider otherColl) 
    {
        if (otherColl.tag == SenceGameObject.PLAYER_BULLET_TAG) 
        {
            Destroy(gameObject);
        }
    }
	void Update ()
    {
        timer += Time.deltaTime;
        if (timer > 10f && false)
        {
            Destroy(gameObject);
        }
	
	}
}
