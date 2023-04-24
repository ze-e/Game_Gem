using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EmptyGrid : MonoBehaviour
{
    public GameObject objectToSpawn;
    public float maxSeconds = 10;
    float delayTime;

    public EmptyGrid(GameObject _objectToSpawn, float _maxSeconds)
    {
        objectToSpawn = _objectToSpawn;
        maxSeconds = _maxSeconds;
    }

    private void Start()
    {
        delayTime = Random.Range(maxSeconds/2 + 1, maxSeconds);
        StartCoroutine(CallMethodAfterDelay(delayTime, SpawnNewDirt));
    }

    IEnumerator CallMethodAfterDelay(float delayInSeconds, System.Action methodToCall)
    {
        yield return new WaitForSeconds(delayInSeconds);
        methodToCall?.Invoke();
    }

    void SpawnNewDirt()
    {
        GameObject newDirt = Instantiate(objectToSpawn, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}
