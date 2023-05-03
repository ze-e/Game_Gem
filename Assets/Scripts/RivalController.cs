using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RivalController : PlayerController, IController
{
    // state
    enum RivalState {Check, Mine, FollowGem, Walk}
    [SerializeField]
    RivalState currentState;
    [SerializeField]
    GameObject target;

    public GameObject gemPrefab;

    private void Start()
    {
        animator = GetComponent<Animator>();
        SetRandomColor();
        currentState = RivalState.Check;
        health = maxHealth;
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
                case RivalState.Check:
                    Check();
                    break;
                case RivalState.Mine:
                    Mine();
                    break;
            }
        }
        else
        {
            switch (currentState)
            {
                case RivalState.FollowGem:
                    Follow("Gem");
                    break;
                case RivalState.Walk:
                    Follow("Dirt");
                    break;
            }
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
        Collider2D col = CheckDirt();
        if (col != null)
        {
            if (pickLayer <= col.gameObject.GetComponent<Dirt>().depth)
            {
                animator.SetBool("Mining", true);
                currentState = RivalState.Mine;
            }
            else currentState = RivalState.Walk;
        }
        else if (HasGem()) currentState = RivalState.FollowGem;
        else currentState = RivalState.Walk;
    }

    bool HasGem()
    {
        // Check if there is dirt at the player's position
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 1f);
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
        Collider2D col = CheckDirt();
        if (Throttled(speed * 3) && col == null) { 
            currentState = RivalState.Check; 
            animator.SetBool("Mining", false);
        }
        else
        {
            Dirt dirt = col.gameObject.GetComponent<Dirt>();
            if (dirt != null){
                if (pickLayer <= dirt.depth) DamageDirt(dirt);
                else
                {
                    currentState = RivalState.Check;
                    animator.SetBool("Mining", false);
                }
            }
        }
    }
    

    void Follow(string tag)
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
                currentState = RivalState.Check;
            }
        }
        else GetTarget(tag);
    }

    void GetTarget(string tag)
    {
        GameObject[] targets = GameObject.FindGameObjectsWithTag(tag);

        if (targets.Length == 0)
        {
            currentState = RivalState.Check;
            return;
        }

        GameObject nearest = targets[0];
        float nearestDistance = Vector2.Distance(transform.position, nearest.transform.position);

        for (int i = 1; i < targets.Length; i++)
        {
            float distance = Vector2.Distance(transform.position, targets[i].transform.position);
            if (distance < nearestDistance)
            {
                nearest = targets[i];
                nearestDistance = distance;
            }
        }

        target = nearest;
    }

    public void Damage(float amount)
    {
        DamageAnim();
        health -= amount;
        if (health < 1) Die();
        Manager.Instance.ReddenSprite(gameObject, health, maxHealth);
    }

    void Die()
    {
        Manager.Instance.DeathAnim(transform.position);
        foreach (GemScrObj _gem in Gems)
        {
            var newGem = Instantiate(gemPrefab, transform.position, Quaternion.identity);
            var newGemScr = newGem.GetComponent<Gem>();
            newGemScr.gemData = _gem;
            newGemScr.AttachData();
        }
        Manager.Instance.AddGhost();
        Destroy(gameObject);
    }

    #region sprite

    void HandleSprite () {
        Vector2 currentPosition = transform.position;
        if (!spriteRenderer) spriteRenderer = GetComponent<SpriteRenderer>();
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
