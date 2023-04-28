using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GhostController : MonoBehaviour
{
    // state
    enum GhostState { FollowPlayer, Attack }
    GhostState currentState;
    GameObject target;
    SpriteRenderer spriteRenderer;

    public float maxHealth = 25f;
    float health;
    public float speed = 5f;

    Animator animator;

    //throttle
    protected int throttle = 0;
    protected int throttleBy = 100;

    private void Start()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        SetRandomColor();
        currentState = GhostState.FollowPlayer;
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

        if (Throttled(speed))
        {
            switch (currentState)
            {
                case GhostState.FollowPlayer:
                    Follow("Player");
                    break;
            }
        }
    }

    protected bool Throttled(float speed)
    {
        return throttle % throttleBy * speed == 0;
    }

    void SetRandomColor()
    {
        transform.GetComponent<SpriteRenderer>().color = new Color(Random.value, Random.value, Random.value);
    }

    bool HasPlayer()
    {
        // Check if there is dirt at the player's position
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 1f);
        Collider2D[] playerCol = colliders.Where(c => c.GetComponent<PlayerController>() != null).ToArray();
        foreach (Collider2D collider in playerCol)
        {
            PlayerController player = collider.GetComponent<PlayerController>();
            if (player != null)
            {
                return true;
            }
        }
        return false;
    }

    PlayerController ReturnPlayer()
    {
        // Check if there is dirt at the player's position
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 1f);
        Collider2D[] playerCol = colliders.Where(c => c.GetComponent<PlayerController>() != null).ToArray();
        foreach (Collider2D collider in playerCol)
        {
            PlayerController player = collider.GetComponent<PlayerController>();
            if (player != null)
            {
                return player;
            }
        }
        return null;
    }

    void Attack()
    {
        animator.SetBool("Attacking", true);
        if (Throttled(speed * 3) && !HasPlayer())
        {
            currentState = GhostState.FollowPlayer;
            animator.SetBool("Attacking", false);
        }
        else
        {
            PlayerController player = ReturnPlayer();
            if (player != null)
            {
                player.Damage(1);
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
                currentState = GhostState.Attack;
            }
        }
        else GetTarget(tag);
    }

    void GetTarget(string tag)
    {
        GameObject[] targets = GameObject.FindGameObjectsWithTag(tag);

        if (targets.Length == 0)
        {
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
        health -= amount;
        if (health < 1) Die();
        Manager.Instance.RaiseOpacity(gameObject, health, maxHealth);
    }

    void Die()
    {
        Destroy(gameObject);
    }

    #region sprite

    void HandleSprite()
    {
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

}
#endregion