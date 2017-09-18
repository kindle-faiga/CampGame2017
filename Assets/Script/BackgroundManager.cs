using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundManager : MonoBehaviour 
{
    [SerializeField]
    private Sprite[] sprites;
    private SpriteRenderer spriteRenderer;
    private SpriteRenderer spriteRendererBack;
    private Sprite defaultSprite;
    private int count = 0;
    private int maxCount;

	void Start () 
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRendererBack = GameObject.Find("Background").GetComponent<SpriteRenderer>();
        defaultSprite = spriteRenderer.sprite;
        maxCount = sprites.Length;
	}
	
    public void SetRestart()
    {
        spriteRenderer.sprite = defaultSprite;
        spriteRendererBack.sprite = defaultSprite;
        count = 0;
    }

    public void ChangeSprite()
    {
		if (count < maxCount-1)
		{
            ++count;
		}
        else
        {
            count = 0;
        }

        spriteRendererBack.sprite = spriteRenderer.sprite;
        spriteRenderer.sprite = sprites[count];

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
