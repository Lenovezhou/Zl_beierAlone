using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Datas;
using UnityEngine.UI;

public enum handchoise
{
    L_hand=0,
    R_hand=1,
    none
}
public class PlayerController : MonoBehaviour {
    /// <summary>
    /// 左手
    /// </summary>

    public HandControl handcontrollsLL;
    /// <summary>
    /// 右手
    /// </summary>
    public HandControl handcontrollsRR;
    /// <summary>
    /// 左炮台
    /// </summary>
    public GameObject HandlerLL;
    /// <summary>
    /// you炮台
    /// </summary>
    public GameObject HandlerRR;
    public List<GameObject> prefabs;
    //左右手枚举
    public handchoise hc;

    public float faraway=50f;
    /// <summary>
    /// 左右控制器发射的射线
    /// </summary>
    private Ray handray_tempR,handray_tempL;
    private RaycastHit hand_hitR, hand_hitL; 
    private List<LineRenderer> t_lines;
    private GameObject Player;
    private float timer,nextfiretime=0.2f;
    private bool issende;
    private bool isfindLL=false, isfindRR=false,isfindfireLL,isfindfireRR;
    private GameObject temp_handlerLL, temp_handlerRR;
    private float clamprotate;

    private Image L_image_green,R_image_green;
    private float L_temp_dis,R_temp_dis;

    /// <summary>
    /// 锁定标志
    /// </summary>
    private GameObject Lock_L, Lock_R;
    private GameObject Lock_Lparent, Lock_Rparent;
    private Transform Lock_Ltarget, Lock_Rtarget;

    //锁定累计时间
    private float Left_Locktimer=0f, Riget_Locktimer=0;


    public  GameObject targetL,targetR;
    void Start ()
    {
        prefabs.Add((GameObject)Resources.Load("bullent1"));
        t_lines=new List<LineRenderer>();
        GameObject tempL = Resources.Load("NeedleCircle") as GameObject;
        GameObject tempR = Resources.Load("NeedleCircle") as GameObject;
        Lock_L = Instantiate(Recever.rInstance.LockHubObj) as GameObject;
        Lock_R = Instantiate(Recever.rInstance.LockHubObj) as GameObject;
        Lock_L.SetActive(false);
        Lock_R.SetActive(false);
        targetL = Instantiate(tempL)as GameObject;
        targetR = Instantiate(tempR) as GameObject;
        targetL.AddComponent<SightbeadControll>();
        targetR.AddComponent<SightbeadControll>();
        targetL.name = "targetL";
        targetR.name = "targetR";
        //targetL.transform.localScale = new Vector3(0.125f, 0.125f, 0.125f);
        //targetR.transform.localScale = new Vector3(0.125f, 0.125f, 0.125f);
      
	}

    /// <summary>
    /// 反复寻找手柄，炮台，直到找到为止
    /// </summary>
        void Onfind() 
        {
            if (Player == null)
            {
                Player = Recever.rInstance.PlayerData.gameObject.transform.FindChild("_player").gameObject;
            }
            else if (Recever.rInstance.PlayerData != null &&
                !Recever.rInstance.IsShaking)
            {
            }
            if (Recever.rInstance.PlayerData != null)
            {
                //Debug.Log("PlayerData");
                if (HandlerLL == null || HandlerRR == null)
                {
                    HandlerLL = Recever.rInstance.PlayerData.transform.FindChild("_player").FindChild("FireLL").gameObject;
                    HandlerRR = Recever.rInstance.PlayerData.transform.FindChild("_player").FindChild("FireRR").gameObject; 
              
                }
                if (handcontrollsLL == null)
                {
                    handcontrollsLL = Recever.rInstance.PlayerData.transform.FindChild("Controller (left)").GetComponent<HandControl>();
                    isfindLL = true;
                }
                if (handcontrollsRR == null)
                {
                    handcontrollsRR = Recever.rInstance.PlayerData.transform.FindChild("Controller (right)").GetComponent<HandControl>();
                    isfindRR = true;
                }
                if (isfindRR)
                {
                    handcontrollsRR.playercontroller = this;
                    //LineRenderer lineRR = handcontrollsRR.GetComponent<LineRenderer>();
                    //lineRR.SetPosition(0, lineRR.gameObject.transform.position);
                    //lineRR.SetPosition(1, lineRR.gameObject.transform.position);
                    //lineRR.SetColors(Color.yellow, Color.red);
                    //lineRR.SetWidth(0.02f, 0.02f);
                    //lineRR.material = Resources.Load<Material>("handlerline");
                    //t_lines.Add(lineRR);

                }
                if (isfindLL)
                {
                    handcontrollsLL.playercontroller = this;
                    //LineRenderer lineLL = handcontrollsLL.GetComponent<LineRenderer>();
                    //lineLL.SetPosition(0, lineLL.gameObject.transform.position);
                    //lineLL.SetPosition(1, lineLL.gameObject.transform.position);
                    //lineLL.SetColors(Color.yellow, Color.red);
                    //lineLL.SetWidth(0.02f, 0.02f);
                    //lineLL.material = Resources.Load<Material>("handlerline");
                    //t_lines.Add(lineLL);

                }
            }
            
            if (!handcontrollsLL || !handcontrollsRR)
            {

                //Debug.Log("请检查两个控制器是否同时开启");


            }

            /*   //if为测试单机使用：
               if (handcontrollsLL == null || handcontrollsRR == null)
               {
                   // //Debug.Log(gameObject.transform.FindChild("_player").FindChild("Controller (left)"));
                   handcontrollsLL = gameObject.transform.FindChild("Controller (left)").gameObject.GetComponent<HandControl>();
                   handcontrollsRR = gameObject.transform.FindChild("Controller (right)").gameObject.GetComponent<HandControl>();
                   handray_tempR.origin = handcontrollsLL.transform.position;
                   handray_tempR.direction = handcontrollsLL.transform.forward;
                   handray_tempL.origin = handcontrollsRR.transform.position;
                   handray_tempL.direction = handcontrollsRR.transform.forward;
               }
             //if为测试单机使用：
               if (HandlerLL == null || HandlerRR == null)
               {
                   HandlerLL = gameObject.transform.FindChild("_player").FindChild("FireLL").gameObject;
                   HandlerRR = gameObject.transform.FindChild("_player").FindChild("FireRR").gameObject;
               }
               if (Physics.Raycast(handray_tempR, out hand_hitR, 1000f))
               {
                   //Debug.Log("<color=yellow>" + handray_tempR.origin + handray_tempR.direction + handcontrollsRR.transform.forward + hand_hitR.point + "</color>");
                   HandlerRR.transform.LookAt(hand_hitR.point);
                   //lines[0].SetPosition(1, hand_hitR.point);
                   //  //Debug.Log("<color=red>" + hand_hitR.point + "</color>", gameObject);

                   Debug.DrawLine(handcontrollsRR.transform.position, hand_hitR.point);
               }
               else
               {
                   HandlerRR.transform.LookAt(handray_tempR.origin + handray_tempR.direction * 100f);
                   //   lines[0].SetPosition(1, handray_tempR.origin + handray_tempR.direction * 100f);
               }
               if (Physics.Raycast(handray_tempL, out hand_hitL, 1000f))
               {
                   HandlerLL.transform.LookAt(hand_hitL.point);
               }
               else
               {
                   HandlerLL.transform.LookAt(handray_tempL.origin + handray_tempL.direction * 100f);
               }*/
        }

    /// <summary>
    /// 准星位置
    /// </summary>
        void ChangeShightBeat ()
        {
            //  右手准星终点除UI外
            if (Recever.rInstance.GameState == Datas.GameState.Playing)
            {
                targetR.transform.position = handcontrollsRR.transform.forward * LookAt.FAR_DISTANCE + handcontrollsRR.transform.position;
            }
            else
            {
                targetR.transform.position = handcontrollsRR.transform.forward * LookAt.MIN_DISTANCE + handcontrollsRR.transform.position;
            }
            //  左手准星终点除UI外
            if (Recever.rInstance.GameState == Datas.GameState.Playing)
            {
                targetL.transform.position = handcontrollsLL.transform.forward * LookAt.FAR_DISTANCE + handcontrollsLL.transform.position ;
            }
            else
            {
                targetL.transform.position = handcontrollsLL.transform.forward * LookAt.MIN_DISTANCE + handcontrollsLL.transform.position;

            }
        }

    /// <summary>
    /// 左手锁定
    /// </summary>
        void LockLeftTarget() 
        {
            if (Lock_Lparent != null && Left_Locktimer <= 1f)
            {
                Left_Locktimer += Time.deltaTime;

                if (!Lock_L.activeSelf)
                {
                //    Audiocontroller.instans.PlayGameAudio("suoding");
                    Lock_L.SetActive(true);
                }
                Vector3 normalface = Vector3.Normalize(Lock_Lparent.transform.position - Camera.main.transform.position);
                Lock_L.transform.position = normalface * LookAt.CURR_DISTANCE;
                Lock_L.transform.LookAt(Camera.main.transform);
            }
            else
            {
                Lock_Lparent = null;
                Left_Locktimer = 0f;
                if (Lock_L.activeSelf)
                {
                    Lock_L.SetActive(false);
                }
            }

        }

    /// <summary>
    /// 右手锁定
    /// </summary>
        void LockRightTarget()
        {
            if (Lock_Rparent != null && Riget_Locktimer <= 1f)
            {
                Riget_Locktimer += Time.deltaTime;
                if (!Lock_R.activeSelf)
                {
              //      Audiocontroller.instans.PlayGameAudio("suoding");

                    Lock_R.SetActive(true);
                }
                Vector3 normalface = Vector3.Normalize(Lock_Rparent.transform.position - Camera.main.transform.position);
                Lock_R.transform.position = normalface * LookAt.CURR_DISTANCE;
                Lock_R.transform.LookAt(Camera.main.transform);
            }
            else
            {
                Lock_Rparent = null;
                Riget_Locktimer = 0f;
                if (Lock_R.activeSelf)
                {
                    Lock_R.SetActive(false);
                }
            }

        }

    /// <summary>
    /// 左手射线及UI时处理，并且检测第三种武器时的锁定目标，Lock_Lparent
    /// </summary>
        void LeftRay() 
        {
            //左手射线
            if (Physics.Raycast(handray_tempL, out hand_hitL, 3000f))
            {
                Recever.rInstance.targetL = hand_hitL.collider.gameObject;
                if (hand_hitL.collider.tag == SenceGameObject.AI_TAG || hand_hitL.collider.tag == "UI"
                    || hand_hitL.collider.tag.Equals(SenceGameObject.Weapon_TAG))
                {
                    if (!Recever.rInstance.player.useWeapon.Name.Equals("weapon3"))
                    {
                        //区别瞄准的物体
                        //if (hand_hitL.collider.GetComponent<Rigidbody>())
                        //{
                        //    HandlerLL.transform.LookAt(Vector3.Normalize(hand_hitL.collider.GetComponent<Rigidbody>().velocity)
                        //        + (hand_hitL.collider.transform.position));
                        //}
                        //else
                        //{
                            HandlerLL.transform.LookAt(hand_hitL.point);
                        //}
                    }
                    else
                    {
                        HandlerLL.transform.eulerAngles = Vector3.zero;
                        if (hand_hitL.collider.CompareTag(SenceGameObject.AI_TAG) || hand_hitL.collider.CompareTag(SenceGameObject.Weapon_TAG))
                        {
                          //  AI ai = (hand_hitL.collider.GetComponent<SenceGameObject>().SenceObject)as AI;
                            if (hand_hitL.collider.GetComponent<SenceGameObject>()._pc_man !=null
                                && hand_hitL.collider.GetComponent<SenceGameObject>()._pc_man.ai_type == AIType.Stone)
                            {
                            }
                            else
                            {
                                Lock_Lparent = hand_hitL.collider.gameObject;
                            }
                        }
                    }
                    Debug.DrawLine(HandlerRR.transform.position, hand_hitL.point);
                    if (hand_hitL.collider.transform.GetComponent<Image>())
                    {
                        hand_hitL.collider.transform.GetComponent<Image>().color = Color.green;
                        L_image_green = hand_hitL.collider.transform.GetComponent<Image>();
                    }
                    if (Recever.rInstance.left_key)
                    {
                        GameMenu.instance.buttonName = hand_hitL.collider.name;
                    }
                }
            }
            else
            {
                Recever.rInstance.targetL = null;
                if (!Recever.rInstance.player.useWeapon.Name.Equals("weapon3"))
                {
                    HandlerLL.transform.LookAt(handray_tempL.origin + handray_tempL.direction * LookAt.CURR_DISTANCE);
                }
                else
                {
                    HandlerLL.transform.eulerAngles = Vector3.zero;
                }
                if (L_image_green)
                {
                    L_image_green.color = Color.white;
                }
            }
        }

    /// <summary>
        /// 右手射线及UI的处理,并且检测第三种武器时的锁定目标，Lock_Rparent
    /// </summary>
        void RightRay() 
        {
            //右手手射线
            if (Physics.Raycast(handray_tempR, out hand_hitR, 3000f))
            {
                if (hand_hitR.collider.tag == SenceGameObject.AI_TAG || hand_hitR.collider.tag == "UI"
                    || hand_hitR.collider.tag.Equals(SenceGameObject.Weapon_TAG))
                {
                    //第三种子弹时不能让炮台旋转
                    if (!Recever.rInstance.player.useWeapon.Name.Equals("weapon3"))
                    {
                        //区别瞄准的物体
                        //if (hand_hitR.collider.GetComponent<Rigidbody>())
                        //{
                        //    HandlerRR.transform.LookAt(Vector3.Normalize(hand_hitR.collider.GetComponent<Rigidbody>().velocity)
                        //        + (hand_hitR.collider.transform.position));
                        //}
                        //else
                        //{
                            HandlerRR.transform.LookAt(hand_hitR.point);
                        //}
                    }
                    else
                    {
                        HandlerRR.transform.eulerAngles = Vector3.zero;
                        if (hand_hitR.collider.CompareTag(SenceGameObject.AI_TAG) || hand_hitR.collider.CompareTag(SenceGameObject.Weapon_TAG))
                        {
                            AI ai = (hand_hitR.collider.GetComponent<SenceGameObject>().SenceObject) as AI;
                            if (hand_hitR.collider.GetComponent<SenceGameObject>()._pc_man != null
                                && hand_hitR.collider.GetComponent<SenceGameObject>()._pc_man.ai_type == AIType.Stone)
                            {
                            }
                            else
                            {
                                Lock_Rparent = hand_hitR.collider.gameObject;
                            }
                        }
                    }
                    //  UI界面射线检测
                    if (hand_hitR.collider.transform.GetComponent<Image>())
                    {
                        hand_hitR.collider.transform.GetComponent<Image>().color = Color.green;
                        R_image_green = hand_hitR.collider.gameObject.transform.GetComponent<Image>();
                    }
                    if (Recever.rInstance.right_key)
                    {
                        GameMenu.instance.buttonName = hand_hitR.collider.name;
                    }
                }
            }
            else
            {
                Recever.rInstance.targetR = null;
                //第三种子弹时不能让炮台旋转
                if (!Recever.rInstance.player.useWeapon.Name.Equals("weapon3"))
                {
                    HandlerRR.transform.LookAt(handray_tempR.origin + handray_tempR.direction * LookAt.CURR_DISTANCE);
                }
                else
                {
                    HandlerRR.transform.eulerAngles = Vector3.zero;
                }
                Debug.DrawLine(HandlerRR.transform.position, handray_tempR.origin + handray_tempR.direction * LookAt.CURR_DISTANCE);
                if (R_image_green)
                {
                    R_image_green.color = Color.white;
                }
            }
        }


    void Update()
    {
        Onfind();
        LockRightTarget();
        LockLeftTarget();
        ChangeShightBeat();
        LeftRay();
        RightRay();
        /*左右手射线起点和方向*/
        handray_tempR.origin = handcontrollsRR.transform.position;
        handray_tempR.direction = (handcontrollsRR.transform.forward);
        handray_tempL.origin = handcontrollsLL.transform.position;
        handray_tempL.direction = (handcontrollsLL.transform.forward);
        Debug.DrawLine(handray_tempR.origin, handray_tempR.origin * LookAt.FAR_DISTANCE);
        Debug.DrawLine(handray_tempL.origin, handray_tempL.origin * LookAt.FAR_DISTANCE);
    }
}
