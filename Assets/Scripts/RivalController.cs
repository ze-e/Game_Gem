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
    //reset pathfinding
    public float targetDelay = 10f;

    //debug
    public GameObject miningObject;

    private void Start()
    {
        animator = GetComponent<Animator>();
        SetRandomColor();
        currentState = RivalState.Check;
        health = maxHealth;
        lastTargets = new List<GameObject>();

        //reset pathfinding
        StartCoroutine(RepeatFindNewTarget(10f));

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

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.tag == "Item")
        {
            Item _item = collider.GetComponent<Item>();
            AddItem(_item);
            Destroy(collider.gameObject);
        }

        if (collider.tag == "Explosion")
        {
            Damage(collider.GetComponent<Explosion>().damage);
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
        List<GameObject> colliderList = new List<GameObject>();
        foreach (var item in colliders)
        {
            if (item.CompareTag("Dirt"))
            {
                colliderList.Add(item.gameObject);
            }
        }

        if (colliderList.Count > 0)
        {
            foreach (var item in colliderList)
            {
                if (item.GetComponent<Dirt>() != null && CanMine(item) == true)
                {
                    miningObject = item;
                    if (item.CompareTag("Rival")) {
                        foreach (var i in colliderList)
                        {
                            Debug.Log(i);
                        }
                        Debug.LogError("how many times do I have to check this??????" + miningObject); 
                    }
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
        if (equipment.Contains(EquippedTypes.TNT))
        {
            equipment.Remove(EquippedTypes.TNT);
            Instantiate(TNTPrefab, transform.position, Quaternion.identity);
            return;
        }
        else
        {
            // Check if there is dirt at the player's position
            Collider2D[] colliders = Physics2D.OverlapCircleAll(GetPickPos(), 2f);
            if (CheckGem(colliders)) return;
            else if (CheckDirt(colliders)) return;
            currentState = RivalState.Walk;
        }
    }


    void Mine()
    {
        Collider2D col = CheckDirt();
        if (col == null) {
            miningObject = null;
            currentState = RivalState.Walk;
            return;
        };
        Dirt dirt = col.gameObject.GetComponent<Dirt>();
        if (dirt != null && CanMine(dirt)) {
            //sfx
            Manager.Instance.PlaySFX(audioSource, "mine");

            //attack dirt
            DamageDirt(dirt);
        }
        else
        {
            //sfx
            audioSource.Stop();

            //anim
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
            transform.position = Vector2.MoveTowards(transform.position, target.transform.position, speed * Time.deltaTime);

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
            if (distance < nearestDistance && (tag == "Gem" || CanMine(targets[i])) && !lastTargets.Contains(targets[i]) && CanMove(nearest))
            {
                nearest = targets[i];
                nearestDistance = distance;
            }
        }

        if ((tag == "Gem" || CanMine(nearest)) && CanMove(nearest))  target = nearest; 
        else
        {
            //currentState = RivalState.Walk;
            currentState = RivalState.Check;
        }
    }

    private IEnumerator RepeatFindNewTarget(float delay)
    {
            if (currentState == RivalState.Walk || currentState == RivalState.FollowGem)
            {
                GetTarget(currentState == RivalState.Walk ? "Dirt" : "Walk");
            }

            yield return new WaitForSeconds(delay);
    }


    public new void Damage(float amount)
    {
        Manager.Instance.PlaySFX(audioSource, "hurt");
        DamageAnim();
        health -= amount;
        if (health < 1) Die();
        Manager.Instance.ReddenSprite(gameObject, health, maxHealth);
    }

    public new void Heal()
    {
        health = maxHealth;
    }

    void Die()
    {
        Manager.Instance.PlaySFX(audioSource, "die");
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

    bool CanMove(GameObject gameObject)
    {
        Vector2 startPos = transform.position;
        Vector2 endPos = gameObject.transform.position;

        // Perform the raycast
        RaycastHit2D hit = Physics2D.Linecast(startPos, endPos);

        if (hit.collider != null && (hit.collider.CompareTag("Wall") || hit.collider.CompareTag("Empty")))
        {
            return false; 
        }

        return true; 
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
