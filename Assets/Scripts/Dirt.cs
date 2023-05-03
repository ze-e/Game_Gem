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

    //int depth;
    //int maxDepth = 10;

    //Color initColor;

    void Start()
    {
        SetLayer();
        //depth = Random.Range(3, maxDepth);
        //initColor = gameObject.GetComponent<SpriteRenderer>().color;
    }

    void SetLayer()
    {
        SetSprite();
        maxHealth = Random.Range(maxHealth / 2 + 1, toughness);
        health = maxHealth;
        luck = Random.Range(maxLuck / 2 + 1, maxLuck);
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

        // set depth 
        //SetDepth();
    }

    //void SetDepth()
    //{
    //    var renderer = gameObject.GetComponent<SpriteRenderer>();
    //    switch (depth)
    //    {
    //        case 3:
    //            renderer.color = new Color(106, 37,0);
    //            break;
    //        case 2:
    //            renderer.color =  new Color(224, 79, 0);
    //            break;
    //        case 1:
    //            renderer.color = new Color(106, 106, 106);
    //            break;
    //        default:
    //            if(depth > 3) renderer.color = initColor;
    //            break;
    //    }
    //}

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
        //else if (health == 0 && depth > 1) DestoryLayer();
        //else if (health == 0 && depth == 1) DestroyDirt();
        else if (health == 0) DestroyDirt();
    }

    void DestoryLayer()
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
        health = maxHealth;
        //depth--;
        SetLayer();
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
