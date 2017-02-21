using UnityEngine;
using System.Collections;

/// <summary>
/// 每个怪物都要添加上这个脚本
/// </summary>
public class Collections2 : MonoBehaviour {
    public bool isRendering=false;
    private float lastTime=0;
    private float curtTime=0;

	void Start () {
	
	}
	
	void Update () 
    {
        if (this.transform.FindChild("child"))
        {
            this.transform.FindChild("child").transform.LookAt(Camera.main.transform.position);
        }
        isRendering = curtTime != lastTime ? true : false;
        lastTime = curtTime;
	}
    void OnWillRenderObject()
    {
        curtTime=Time.time;
    }
    //void OnBecameVisible() {
    //    isRendering = true;
    //}
    //void OnBecameInvisible() {
    //    isRendering = false;
    //}
}
 
 
