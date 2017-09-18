using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TapUIManager : MonoBehaviour 
{
    [SerializeField]
    private bool inverse = false;
    
    private SpriteRenderer spriteRender;
    private float elapsed = 0;
    private bool isTapped = false;

	void Start () 
    {
        spriteRender = GetComponent<SpriteRenderer>();
	}
	
    public void Tap()
    {
        if(!isTapped)
        {
            isTapped = true;
			Color color = spriteRender.color;
            color.a = 0.5f;
			spriteRender.color = color;
            transform.position = new Vector3(transform.position.x, transform.position.y + (inverse ? 3.0f : -3.0f), transform.position.z);
            iTween.ScaleTo(gameObject, new Vector3(3.0f, 3.0f, 1.0f), 2.5f);
        }
    }

    void Update () 
    {
        if (!isTapped)
        {
            elapsed += Time.deltaTime * 2.0f;

            Color color = spriteRender.color;
            color.a = Mathf.Abs(Mathf.Sin(elapsed));
            spriteRender.color = color;
        }
	}
}
