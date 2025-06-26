using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieSpawner : MonoBehaviour
{
    [SerializeField] private GameObject zombie;

    float randomInitialSpawn;

    [SerializeField] private float minSpawnTime;
    [SerializeField] private float maxSpawnTime;

    private void Start()
    {
        randomInitialSpawn = Random.Range(minSpawnTime, maxSpawnTime);

        StartCoroutine(BeginZombieSpawn(randomInitialSpawn));
    }

    private IEnumerator BeginZombieSpawn(float spawnTime)
    {
        spawnTime = Random.Range(minSpawnTime, maxSpawnTime);

        yield return new WaitForSeconds(spawnTime);

        Instantiate(zombie, this.transform.position, this.transform.rotation);

        StartCoroutine(BeginZombieSpawn(spawnTime));
    }
}
