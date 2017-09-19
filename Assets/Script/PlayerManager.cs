using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerStatus
{
	Stand,
    Jump,
    Release,
    Charge,
    Dead,
    End
};

public class PlayerManager : MonoBehaviour
{
	[SerializeField]
	private Transform anotherPlayer;
    //上下反転したキャラクターか否か
    [SerializeField]
    private bool inverse;
    //PCデバッグ時のジャンプをするキー
    [SerializeField]
    private KeyCode jumpKey;
    //キャラクターの画像
    [SerializeField]
    private Sprite[] sprite;

    private Rigidbody2D rb;
    private GameManager gameManager;
    private GameObject tapObject;
    private GameObject mainCamera;
    private GameObject logo;
    private SpriteRenderer spriteRenderer;
    private PlayerState playerState;
    private AudioSource audioSource;
    private GameObject blockCounter;
    private float depth = 10.0f;

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
    //音の種類
    private AudioClip[] clips;

	//キャラクターの状態
	private Vector3 defaultPos;
    private float defaultHeight = 0;
    private bool isJump = false;
    private bool isStart = false;
    private PlayerStatus playerStatus;
    private Vector2 defaultVelocity;
    private Vector3 defaultAngle;

    private int animationCount = 0;

    private void Start()
    {
        //キャラクターの物理挙動取得
        rb = GetComponent<Rigidbody2D>();
        //タップ判定取得
        tapObject = GameObject.Find("Tap_Fields/" + transform.name);
        //カメラ情報取得
        mainCamera = GameObject.Find("Main Camera");
        //背景とロゴ情報取得
        logo = GameObject.Find("Logo");
        //キャラクターの原画場情報取得
        spriteRenderer = GetComponent<SpriteRenderer>();
        //ゲームの管理情報取得
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        //音取得
        audioSource = GetComponent<AudioSource>();
        if (!inverse) blockCounter = GameObject.Find("BlockCounter");

        //ジャンプの強さ、速度の初期化
        playerState = GetComponentInParent<PlayerState>();
        jumpForce = playerState.GetJumpForce();
        maxSpeed = playerState.GetMaxSpeed();
        acceleration = playerState.GetAcceleration();
        deceleration = playerState.GetDeceleration();
        clips = playerState.GetAudioClips();

        defaultPos = transform.position;

        playerStatus = PlayerStatus.Release;
        speed = maxSpeed;

        defaultVelocity = rb.velocity;
        defaultAngle = transform.eulerAngles;

        if (!isStart)
        {
            rb.isKinematic = true;
        }
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

					if (hit && hit.collider.gameObject.Equals(tapObject))
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
		return Physics2D.Raycast((Vector2)ray.origin, (Vector2)ray.direction, depth, 1 << LayerMask.NameToLayer("Tap"));
	}

	//キャラクター同士の接触時の判定
	void OnCollisionEnter2D(Collision2D col)
	{
		if (col.gameObject.tag.Equals("Player"))
		{
			speed = 0;
			//playerStatus = PlayerStatus.Stand;
            audioSource.PlayOneShot(clips[3]);
			StartCoroutine(WaitForStand());

            if (Mathf.Abs(defaultHeight) < 0.0001f)
            {
                defaultHeight = transform.position.y;
            }
            else
            {
                //Debug.Log( defaultHeight - transform.position.y);
                if(logo.activeSelf)iTween.MoveTo(logo, iTween.Hash("x", logo.transform.position.x - 1.0f, "time", 5.0f));
                iTween.MoveTo(gameObject, iTween.Hash("y", defaultHeight + (inverse ? -0.02f : 0.02f), "time", 0.5f));

				if (!inverse)
				{
					iTween.MoveTo(mainCamera, iTween.Hash("x", transform.position.x + 5.0f, "time", 2.0f));
				}
            }
		}
	}

    public void SetRestart()
    {
        transform.eulerAngles = defaultAngle;
        defaultHeight = 0;
        transform.position = defaultPos;
		playerStatus = PlayerStatus.Release;
		speed = maxSpeed;
        rb.velocity = defaultVelocity;
        GetComponent<BoxCollider2D>().isTrigger = false;
        if (!inverse) blockCounter.GetComponent<BoxCollider2D>().enabled = true;
    }

    public void SetKey()
    {
        jumpKey = KeyCode.M;
    }

	//死亡時の処理
	//DeadManager.csから呼ばれる
	public void Dead()
	{
		//複数回死亡処理がないよう判定
		if (!playerStatus.Equals(PlayerStatus.Dead) && !playerStatus.Equals(PlayerStatus.End))
		{
            audioSource.PlayOneShot(clips[1]);
			playerStatus = PlayerStatus.Dead;
			//死亡時の画像に切り替え
			spriteRenderer.sprite = sprite[4];
			//数秒後吹っ飛ぶまでの遅延
			StartCoroutine(WaitForDead());
			//キャラクターが激突しないよう当たり判定を解除
			GetComponent<BoxCollider2D>().isTrigger = true;
            if(!inverse)blockCounter.GetComponent<BoxCollider2D>().enabled = false;
		}
	}

    public void SetStart()
    {
        isStart = true;
        rb.isKinematic = false;
    }

    //ジャンプキーを押した時の処理
    private void Jump()
    {
        if (playerStatus.Equals(PlayerStatus.Stand) && !isJump)
        {
            StartCoroutine(WaitForJump());
        }
    }

    private void Release()
    {
        if (!playerStatus.Equals(PlayerStatus.Stand))
        {
            if (!playerStatus.Equals(PlayerStatus.Charge)) spriteRenderer.sprite = sprite[2];
            playerStatus = PlayerStatus.Release;
        }
    }

    private void Update()
    {
        //死亡時は入力を受け付けない
        if (isStart && !playerStatus.Equals(PlayerStatus.Dead) && !playerStatus.Equals(PlayerStatus.End))
        {
#if UNITY_EDITOR
            if (Input.GetKeyDown(jumpKey))
            {
                Jump();
            }
#endif
#if UNITY_ANDROID
            if (GetTouchAction(TouchPhase.Began))
            {
                Jump();
            }
#endif

#if UNITY_EDITOR
            if (Input.GetKeyUp(jumpKey))
            {
                Release();
            }
#endif
#if UNITY_ANDROID
            if (GetTouchAction(TouchPhase.Ended))
            {
                Release();
            }
#endif
        }
    }

    private void FixedUpdate()
    {
        if (isStart)
        {
            if (isJump)
            {
                rb.AddForce(new Vector2(0, inverse ? -jumpForce : jumpForce));
                isJump = false;
            }

            switch (playerStatus)
            {
                case PlayerStatus.Stand:
                    ++animationCount;
                    rb.velocity = new Vector2(0, rb.velocity.y);
                    if (40 < animationCount && spriteRenderer.sprite.Equals(sprite[0]))
                    {
                        animationCount = 0;
                        spriteRenderer.sprite = sprite[5];
                    }
                    else if(40 < animationCount && spriteRenderer.sprite.Equals(sprite[5]))
                    {
                        animationCount = 0;
                        spriteRenderer.sprite = sprite[0];
                    }
                    else if(spriteRenderer.sprite.Equals(sprite[2]))
                    {
                        spriteRenderer.sprite = sprite[0];
                        animationCount = 0;
                    }
                    break;
                case PlayerStatus.Charge:
                    rb.velocity = new Vector2(0, rb.velocity.y);
                    break;
                case PlayerStatus.Jump:
                    rb.velocity = new Vector2(speed, rb.velocity.y);
                    if (speed < maxSpeed)
                    {
                        speed += acceleration;
                    }
                    break;
                case PlayerStatus.Release:
                    rb.velocity = new Vector2(speed, rb.velocity.y);
                    float dis = transform.position.x - anotherPlayer.position.x;
                    if (0 < speed)
                    {
                        speed -= (deceleration + (dis * 0.025f));
                    }
                    break;
                case PlayerStatus.Dead:
                    rb.velocity = new Vector2(rb.velocity.x * 0.5f, rb.velocity.y * 0.5f);
                    transform.eulerAngles += new Vector3(0, 0, 1.0f);
                    break;
                case PlayerStatus.End:
                    rb.velocity = new Vector2(rb.velocity.x * 2.0f, rb.velocity.y * 2.0f);
                    break;
            }
        }
	}

    //ジャンプ時の溜め
	IEnumerator WaitForJump()
	{
        playerStatus = PlayerStatus.Charge;
        spriteRenderer.sprite = sprite[3];
        yield return new WaitForSeconds(0.25f);
        audioSource.PlayOneShot(clips[0]);
        if (playerStatus.Equals(PlayerStatus.Charge))
        {
            playerStatus = PlayerStatus.Jump;
        }
        else if(playerStatus.Equals(PlayerStatus.Release))
        {
            speed = maxSpeed * 0.1f;
        }
        isJump = true;
		spriteRenderer.sprite = sprite[1];
	}
    //着地時の溜め
	IEnumerator WaitForStand()
	{
		spriteRenderer.sprite = sprite[3];
		yield return new WaitForSeconds(0.25f);
        playerStatus = PlayerStatus.Stand;
        if(spriteRenderer.sprite.Equals(sprite[3]))spriteRenderer.sprite = sprite[0];
        //if(!playerState.Equals(PlayerStatus.Dead) || !playerState.Equals(PlayerStatus.End))spriteRenderer.sprite = sprite[0];
	}
    //死亡後の溜め
	IEnumerator WaitForDead()
	{
        gameManager.StopBGM();
		yield return new WaitForSeconds(1.5f);
        playerStatus = PlayerStatus.End;
        audioSource.PlayOneShot(clips[2]);
        gameManager.SetDeadCount();
	}
}
