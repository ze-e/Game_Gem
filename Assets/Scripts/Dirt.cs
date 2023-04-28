using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Dirt : MonoBehaviour
{
    public Sprite[] sprites;
    public GameObject[] gemPrefabs;

    public GameObject gridSpacePrefab;
    public GameObject dirtSprite;
    public GameObject healthText;

    public int toughness;
    public int maxLuck;
    float maxHealth;
    int luck;
    float health;

    void Start()
    {
        SetSprite();
        maxHealth = Random.Range(maxHealth / 2 + 1, toughness);
        health = maxHealth;
        luck = Random.Range(maxLuck/2 + 1, maxLuck);
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
        if (health > 0)
        {
            // reduce health
            health--;
            if (health < 1) health = 0;
            Manager.Instance.RaiseOpacity(dirtSprite, health, maxHealth);
        }
        //destroy when run out of health
        else if (health == 0) DestroyDirt();
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
        SpawnEmptyGridSpace();
        Destroy(gameObject);
    }

    void SpawnEmptyGridSpace()
    {
        Instantiate(gridSpacePrefab, transform.position, Quaternion.identity);
    }

}
