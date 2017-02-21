using UnityEngine;
using System.Collections;

public class Guided : MonoBehaviour {
    public Transform Target = null;
    public Rigidbody rig;
    public float Velocity;
    private bool is_go;

    private float _time = 0;
    private bool particle_ison=false;
    private GameObject particle;
    private float _velocity;
    void Start () 
    {
	    rig = GetComponent<Rigidbody>();
        is_go = false;
        rig.useGravity = false;
        if (MsgCenter.Instance != null)
        {
            Velocity = 100f;
        }
        else if (Recever.rInstance != null)
        {
            Velocity = 120f;
        }
        _velocity = 50;
        rig.velocity = transform.forward * _velocity;
        particle = this.transform.FindChild("tuowei2").gameObject;
       // rig.isKinematic = false;
	}

	// Update is called once per frame
	void Update ()
    {
        _velocity += 0.5f;
        _velocity = Mathf.Min(_velocity, Velocity);

        if (Target != null && Target.gameObject.activeSelf)
        {
            //Debug.Log("<color=yellow>导弹正向    " + Target.name + "     lerp   </color>");
            
            transform.forward = Vector3.Lerp(transform.forward,
                (Target.position - transform.position).normalized,
                0.1f);
            rig.velocity = (Target.position - transform.position).normalized * _velocity;
            rig.WakeUp();
        }
        else
        {
            //Debug.Log("<color=yellow>导弹没有目标，将沿着自身速度方向前进</color>方向为" + rig.velocity.normalized.ToString() + "速度为" + Velocity);
            rig.velocity = transform.forward * _velocity;
        }

        //  导弹发射后激活粒子
        if (particle && !particle_ison && Vector3.SqrMagnitude(rig.velocity) > 0.2f)
        {
            particle.SetActive(true);
            particle_ison = true;
        }
	}

    public virtual void OnTriggerEnter(Collider otherColl)
    {

        //Debug.Log("GuildedIstrriger"+otherColl.name);
    }

}
