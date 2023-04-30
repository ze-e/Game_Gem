using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionText : MonoBehaviour
{
    public float duration = 100;
    public float speed = 2f;
    public float size = 1f;

    private SpriteRenderer spriteRenderer;
    bool isBelowTopEdge = true;

    private void Start()
    {
        transform.localScale = new Vector3(size, size, size);
        spriteRenderer = GetComponent<SpriteRenderer>();
        StartCoroutine(FadeOut());
    }

    void FixedUpdate()
    {
        if (isBelowTopEdge)
        {
            Move();
            //GetTopEdge();
        }
    }

    void Move()
    {
            transform.position += new Vector3(0, speed * Time.deltaTime, 0);
    }

    void GetTopEdge()
    {
        // Get the position of the top edge of the camera
        float topEdgePosition = Camera.main.transform.position.y + Camera.main.orthographicSize;

        // Check if the object is below the top edge of the camera by at least 10 pixels
        if (transform.position.y > topEdgePosition - 10)
        {
            isBelowTopEdge = false;
        }
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
            //Manager.Instance.RaiseOpacity(gameObject, elapsedTime, duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        Destroy(gameObject);
    }
}
