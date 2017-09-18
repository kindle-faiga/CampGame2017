using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPointEffect : MonoBehaviour 
{
    [SerializeField]
    private bool inverse = false;

	IEnumerator Start () 
    {
        iTween.MoveTo(gameObject, iTween.Hash("y", inverse ? -3.0f : 3.0f, "time", 1.5f));
        iTween.ValueTo(gameObject, iTween.Hash("from", 0f, "to", 1.0f, "time", 1.0f, "onupdate", "setValue"));
		yield return new WaitForSeconds(2.0f);
        iTween.ValueTo(gameObject, iTween.Hash("from", 1.0f, "to", 0, "time", 2.0f, "onupdate", "setValue"));
	}

    private void setValue(float val)
    {
		Color color = GetComponent<SpriteRenderer>().color;
		color.a = val;
		GetComponent<SpriteRenderer>().color = color;
    }
}
