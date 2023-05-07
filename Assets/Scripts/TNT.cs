using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TNT : MonoBehaviour
{
    public float explosionDelay = 3.0f; // the delay in seconds before the TNT explodes
    public GameObject explosionPrefab; // the prefab for the explosion effect

    private bool exploded = false;
    //throttle
    int throttle = 0;
    public int throttleBy = 100;

    // Start is called before the first frame update
    void Start()
    {
        // start the countdown to the explosion
        StartCoroutine(ExplodeAfterDelay());
    }

    //private void FixedUpdate()
    //{
    //    if (throttle % throttleBy == 0)
    //    {
            //Manager.Instance.RaiseOpacity(gameObject, throttle, explosionDelay * 10);
    //    }
    //    throttle++;
    //}



    IEnumerator ExplodeAfterDelay()
    {
        // wait for the explosion delay
        yield return new WaitForSeconds(explosionDelay);

        // if the TNT hasn't exploded yet, explode now
        if (!exploded)
        {
            Explode();
        }
    }

    void Explode()
    {
        // instantiate the explosion prefab at the TNT's position
        Instantiate(explosionPrefab, transform.position, Quaternion.identity);

        // destroy the TNT
        Destroy(gameObject);
    }
}