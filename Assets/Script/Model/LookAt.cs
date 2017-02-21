using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class LookAt : MonoBehaviour {
    public const float MIN_DISTANCE = 170;
    public const float FAR_DISTANCE = 250;
    public const float CURR_DISTANCE = 70;
    public const float LOCK_TIME = 1;
    //  手柄
    public GameObject ReinObj;
    //  是否有作用
    public bool Used = false;

    //public bool Enable = true;
    LineRenderer line;
    Ray ray;
    RaycastHit hit;
    SenceGameObject obj_data;
    bool hit_is_ai = false;
	// Use this for initialization
	void Start () {
        ray = new Ray();
        _tmpTime_fl = 0;
	}

    GameObject temphit;
    Vector3 tmp_vec;
    Vector3 curr_posi;
    float _tmpTime_fl;
	// Update is called once per frame
	void Update ()
    {
        //Debug.Log("update:" + ReinObj.name + (Used).ToString());
        if (Used && ReinObj != null )
        {
            //Debug.Log("rayrayray" + ReinObj.transform.position);
            ray.origin = ReinObj.transform.position;
            ray.direction = ReinObj.transform.forward;
            Debug.DrawLine(ray.origin, ray.direction.normalized * 3000f, Color.blue);
            if (Physics.Raycast(ray, out hit, 3000))
            {
                Debug.DrawLine(ray.origin, hit.point, Color.green);
                //print(hit.collider.gameObject.name + ":" + hit.collider.gameObject.tag);
                if ((hit.collider.gameObject.tag == SenceGameObject.AI_TAG ||
                    hit.collider.gameObject.tag == SenceGameObject.Weapon_TAG) &&
                    !MsgCenter.Instance.player.useWeapon.Name.Equals("weapon3"))
                {
                    //transform.LookAt(hit.collider.transform.position);
                    obj_data = hit.collider.gameObject.GetComponent<SenceGameObject>();
                    hit_is_ai = true;
                }
                else if ((hit.collider.gameObject.tag == SenceGameObject.AI_TAG ||
                    hit.collider.gameObject.tag == SenceGameObject.Weapon_TAG) &&
                    MsgCenter.Instance.player.useWeapon.Name.Equals("weapon3"))
                {
                    obj_data = hit.collider.gameObject.GetComponent<SenceGameObject>();
                    hit_is_ai = true;
                }
                else if (hit.collider.gameObject.tag == SenceGameObject.UI_TAG &&
                    (hit.collider.gameObject.name.Equals("NameButton") ||
                    hit.collider.gameObject.name.Equals("CreatPlayerButton") ||
                    hit.collider.gameObject.name.Equals("StartGameButton") ||
                    hit.collider.gameObject.name.Equals("RedoButton") ||
                    hit.collider.gameObject.name.Equals("ExitButton"))
                    )
                {
                    hit.collider.GetComponent<Image>().color = Color.green;
                    temphit = hit.collider.gameObject;
                    if (MsgCenter.Instance.player.RightRein.Key || MsgCenter.Instance.player.LeftRein.Key)
                    {

                        //Debug.Log("IFIFIFIFIIFIFIFIIFIFIF");
                        MsgCenter.Instance.Menu.Click(hit.collider.gameObject.name);
                    }
                }
                if (!MsgCenter.Instance.player.useWeapon.Name.Equals("weapon3"))
                {
                    //Debug.Log("lookatlookatlookat"+ReinObj.transform.position);
                    transform.LookAt(ReinObj.transform.forward.normalized * 1000);
                }
                else
                {
                 //   Debug.Log("elseelseelse"); 
                    transform.forward = Vector3.forward;
                }
            }
            else
            {
                if (temphit != null)
                {
                    temphit.GetComponent<Image>().color = Color.white;
                }
              //  Debug.Log("elseflselfsleflsfklsekflks");
                if (this.gameObject.name.Equals("FireLL"))
                {
                    //MsgCenter.Instance.LeftHub.transform.localScale = Vector3.one;
                    MsgCenter.Instance.LeftHub.transform.position = 
                        ReinObj.transform.forward * MIN_DISTANCE + ReinObj.transform.position;
                }
                else
                {
                    //MsgCenter.Instance.RightHub.transform.localScale = Vector3.one;
                    MsgCenter.Instance.RightHub.transform.position =
                        ReinObj.transform.forward * MIN_DISTANCE + ReinObj.transform.position;
                }
                if (!MsgCenter.Instance.player.useWeapon.Name.Equals("weapon3"))
                {
                    transform.LookAt(ReinObj.transform.forward.normalized * 1000);
                }
                else 
                {
                    transform.forward = Vector3.forward;
                }
            }
            if (hit_is_ai)
            {
                if (this.gameObject.name.Equals("FireLL"))
                {
                    MsgCenter.Instance.left_fire_target = obj_data;
                }
                else
                {
                    MsgCenter.Instance.right_fire_target = obj_data;
                }
                hit_is_ai = false;
            }
            //  判断是weapon3时
            if (MsgCenter.Instance.player.useWeapon.Name.Equals("weapon3"))
            {
                //if (obj_data != null)
                //{
                //    Debug.Log((this.gameObject.name.Equals("FireLL") ? "left.isVisible = " : "right.isVisible = ") + obj_data.isVisible);
                //}

                if (obj_data != null)
                {
                    _tmpTime_fl += Time.deltaTime;
                    if (_tmpTime_fl < LOCK_TIME)
                    {
                        if (this.gameObject.name.Equals("FireLL"))
                        {
                            MsgCenter.Instance.LeftLockHub.transform.position =
                                (obj_data.transform.position - ReinObj.transform.position).normalized * CURR_DISTANCE
                                + ReinObj.transform.position;
                            if ( !MsgCenter.Instance.LeftLockHub.activeSelf)
                            {
                                MsgCenter.Instance.LeftLockHub.SetActive(true);
                                Audiocontroller.instans.PlayBossAudio("suoding");
                            }
                            //Debug.Log("dis……");
                        }
                        else
                        {
                            MsgCenter.Instance.RightLockHub.transform.position =
                                (obj_data.transform.position - ReinObj.transform.position).normalized * CURR_DISTANCE
                                + ReinObj.transform.position;
                            if (!MsgCenter.Instance.RightLockHub.activeSelf)
                            {
                                MsgCenter.Instance.RightLockHub.SetActive(true);
                                Audiocontroller.instans.PlayBossAudio("suoding");
                            }
                            //Debug.Log("dis……");
                        }
                    }
                    else 
                    {
                        _tmpTime_fl = 0;
                        obj_data = null;
                    }
                }
                else
                {
                    if (this.gameObject.name.Equals("FireLL"))
                    {
                        MsgCenter.Instance.LeftLockHub.SetActive(false);
                        MsgCenter.Instance.left_fire_target = null;
                    }
                    else
                    {
                        MsgCenter.Instance.RightLockHub.SetActive(false);
                        MsgCenter.Instance.right_fire_target = null;
                    }
                }
            }
            else 
            {
                if (this.gameObject.name.Equals("FireLL"))
                {
                    MsgCenter.Instance.LeftLockHub.SetActive(false);
                    MsgCenter.Instance.left_fire_target = null;
                }
                else
                {
                    MsgCenter.Instance.RightLockHub.SetActive(false);
                    MsgCenter.Instance.right_fire_target = null;
                }
            }
            RefresCurrentPo();
        }
	}

    private Vector3 _tmp_V3;
    public void RefresCurrentPo() 
    {
        if (MsgCenter.Instance.GameState != Datas.GameState.Playing)
        {
            //Debug.Log("startstart");
            _tmp_V3 = ReinObj.transform.forward * MIN_DISTANCE + ReinObj.transform.position;
        }
        else
        {
            _tmp_V3 = ReinObj.transform.forward * FAR_DISTANCE + ReinObj.transform.position;
        }

        //ReinObj.transform.position = _tmp_V3;
        ////_tmp_V3 = ReinObj.transform.forward * MIN_DISTANCE + ReinObj.transform.position;
        ////_tmp_V3.y -= 3;
        if (this.gameObject.name.Equals("FireLL"))
        {
            //Debug.Log("FireLL:" + _tmp_V3);
            MsgCenter.Instance.LeftHub.transform.position = _tmp_V3;
        }
        else
        {
            //Debug.Log("FireRR:" + _tmp_V3);
            MsgCenter.Instance.RightHub.transform.position = _tmp_V3;
        }
    }
}
