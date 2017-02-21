using UnityEngine;
using System.Collections;

public enum aspeacket 
{
    forword,up,left
}

public class RotateObj : MonoBehaviour
{
    public aspeacket asp;
    private Vector3 main;
    public float RotateSpeed = 10;
    // Use this for initialization
    void Start()
    {
        switch (asp)
        {
            case aspeacket.forword:
                main = Vector3.forward;
                break;
            case aspeacket.up:
                main = Vector3.up;
                break;
            case aspeacket.left:
                main = transform.right;
                break;
            default:
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
   
        transform.Rotate(main, RotateSpeed * Time.deltaTime);
    }
}
