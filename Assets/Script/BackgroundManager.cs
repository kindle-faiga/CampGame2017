using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundManager : MonoBehaviour 
{
    [SerializeField]
    private Sprite sprite;
    private SpriteRenderer spriteRenderer;
    private Sprite defaultSprite;

	void Start () 
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
	}
	
    public void SetRestart()
    {
        spriteRenderer.sprite = defaultSprite;
    }

    public void ChangeSprite()
    {
        spriteRenderer.sprite = sprite;

        Color color = spriteRenderer.color;
		color.a = 0f;
        spriteRenderer.color = color;
    }

    private void Update()
    {
        if (spriteRenderer.color.a < 1.0f)
        {
            Color color = spriteRenderer.color;
            color.a += 0.01f;
            spriteRenderer.color = color;
        }
    }
}
