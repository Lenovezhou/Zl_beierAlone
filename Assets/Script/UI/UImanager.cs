using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public enum playertest 
{
    none,Firststemp,Secondstemp,Thirdstemp
}
public class UImanager : MonoBehaviour {
    public static UImanager instance;
    public GameObject firstshow,first,second,third;
    public GameObject startgamebutton;
    public playertest teststemp=playertest.none;
    private GameObject temp;
    void Awake() 
    {
     
        instance = this;
    }

	// Use this for initialization
	void Start () {
        firstshow = transform.FindChild("StartCanvas").gameObject;
        firstshow.SetActive(true);
        first = firstshow.transform.FindChild("first").gameObject;
        second = firstshow.transform.FindChild("second").gameObject;
        third = firstshow.transform.FindChild("third").gameObject;
        startgamebutton = firstshow.GetComponentInChildren<Button>().gameObject;
        //GameObject startgamebutton = transform.FindChild("");
       // GameObject firstshow = transform.FindChild("");

	}

    public void ChangePanel(GameObject obj) 
    {
        obj.SetActive(true);
        if (temp)
        {
            temp.SetActive(false);
        }
        temp = obj;
    }

	// Update is called once per frame
	void Update () 
    {
        if (teststemp == playertest.none && Camera.main.transform.eulerAngles.z < 20f && Camera.main.transform.eulerAngles.z > 0
            || Camera.main.transform.eulerAngles.z <= 360f && Camera.main.transform.eulerAngles.z >= 340f)
        {
            teststemp = playertest.Firststemp;
            ChangePanel(second);
        }
        if (teststemp==playertest.Firststemp && Recever.rInstance.left_key && Recever.rInstance.right_key)
        {
            teststemp = playertest.Secondstemp;
            ChangePanel(third);
        }
        //if (teststemp==playertest.Thirdstemp && )
        //{
            
        //}
	}
}
