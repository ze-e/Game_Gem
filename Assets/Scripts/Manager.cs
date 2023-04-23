using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manager : MonoBehaviour
{
    public static Manager Instance { get; private set; }

    public GameObject gemPrefab;
    public Transform boundary;
    public float spawnInterval = 10f;

    private List<GameObject> gems = new List<GameObject>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        StartCoroutine(SpawnGems());
    }

    private IEnumerator SpawnGems()
    {
        while (true)
        {
            // Wait for the spawn interval
            yield return new WaitForSeconds(spawnInterval);

            // Get a random position within the boundary
            Vector3 randomPosition = new Vector3(
                Random.Range(boundary.position.x - boundary.localScale.x / 2f, boundary.position.x + boundary.localScale.x / 2f),
                Random.Range(boundary.position.y - boundary.localScale.y / 2f, boundary.position.y + boundary.localScale.y / 2f),
                0f
            );

            // Check if there are no other gems at the spawn location
            bool canSpawn = true;
            foreach (GameObject gem in gems)
            {
                if (gem != null && Vector3.Distance(gem.transform.position, randomPosition) < 1f)
                {
                    canSpawn = false;
                    break;
                }
            }

            // Spawn a new gem if possible
            if (canSpawn)
            {
                GameObject gem = Instantiate(gemPrefab, randomPosition, Quaternion.identity);
                gems.Add(gem);
            }
        }
    }
}
