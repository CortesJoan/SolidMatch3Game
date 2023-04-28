using UnityEngine;

public class Gem : MonoBehaviour
{
    public Sprite[] gemSprites;

    private SpriteRenderer spriteRenderer;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        int randomIndex = Random.Range(0, gemSprites.Length);
        spriteRenderer.sprite = gemSprites[randomIndex];
    }
}