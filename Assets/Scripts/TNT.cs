using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TNT : MonoBehaviour
{
    public float explosionDelay = 3.0f; // the delay in seconds before the TNT explodes
    public GameObject explosionPrefab; // the prefab for the explosion effect
    SpriteRenderer spriteRenderer;
    Color originalColor;

    [SerializeField]
    AudioSource audioSource;

    // Start is called before the first frame update
    void Start()
    {
        // start the countdown to the explosion
        StartCoroutine(ExplodeAfterDelay());
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        originalColor = spriteRenderer.color;
        InvokeRepeating("FlashWhite", 0.25f, 0.25f);
    }

    void FlashWhite()
    {
        StartCoroutine(FlashWhiteCoroutine());
    }

    IEnumerator FlashWhiteCoroutine()
    {
        // Set the sprite color to white over 0.1 seconds using Color.Lerp()
        float t = 0;
        while (t < 1)
        {
            Manager.Instance.PlaySFX(audioSource, "tick");
            spriteRenderer.color = Color.Lerp(originalColor, Color.grey, t);
            t += Time.deltaTime / 0.1f;
            yield return null;
        }

        // Set the sprite color back to its original color over 0.1 seconds using Color.Lerp()
        t = 0;
        while (t < 1)
        {
            spriteRenderer.color = Color.Lerp(Color.grey, originalColor, t);
            t += Time.deltaTime / 0.1f;
            yield return null;
        }
    }

    IEnumerator ExplodeAfterDelay()
    {
        // wait for the explosion delay
        yield return new WaitForSeconds(explosionDelay);
        audioSource.Stop();
        Manager.Instance.PlaySFX(audioSource, "explosion");
        Invoke("Explode", audioSource.clip.length);
    }

    void Explode()
    {
        // instantiate the explosion prefab at the TNT's position
        Instantiate(explosionPrefab, transform.position, Quaternion.identity);

        // destroy the TNT
        Destroy(gameObject);
    }
}