using UnityEngine;
using System.Collections;

public class Follow : MonoBehaviour
{
    public Vector3 Target;
    public float RunSpeed = 0.5f;
    public float RotateSpeed = 0.05f;
    public int degree = 0;
    public bool ToStartPosition;
    GameObject clone;
    void Start()
    {
        clone = new GameObject();
        clone.transform.SetParent(this.transform);
        clone.transform.localPosition = Vector3.zero;
        transform.eulerAngles = new Vector3(0,Random.Range(180,360),0);
    }

    // Update is called once per frame
    void Update()
    {
        if (Target != null &&
            MsgCenter.Instance.GameState == Datas.GameState.Playing)
        {
            clone.transform.LookAt(Target);
            transform.Translate(new Vector3(0, 0, MsgCenter.Instance.AIMoveSpeed));
            transform.rotation = Quaternion.Lerp(transform.rotation, clone.transform.rotation, RotateSpeed);
        }


    }
}
