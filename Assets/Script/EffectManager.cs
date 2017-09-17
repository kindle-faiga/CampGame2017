using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectManager : MonoBehaviour 
{
    private GameObject effectObject;
	private bool isHit = false;

    private void Start()
    {
        effectObject = Resources.Load("Prefab/Effect") as GameObject;
    }

	private void OnTriggerEnter2D(Collider2D other)
	{
		if (other.tag.Equals("Player"))
		{
            if (!isHit && other.name.Equals(transform.name))
			{
                isHit = true;
                Instantiate(effectObject,other.transform.position,transform.rotation);
			}
		}
	}
}
