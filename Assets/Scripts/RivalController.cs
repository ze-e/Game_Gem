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
    List<GameObject> lastTargets;

    public GameObject gemPrefab;

    //pathfinding
    public float speed = 5f;
    public float raycastDistance = 1f;
    public float raycastAngle = 15f;

    private void Start()
    {
        animator = GetComponent<Animator>();
        SetRandomColor();
        currentState = RivalState.Check;
        health = maxHealth;
        lastTargets = new List<GameObject>();
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

    bool CheckGem(Collider2D[] colliders)
    {
        Collider2D[] gemCol = colliders.Where(c => c.GetComponent<Gem>() != null).ToArray();
        if (gemCol.Length > 0)
        {
            currentState = RivalState.FollowGem;
            return true;
        }
        return false;
    }

    bool CheckDirt(Collider2D[] colliders)
    {
        Collider2D[] dirtCol = colliders.Where(c => c.GetComponent<Dirt>() != null).ToArray();

        if (dirtCol.Length > 0)
        {
            foreach (var item in dirtCol)
            {
                if (CanMine(item.gameObject))
                {
                    animator.SetBool("Mining", true);
                    currentState = RivalState.Mine;
                    return true;
                }
            }
        }
        return false;
    }

    void Check()
    {
        // Check if there is dirt at the player's position
        Collider2D[] colliders = Physics2D.OverlapCircleAll(GetPickPos(), 2f);
        if (CheckGem(colliders)) return;
        else if (CheckDirt(colliders)) return;
        currentState = RivalState.Walk;
    }


    void Mine()
    {
        Collider2D col = CheckDirt();
        if (col == null) return;
        Dirt dirt = col.gameObject.GetComponent<Dirt>();
        if (dirt != null && CanMine(dirt)) DamageDirt(dirt);
        else
        {
            animator.SetBool("Mining", false);
            if (CheckGem(Physics2D.OverlapCircleAll(GetPickPos(), 2f)))
            {
                currentState = RivalState.FollowGem;
                return;
            }
            currentState = RivalState.Walk;
        }
    }
    

    void Follow(string tag)
    {
        animator.SetBool("Walking", true);
        if (target != null)
        {
            // Move towards the target at the specified speed
            AvoidObstacles();
            //transform.position = Vector2.MoveTowards(transform.position, target.transform.position, speed * Time.deltaTime);

            // Check if the target has been reached
            if (transform.position == target.transform.position)
            {
                // Set target to null
                lastTargets.Add(target);
                if (lastTargets.Count > 3) lastTargets.RemoveAt(0);
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
            if (distance < nearestDistance && (tag == "Gem" || CanMine(targets[i])) && !lastTargets.Contains(targets[i]))
            {
                nearest = targets[i];
                nearestDistance = distance;
            }
        }

        if (tag == "Gem" || CanMine(nearest))  target = nearest; 
        else
        {
            currentState = RivalState.Walk;
        }
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

    bool CanMine(GameObject gameObject)
    {
        Dirt dirt = gameObject.GetComponent<Dirt>();
        if (dirt == null) return false;
        return pickLayer <= dirt.depth;
    }

    bool CanMine(Dirt dirt)
    {
        if (dirt == null) return false;
        return pickLayer <= dirt.depth;
    }

    void AvoidObstacles()
    {
        Vector2 currentDirection = Vector2.zero;
        Vector2 targetPosition = target.transform.position;

        // Calculate direction to target position
        Vector2 directionToTarget = targetPosition - (Vector2)transform.position;
        currentDirection = Vector2.MoveTowards(currentDirection, directionToTarget, Time.deltaTime * speed);
        
        // Cast ray in current direction
        RaycastHit2D hit = Physics2D.Raycast(transform.position, currentDirection, raycastDistance);
        if (hit.collider != null) {
            var ran = Random.Range(0, 100);
            if (ran == 1)
            {
                GetTarget(currentState == RivalState.FollowGem ? "Gem" : "Dirt");
                return;
            }
            // Calculate new direction by rotating current direction
            float angle = Mathf.Sign(Random.value - 0.5f) * raycastAngle;
            currentDirection = Quaternion.AngleAxis(angle, Vector3.back) * currentDirection;
            // Cast ray in new direction
            hit = Physics2D.Raycast(transform.position, currentDirection, raycastDistance);
        }
        
        // Move in current direction
        transform.position += (Vector3)currentDirection * Time.deltaTime * speed;
    }

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
