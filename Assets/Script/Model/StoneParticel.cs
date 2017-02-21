using UnityEngine;
using System.Collections;

public class StoneParticel : MonoBehaviour {

    private GameObject particle;

	void Start () {
        particle = transform.FindChild("Enemy Explode").gameObject;
	}
    void OnDestroy()
    {
        if (particle)
        {
            particle.SetActive(true);
        }
    }
	void Update () {
	
	}
}
