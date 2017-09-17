﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BlockStatus
{
    Default,
    Sway
};

public class BlockManager : MonoBehaviour 
{
    [SerializeField]
    BlockStatus blockStatus = BlockStatus.Default;
    private float elapsed = 0;
	
	void Start () 
    {
		
	}

    public void ChangeState(BlockStatus _blockStatus)
    {
        blockStatus = _blockStatus;
    }
	
	void FixedUpdate () 
    {
        switch (blockStatus)
		{
            case BlockStatus.Default:
				break;
            case BlockStatus.Sway:
                elapsed += Time.deltaTime * 2.0f;
                transform.position = new Vector3(transform.position.x + Mathf.Sin(elapsed) * 0.05f, transform.position.y, transform.position.z);
				break;
		}
	}
}