using UnityEngine;
using System.Collections;

public enum AIState 
{
    Attack,
    None
}

public class Follo : MonoBehaviour
{
    public bool IsBack = false;
    public Transform FlyTemp;
    public float off_EndX = 10f;
    public float off_UpX = 100;//offset_Y,offset_Y,offset_Y,offset_Y,offset_Y,;
    public float off_UpY_min = 10;
    public float off_UpY_max = 20;
    public Vector3 StartPos;
    public Vector3 EndPos;
    public Vector3 UPPos;
    public Vector3 Target;
    public float RunSpeed = 0.6f;
    public float RotateSpeed = 0.03f;
    GameObject clone;

    public bool ToStartPosition;
    public int degree = 0;
    public AIState state;
    public float m_time,clamptime=5f;

    public float lataRota;
    public float nowRota;
    public Transform Up;
    public bool isTurn;
    public bool isReturn;
    public float speed = 1f;

    private Datas.AIType aitype;


    public virtual void Start()
    {
        state = AIState.Attack;
        aitype = gameObject.GetComponent<SenceGameObject>()._pc_man.ai_type;
        if (Recever.rInstance != null && aitype != Datas.AIType.LifeAndAttack)
        {
            StartPos = transform.position;
        }
        else if (Recever.rInstance != null && aitype == Datas.AIType.LifeAndAttack)
        {
            StartPos = Recever.rInstance.StartPosition.position;
            EndPos = Recever.rInstance.EndPosition.position;
        }
        else if(MsgCenter.Instance != null) {
            StartPos = transform.position;
        }
        //Debug.Log("1111111111111111111111111AIType.SmartAI" + StartPos);
        clone = new GameObject();
        clone.transform.SetParent(this.transform);
        clone.transform.localPosition = Vector3.zero;
        FlyTemp = this.transform.FindChild("Enemy");
		if (gameObject.GetComponent<SenceGameObject> ()._pc_man.ai_type == Datas.AIType.LifeAndAttack) 
        {
			RotateSpeed = 0.01f;
            Target = EndPos;  /*为了changetarget()*/
		} 
        else 
        {
			transform.eulerAngles = new Vector3(0, Random.Range(180, 360), 0);
			PosRange();
            Target = EndPos;
		}
    }

    private void ChangeTarget() 
    {
        Target = Target == StartPos ? EndPos : StartPos;
    }

    // Update is called once per frame
    public void Update()
    {
        //Debug.Log(lataRota + "<<lataRota " + nowRota + "  <<<<  now  " + (lataRota - nowRota));
        Fly();
        nowRota = transform.localEulerAngles.y;
        if (Target == EndPos)
        {
            state = AIState.Attack;
        }
        else
        {
            state = AIState.None;
        }
        //if (transform.position.z <= EndPos.z&& IsBack == false)
        //{
        //    Target = UPPos;
        //}
        //if (transform.position.z <= 0)
        //{
        //    IsBack = true;
        //    Target = StartPos;
        //}
        //if (transform.position.z >= StartPos.z&&IsBack)
        //{
        //    PosRange();
        //    IsBack = false;
        //    Target = EndPos;
        //}
    }

    void LateUpdate()
    {
		lataRota = transform.localEulerAngles.y;
    }
    public virtual void PosRange()
    {
        Vector3 temp = Camera.main.transform.position;
        EndPos = new Vector3(Random.Range(temp.x - off_EndX, temp.x + off_EndX), Random.Range(temp.y + 0, temp.y + 0), Random.Range(temp.z + 40, temp.z + 60));
        UPPos = new Vector3(Random.Range(temp.x - off_UpX, temp.x + off_UpX), Random.Range(temp.y - 10, temp.y + 20), Random.Range(temp.z - 50, temp.z - 100));
        if (UPPos.y < 5 && UPPos.y >= 0)
        {
            UPPos.y += 10;
        }
        else if (UPPos.y > -5 && UPPos.y < 0)
        {
            UPPos.y -= 10;
        }
    }

    public virtual void Fly()
    {
        Incline();
    }

    bool ischange = false;

    void Incline()
    {

        
        clone.transform.LookAt(Target);
        if (MsgCenter.Instance != null && MsgCenter.Instance.GameState == Datas.GameState.Playing)
        {
            transform.Translate(new Vector3(0, 0, RunSpeed));
            transform.rotation = Quaternion.Lerp(transform.rotation, clone.transform.rotation, RotateSpeed);
        }
        else if (Recever.rInstance != null && Recever.rInstance.GameState == Datas.GameState.Playing)
        {
            if (m_time < clamptime + 1f)
            {
                m_time += Time.deltaTime;
            }
            if (aitype != Datas.AIType.LifeAndAttack)
            {
                if (m_time > clamptime && (Vector3.Distance(transform.position, StartPos) <= 5f))
                {
                    Debug.Log("1"+this.name);
                    isTurn = true;
                }
            }
            else
            {
                if (m_time > clamptime && (Vector3.Distance(transform.position, StartPos) <= 5f || Vector3.Distance(transform.position, EndPos) <= 5f))
                {
                    Debug.Log("Boss倾斜");
                    if (ischange)
                    {
                        ChangeTarget();
                        ischange = false;
                    }
                    isTurn = true;
                }
                else if (!ischange)
                {
                    ischange = true;
                }
            }
        }
        nowRota = transform.localEulerAngles.y;
        float temp = Vector3.Angle(transform.forward, clone.transform.forward);

        if (isTurn)
        {
            float tt = FlyTemp.localEulerAngles.z;
            if (temp > 20)
            {
                /* latarota在LateUpdate中赋值
                 * nowRota在 update中赋值
                 */
                if (lataRota - nowRota > 0)
                {
                    tt += speed;// Mathf.Lerp(tt, 45, speed);
                }
                else
                {
                    //tt = Mathf.Lerp(tt, -50, 0.001f);
                    tt -= speed;
                }
                // Debug.Log("TT " + tt);
                if (tt < 320 && tt > 180)
                {
                    tt = 320;
                    //Debug.Log("111111111111111111111");
                }
                if (tt > 40 && tt < 180)
                {
                    //Debug.Log("22222222222222222222");
                    tt = 40;
                }
            }
            else
            {
                if (Mathf.Abs(lataRota - nowRota) < 1)
                {
                    //Debug.Log("Lerp  0000");
                    if (tt > 0 && tt < 180)
                    {
                        tt = Mathf.Lerp(tt, 0, 0.02f);
                    }
                    if (tt > 180 && tt < 360)
                    {
                        tt = Mathf.Lerp(tt, 360, 0.02f);
                    }
                    if (tt - 0 <= 3f)
                    {
                        tt = 0;
                        isTurn = false;
                    }
                }
            }
            FlyTemp.localEulerAngles = new Vector3(0, 0, tt);
        }
    }
 }
