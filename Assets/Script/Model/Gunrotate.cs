using UnityEngine;
using System.Collections;

public class Gunrotate : MonoBehaviour {

    public float RotateSpeed;
	void Start () 
    {
	
	}
	
	
	void Update () 
    {
        if (MsgCenter.Instance != null)
        {
          //  Debug.Log(MsgCenter.Instance.left_key + "MsgCenter.Instance.left_key" + MsgCenter.Instance.right_key + "MsgCenter.Instance.right_key");
            if (MsgCenter.Instance.left_key)
            {
                if (this.gameObject.name.Equals("paoL"))
                {
                    transform.RotateAround(transform.forward, Time.deltaTime * RotateSpeed);
                }
            }
            //else if (!MsgCenter.Instance.left_key)
            //{
            //    if (this.gameObject.name.Equals("paoL"))
            //    {
            //      transform.localRotation = Quaternion.Lerp(transform.rotation, Quaternion.identity, 0.05f);
            //    }
            //}
            if (MsgCenter.Instance.right_key)
            {
                if (this.gameObject.name.Equals("paoR"))
                {
                    transform.RotateAround(transform.forward, Time.deltaTime * RotateSpeed);
                }
            }
            //else if (!MsgCenter.Instance.left_key)
            //{
            //    if (this.gameObject.name.Equals("paoR"))
            //    {
            //        transform.localRotation = Quaternion.Lerp(transform.rotation, Quaternion.identity, 0.05f);
            //    }
            //}
        }
       if (Recever.rInstance != null)
        {
          //  Debug.Log("Recever.rInstance.left_key" + Recever.rInstance.left_key+Recever.rInstance.right_key);
            if (Recever.rInstance.left_key)
            {
                if (this.gameObject.name.Equals("paoL"))
                {
                //    Debug.Log("lllllllllllllllllllll");
                    transform.RotateAround(transform.forward, Time.deltaTime * RotateSpeed);
                }
            }
            //else if (!Recever.rInstance.left_key)
            //{
            //    if (this.gameObject.name.Equals("paoL"))
            //    {
            //        transform.localRotation = Quaternion.Lerp(transform.rotation, Quaternion.identity, 0.05f);
            //    }
            //}
            if (Recever.rInstance.right_key)
            {
                if (this.gameObject.name.Equals("paoR"))
                {
                    transform.RotateAround(transform.forward, Time.deltaTime * RotateSpeed);
                }
            }
            //else if (!Recever.rInstance.left_key)
            //{
            //    if (this.gameObject.name.Equals("paoR"))
            //    {
            //        transform.localRotation = Quaternion.Lerp(transform.rotation, Quaternion.identity, 0.05f);
            //    }
            //}
        }
       
	}
}
