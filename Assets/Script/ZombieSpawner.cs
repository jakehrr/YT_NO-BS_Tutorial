using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieSpawner : MonoBehaviour
{
    [SerializeField] private GameObject zombie;

    float randomInitialSpawn;

    [SerializeField] private float minSpawnTime;
    [SerializeField] private float maxSpawnTime;

    private GameManager manager;

    private void Start()
    {
        manager = GameObject.Find("Game Manager").GetComponent<GameManager>();

        randomInitialSpawn = Random.Range(minSpawnTime, maxSpawnTime);

        StartCoroutine(BeginZombieSpawn(randomInitialSpawn));
    }

    private IEnumerator BeginZombieSpawn(float spawnTime)
    {
        yield return new WaitForSeconds(spawnTime);

        if(manager.currentZombieCount >= manager.maxZombieCount) { yield break; }

        Instantiate(zombie, transform.position, transform.rotation);

        manager.currentZombieCount++;
        manager.currentZombiesAlive++;

        float nextSpawnTime = Random.Range(minSpawnTime, maxSpawnTime);
        StartCoroutine(BeginZombieSpawn(nextSpawnTime));
    }
}
