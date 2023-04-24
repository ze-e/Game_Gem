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
    public float miningSpeed = 3f;

    //throttle
    int throttle = 0;
    int throttleBy = 100;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }


    private void Update()
    {
        // throttler to avoid retriggering events
        throttle++;
        if (throttle == throttleBy * 10) throttle = 0;

        if (!isMining) Move();

        if (Input.GetKey(KeyCode.Space))
        {
            animator.SetBool("Mining", true);
            isMining = true;

            if (Throttled(miningSpeed))
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

    bool Throttled(float speed)
    {
        return throttle % throttleBy * speed == 0;
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
            Dirt dirt = hit.collider.GetComponent<Dirt>();
            
            if (dirt != null)
            {
                // Start mining the dirt
                DamageDirt(dirt);
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

    #region utility

    #endregion

    #region deprecated
    IEnumerator DelayCoroutine<T>(float delay, System.Action<T> delayedMethod, T arg)
    {
        yield return new WaitForSeconds(delay);
        delayedMethod(arg);
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
    #endregion deprecated
}
