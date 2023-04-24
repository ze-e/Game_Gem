using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // Move
    public float speed = 5f;
    private Animator animator;

    //Mine
    bool isMining = false;
    bool canMine = true;
    public float miningSpeed = 3f;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if(!isMining) Move();

        if (Input.GetKey(KeyCode.Space))
        {
            animator.SetBool("Mining", true);
            isMining = true;
            
            if (canMine)
            {
                Mine();
            }
        }
        if (Input.GetKeyUp(KeyCode.Space))
        {
            animator.SetBool("Mining", false);
            isMining = false;
            StopCoroutine("CooldownCoroutine");
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
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 0.1f);
        if (hit.collider != null)
        {
            Debug.Log("hit" + hit.collider.name);
            Dirt dirt = hit.collider.GetComponent<Dirt>();

            if (dirt != null)
            {
                // Start mining the dirt
                StartCoroutine(CooldownCoroutine(miningSpeed, canMine, DamageDirt, dirt));
            }
        }
    }

    IEnumerator CooldownCoroutine<T>(float delay, bool methodBool, System.Action<T> delayedMethod, T arg)
    {
        #pragma warning disable IDE0059 // Unnecessary assignment of a value
        methodBool = false;
        yield return new WaitForSeconds(delay);
        delayedMethod(arg);
        methodBool = true;
        #pragma warning restore IDE0059 // Unnecessary assignment of a value
    }

    private void DamageDirt(Dirt dirt)
    {
        if (dirt != null)
        {
            dirt.Damage();
        }
    }
}
