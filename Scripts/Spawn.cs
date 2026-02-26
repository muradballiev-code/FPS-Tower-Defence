using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawn : MonoBehaviour
{
    private int spawnedCount;
    [SerializeField]
    private int maxEnemies = 15; //Set max Enemies count
    private float nextSpawnTime;
    [SerializeField]
    private float firstSpawnDelay = 2f;  //Set how many seconds the first enemy should appear
    [SerializeField]
    private float spawnInterval = 6f; //Set interval between spawn

    private void Start()
    {
        UIManager.Instance.EnemyCount(maxEnemies);
        SpawnManager.Instance.GetEnemiesMax(maxEnemies);
        nextSpawnTime = Time.time + firstSpawnDelay; //Set timing to start spawn Enemy
    }

    private void Update()
    {
        //check how many enemies have spawned and whether this exceeds the limit, if YES then do nothing
        if (spawnedCount >= maxEnemies)
        {
            return;
        }

        //start spawn enemies
        if (Time.time >= nextSpawnTime)
        {
            SpawnEnemy();
            nextSpawnTime = Time.time + Random.Range(firstSpawnDelay, spawnInterval);
        }
    }

    void SpawnEnemy()
    {
        SpawnManager.Instance.StartEnemySpawn();
        spawnedCount ++;
    }
    
    //send how many enemis can be spawn to SpawnManager script 
    public int GetMaxEnemies()
    {
        return maxEnemies;
    }
}
