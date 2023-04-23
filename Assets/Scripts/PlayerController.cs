using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // Move
    public float speed = 5f;
    private Animator animator;

    //Mine
    public float miningRate = 1f; 
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
            if(!isMining) StartCoroutine(Mine());
        }
        if (Input.GetKeyUp(KeyCode.Space))
        {
            animator.SetBool("Mining", false);
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

    IEnumerator Mine()
    {
        Debug.Log("Mining");
        // Check if there is dirt at the player's position
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 0.5f);
        foreach (Collider2D collider in colliders)
        {
            Dirt dirt = collider.GetComponent<Dirt>();
            if (dirt != null && !isMining)
            {
                isMining = true;
                // Start mining the dirt
                Debug.Log("FoundDirt");
                animator.SetBool("Mining", true);
                DamageDirt(dirt);
                yield return new WaitForSeconds(miningRate);
                isMining = false;
                break;
            }
            else
            {
                isMining = false;
                animator.SetBool("Mining", false);
            }
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
