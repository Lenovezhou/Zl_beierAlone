using UnityEngine;
using System.Collections;

public class FirePointcontroller : MonoBehaviour {
    public bool inInit = false;
    public bool isServer = false;
    private GameObject firePoint_Flower;
    private float timer;

    public const float LIFE_TIME = 0.15f;
	// Use this for initialization
	void Start () {
        
	}

    public void Init(bool _isServer,bool canfire) 
    {
        if (_isServer)
        {
            firePoint_Flower = Instantiate(MsgCenter.Instance.fire_point_particle) as GameObject;
        }
        else
        {
            firePoint_Flower = Instantiate(Recever.rInstance.fire_point_particle) as GameObject;
        }

        //   firePointR = Instantiate(Recever.rInstance.fire_point_particle) as GameObject;
        firePoint_Flower.SetActive(false);
        //   firePointR.SetActive(false);
        firePoint_Flower.transform.parent = this.transform;
        firePoint_Flower.transform.localPosition = Vector3.zero;
        firePoint_Flower.transform.forward = transform.parent.forward;
        firePoint_Flower.transform.position = new Vector3(firePoint_Flower.transform.position.x,
            firePoint_Flower.transform.position.y, firePoint_Flower.transform.position.z + 0.3f);
        firePoint_Flower.transform.eulerAngles = new Vector3(0, 270f, 0);
        inInit = canfire;
        isServer = _isServer;
    }

    void OnEnable() 
    {
       
    }
   
	void Update () {
        if (isServer )
        {
            if(MsgCenter.Instance.GameState == Datas.GameState.Playing 
                && inInit
                && MsgCenter.Instance.player != null
                && MsgCenter.Instance.player.useWeapon.Name != "weapon3")
            {
                if (this.transform.parent.name.Equals("FireLL"))
                {
                    if (MsgCenter.Instance.left_key && !firePoint_Flower.activeSelf)
                    {
                        firePoint_Flower.SetActive(true);
                    }
                    if (firePoint_Flower.activeSelf)
                    {
                        timer += Time.deltaTime;
                        if (timer > LIFE_TIME)
                        {
                            timer = 0;
                            firePoint_Flower.SetActive(false);
                        }
                    }
                }
                else
                {
                    if (MsgCenter.Instance.right_key && !firePoint_Flower.activeSelf)
                    {
                        firePoint_Flower.SetActive(true);
                    }
                    if (firePoint_Flower.activeSelf)
                    {
                        timer += Time.deltaTime;
                        if (timer > LIFE_TIME)
                        {
                            timer = 0;
                            firePoint_Flower.SetActive(false);
                        }
                    }
                }
            }
            
        }
        else 
        {
            //Debug.Log(GameMenu.instance.muenu_state==null);
            if (inInit &&
                Recever.rInstance.GameState == Datas.GameState.Playing &&
                Recever.rInstance.player  != null 
                && Recever.rInstance.player.useWeapon.Name != "weapon3")
            {
                if (this.transform.parent.name.Equals("FireLL"))
                {
                    if (Recever.rInstance.left_key && !firePoint_Flower.activeSelf)
                    {
                        firePoint_Flower.SetActive(true);
                    }
                    if (firePoint_Flower.activeSelf)
                    {
                        timer += Time.deltaTime;
                        if (timer > LIFE_TIME)
                        {
                            timer = 0;
                            firePoint_Flower.SetActive(false);
                        }
                    }
                }
                else
                {

                    if (Recever.rInstance.right_key && !firePoint_Flower.activeSelf)
                    {
                        firePoint_Flower.SetActive(true);
                    }
                    if (firePoint_Flower.activeSelf)
                    {
                        timer += Time.deltaTime;
                        if (timer > LIFE_TIME)
                        {
                            timer = 0;
                            firePoint_Flower.SetActive(false);
                        }
                    }
                }
            }
            
        }
	}
}
