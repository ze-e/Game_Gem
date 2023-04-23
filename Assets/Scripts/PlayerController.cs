using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // Move
    public float speed = 5f;
    private Animator animator;

    //Mine
    bool isMining = false;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if(!isMining) Move();

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if(!isMining) Mine();
        }
        if (Input.GetKeyUp(KeyCode.Space))
        {
            animator.SetBool("Mining", false);
            isMining = false;
        }
    }

    void Move()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        Vector3 direction = new Vector3(horizontal, vertical, 0f).normalized;

        if (direction.magnitude > 0f)
        {
            // movement
            transform.position += direction * speed * Time.deltaTime;

            // animation
            animator.SetBool("Walking", true);
            if (horizontal < 0f)
            {
                transform.localScale = new Vector3(-1f, 1f, 1f);
            }
            else if (horizontal > 0f)
            {
                transform.localScale = new Vector3(1f, 1f, 1f);
            }
        }
        else
        {
            // animation
            animator.SetBool("Walking", false);
        }
    }

    void Mine()
    {
        // Check if there is dirt at the player's position
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 0.5f);
        foreach (Collider2D collider in colliders)
        {
            Dirt dirt = collider.GetComponent<Dirt>();
            if (dirt != null && !isMining)
            {
                // Start mining the dirt
                isMining = true;
                animator.SetBool("Mining", true);
                StartCoroutine(MineCoroutine(dirt));
                break;
            }
        }
    }

    private IEnumerator MineCoroutine(Dirt dirt)
    {
        while (isMining && dirt != null)
        {
            // Wait for the cooldown period
            yield return new WaitForSeconds(3f);

            // Damage the dirt
            DamageDirt(dirt);
        }
    }

    private void DamageDirt(Dirt dirt)
    {
        if (dirt != null)
        {
            dirt.Damage();
        }
    }
}
