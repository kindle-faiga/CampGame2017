using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BlockStatus
{
    Default,
    Sway,
    Flag,
};

public class BlockManager : MonoBehaviour 
{
    [SerializeField]
    BlockStatus blockStatus = BlockStatus.Default;
    [SerializeField]
    private bool isCreater = false;
    BlockStatus defaultBlockStatus;
    private GameManager gameManager;
    private float elapsed = 0;
    private bool isPass = false;
    private bool isCreate = false;
	
	void Start () 
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();

        defaultBlockStatus = blockStatus;
	}

    public void SetRestart()
    {
        blockStatus = defaultBlockStatus;
        isPass = false;
    }

	private void OnTriggerEnter2D(Collider2D other)
	{
        if (other.tag.Equals("BlockCounter"))
		{
            if (!isPass)
            {
                isPass = true;
                //blockStatus = BlockStatus.Default;
                gameManager.BlockCount();

				if (blockStatus.Equals(BlockStatus.Flag))
				{
					GameObject effectObject = Resources.Load("Prefab/CheckPointEffect") as GameObject;
					GameObject obj = Instantiate(effectObject, transform.position, transform.rotation) as GameObject;
					Destroy(obj, 4.0f);

					GameObject.Find("Background_Change").GetComponent<BackgroundManager>().ChangeSprite();
				}
            }

            if(!isCreate && isCreater)
            {
                isCreate = true;

                if (blockStatus.Equals(BlockStatus.Flag))
                {
                    Vector3 targetPos = transform.FindChild("Target").transform.position;

                    for (int i = 0; i < 3; ++i)
                    {
                        int n = Random.Range(1, 5);
                        GameObject obj = Resources.Load("Prefab/RandomBlocks/Blocks"+n) as GameObject;
                        Instantiate(obj, new Vector3(targetPos.x + i * 30.0f,transform.position.y,transform.position.z), transform.rotation);
                    }
                }
            }
		}
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
