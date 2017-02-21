using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class ChangeWeapon : MonoBehaviour
{
    private Dictionary<string, GameObject> weaponList = new Dictionary<string, GameObject>();
    private bool IsRight;
    private GameObject currWeapon;
    void Start()
    {
        Messenger.AddListener<string>(GameEvent.ChangeWeapon, ChangeValue);
        Messenger.AddListener<string>(GameEvent.ClientChangeWeapon, ChangeValue111);
        Messenger.AddListener<string>(GameEvent.DisplayWeapon, DisplayWeapon);
        Messenger.AddListener<string>(GameEvent.HideWeapon, HideWeapon);
        LastWeapon = this.transform.GetChild(0).gameObject;
        if (gameObject.name.Equals("FireLL"))
        {
            IsRight = true;
        }
    }
    private GameObject LastWeapon;
    private void ChangeValue(string msg)
    {
         //Debug.Log("ChangeValue:" + msg);
        GameObject one = null;
        if (LastWeapon != null)
        {
            LastWeapon.SetActive(false);
            if (!weaponList.ContainsKey(MsgCenter.Instance.DefaultWeapon))
            {
                weaponList.Add(MsgCenter.Instance.DefaultWeapon, LastWeapon);
            }
        }
        if (weaponList.ContainsKey(msg))
        {
            one = weaponList[msg];
            WeaponControll weapon = one.GetComponent<WeaponControll>();
            if (msg != MsgCenter.Instance.DefaultWeapon &&
                weapon != null)
            {
                weapon.LifeTime = MsgCenter.Instance.player.useWeapon.LifeTime;
                weapon._time = 0;
            }
            one.SetActive(true);
        }
        else
        {
            GameObject go;
            if (IsRight)
            {
                go = MsgCenter.Instance.RightWeapons[msg];
                if (msg != MsgCenter.Instance.DefaultWeapon)
                {
                    WeaponControll weapon = go.AddComponent<WeaponControll>();
                    weapon.LifeTime = MsgCenter.Instance.player.useWeapon.LifeTime;
                    //weapon.enabled = true;
                }
            }
            else
            {
                go = MsgCenter.Instance.LeftWeapons[msg];
            }
            one = Instantiate(go);
            weaponList.Add(msg, one);
            one.SetActive(true);
            one.transform.SetParent(this.transform, false);
            
            one.transform.localPosition = Vector3.zero;
            one.transform.localEulerAngles = Vector3.zero;
            one.transform.localScale = Vector3.one;

        }
        currWeapon = one;
        LastWeapon = one;
    }

    private void HideWeapon(string msg) 
    {
        if (currWeapon != null &&
            gameObject.name.Equals(msg))
        {
            currWeapon.SetActive(false);
        }
    }

    private void DisplayWeapon(string msg) 
    {
        if (currWeapon != null &&
            gameObject.name.Equals(msg))
        {
            currWeapon.SetActive(true);
        }
    }

    private void ChangeValue111(string msg)
    {
    //    Debug.LogError("ChangeValue:" + msg);
        GameObject one = null;
        if (LastWeapon != null)
        {
            LastWeapon.SetActive(false);
            if (!weaponList.ContainsKey(Recever.rInstance.DefaultWeapon))
            {
                weaponList.Add(Recever.rInstance.DefaultWeapon, LastWeapon);
            }
        }
        if (weaponList.ContainsKey(msg))
        {
            one = weaponList[msg];
            one.SetActive(true);
        }
        else 
        {
            GameObject go;
            if (IsRight)
            {
                go = Recever.rInstance.RightWeapons[msg];
            }
            else
            {
                go = Recever.rInstance.LeftWeapons[msg];
            }
            one = Instantiate(go);
            if (!weaponList.ContainsKey(msg))
            {
                weaponList.Add(msg, one);
            }

            one.SetActive(true);
            one.transform.localPosition = Vector3.zero;
            one.transform.localEulerAngles = Vector3.zero;
            one.transform.localScale = Vector3.one;
            one.transform.SetParent(this.transform, false);
        }
       
        LastWeapon = one;
    }

}
