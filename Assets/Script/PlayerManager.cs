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
    private float defaultSpeed = 5.0f;
	[SerializeField]
	private float moveSpeed = 5.0f;
	[SerializeField]
	private float speedDelay = 0.05f;
    private float speed = 0;
    [SerializeField]
    private KeyCode jumpKey;

    private Rigidbody2D rb;
    private GameObject tapObject;
    private float depth = 10.0f;
    private bool isJump = false;
    private bool isGround = true;
    private bool isReleased = false;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        tapObject = GameObject.Find("Tap_Fields/" + transform.name);
    }

	void OnCollisionEnter2D(Collision2D col)
	{
        if (col.gameObject.tag.Equals("Player"))
        {
            isGround = true;
            isReleased = false;
            speed = 0;
        }
	}

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

					if (hit && hit.collider.gameObject.Equals(tapObject))
					{
                        return true;
					}
				}
			}
		}

        return false;
    }

    private RaycastHit2D IsSelected(Vector3 position)
    {
        Ray ray = Camera.main.ScreenPointToRay(position);
        return Physics2D.Raycast((Vector2)ray.origin, (Vector2)ray.direction, depth, 1 << LayerMask.NameToLayer("Tap"));
    }

    private void Update()
    {
        if (!isJump && isGround)
        {
#if UNITY_EDITOR
            if (Input.GetKeyDown(jumpKey))
            {
                isJump = true;
                isGround = false;
                speed = defaultSpeed;
            }
#endif
#if UNITY_ANDROID
            if(GetTouchAction(TouchPhase.Began))
            {
                isJump = true;
				isGround = false; 
                speed = defaultSpeed;
			}
#endif
        }

        if (!isGround)
        {
#if UNITY_EDITOR
            if (Input.GetKeyUp(jumpKey))
            {
                isReleased = true;
            }
#endif
#if UNITY_ANDROID
            if (GetTouchAction(TouchPhase.Ended))
            {
                isReleased = true;
            }
#endif
        }
	}

    private void FixedUpdate()
    {
        if(isJump)
        {
            rb.AddForce(new Vector2(0, inverse ? -jumpForce : jumpForce));
            //rb.AddForce(new Vector2(jumpForce, 0));
            isJump = false;
        }

        rb.velocity = new Vector2(speed, rb.velocity.y);
        /*
        if (isGround)
        {
            rb.velocity = new Vector2(0, 0);
        }
        else
        {
            //rb.velocity = new Vector2(moveSpeed, (inverse ? -speed : speed));
            if (speed < 0)
            {
                rb.velocity = new Vector2(rb.velocity.x, (inverse ? -speed : speed) * Mathf.Abs(transform.position.y) * 0.5f);
            }
            else
            {
                rb.velocity = new Vector2(rb.velocity.x, (inverse ? -speed : speed));
            }
        }
        */

#if UNITY_EDITOR
        //if (-5.0f < speed && isReleased)
        if(0 < speed && isReleased)
        {
            speed -= speedDelay;
        }
#endif
#if UNITY_ANDROID
        //if (-5.0f < speed && isReleased)
        if (0 < speed && isReleased)
		{
            speed -= speedDelay;
		}
#endif
	}
}
