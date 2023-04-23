using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dirt : MonoBehaviour
{
    public Sprite[] sprites;
    public GameObject[] gemPrefabs;

    public int maxToughNess;
    public int maxLuck;
    int luck;
    int toughness;
    int health;

    void Start()
    {
        SetSprite();
        toughness = Random.Range(0, maxToughNess);
        luck = Random.Range(0, maxLuck);
    }

    void SetSprite()
    {
        // Select a random sprite from the array
        int index = Random.Range(0, sprites.Length);
        Sprite sprite = sprites[index];

        // Apply the sprite to the object
        GetComponent<SpriteRenderer>().sprite = sprite;

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
    }

    public void Damage()
    {
        health--;
        if (health < 1) DestroyDirt();
    }

    public void Restore()
    {
        health = toughness;
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
        Destroy(gameObject);
    }
}
