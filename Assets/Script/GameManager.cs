using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour 
{
    [SerializeField]
    private int blockCount = 0;

    private GameObject scoreUI;

    private GameObject tapObject;
    private GameObject tapObjectInverse;
	private GameObject mainCamera;
    private GameObject logo;
    private PlayerManager playerManager;
    private PlayerManager playerManagerInverse;
    private EffectManager effectManager;
    private EffectManager effectManagerInverse;
    //private BlockCreater blockCreater;
    private List<MobManager> mobManagers = new List<MobManager>();
    private TapUIManager tapUIManager;
    private TapUIManager tapUIManagerInverse;
    private AudioSource[] audioSource;
    private bool isOtherDead = false;
    private bool isStart = false;
    private bool tapStart = false;
    private bool tapInverseStart = false;
    private bool isRestart = false;

    private void Start()
    {
        scoreUI = GameObject.Find("UI/Panel/Image");
        scoreUI.SetActive(false);
        tapObject = GameObject.Find("Tap_Fields/Player");
        tapObjectInverse = GameObject.Find("Tap_Fields/Player_Inverse");
        playerManager = GameObject.Find("Players/Player").GetComponent<PlayerManager>();
        playerManagerInverse = GameObject.Find("Players/Player_Inverse").GetComponent<PlayerManager>();
        effectManager = GameObject.Find("EffectPoint/Player").GetComponent<EffectManager>();
        effectManagerInverse = GameObject.Find("EffectPoint/Player_Inverse").GetComponent<EffectManager>();
		mainCamera = GameObject.Find("Main Camera");
        transform.position = mainCamera.transform.position;
		//blockCreater = GameObject.Find("Field/Blocks").GetComponent<BlockCreater>();
        tapUIManager = GameObject.Find("TapSpace").GetComponent<TapUIManager>();
        tapUIManagerInverse = GameObject.Find("TapSpaceInverse").GetComponent<TapUIManager>();
        audioSource = GetComponents<AudioSource>();
        logo = GameObject.Find("Logo");

        foreach (GameObject m in GameObject.FindGameObjectsWithTag("Mob"))
        {
            mobManagers.Add(m.GetComponent<MobManager>());
        }
    }

    public int GetBlockCount()
    {
        return blockCount;
    }

    public void BlockCount()
    {
        ++blockCount;
        //blockCreater.CreateBlock();
    }

	//タッチ、タップの取得（変更を禁ず）
    private bool GetTouchAction(TouchPhase phase, GameObject target)
	{
		if (0 < Input.touchCount)
		{
			for (int i = 0; i < Input.touchCount; i++)
			{
				Touch t = Input.GetTouch(i);

				if (t.phase == phase)
				{
					RaycastHit2D hit = IsSelected(t.position);

                    if (hit && (hit.collider.gameObject.Equals(target)))
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

    public void StopBGM()
    {
        audioSource[0].Stop();
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
        if (!isStart)
        {
            isStart = true;
            playerManager.SetStart();
            playerManagerInverse.SetStart();

            audioSource[0].Play();

            foreach (MobManager m in mobManagers)
            {
                m.SetStart();
            }
        }
    }

    private void Update()
    {
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.Z))
		{
            if(!tapStart)
            {
                tapStart = true;
                audioSource[1].PlayOneShot(audioSource[1].clip);
                tapUIManager.Tap();
            }

			if (tapInverseStart)
			{
				SetStart();
			}
		}

		if (Input.GetKeyDown(KeyCode.X))
		{
            if (!tapInverseStart)
			{
				tapInverseStart = true;
				audioSource[1].PlayOneShot(audioSource[1].clip);
				tapUIManagerInverse.Tap();
			}

			if (tapStart)
			{
				SetStart();
			}
		}

        if(isRestart)
        {
			if (Input.GetKeyDown(KeyCode.Z))
			{
                tapUIManager.Tap();
                tapUIManagerInverse.gameObject.SetActive(false);
				StartCoroutine(WaitForStart());
			}
			else if (Input.GetKeyDown(KeyCode.X))
			{
                tapUIManagerInverse.Tap();
                audioSource[1].PlayOneShot(audioSource[1].clip);
				SceneManager.LoadScene(SceneManager.GetActiveScene().name);
			}
        }
#endif
#if UNITY_ANDROID
        if (GetTouchAction(TouchPhase.Began, tapObject))
		{
			if (!tapStart)
			{
                tapStart = true;
				audioSource[1].PlayOneShot(audioSource[1].clip);
				tapUIManager.Tap();
			}

            if (tapInverseStart)
            {
                SetStart();
            }
		}

        if (GetTouchAction(TouchPhase.Began, tapObjectInverse))
		{
			if (!tapInverseStart)
			{
				tapInverseStart = true;
				audioSource[1].PlayOneShot(audioSource[1].clip);
				tapUIManagerInverse.Tap();
			}

            if (tapStart)
			{
				SetStart();
			}
		}

		if (isRestart)
		{
			if (GetTouchAction(TouchPhase.Began, tapObject))
			{
                tapUIManager.Tap();
                tapUIManagerInverse.gameObject.SetActive(false);
                StartCoroutine(WaitForStart());
			}
            else if(GetTouchAction(TouchPhase.Began, tapObjectInverse))
            {
                audioSource[1].PlayOneShot(audioSource[1].clip);
                tapUIManagerInverse.Tap();
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }
		}
#endif
	}

	IEnumerator WaitForRestart()
	{
        yield return new WaitForSeconds(1.0f);
        scoreUI.SetActive(true);
        scoreUI.GetComponentsInChildren<Text>()[0].text = ("キョリ : "+blockCount*100 + "m");
        scoreUI.GetComponentsInChildren<Text>()[1].text = ("キョリ : " + blockCount * 100 + "m");
        tapUIManager.SetRestart();
        if(!tapUIManagerInverse.gameObject.activeSelf)tapUIManagerInverse.gameObject.SetActive(true);
        tapUIManagerInverse.SetRestart();
		yield return new WaitForSeconds(0.5f);
        isRestart = true;
	}

    IEnumerator WaitForStart()
    {
		isRestart = false;
        scoreUI.SetActive(false);
        blockCount = 0;
        isOtherDead = false;
		audioSource[1].PlayOneShot(audioSource[1].clip);
        logo.SetActive(false);
        iTween.MoveTo(mainCamera, iTween.Hash("x", transform.position.x, "time", 2.0f));
		yield return new WaitForSeconds(1.0f);
        audioSource[0].Play();
        playerManager.SetRestart();
        playerManagerInverse.SetRestart();
        effectManager.SetRestart();
        effectManagerInverse.SetRestart();

        foreach (GameObject b in GameObject.FindGameObjectsWithTag("Block"))
        {
            b.GetComponent<BlockManager>().SetRestart();
        }
    }
}
