using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerController : MonoBehaviour, IController
{
    enum EquippedTypes { Pick, Machete, TNT }

    // Move
    public float _speed = 5f;
    public float speed { get { return _speed; } set { _speed = value; } }
    protected Animator animator;

    // Mine
    bool isUsingAction = false;
    public float _miningSpeed = 3f;
    public float miningSpeed { get { return _miningSpeed; } set { _miningSpeed = value; } }

    // powerup
    float cooldown = 60f;

    // health
    protected float health;

    public float _maxHealth = 10;
    public float maxHealth { get { return _maxHealth; } set { _maxHealth = value; } }


    // inventory
    protected List<GemScrObj> Gems = new List<GemScrObj>();
    List<EquippedTypes> equipment = new List<EquippedTypes> { EquippedTypes.Pick, EquippedTypes.Machete };
    EquippedTypes primaryEquipped = EquippedTypes.Pick;

    //throttle
    protected int throttle = 0;
    protected int throttleBy = 100;

    private void Start()
    {
        animator = GetComponent<Animator>();
        health = maxHealth;
    }


    private void Update()
    {
        // throttler to avoid retriggering events
        throttle++;
        if (throttle == throttleBy * 10) throttle = 0;

        if (!isUsingAction) Move();

        if (Input.GetKey(KeyCode.Space))
        {
            StartEquipped();
        }
        if (Input.GetKeyUp(KeyCode.Space))
        {
            StopEquipped();
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            ChangeEquipped(1);
        }
        if (Input.GetKeyDown(KeyCode.Q))
        {
            ChangeEquipped(-1);
        }
    }

    protected bool Throttled(float speed)
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

    public void Heal()
    {
        health = maxHealth;
    }

    void StartEquipped()
    {
        isUsingAction = true;
        switch (primaryEquipped)
        {
            case EquippedTypes.Pick:
                animator.SetBool("Mining", true);

                if (Throttled(miningSpeed))
                {
                    Mine();
                }
                break;

            case EquippedTypes.Machete:
                animator.SetBool("Attacking", true);

                if (Throttled(miningSpeed))
                {
                    Attack();
                }
                break;
        }
    }

    void StopEquipped()
    {
        isUsingAction = false;
        switch (primaryEquipped)
        {
            case EquippedTypes.Pick:
                animator.SetBool("Mining", false);
                break;
            case EquippedTypes.Machete:
                animator.SetBool("Attacking", false);
                break;
        }
    }

    void ChangeEquipped(int changeBy)
    {
        int curIndex = equipment.IndexOf(primaryEquipped);
        if (curIndex + changeBy == equipment.Count) curIndex = 0;
        else if (curIndex + changeBy < 0) curIndex = equipment.Count - 1;
        else curIndex = curIndex + changeBy;
        primaryEquipped = equipment[curIndex];
        Manager.Instance.UpdateEquip(equipment.Select(i => i.ToString()).ToArray(), primaryEquipped.ToString());
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

    public void Damage(int damageBy)
    {
        health -= damageBy;
        if (health < 1) Die();
    }

    void Die()
    {
        Debug.Log("Game Over");
        Application.Quit();
    }

    bool Attack()
    {
        // Check if there is enemy at the player's position
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 1f);
        Collider2D[] enemyCol = colliders.Where(c => c.GetComponent<RivalController>() != null).ToArray();
        foreach (Collider2D collider in enemyCol)
        {
            RivalController enemy = collider.GetComponent<RivalController>();
            if (enemy != null)
            {
                // Attack the enemy
                enemy.Damage(1f);
            }
        }
        return false;
    }

    protected void DamageDirt(Dirt dirt)
    {
        if (dirt != null)
        {
            dirt.Damage();
        }
    }


    public IEnumerator ResetStat(float stat, Color statColor)
    {
        float init = stat;
        yield return new WaitForSeconds(cooldown);
        stat = init;
        Manager.Instance.ShowText(transform, "\n" + stat + " reset", statColor);
        Debug.Log("\n" + stat + " reset");
    }

    #region inventory

    public void AddGem(GameObject _item)
    {
        GemScrObj gemData = _item.GetComponent<Gem>().gemData;
        Gems.Add(gemData);
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
