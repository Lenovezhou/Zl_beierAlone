using UnityEngine;
using System.Collections;
using System.Collections.Generic;


    public class Collections1 : MonoBehaviour {
    public List<GameObject> enemys = new List<GameObject>();
    public List<GameObject> select_enemys = new List<GameObject>();
    public GameObject istarget;
    public bool iswepon3 = false;
	void Start () {
	
	}

    void Selectionenemy()
    {
        if (iswepon3)
        {
            for (int i = 0; i < enemys.Count; i++)
            {
                //Vector3 v1 = Camera.main.WorldToScreenPoint(enemys[i].transform.position);
                // Debug.Log(v1 + "..." + enemys[i].transform.position);
                //if ((v1.x < Screen.width && v1.x > 0) && (v1.y > 0 && v1.y < Screen.height) && !select_enemys.Contains(enemys[i]))
                //{
                //    select_enemys.Add(enemys[i]);
                //}
                //if ((v1.x < 0 || v1.x > Screen.width || v1.y < 0 || v1.y > Screen.height) && select_enemys.Contains(enemys[i]))
                //{
                //    select_enemys.Remove(enemys[i]);
                //}
                if (enemys[i] == null)
                {
                    enemys.Remove(enemys[i]);
                }
                if (enemys[i].GetComponent<Collections2>().isRendering && !select_enemys.Contains(enemys[i]))
                {
                    select_enemys.Add(enemys[i]);
                    if (enemys[i].transform.FindChild("child"))
                    {
                        enemys[i].transform.FindChild("child").gameObject.SetActive(true);
                    }
                    else
                    {
                        GameObject Go = Instantiate(istarget) as GameObject;
                        Go.transform.SetParent(enemys[i].transform);
                        Go.transform.localPosition = Vector3.zero;
                        Go.name = "child";
                    }

                }
                else if (!enemys[i].GetComponent<Collections2>().isRendering && select_enemys.Contains(enemys[i]))
                {
                    select_enemys.Remove(enemys[i]);
                    if (enemys[i].transform.FindChild("child"))
                    {
                        enemys[i].transform.FindChild("child").gameObject.SetActive(false);
                    }
                    else
                    {

                    }
                }
            }
        }
        else
        {
            for (int i = 0; i < enemys.Count; i++)
            {
                select_enemys.Remove(enemys[i]);
                if (enemys[i].transform.FindChild("child") && enemys[i].transform.FindChild("child").gameObject.activeSelf)
                {
                    enemys[i].transform.FindChild("child").gameObject.SetActive(false);
                }
            }
        }
    }
	
	void Update () {
	//生成AI时添加进enemys，调用Selectionenemy（）；
    //每个AI和石头（敌人）都要添加检测是否在视野范围内的脚本Collections2
	}
}
