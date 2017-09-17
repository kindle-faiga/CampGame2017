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
    [SerializeField]
    private Sprite[] sprite;

    private Rigidbody2D rb;
    private GameObject tapObject;
    private GameObject mainCamera;
    private BlockCreater blockCreater;
    private SpriteRenderer spriteRenderer;
    private float depth = 10.0f;
    private bool isJump = false;
    private bool isGround = true;
    private bool isReleased = false;
    private bool isCharge = false;
    private bool isDead = false;
    private bool isEnd = false;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        tapObject = GameObject.Find("Tap_Fields/" + transform.name);
        blockCreater = GameObject.Find("Field/Blocks").GetComponent<BlockCreater>();
        mainCamera = GameObject.Find("Main Camera");
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void Dead()
    {
        if (!isDead)
        {
            isDead = true;
            spriteRenderer.sprite = sprite[4];
            StartCoroutine(WaitForDead());
            //GetComponent<BoxCollider2D>().enabled = false;
            GetComponent<BoxCollider2D>().isTrigger = true;
        }
    }

	void OnCollisionEnter2D(Collision2D col)
	{
        if (col.gameObject.tag.Equals("Player"))
        {
            isGround = true;
            isReleased = false;
            speed = 0;
            iTween.MoveTo(mainCamera, iTween.Hash("x", transform.position.x + 5.0f, "time", 2.0f));
            blockCreater.CreateBlock();
            StartCoroutine(WaitForStand());
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
        if (!isDead)
        {
            if (!isJump && !isCharge && isGround)
            {
#if UNITY_EDITOR
                if (Input.GetKeyDown(jumpKey))
                {
                    StartCoroutine(WaitForJump());
                }
#endif
#if UNITY_ANDROID
                if (GetTouchAction(TouchPhase.Began))
                {
                    StartCoroutine(WaitForJump());
                }
#endif
            }

#if UNITY_EDITOR
            if (Input.GetKeyUp(jumpKey))
            {
                isReleased = true;
                if (!isCharge) spriteRenderer.sprite = sprite[2];
            }
#endif
#if UNITY_ANDROID
            if (GetTouchAction(TouchPhase.Ended))
            {
                isReleased = true;
                if (!isCharge) spriteRenderer.sprite = sprite[2];
            }
#endif
        }
    }

    private void FixedUpdate()
    {
        if (isDead)
        {
            if (isEnd)
            {
                rb.velocity = new Vector2(rb.velocity.x * 2.0f, rb.velocity.y * 2.0f);
            }
            else
            {
                rb.velocity = new Vector2(rb.velocity.x * 0.1f, rb.velocity.y * 0.1f);
            }
            transform.eulerAngles += new Vector3(0, 0, 1.0f);
        }
        else
        {
            if (isJump)
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

            if (!isGround && !isReleased && speed < defaultSpeed)
            {
                speed += 0.5f;
            }

            //if (-5.0f < speed && isReleased)
            if (0 < speed && isReleased)
            {
                speed -= speedDelay;
            }
        }
	}

	IEnumerator WaitForJump()
	{
        isCharge = true;
        spriteRenderer.sprite = sprite[3];
        yield return new WaitForSeconds(0.25f);
		isJump = true;
        isCharge = false;
		isGround = false;
		spriteRenderer.sprite = sprite[1];
	}

	IEnumerator WaitForStand()
	{
		spriteRenderer.sprite = sprite[3];
		yield return new WaitForSeconds(0.25f);
        spriteRenderer.sprite = sprite[0];
	}

	IEnumerator WaitForDead()
	{
		yield return new WaitForSeconds(1.5f);
        isEnd = true;
	}
}
