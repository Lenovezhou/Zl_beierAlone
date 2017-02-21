using UnityEngine;
using System.Collections;
using Datas;

public class AI_Logic : MonoBehaviour
{

    public const float RANGE = 4;

    private Ray _ray;
    private RaycastHit _hit;

    public void OnLogic(SenceGameObject target_data, GameObject LeftFirePoint, GameObject RightFirePoint)
    {
        switch (target_data._pc_man.ai_type)
        {
            case AIType.Lithe:
                AIWalkLine(target_data, LeftFirePoint, RightFirePoint);
                break;
            case AIType.Attack:
                AIWalkWinding(target_data, LeftFirePoint, RightFirePoint);
                break;
            case AIType.LifeAndAttack:
                BossWalk(target_data, LeftFirePoint, RightFirePoint);
                break;
            case AIType.SmartAI:
                AIWalkNew(target_data, LeftFirePoint, RightFirePoint);
                break;
            case AIType.Stone:
                StoneWalkLine(target_data, LeftFirePoint, RightFirePoint);
                break;
        }

        target_data._pc_man.Position = transform.position;
        target_data._pc_man.Rotation = transform.rotation;
    }

    public void LuachBullet(SenceGameObject target_data, GameObject FirePoint)
    {
        Bullet bullet = new Bullet(-1, target_data._pc_man.useWeapon.Name, target_data._pc_man.Attack,
                        FirePoint.transform.position, FirePoint.transform.rotation,
                        ObjectState.None, ObjectType.Bullet, true);
        SenceGameObject bullet_data = MsgCenter.Instance.CreateBullet(bullet, target_data._pc_man.ID, true);
        target_data.LeftFlower.SetActive(true);
        target_data.RightFlower.SetActive(true);
        switch (target_data._pc_man.Index)
        {
            case 0:
                Audiocontroller.instans.PlayGameAudio("Ss01");/*第一种AI发射子弹声音*/
                break;
            case 1:
                Audiocontroller.instans.PlayGameAudio("Ss02");/*第二种AI发射子弹声音*/
                break;
            case 2:
                Audiocontroller.instans.PlayGameAudio("kehuan");/*第三种AI发射子弹声音*/
                break;
            default:
                Debug.LogError("AI发射子弹对应声音错误 ");
                break;
        }
        Rigidbody rig1 = bullet_data.gameObject.GetComponent<Rigidbody>();
        bullet_data.transform.position = FirePoint.transform.position;
        bullet_data.gameObject.SetActive(true);
        _ray = new Ray(FirePoint.transform.position, MsgCenter.Instance.PlayerData.transform.position - FirePoint.transform.position);
        if(Physics.Raycast(_ray,out _hit,10000))
        {
            RaycastHit[] casthits = Physics.RaycastAll(_ray);
            if (casthits[0].collider.CompareTag(SenceGameObject.PLAYER_TAG))
            {
                if (MsgCenter.Instance.HurtPoints.ContainsKey(bullet_data.SenceObject.ID))
                {
                    MsgCenter.Instance.HurtPoints[bullet_data.SenceObject.ID] = casthits[0].point;
                }
                else 
                {
                    MsgCenter.Instance.HurtPoints.Add(bullet_data.SenceObject.ID, casthits[0].point);
                }
            }      
            else if (casthits[1].collider.CompareTag(SenceGameObject.PLAYER_TAG)) 
            {
                if (MsgCenter.Instance.HurtPoints.ContainsKey(bullet_data.SenceObject.ID))
                {
                    MsgCenter.Instance.HurtPoints[bullet_data.SenceObject.ID] = casthits[1].point;
                }
                else
                {
                    MsgCenter.Instance.HurtPoints.Add(bullet_data.SenceObject.ID, casthits[1].point);
                }
            }
        }
        rig1.AddForce(FirePoint.transform.forward * MsgCenter.LUACH_FORCE);
        //Debug.Log(bullet_data.gameObject.name);
    }

    public void AIWalkLine(SenceGameObject target_data, GameObject LeftFirePoint, GameObject RightFirePoint)
    {
        
        //if ((target_data.transform.position - target_data.ai_end_point).magnitude <= 5)
        //{
        //    MsgCenter.Instance.DestroySenceObject(target_data, 1.0f);
        //}

        //  如果到达第一个目标点
        if ((target_data.transform.position - target_data.follow.Target).magnitude <= 20 &&
            !target_data.ai_in_end)
        {
            target_data.follow.Target = target_data.ai_end_point;
            target_data.ai_in_end = true;
        }

        if (!target_data.ai_in_end)
        {
            target_data.luach_time += Time.deltaTime;
            //Debug.Log(target_data.luach_time + " <= " + target_data._pc_man.LaunchSpeed);
            if (target_data.luach_time >= target_data._pc_man.LaunchSpeed)
            {

                target_data.luach_time = 0;
                LuachBullet(target_data, LeftFirePoint);
                //target_data.LeftFlower.SetActive(true);
                if (target_data.RightFirePoint != null)
                {
                    LuachBullet(target_data, RightFirePoint);
                    //target_data.RightFlower.SetActive(true);
                }
            }
        }
    }

    public void StoneWalkLine(SenceGameObject target_data, GameObject LeftFirePoint, GameObject RightFirePoint)
    {

        if ((target_data.transform.position - target_data.ai_end_point).magnitude <= 5)
        {
            MsgCenter.Instance.DestroySenceObject(target_data, 1.0f);
        }

        //  如果到达第一个目标点
        if ((target_data.transform.position - target_data.follow.Target).magnitude <= 20 &&
            !target_data.ai_in_end)
        {
            target_data.follow.Target = target_data.ai_end_point;
            target_data.ai_in_end = true;
        }
        //Debug.Log(target_data.gameObject.name + "::::" + (target_data.transform.position - MsgCenter.Instance.PlayerData.transform.position).magnitude);
        if (!target_data.ai_in_end && (target_data.transform.position - MsgCenter.Instance.PlayerData.transform.position).magnitude <= 10)
        {
            //Debug.Log("<<<<<<<<<<<<<<");
            _ray = new Ray(target_data.transform.position, target_data.transform.forward);
            if (Physics.Raycast(_ray, out _hit, 1000))
            {
                //Debug.Log("hit.name = " + _hit.transform.gameObject.name);
                if (_hit.transform.gameObject.CompareTag(SenceGameObject.PLAYER_TAG))
                {
                    Debug.Log("hit = " + _hit.transform.gameObject.name);
                    MsgCenter.Instance.HurtPoints.Add(target_data.SenceObject.ID, _hit.point);
                }
            }
        }
        
    }
    /// <summary>
    /// 停在某个位置后  朝向玩家发射子弹
    /// </summary>
    /// <param name="target_data"></param>
    /// <param name="LeftFirePoint"></param>
    /// <param name="RightFirePoint"></param>
    public void AIWalkWinding(SenceGameObject target_data, GameObject LeftFirePoint, GameObject RightFirePoint)
    {
        if (target_data.ai_in_end &&
            !target_data.follow.ToStartPosition &&
            (target_data.transform.position - target_data.ai_end_point).magnitude <= 5)
        {
            target_data.follow.degree++;
            target_data.follow.ToStartPosition = true;
            target_data.follow.Target = target_data.ai_start_position;
            if (target_data.follow.degree >= 3)
            {
                //Debug.Log("destroy : target_data.follow.degree >= 3");
                MsgCenter.Instance.DestroySenceObject(target_data, 1.0f);
            }
        }

        if (target_data.ai_in_end &&
            (target_data.transform.position - target_data.ai_start_position).magnitude <= 5)
        {
            //target_data.follow.degree++;
            target_data.follow.ToStartPosition = false;
            target_data.follow.Target = target_data.ai_target_point;
            target_data.ai_in_end = false;
        }
        //  如果到达第一个目标点
        if ((target_data.transform.position - target_data.follow.Target).magnitude <= 20 &&
            !target_data.ai_in_end)
        {
            target_data.follow.Target = target_data.ai_end_point;
            target_data.ai_in_end = true;
        }

        if (!target_data.ai_in_end)
        {
            target_data.luach_time += Time.deltaTime;
            //Debug.Log(target_data.luach_time + " <= " + target_data._pc_man.LaunchSpeed);
            if (target_data.luach_time >= target_data._pc_man.LaunchSpeed)
            {

                target_data.luach_time = 0;
                LuachBullet(target_data, LeftFirePoint);
                //target_data.LeftFlower.SetActive(true);
                if (target_data.RightFirePoint != null)
                {
                    LuachBullet(target_data, RightFirePoint);
                    //target_data.RightFlower.SetActive(true);
                }
            }
        }
    }


    public void AIWalkNew(SenceGameObject target_data, GameObject LeftFirePoint, GameObject RightFirePoint)
    {
        //Debug.Log(Vector3.Distance(target_data.follow.transform.position, target_data.follow.EndPos));
        if (Vector3.Distance(target_data.follow.transform.position,target_data.follow.EndPos)<10.0f&& 
            !target_data.follow.IsBack)
        {
            target_data.follow.isTurn = false;
            target_data.follow.Target = target_data.follow.UPPos;
        }
        if (Vector3.Distance(target_data.follow.transform.position,target_data.follow.UPPos)<10.0f)
        {
            target_data.follow.IsBack = true;
            target_data.follow.Target = target_data.follow.StartPos;
        }

        if (Vector3.Distance(target_data.follow.transform.position,target_data.follow.StartPos)<10.0f&& target_data.follow.IsBack)
        {
            target_data.follow.isTurn = true;
            target_data.follow.PosRange();
            target_data.follow.IsBack = false;
            //if (target_data.follow.EndPos.z > 0)
            //{
                //target_data.follow.EndPos = new Vector3(target_data.follow.EndPos.x, target_data.follow.EndPos.y, -target_data.follow.EndPos.z);
            //}
            target_data.follow.Target = target_data.follow.EndPos;
        }
        float tempAngle=Vector3.Angle(-target_data.follow.transform.forward, Vector3.forward);
        if (!target_data.ai_in_end && !target_data.follow.IsBack&&target_data.follow.transform.position.z>0&&
            target_data.follow.state == AIState.Attack && (tempAngle > 0 && tempAngle < 20))
        {
            target_data.luach_time += Time.deltaTime;
            //Debug.Log(target_data.luach_time + " <= " + target_data._pc_man.LaunchSpeed);
            if (target_data.luach_time >= target_data._pc_man.LaunchSpeed)
            {
                target_data.luach_time = 0;
                LuachBullet(target_data, LeftFirePoint);
                //target_data.LeftFlower.SetActive(true);
                if (target_data.RightFirePoint != null)
                {
                    LuachBullet(target_data, RightFirePoint);
                    //target_data.RightFlower.SetActive(true);
                }
            }
        }
    }
    bool isone=false;
    Vector3 temp;
    public void BossWalk(SenceGameObject target_data, GameObject LeftFirePoint, GameObject RightFirePoint)
    {
        //  如果到达第一个目标点
        //if (target_data.Stone.enabled &&
        //    (target_data.transform.position - target_data.Stone.Tartget).magnitude <= 30)
        //{
        //    target_data.Stone.enabled = false;
        //    target_data.ai_in_end = true;
        //    //Debug.Log("target_data.Stone.enabled = false;");
        //}
        if (!isone)
        {
            temp = target_data.follow.Target;
        }
        if (target_data.follow.enabled)
        {
			if (Vector3.Distance(target_data.transform.position, target_data.follow.EndPos) <= 2 &&
                !target_data.follow.IsBack)
			{
				target_data.follow.RunSpeed = 0.1f;
				target_data.follow.RotateSpeed = 0.01f;
                target_data.follow.isTurn = true;
				target_data.follow.Target = target_data.follow.UPPos;
                target_data.follow.IsBack = true;
            }
            else if (target_data.follow.IsBack && Vector3.Distance(target_data.transform.position, target_data.follow.Target) <= 2)
            {
             //   target_data.follow.Target = target_data.follow.Target == MsgCenter.Instance.EndPosition.position ? temp : MsgCenter.Instance.EndPosition.position;
                //target_data.follow.enabled = false;
                target_data.follow.isTurn = true;
				target_data.follow.Target = target_data.follow.Target ==target_data.follow.EndPos?target_data.follow.UPPos:target_data.follow.EndPos;
            }          
            //Debug.Log("target_data.Stone.enabled = false;");
        }
        else 
        {
            target_data.transform.eulerAngles =new Vector3( target_data.transform.eulerAngles.x,Mathf.Lerp(target_data.transform.eulerAngles.y,
                MsgCenter.Instance.EndPosition.eulerAngles.y, 0.002f),target_data.transform.eulerAngles.z);
        }
        
        if (target_data.follow.IsBack)
        {
            target_data.luach_time += Time.deltaTime;
            //Debug.Log(target_data.luach_time + " <= " + target_data._pc_man.LaunchSpeed);
            if (target_data.luach_time >= target_data._pc_man.LaunchSpeed)
            {
                target_data.luach_time = 0;
                LuachBullet(target_data, LeftFirePoint);
                //target_data.LeftFlower.SetActive(true);
                if (target_data.RightFirePoint != null)
                {
                    LuachBullet(target_data, RightFirePoint);
                    //target_data.RightFlower.SetActive(true);
                }
            }
        }
    }
}
