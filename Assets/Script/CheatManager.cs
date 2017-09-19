using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheatManager : MonoBehaviour
{
    private PlayerManager playerManager;

    void Start()
    {
        playerManager = GetComponent<PlayerManager>();
    }

    private void Update()
    {
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.Q))
        {
            playerManager.SetKey();
        }
#endif
    }
}