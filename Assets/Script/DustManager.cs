using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DustManager : MonoBehaviour {
    Vector3 moveDistance;

    // Use this for initialization
    void Start () {
        moveDistance = new Vector3(0.0012f, 0,0);
    }
	
	// Update is called once per frame
	void Update () {
        transform.position = transform.position + moveDistance;
        if (transform.position.x < -13.8f)
        {
            transform.position = new Vector3(13.8f, 0, 0);

        }
    }
}
