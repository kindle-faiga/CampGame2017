using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour 
{
    [SerializeField]
    private bool inverse;
    [SerializeField]
    private float jumpForce = 100.0f;
	[SerializeField]
	private float speed = 10.0f;
    [SerializeField]
    private KeyCode jumpKey;
    private Rigidbody2D rb;
    private bool isJump = false;

	private void Start () 
    {
        rb = GetComponent<Rigidbody2D>();
	}
	
	private void Update ()
    {
        if(!isJump)
        {
            if (Input.GetKeyDown(jumpKey))
            {
                isJump = true;
            }
        }
	}

    private void FixedUpdate()
    {
        if(isJump)
        {
            rb.AddForce(new Vector2(0, inverse ? -jumpForce : jumpForce));
            isJump = false;
        }

        if (Input.GetKey(jumpKey))
		{
            transform.position += new Vector3(speed, 0, 0);
		}
    }
}
