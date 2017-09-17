using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockCreater : MonoBehaviour 
{
    [SerializeField]
    private float maxSize = 5.0f;
    private float endBlockPos = 0;
    private GameObject blockObject;

	void Start () 
    {
        blockObject = Resources.Load("Prefab/Block") as GameObject;

        foreach (GameObject b in GameObject.FindGameObjectsWithTag("Block"))
        {
            if(endBlockPos < b.transform.localPosition.x)
            {
                endBlockPos = b.transform.localPosition.x;
            }
        }
	}

    public void CreateBlock()
    {
        float sizeX = Random.Range(1,maxSize);
        Vector3 size = new Vector3(sizeX, 0.5f, 1.0f);
        float posX = Random.Range(8, 15);
        Vector3 pos = new Vector3(endBlockPos + posX,0,0);

        GameObject obj = Instantiate(blockObject);
        obj.transform.parent = transform;
        obj.transform.localScale = size;
        obj.transform.localPosition = pos;

        endBlockPos = obj.transform.localPosition.x;
    }
}
