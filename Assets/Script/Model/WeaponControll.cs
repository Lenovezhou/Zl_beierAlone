using UnityEngine;
using System.Collections;

public class WeaponControll : MonoBehaviour {
    public float LifeTime = -1;
    public float _time;
	
	// Update is called once per frame
	void Update () 
    {
        if (LifeTime > 0)
        {
            _time += Time.deltaTime;
            if (_time >= LifeTime)
            {
                LifeTime = -1;
                _time = 0;
                MsgCenter.Instance.ChangeWeapon(MsgCenter.Instance.WeaponData[MsgCenter.Instance.DefaultWeapon]);
                MsgCenter.Instance.AddToSendList(MsgCenter.Instance.PlayerData);
                Messenger.Broadcast<string>(GameEvent.ChangeWeapon, MsgCenter.Instance.DefaultWeapon);
                MsgCenter.Instance.HideBullet();
            }
        }
        
	}
}
