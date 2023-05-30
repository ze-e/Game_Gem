using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : MonoBehaviour
{
    public Sprite dirtSprite;
    public Sprite[] sprites;
    // Start is called before the first frame update
    void Start()
    {
        SetSprite();
        DrawDirtSprite();
    }

    void SetSprite()
    {

        // Select a random sprite from the array
        int index = Random.Range(0, sprites.Length);
        Sprite chosenSprite = sprites[index];

        // Apply the sprite to the object
        SpriteRenderer spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = chosenSprite;
        AdjustColliderToSprite();
    }

    void AdjustColliderToSprite()
    {
        // Get the sprite renderer of the game object
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();

        // Get the size of the sprite
        Vector2 spriteSize = spriteRenderer.bounds.size;

        // Get the BoxCollider2D component of the game object
        BoxCollider2D collider = GetComponent<BoxCollider2D>();

        // Set the size of the BoxCollider2D to match the size of the sprite
        collider.size = spriteSize * 0.5f;
    }

    void OnDrawGizmos()
    {
        BoxCollider2D collider = GetComponent<BoxCollider2D>();
        if (collider != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(transform.position + new Vector3(collider.offset.x, collider.offset.y, 0f), new Vector3(collider.size.x, collider.size.y, 0f));
        }
    }

    void DrawDirtSprite()
    {
        // Create a new gameobject for the dirt sprite
        GameObject dirtObject = new GameObject("DirtSprite");
        dirtObject.transform.position = transform.position;
        dirtObject.transform.SetParent(transform);

        // Add a sprite renderer and set the sprite
        SpriteRenderer spriteRenderer = dirtObject.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = dirtSprite;

        // Set the sorting order to be behind the parent object
        spriteRenderer.sortingOrder = 1;

        // Offset the position of the dirt sprite to be directly underneath the parent object
        Vector3 dirtPosition = dirtObject.transform.position;
        //dirtPosition.y -= (spriteRenderer.bounds.size.y / 2f);
        dirtObject.transform.position = dirtPosition;
    }

}
