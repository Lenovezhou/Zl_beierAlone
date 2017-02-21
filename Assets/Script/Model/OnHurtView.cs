using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class OnHurtView : MonoBehaviour {
    public GameObject Img;
    public GameObject shaketarget;
    private GameObject img;

    private Vector3 originPosition;
    private Quaternion originRotation;
    public float shake_decay;
    public float shake_time;
    public float shake_intensity;

    private Vector3 last_position;
    private float _time;
    bool isShake = false;
    private SenceGameObject tmp_obj_data;
    private Ray ray;
    private RaycastHit hit;
    void Start() 
    {
        _time = 0;
        last_position = transform.position;
    }

    void Update() 
    {   
            //只震动shake_time时间
        if (shake_intensity > 0)
            {
               // _time += Time.deltaTime;
               // Recever.rInstance.PlayerData.transform.position = originPosition + Random.insideUnitSphere * shake_intensity;
              shaketarget.transform.rotation = new Quaternion(
                 originRotation.x + Random.Range(-shake_intensity, shake_intensity) * 0.2f,
                 originRotation.y + Random.Range(-shake_intensity, shake_intensity) * 0.2f,
                 originRotation.z + Random.Range(-shake_intensity, shake_intensity) * 0.2f,
                 originRotation.w + Random.Range(-shake_intensity, shake_intensity) * 0.2f);
                shake_intensity -= shake_decay;
            }
            else {
                _time = 0;
           //     Debug.Log("开始Lerp"+Camera.main.transform.position);
           // transform.localPosition = Vector3.Lerp(transform.localPosition,
           //     new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y, Camera.main.transform.position.z + 0.1f), 0.05f);
           // Vector3 templook = new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y + 2f, Camera.main.transform.position.z + 0.1f) -
           //transform.position;
           //     // 让飞机始终看向主角头上2f的距离
           //      transform.up = templook;
            }
        }

    void Shake()
    {
      //  MsgCenter.Instance.IsShaking = true;
        originPosition = transform.position;
        originRotation = transform.rotation;
        shake_intensity = 0.05f;
        shake_decay = 0.002f;
    }

    void OnTriggerEnter(Collider coll) 
    {
        if(coll.CompareTag(SenceGameObject.AI_BULLET_TAG) ||
            coll.CompareTag(SenceGameObject.AI_TAG))  
        {
            tmp_obj_data = coll.gameObject.GetComponent<SenceGameObject>();
            if (MsgCenter.Instance != null)
            {
                Shake();
                //可以添加护盾音效
                Audiocontroller.instans.PlayEffectAudio("hudun");/*被敌人击中时发出的声音*/
                if(MsgCenter.Instance.HurtPoints.ContainsKey(tmp_obj_data.SenceObject.ID))
                {
                    img = Instantiate(Img) as GameObject;
                    img.gameObject.transform.position = 
                        MsgCenter.Instance.HurtPoints[tmp_obj_data.SenceObject.ID];
                }
                else
                {
                    //Debug.LogError("server hurt err:" + tmp_obj_data.gameObject.name);
                }
            }
            else 
            {
               // Debug.Log("<color=yellow>击中" + tmp_obj_data.SenceObject + "</color>");
                img = Instantiate(Img) as GameObject;
                if (Recever.rInstance.HurtPoints.ContainsKey(tmp_obj_data.SenceObject.ID))
                {
                    img.gameObject.transform.position = Recever.rInstance.HurtPoints[tmp_obj_data.SenceObject.ID];
                }
                else
                {
                    ray = new Ray(tmp_obj_data.transform.position, transform.position - tmp_obj_data.transform.position);
                    if (Physics.Raycast(ray, out hit))
                    {
                        img.gameObject.transform.position = hit.point;
                    }
                }
            }
            if (img != null)
            {
                img.gameObject.name = "Hurt";
                img.gameObject.transform.LookAt(Camera.main.transform);
                img.gameObject.SetActive(true);
                isShake = true;
            }
        }
    }
}
