using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RivalController : PlayerController
{
    // state
    enum RivalState {Idle, Check, Mine, FollowGem, Walk}
    RivalState currentState;
    GameObject target;
    SpriteRenderer spriteRenderer;

    private void Start()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        SetRandomColor();
        currentState = RivalState.Idle;
    }

    private void FixedUpdate()
    {
        if (target != null)
        {
            HandleSprite();
        }
    }

    private void Update()
    {
        // throttler to avoid retriggering events
        throttle++;
        if (throttle == throttleBy * 10) throttle = 0;

        if(Throttled(speed))
        {
            switch (currentState)
            {
                case RivalState.Idle:
                    SetRandomState();
                    break;
                case RivalState.Check:
                    Check();
                    break;
                case RivalState.Mine:
                    Mine();
                    break;
                case RivalState.FollowGem:
                    FollowGem();
                    break;
                case RivalState.Walk:
                    Walk();
                    break;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Gem"))
        {
            GameObject gem = collision.gameObject; 


            // Set RivalScore to 100
            Manager.Instance.AddRivalScore(gem.GetComponent<Gem>().score);
            Destroy(gem);

            currentState = RivalState.Idle;
        }
    }

    void SetRandomColor()
    {
        transform.GetComponent<SpriteRenderer>().color = new Color(Random.value, Random.value, Random.value);
    }

    void SetRandomState()
    {
        currentState = (RivalState)Random.Range(0, System.Enum.GetValues(typeof(RivalState)).Length);
    }

    void Check()
    {
        if (HasDirt())
        {
            animator.SetBool("Mining", true);
            currentState = RivalState.Mine;
        }
        if (HasGem()) currentState = RivalState.FollowGem;
        else currentState = RivalState.Idle;
    }

    void FollowGem()
    {
        animator.SetBool("Walking", true);
        if (target != null)
        {
            // Move towards the target at the specified speed
            transform.position = Vector2.MoveTowards(transform.position, target.transform.position, speed * Time.deltaTime);

            // Check if the target has been reached
            if (transform.position == target.transform.position)
            {
                // Set target to null
                target = null;
                animator.SetBool("Walking", false);
                currentState = RivalState.Idle;
            }
        }
        else GetTargetGem();
    }

    private void GetTargetGem()
    {
        // Find all GameObjects with the tag "Gem"
        GameObject[] gems = GameObject.FindGameObjectsWithTag("Gem");

        // If there are no Gems, return
        if (gems.Length == 0)
        {
            currentState = RivalState.Idle;
            return;
        }

        // Set the first Gem as the initial target
        GameObject nearestGem =  gems[0];
        float nearestDistance = Vector2.Distance(transform.position, nearestGem.transform.position);

        // Loop through all Gems to find the nearest one
        for (int i = 1; i < gems.Length; i++)
        {
            float distance = Vector2.Distance(transform.position, gems[i].transform.position);
            if (distance < nearestDistance)
            {
                nearestGem = gems[i];
                nearestDistance = distance;
            }
        }

        // Set the nearest Gem as the target
        target = nearestGem;
    }

    bool HasDirt()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 0.1f);
        if (hit.collider != null)
        {
            Dirt dirt = hit.collider.GetComponent<Dirt>();

            if (dirt != null)
            {
                return true;
            }
        }
        return false;
    }

    Dirt ReturnDirt()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 0.1f);
        if (hit.collider != null)
        {
            Dirt dirt = hit.collider.GetComponent<Dirt>();

            if (dirt != null)
            {
                return dirt;
            }
        }
        return null;
    }

    bool HasGem()
    {
        // Check if there is dirt at the player's position
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 0.1f);
        Collider2D[] gemCol = colliders.Where(c => c.GetComponent<Gem>() != null).ToArray();
        foreach (Collider2D collider in gemCol)
        {
            Gem gem = collider.GetComponent<Gem>();
            if (gem != null)
            {
                return true;
            }
        }
        return false;
    }

    void Mine()
    {
        if (Throttled(speed * 3) && !HasDirt()) { 
            currentState = RivalState.Check; 
            animator.SetBool("Mining", false);
        }
        else
        {
            Dirt dirt = ReturnDirt();
            if (dirt != null)
            {
                DamageDirt(dirt);
            }
        }
    }

    void Walk()
    {
        animator.SetBool("Walking", true);
        if (target != null)
        {
            // Move towards the target at the specified speed
            transform.position = Vector2.MoveTowards(transform.position, target.transform.position, speed * Time.deltaTime);

            // Check if the target has been reached
            if (transform.position == target.transform.position)
            {
                // Set target to null
                target = null;

                animator.SetBool("Walking", false);
                currentState = RivalState.Idle;
            }
        }
        else GetTargetDirt();
    }

    private void GetTargetDirt()
    {
        GameObject[] dirt = GameObject.FindGameObjectsWithTag("Dirt");

        if (dirt.Length == 0)
        {
            currentState = RivalState.Idle;
            return;
        }

        GameObject nearestGem = dirt[0];
        float nearestDistance = Vector2.Distance(transform.position, nearestGem.transform.position);

        for (int i = 1; i < dirt.Length; i++)
        {
            float distance = Vector2.Distance(transform.position, dirt[i].transform.position);
            if (distance < nearestDistance)
            {
                nearestGem = dirt[i];
                nearestDistance = distance;
            }
        }

        target = nearestGem;
    }

    #region sprite

    void HandleSprite () {
        Vector2 currentPosition = transform.position;

        if (currentPosition.x < target.transform.position.x)
        {
            spriteRenderer.flipX = false; // face right
        }
        else if (currentPosition.x > target.transform.position.x)
        {
            spriteRenderer.flipX = true; // face left
        }
    }

    #endregion

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
