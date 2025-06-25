using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ZombieSpawner : MonoBehaviour
{
    [SerializeField] private GameObject zombie;

    float randomInitialSpawn;

    [SerializeField] private float minSpawn;
    [SerializeField] private float maxSpawn;

    private void Start()
    {
        randomInitialSpawn = Random.Range(minSpawn, maxSpawn);

        StartCoroutine(SpawnZombie(randomInitialSpawn));
    }

    private IEnumerator SpawnZombie(float spawnTime)
    {
        spawnTime = Random.Range(minSpawn, maxSpawn);
        Debug.Log(spawnTime);

        yield return new WaitForSeconds(spawnTime); 

        Instantiate(zombie, this.transform.position, this.transform.rotation);

        StartCoroutine(SpawnZombie(spawnTime));
    }
}
