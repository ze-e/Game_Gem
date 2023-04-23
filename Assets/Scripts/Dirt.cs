using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dirt : MonoBehaviour
{
    public Sprite[] sprites;
    public GameObject[] gemPrefabs;

    public GameObject dirtSprite;
    
    public int toughness;
    public int maxLuck;
    int luck;
    int maxHealth;
    int health;

    void Start()
    {
        SetSprite();
        maxHealth = Random.Range(1, toughness);
        luck = Random.Range(1, maxLuck);
    }

    void SetSprite()
    {

        // Select a random sprite from the array
        int index = Random.Range(0, sprites.Length);
        Sprite chosenSprite = sprites[index];

        // Apply the sprite to the object
        SpriteRenderer spriteRenderer = dirtSprite.GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = chosenSprite;

        // Randomly flip or rotate the sprite
        bool flipX = Random.value < 0.5f;
        bool flipY = Random.value < 0.5f;
        float rotation = Random.Range(0, 4) * 90f;

        Vector3 scale = transform.localScale;
        if (flipX)
        {
            scale.x *= -1;
        }
        if (flipY)
        {
            scale.y *= -1;
        }
        transform.localScale = scale;

        transform.Rotate(new Vector3(0, 0, rotation));

        // reset opacity
        Color newColor = spriteRenderer.color;
        newColor.a = 1;
        spriteRenderer.color = newColor;
    }

    public void Damage()
    {
        // reduce health
        health--;

        // Reduce opacity of SpriteRenderer based on remaining health
        SpriteRenderer spriteRenderer = dirtSprite.GetComponent<SpriteRenderer>();
        float opacity = (float)health / (float)maxHealth;
        Color newSpriteColor = spriteRenderer.color;
        newSpriteColor.a = Mathf.Abs(opacity);
        spriteRenderer.color = newSpriteColor;

        //destroy when run out of health
        if (health < 1) DestroyDirt();
    }

    void DestroyDirt()
    {
        int pull = Random.Range(0, luck);
        if (pull == 1)
        {
            int chosenGem = Random.Range(0, gemPrefabs.Length);
            if (chosenGem < gemPrefabs.Length)
            {
                Instantiate(gemPrefabs[chosenGem], transform.position, Quaternion.identity);
            }
        }
        SpawnNewDirt();
        Destroy(gameObject);
    }

    void SpawnNewDirt()
    {
        GameObject newDirt = Instantiate(gameObject, transform.position, Quaternion.identity);
        newDirt.GetComponent<Dirt>().toughness = toughness;
        newDirt.GetComponent<Dirt>().maxLuck = maxLuck;
    }
}
