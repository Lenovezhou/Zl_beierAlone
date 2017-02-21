using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Datas;
public class HP_View : MonoBehaviour {
    public Role role;
    float maxHp;
    Slider hpSlider;
    TextMesh grade;
    Image roleimage;
    Image s_image;
    float learny;
	public void Init () {
        if (transform.FindChild("_player") != null)
        {
            hpSlider = transform.FindChild("_player").FindChild("Canvas").FindChild("Slider").GetComponent<Slider>();
            roleimage = hpSlider.transform.FindChild("Fill Area").FindChild("Fill").GetComponent<Image>();
            s_image = hpSlider.transform.FindChild("HPImage").GetComponent<Image>();
        }
        else if (transform.FindChild("Canvas") != null)
        {
            hpSlider = transform.FindChild("Canvas").FindChild("Slider").GetComponent<Slider>();
        }

        hpSlider.maxValue = role.HP;
        if (MsgCenter.Instance != null) {
            learny = hpSlider.maxValue / MsgCenter.Instance.PlayerHP.Length;
        } else {
            learny = hpSlider.maxValue / Recever.rInstance.PlayerHP.Length;
        }
       
        Transform tran = transform.FindChild("Grade");
        if (tran != null)
        {
            grade = tran.GetComponent<TextMesh>();
        }
	}
    int hp_index;
	void Update () 
    {
        if (hpSlider != null)
        {
            if (this.gameObject.tag.Equals(SenceGameObject.PLAYER_TAG))
            {
                if (role.HP > hpSlider.maxValue)
                {
                    role.HP =(int)hpSlider.maxValue;
                }
//				Debug.Log (role.HP);
                hp_index = (int)(role.HP / learny);

                if (MsgCenter.Instance != null 
                    //&&
                    //roleimage.overrideSprite.name != MsgCenter.Instance.PlayerHP[hp_index].name
                    )
                {
                    if (s_image)
                    {
                        s_image.overrideSprite = MsgCenter.Instance.PlayerHP[hp_index];
                    }
                    roleimage.overrideSprite = MsgCenter.Instance.PlayerHP[hp_index];
                }
                else if (Recever.rInstance != null 
                    //&& roleimage.overrideSprite.name != Recever.rInstance.PlayerHP[hp_index].name
                    )
                {
                    if (s_image)
                    {
                        s_image.overrideSprite = Recever.rInstance.PlayerHP[hp_index];
                    }
                   
                    roleimage.overrideSprite = Recever.rInstance.PlayerHP[hp_index];
                }
            }
            else
            {
                //Debug.Log("HPHPHPHPHP");
                if (this.gameObject.CompareTag(SenceGameObject.AI_TAG))
                {
                    Datas.AI ai = (this.gameObject.GetComponent<SenceGameObject>().SenceObject) as AI;
                    if (ai.ai_type.Equals(AIType.LifeAndAttack))
                    {
                        //Debug.Log("BOSSBOSSBOSSBOSSBOSS+"+role.HP);
                    }
                }
                hpSlider.value = role.HP;
            }
        }

        if (grade != null)
        {
            grade.text = role.Grade.ToString();
        }
	}
}
