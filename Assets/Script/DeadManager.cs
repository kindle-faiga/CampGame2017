using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DeadManager : MonoBehaviour 
{
	private void OnTriggerEnter2D(Collider2D other)
	{
        if(other.tag.Equals("Player"))
        {
            other.GetComponent<PlayerManager>().Dead();
            StartCoroutine(WaitForJump());
        }
	}

	IEnumerator WaitForJump()
	{
		yield return new WaitForSeconds(3.0f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
	}
}
