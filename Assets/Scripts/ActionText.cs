using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionText : MonoBehaviour
{
    public float duration = 100;

    public float speed = 2f;

    private SpriteRenderer spriteRenderer;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        StartCoroutine(FadeOut());
    }

    void FixedUpdate()
    {
        Move();
    }

    void Move()
    {
        transform.position += new Vector3(0, speed * Time.deltaTime, 0);
    }


    private IEnumerator FadeOut()
    {
        Color spriteColor = spriteRenderer.color;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            float alpha = Mathf.Lerp(1f, 0f, elapsedTime / duration);
            spriteColor.a = alpha;
            spriteRenderer.color = spriteColor;
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        Destroy(gameObject);
    }
}
