using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MobStatus
{
	Stand,
	Jump,
	Release,
	Charge,
};

public class MobManager : MonoBehaviour
{
	//上下反転したキャラクターか否か
	[SerializeField]
	private bool inverse;
	//キャラクターの画像
	[SerializeField]
	private Sprite[] sprite;

	private Rigidbody2D rb;
	private SpriteRenderer spriteRenderer;
	private PlayerState playerState;

	//ジャンプの強さ
	private float jumpForce;
	//横軸の速度
	private float speed;
	//横軸の最高スピード
	private float maxSpeed;
	//横軸の加速度
	private float acceleration;
	//横軸の減衰速度
	private float deceleration;

	//キャラクターの状態
	private Vector3 defaultPos;
	private bool isJump = false;
    private bool isStart = false;
	private MobStatus mobStatus;

	private void Start()
	{
		//キャラクターの物理挙動取得
		rb = GetComponent<Rigidbody2D>();
		//キャラクターの原画場情報取得
		spriteRenderer = GetComponent<SpriteRenderer>();

		//ジャンプの強さ、速度の初期化
		playerState = GetComponentInParent<PlayerState>();
		jumpForce = playerState.GetJumpForce();
		maxSpeed = playerState.GetMaxSpeed();
		acceleration = playerState.GetAcceleration();
		deceleration = playerState.GetDeceleration();

		defaultPos = transform.position;

		mobStatus = MobStatus.Release;
		speed = maxSpeed;
	}

	//キャラクター同士の接触時の判定
	void OnCollisionEnter2D(Collision2D col)
	{
		if (col.gameObject.tag.Equals("Mob"))
		{
			speed = 0;
			mobStatus = MobStatus.Stand;
			StartCoroutine(WaitForStand());
		}
	}

	private void OnTriggerEnter2D(Collider2D other)
	{
		if (other.tag.Equals("MobWall"))
		{
            if(isStart)
            {
                Destroy(gameObject);
            }
            else
            {
                transform.position = new Vector3(defaultPos.x, transform.position.y, transform.position.z);   
            }
		}
	}

    public void SetStart()
    {
        isStart = true;
        Destroy(gameObject);
    }

	private void FixedUpdate()
	{
		if (isJump)
		{
			rb.AddForce(new Vector2(0, inverse ? -jumpForce : jumpForce));
			isJump = false;
		}

		switch (mobStatus)
		{
			case MobStatus.Stand:
				rb.velocity = new Vector2(0, rb.velocity.y);
				break;
			case MobStatus.Charge:
				rb.velocity = new Vector2(0, rb.velocity.y);
				break;
			case MobStatus.Jump:
				rb.velocity = new Vector2(speed, rb.velocity.y);
				if (speed < maxSpeed)
				{
					speed += acceleration;
				}
				break;
			case MobStatus.Release:
				rb.velocity = new Vector2(speed, rb.velocity.y);
				if (0 < speed)
				{
					speed -= deceleration;
				}
				break;
		}
	}

	//着地時の溜め
	IEnumerator WaitForStand()
	{
		spriteRenderer.sprite = sprite[3];
		yield return new WaitForSeconds(0.25f);
        isJump = true;
        mobStatus = MobStatus.Jump;
		spriteRenderer.sprite = sprite[1];
        yield return new WaitForSeconds(0.335f);
        mobStatus = MobStatus.Release;
        spriteRenderer.sprite = sprite[2];
	}
}
