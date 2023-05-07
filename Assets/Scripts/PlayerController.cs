using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

    public enum EquippedTypes { Pick, Machete, TNT }

public class PlayerController : MonoBehaviour, IController
{

    protected SpriteRenderer spriteRenderer;

    // Move
    public float _speed = 5f;
    public float speed { get { return _speed; } set { _speed = value; } }
    protected Animator animator;

    // Mine
    bool isUsingAction = false;
    public float _miningSpeed = 3f;
    public float miningSpeed { get { return _miningSpeed; } set { _miningSpeed = value; } }

    // powerup
    float cooldown = 20f;

    // pick
    public float _pickLayer = 10;  //pick can mine up to this layer
    public float pickLayer { get { return _pickLayer; } set { _pickLayer = value; } }

    // health
    protected float health;

    public float _maxHealth = 10;
    public float maxHealth { get { return _maxHealth; } set { _maxHealth = value; } }


    // inventory
    protected List<GemScrObj> Gems = new List<GemScrObj>();
    List<EquippedTypes> equipment = new List<EquippedTypes> { EquippedTypes.Pick, EquippedTypes.Machete };
    EquippedTypes primaryEquipped = EquippedTypes.Pick;

    public GameObject TNTPrefab;

    //throttle
    protected int throttle = 0;
    protected int throttleBy = 100;

    private void Start()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        health = maxHealth;
        UpdateUI();
    }


    private void Update()
    {
        // throttler to avoid retriggering events
        throttle++;
        if (throttle == throttleBy * 10) throttle = 0;

        if (!isUsingAction && Input.anyKey) Move();

        if (Input.GetKey(KeyCode.Space))
        {
            StartEquipped();
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartEquippedAnim();
        }
        if (Input.GetKeyUp(KeyCode.Space))
        {
            StopEquipped();
            StopEquippedAnim();
        }

        if (Input.GetKeyDown(KeyCode.E) && !isUsingAction)
        {
            ChangeEquipped(1);
        }
        if (Input.GetKeyDown(KeyCode.Q) && !isUsingAction)
        {
            ChangeEquipped(-1);
        }
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if(collider.tag == "Item")
        {
            Item _item = collider.GetComponent<Item>();
            AddItem(_item);
            Destroy(collider.gameObject);
        }

        if (collider.tag == "Explosion")
        {
            Damage(8);
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
                spriteRenderer.flipX = true;
            }
            else if (horizontal > 0f)
            {
                spriteRenderer.flipX = false;
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
        UpdateUI();
    }

    void StartEquipped()
    {
        isUsingAction = true;
        switch (primaryEquipped)
        {
            case EquippedTypes.Pick:

                if (Throttled(miningSpeed))
                {
                    Mine();
                }
                break;

            case EquippedTypes.Machete:
                if (Throttled(miningSpeed))
                {
                    Attack();
                }
                break;

            case EquippedTypes.TNT:
                if (Throttled(miningSpeed))
                {
                    equipment.Remove(EquippedTypes.TNT);
                    primaryEquipped = EquippedTypes.Pick;
                    Manager.Instance.UpdateEquip(equipment.Select(i => i.ToString()).ToArray(), EquippedTypes.Pick.ToString());
                    Instantiate(TNTPrefab, transform.position, Quaternion.identity);
                }
                break;
        }
    }

    void StartEquippedAnim()
    {
        switch (primaryEquipped)
        {
            case EquippedTypes.Pick:
                animator.SetBool("Mining", true);
                break;
            case EquippedTypes.Machete:
                animator.SetBool("Attacking", true);
                break;
        }
    }

    void StopEquipped()
    {
        isUsingAction = false;
    }

    void StopEquippedAnim()
    {
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

    protected Collider2D CheckDirt()
    {

        // Check if there is dirt at the player's position
        Collider2D[] colliders = Physics2D.OverlapCircleAll(GetPickPos(), 0.25f);
        Collider2D[] filteredCol = colliders.Where(c => c.GetComponent<Dirt>() != null).ToArray();
        return filteredCol.Length > 0 ? filteredCol[0] : null;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(GetPickPos(), 0.25f);
    }

    void Mine()
    {
        Collider2D col = CheckDirt();
        if (col != null)
        {
            Dirt dirt = col.GetComponent<Dirt>();
            if (dirt != null && pickLayer <= dirt.depth)
            {
                // Start mining the dirt
                DamageDirt(dirt);
            }
        }
    }

    public void Damage(int damageBy)
    {
        DamageAnim();
        health -= damageBy;
        if (health < 1) Die();
        Manager.Instance.UpdateUI("Health", health.ToString());
    }

    protected void DamageAnim()
    {
        Manager.Instance.BloodAnim(transform.position);
    }

    protected void DeathAnim()
    {
        Manager.Instance.BloodAnim(transform.position);
    }

    void Die()
    {
        Manager.Instance.DeathAnim(transform.position);
        Manager.Instance.GameOver();
        Destroy(gameObject);
    }

    bool Attack()
    {
        // Check if there is enemy at the player's position
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 0.25f);
        Collider2D[] enemyCol = colliders.Where(c => c.GetComponent<RivalController>() != null || c.GetComponent<GhostController>() != null).ToArray();
        foreach (Collider2D collider in enemyCol)
        {
            var enemy = collider.GetComponent<RivalController>();
            if (enemy != null)
            {
                // Attack the enemy
                enemy.Damage(1f);
            }

            var ghost = collider.GetComponent<GhostController>();
            if (ghost != null)
            {
                // Attack the ghost
                ghost.Damage(1f);
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
    }

    public void CreateDoppleGanger()
    {
        Instantiate(gameObject, new Vector3(transform.position.x - 1f, transform.position.y, transform.position.z), Quaternion.identity);
    }

    public IEnumerator KillDoppleganger(float lifespan)
    {
        yield return new WaitForSeconds(lifespan);
        Manager.Instance.DeathAnim(transform.position);
        Destroy(gameObject);
    }

    #region inventory

    public void AddGem(GameObject _item)
    {
        GemScrObj gemData = _item.GetComponent<Gem>().gemData;
        Gems.Add(gemData);
    }

    void AddItem(Item _item)
    {
        Manager.Instance.ShowText(transform, _item.name, Color.white);
        equipment.Add(_item.equippedType);
    }

    #endregion

    #region UI
    void UpdateUI()
    {
        Manager.Instance.UpdateUI("Health", health.ToString());
        Manager.Instance.UpdateUI("MaxHealth", maxHealth.ToString());
    }
    #endregion

    #region utility

    protected Vector2 GetPickPos()
    {
        if (!spriteRenderer) spriteRenderer = GetComponent<SpriteRenderer>();
        return new Vector2(!spriteRenderer.flipX ? transform.position.x + 0.25f : transform.position.x - 0.25f, transform.position.y - 0.25f);
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
