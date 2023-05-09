using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    public int damage = 8;
    private void Start()
    {
        DestroyAfterAnimation(gameObject);
    }

    public void DestroyAfterAnimation(GameObject gameObject)
    {
        Animator animator = gameObject.GetComponent<Animator>();
        if (animator != null)
        {
            AnimationClip clip = animator.GetCurrentAnimatorClipInfo(0)[0].clip;
            float clipLength = clip.length;
            Destroy(gameObject, clipLength);
        }
    }
}
