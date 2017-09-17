﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour 
{
    private GameObject tapObject;
    private GameObject tapObjectInverse;
    private PlayerManager playerManager;
    private PlayerManager playerManagerInverse;
    private bool isOtherDead = false;

    private void Start()
    {
        tapObject = GameObject.Find("Tap_Fields/Player");
        tapObjectInverse = GameObject.Find("Tap_Fields/Player_Inverse");
        playerManager = GameObject.Find("Players/Player").GetComponent<PlayerManager>();
        playerManagerInverse = GameObject.Find("Players/Player_Inverse").GetComponent<PlayerManager>();
    }

	//タッチ、タップの取得（変更を禁ず）
	private bool GetTouchAction(TouchPhase phase)
	{
		if (0 < Input.touchCount)
		{
			for (int i = 0; i < Input.touchCount; i++)
			{
				Touch t = Input.GetTouch(i);

				if (t.phase == phase)
				{
					RaycastHit2D hit = IsSelected(t.position);

                    if (hit && (hit.collider.gameObject.Equals(tapObject) || hit.collider.gameObject.Equals(tapObjectInverse)))
					{
						return true;
					}
				}
			}
		}

		return false;
	}
	//タッチ、タップの取得（変更を禁ず）
	private RaycastHit2D IsSelected(Vector3 position)
	{
		Ray ray = Camera.main.ScreenPointToRay(position);
		return Physics2D.Raycast((Vector2)ray.origin, (Vector2)ray.direction, 10.0f, 1 << LayerMask.NameToLayer("Tap"));
	}

    public void SetDeadCount()
    {
        if(isOtherDead)
        {
            StartCoroutine(WaitForRestart());
        }
        else
        {
            isOtherDead = true;
        }
    }

    private void SetStart()
    {
        playerManager.SetStart();
        playerManagerInverse.SetStart();
    }

    private void Update()
    {
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.Z))
		{
            SetStart();
		}
#endif
#if UNITY_ANDROID
		if (GetTouchAction(TouchPhase.Began))
		{
            SetStart();
		}
#endif
	}

	IEnumerator WaitForRestart()
	{
		yield return new WaitForSeconds(2.0f);
		SceneManager.LoadScene(SceneManager.GetActiveScene().name);
	}
}