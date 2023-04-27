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

    // inventory
    protected List<GameObject> Gems = new List<GameObject>();
    List<EquippedTypes> equipment = new List<EquippedTypes> { EquippedTypes.Pick, EquippedTypes.Machete };
    EquippedTypes primaryEquipped = EquippedTypes.Pick;

    //throttle
    protected int throttle = 0;
    protected int throttleBy = 100;

    private void Start()
    {
        animator = GetComponent<Animator>();
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
                //StopCoroutine("CooldownCoroutine");
                break;
            case EquippedTypes.Machete:
                animator.SetBool("Attacking", false);
                break;
        }
    }

    void ChangeEquipped()
    {
        int curIndex = equipment.IndexOf(primaryEquipped);
        curIndex = (curIndex + 1) % equipment.Count;
        primaryEquipped = equipment[curIndex];
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

    void Attack()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 0.1f);
        if (hit.collider != null)
        {
            RivalController enemy = hit.collider.GetComponent<RivalController>();

            if (enemy != null)
            {
                // Attack the enemy
                enemy.Damage(.5f);
            }
        }
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
        Manager.Instance.ShowText(transform,"\n" + stat + " reset", statColor);
        Debug.Log("\n" + stat + " reset");
    }

    #region inventory

    public void AddGem(GameObject _item)
    {
        Gems.Add(_item);
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
