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
            }
#endif
#if UNITY_ANDROID
            if(GetTouchAction(TouchPhase.Began))
            {
                isJump = true;
				isGround = false;
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
            isJump = false;
        }

#if UNITY_EDITOR
        if (Input.GetKey(jumpKey) && !isReleased)
		{
            transform.position += new Vector3(speed, 0, 0);
		}
#endif
#if UNITY_ANDROID
        if(GetTouchAction(TouchPhase.Stationary) && !isReleased)
        {
            transform.position += new Vector3(speed, 0, 0);
        }
#endif
	}
}
