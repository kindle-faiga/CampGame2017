using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DeadManager : MonoBehaviour 
{
    [SerializeField]
    private bool isWall = false;

	private void OnTriggerEnter2D(Collider2D other)
	{
        if(other.tag.Equals("Player"))
        {
            if (!isWall)
            {
				other.GetComponent<PlayerManager>().Dead();
            }
            else if(other.name.Equals(transform.name))
            {
                other.GetComponent<PlayerManager>().Dead();
            }
        }
	}
}
