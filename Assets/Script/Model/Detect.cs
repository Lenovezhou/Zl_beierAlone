using UnityEngine;
using System.Collections;

public class Detect : MonoBehaviour {
    private SenceGameObject obj_data;
    private float _timer = 0;
    private float _clear_time = 3;
    private int _sum = 0;
    public const int MAX_SUM = 5;
    public void OnTriggerEnter(Collider coll) 
    {
        //Debug.Log("OnTriggerEnter");
        if(coll.gameObject.CompareTag(SenceGameObject.AI_TAG) ||
            coll.gameObject.CompareTag(SenceGameObject.AI_STONE))
        {
            _sum++;
            obj_data = coll.gameObject.GetComponent<SenceGameObject>();
            if (_sum >= MAX_SUM || obj_data._pc_man.ai_type == Datas.AIType.LifeAndAttack || obj_data._pc_man.ai_type == Datas.AIType.Stone)
            {
                _sum = 0;
                if (MsgCenter.Instance != null)
                {
                    MsgCenter.Instance.ShowWarming(obj_data._pc_man.ai_type);
                }
                else
                {
                    Recever.rInstance.ShowWarming(obj_data._pc_man.ai_type);
                }
            }
        }
    }

    void Update() 
    {
        _timer += Time.deltaTime;
        if (_timer >= _clear_time)
        {
            _sum = 0;
        }
    }
}
