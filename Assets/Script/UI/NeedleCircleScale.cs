using UnityEngine;
using System.Collections;

public class NeedleCircleScale : MonoBehaviour {

    public Transform NeedleTypeIn;
 //   public Material[] ChangeMaterial;

    float Scale = 0;
    bool IsMax = false;
    bool IsMin = false;

 //   DataMgr Dmgr;
	// Use this for initialization
    //void Start () {
    //    Dmgr = Camera.main.GetComponent<DataMgr>();

    //    Scale = transform.localScale.x;
    //    IsMax = true;

    //    if (Dmgr.IsNeedleTypeRight[0])
    //    {
    //        if (NeedleTypeIn)
    //            NeedleTypeIn.GetComponent<Renderer>().material = ChangeMaterial[0];
    //    }
    //    else if (Dmgr.IsNeedleTypeRight[1] || Dmgr.IsNeedleTypeRight[2])
    //    {
    //        if (NeedleTypeIn)
    //            NeedleTypeIn.GetComponent<Renderer>().material = ChangeMaterial[1];
    //    }
    //    else if (Dmgr.IsNeedleTypeRight[3]||Dmgr.IsNeedleTypeRight[5]) 
    //    {
    //        if (NeedleTypeIn)
    //            NeedleTypeIn.GetComponent<Renderer>().material = ChangeMaterial[2];
    //    }
    //    else if (Dmgr.IsNeedleTypeRight[4])
    //    {
    //        if (NeedleTypeIn)
    //            NeedleTypeIn.renderer.material = ChangeMaterial[4];
    //    }
    //    else if (Dmgr.IsNeedleTypeRight[6])
    //    {
    //        if (NeedleTypeIn)
    //            NeedleTypeIn.renderer.material = ChangeMaterial[5];
    //    }
    //    else if (Dmgr.IsChestZhusheqi)
    //    {
    //        if (NeedleTypeIn)
    //            NeedleTypeIn.renderer.material = ChangeMaterial[3];
    //        transform.parent.transform.LookAt(Camera.main.transform.position);
    //    }
    //    else if (Dmgr.IsGuSuiChou)
    //    {
    //        if (NeedleTypeIn)
    //            NeedleTypeIn.renderer.material = ChangeMaterial[6];
    //        transform.parent.transform.LookAt(Camera.main.transform.position);

    //    }

	
    //}
	
	// Update is called once per frame
	void Update () {
        if (IsMax)
        {
            if (Scale <= 1)
            {
                Scale += 0.04f;
            }
            else
            {
                IsMax = false;
                IsMin = true;
            }
        }

        if (IsMin)
        {
            if (Scale >= 0.1f)
            {
                Scale -= 0.04f;
            }
            else
            {
                IsMax = true;
                IsMin = false;
            }
        }
        transform.localScale = new Vector3(Scale,Scale,Scale);

	
	}
}
