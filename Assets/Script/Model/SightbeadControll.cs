using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class SightbeadControll : MonoBehaviour {
    private float waittimer;
    private float tempwait = -1f;


    private GameObject sightbeat, waitbeat,Dtext;
    private float timerL, timerR;
    private float L_temp_dis;
    private float R_temp_dis;
    private Vector3 small,morethan;
    private Vector3 bigger,normal;
    private GameObject LeftHand, RightHand;

	void Start () {
        sightbeat = transform.FindChild("sight bead").gameObject;
        waitbeat = transform.FindChild("Wait").gameObject;
        Dtext = transform.FindChild("succes").gameObject;
        waitbeat.SetActive(false);
        Dtext.SetActive(false);
        small = new Vector3(0.025f, 0.025f, 0.025f);
        morethan = new Vector3(0.25f, 0.25f, 0.25f);
        bigger = new Vector3(0.2f,0.2f,0.2f);
        normal = new Vector3(0.1f,0.1f,0.1f);
        LeftHand = Recever.rInstance.PlayerData.transform.FindChild("_player").FindChild("FireLL").gameObject;
        RightHand = Recever.rInstance.PlayerData.transform.FindChild("_player").FindChild("FireRR").gameObject;
	}

    void WaitAnim(float timerRL,bool isaddtimer = true) 
    {
      
       // Debug.Log(timerRL);
        if (!isaddtimer &&(timerRL != 0f))
        {
            Debug.Log("重置时间*************");
            timerRL = 0f;
        }
        else
        {
            if (!waitbeat.activeSelf || !Dtext.activeSelf)
            {
                waitbeat.SetActive(true);
                Dtext.SetActive(true);
            }
            timerRL += Time.deltaTime;
           // Debug.Log("timerRL" + timerRL + "timerRL >= waittiemr" + (timerRL >= waittiemr));
           
            Debug.Log("timerRL"+timerRL);
            waitbeat.GetComponent<Image>().fillAmount = timerRL / waittimer;
            // Debug.Log("<color=red>%%%%%%%%%%%%%%%%%%%%%%</color>");
            Dtext.GetComponent<TextMesh>().text = "<color=red>" + Mathf.Round((timerRL / waittimer) * 100) + "%</color>";
            
        }
    }


    void SightPosition(GameObject sight) 
    {
        sight.transform.LookAt(Camera.main.transform.position);

        if (Recever.rInstance != null && Recever.rInstance.player.useWeapon.Name.Equals("weapon3"))
        {
            if (!waitbeat.activeSelf || !Dtext.activeSelf)
            {
                waitbeat.SetActive(true);
                Dtext.SetActive(true);
            }
            //为waittimer赋值
            if (waittimer != tempwait)
            {
                waittimer = Recever.rInstance.player.LaunchSpeed;
                tempwait = waittimer;
            }
            // Dtext.transform.localScale = this.transform.localScale == morethan ? normal : bigger;


            if (this.name.Equals("targetL"))
            {
                if (timerR < waittimer)
                {
                    timerR += Time.deltaTime;
                    waitbeat.GetComponent<Image>().fillAmount = timerR / waittimer;
                    if (timerR <= waittimer / 2 && timerR > waittimer / 4)
                    {
                        Dtext.GetComponent<TextMesh>().text = "<color=green>" + Mathf.Round((timerR / waittimer) * 100) + "%</color>";
                    }
                    if (timerR <= waittimer / 4 && timerR > 0f)
                    {
                        Dtext.GetComponent<TextMesh>().text = "<color=red>" + Mathf.Round((timerR / waittimer) * 100) + "%</color>";
                    }
                    if (timerR > waittimer / 2 && timerR < waittimer)
                    {
                        Dtext.GetComponent<TextMesh>().text = "<color=yellow>" + Mathf.Round((timerR / waittimer) * 100) + "%</color>";
                    }
                    //隐藏weapon3左炮台
                    if (LeftHand != null)
                    {
                        LeftHand.SetActive(false);
                    }
                    else
                    {
                        Debug.Log("找不到左炮台");
                    }
                }
                else
                {
                    //左炮台出现
                    if (LeftHand != null)
                    {
                        LeftHand.SetActive(true);
                    }
                    else
                    {
                        Debug.Log("找不到左炮台");
                    }
                    timerR = waittimer;
                    Dtext.GetComponent<TextMesh>().text = "<color=yellow>" + 100 + "%</color>";
                    if (Recever.rInstance.left_key)
                    {
                        timerR = 0f;
                    }
                }
            }

            if (this.name.Equals("targetR"))
            {
                if (timerL < waittimer)
                {
                    timerL += Time.deltaTime;
                    waitbeat.GetComponent<Image>().fillAmount = timerL / waittimer;
                    if (timerL <= waittimer / 2 && timerL > waittimer / 4)
                    {
                        Dtext.GetComponent<TextMesh>().text = "<color=green>" + Mathf.Round((timerL / waittimer) * 100) + "%</color>";
                    }
                    if (timerL <= waittimer / 4 && timerL > 0f)
                    {
                        Dtext.GetComponent<TextMesh>().text = "<color=red>" + Mathf.Round((timerL / waittimer) * 100) + "%</color>";
                    }
                    if (timerL > waittimer / 2 && timerR < waittimer)
                    {
                        Dtext.GetComponent<TextMesh>().text = "<color=yellow>" + Mathf.Round((timerL / waittimer) * 100) + "%</color>";
                    }
                    //隐藏右炮台
                    if (RightHand != null)
                    {
                        RightHand.SetActive(false);
                    }
                    else
                    {
                        Debug.Log("找不到右炮台");
                    }
                }
                else
                {
                    //右炮台出现
                    if (RightHand != null)
                    {
                        RightHand.SetActive(true);
                    }
                    else
                    {
                        Debug.Log("找不到右炮台");
                    }
                    timerL = waittimer;
                    Dtext.GetComponent<TextMesh>().text = "<color=yellow>" + 100 + "%</color>";
                    if (Recever.rInstance.right_key)
                    {
                        timerL = 0f;
                    }
                }
            }
        }
        else
        {
            if (waitbeat.activeSelf || Dtext.activeSelf)/*进度条及百分比*/
            {
                waitbeat.SetActive(false);
                Dtext.SetActive(false);
            }
            if (!LeftHand.activeSelf || !RightHand.activeSelf)/*左右炮台*/
            {
                LeftHand.SetActive(true);
                RightHand.SetActive(true);
            }


        }
    }


	void Update ()
    {
        SightPosition(this.gameObject);    
	}
}
