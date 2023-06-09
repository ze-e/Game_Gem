using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

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

    public int depth;
    public int maxDepth = 10;

    Color initColor;

    public GameObject wallPrefab;
    public int wallLuck;

    void Start()
    {
        //CreateWall();
        SetLayer();
        depth = maxDepth;
        initColor = dirtSprite.GetComponent<SpriteRenderer>().color;
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.tag == "Explosion")
        {
            Damage(Random.Range((int)(maxHealth / 4) , 100));
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Wall"))
        {
            Destroy(gameObject);
        }
    }

    void CreateWall()
    {
        int pull = Random.Range(0, wallLuck);
        if (pull == 1 && CheckGenerate())
        {
            Instantiate(wallPrefab, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }

    bool CheckGenerate()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 2f);
        Collider2D[] dirtCol = colliders.Where(c => c.GetComponent<Wall>() != null || c.GetComponent<RivalController>() != null).ToArray();
        if (dirtCol.Length > 0)
        {
            return false;
        }
        return true;
    }

    void OnDrawGizmos()
    {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(new Vector3(0f, 0f, 0f), new Vector3(4f, 4f, 4f));
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
        SetDepth();
    }

    void SetDepth()
    {
        var renderer = dirtSprite.GetComponent<SpriteRenderer>();

        switch (depth)
        {
            case 9:
                renderer.color = new Color(69f / 255f, 23f / 255f, 0f);
                break;
            case 8:
                renderer.color = new Color(255f / 255f, 77f / 255f, 0f);
                break;
            case 7:
                renderer.color = new Color(255f / 255f, 85f / 255f, 0f);
                break;
            case 6:
                renderer.color = new Color(255f / 255f, 204f / 255f, 0f);
                break;
            case 5:
                renderer.color = new Color(87f / 255f, 105f / 255f, 0f);
                break;
            case 4:
                renderer.color = new Color(4f / 255f, 77f / 255f, 0f);
                break;
            case 3:
                renderer.color = new Color(0f, 60f / 255f, 110f / 255f);
                break;
            case 2:
                renderer.color = new Color(69f / 255f, 65f / 255f, 107f / 255f);
                break;
            case 1:
                renderer.color = new Color(94f / 255f, 73f / 255f, 81f / 255f);
                break;
            default:
                if (depth > 9) renderer.color = initColor;
                break;
        }
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
        if (health == 0 && depth > 1) DestroyLayer();
        else if (health == 0 && depth == 1) DestroyDirt();
    }

    public void Damage(int amount)
    {
        if (health > 0)
        {
            // reduce health
            health-=amount;
            if (health < 1) health = 0;
            Manager.Instance.RaiseOpacity(dirtSprite, health, maxHealth);
        }
        //destroy when run out of health
        if (health == 0 && depth > 1) DestroyLayer();
        else if (health == 0 && depth == 1) DestroyDirt();
    }

    void DestroyLayer()
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
        depth--;
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
