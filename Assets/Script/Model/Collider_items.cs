using UnityEngine;
using System.Collections;

public class Collider_items : MonoBehaviour {

    public Recever recever;
    private float timer;


	void Start ()
    {
        //if (!this.GetComponent<Rigidbody>())
        //{
        //    Destroy(this,0.1f);
        //}
	}

    void AddToList(GameObject go) 
    {
        go.SetActive(false);
        go.GetComponent<Rigidbody>().isKinematic = true;
        Destroy(this);
        recever.BulletPool.Add(go);
    }

    public void OnTriggerEnter(Collider otherColl)
    {
        if (otherColl.tag == SenceGameObject.PLAYER_BULLET_TAG)
        {
          //  Destroy(gameObject);
          
        //    recever.BulletPool.Add(gameObject);
        }
        if (otherColl.tag == SenceGameObject.AI_TAG || otherColl.tag == SenceGameObject.AI_STONE)
        {
            AddToList(gameObject);
        }
    }
    void OnTriggerStay( Collider othercoll) 
    {
        if (this.tag == SenceGameObject.AI_TAG
            && transform.GetComponent<SenceGameObject>() != null
            && transform.GetComponent<SenceGameObject>()._pc_man.ai_type != Datas.AIType.LifeAndAttack)
        {
          //  Destroy(this.gameObject);
        //    recever.BulletPool.Add(gameObject);
        }
        if (othercoll.tag == SenceGameObject.AI_TAG || othercoll.tag == SenceGameObject.AI_STONE)
        {
            AddToList(gameObject);
        }
    }

	void Update ()
    {
        timer += Time.deltaTime;
        if (timer > 5f)
        {
            AddToList(gameObject);
        }
	}
}
