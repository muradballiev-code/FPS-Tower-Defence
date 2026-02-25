using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    private static SpawnManager _instance;
    public static SpawnManager Instance
    {
        get
        {
            if (_instance == null)
            {
                Debug.LogError("SpawnManager is NULL");
            }

            return _instance;
        }
    }

    private bool _canSpawn = true;
    //private Spawn _enemyMaxAmount;
    private int _enemyMaxAmount;
    [SerializeField]
    private GameObject _enemyPrefab;
    [SerializeField]
    private Transform _enemySpawnPoint;
    [SerializeField]
    private List<GameObject> _enemyList;
    [SerializeField]
    private GameObject _enemyParentContainer;

    private void Awake()
    {
        _instance = this;
        //DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        //get max enemis number from Spawn script
        //_enemyMaxAmount = _enemySpawnPoint.GetComponent<Spawn>();
        //_enemyList = GenereateEnemy(_enemyMaxAmount.GetMaxEnemies());
        _enemyList = GenereateEnemy(_enemyMaxAmount);
    }

    List<GameObject> GenereateEnemy(int enemyMaxAmount)
    {
        for (int i = 0; i < enemyMaxAmount; i++)
        {
            GameObject enemy = Instantiate(_enemyPrefab);
            enemy.transform.parent = _enemyParentContainer.transform;
            enemy.SetActive(false);
            _enemyList.Add(enemy);
        }

        return _enemyList;
    }

    public GameObject StartEnemySpawn()
    {
        if (!_canSpawn)
        return null;

        foreach (var enemy in _enemyList)
        {
            if (enemy.activeInHierarchy == false)
            {
                enemy.transform.position = _enemySpawnPoint.position;
                enemy.transform.rotation = _enemySpawnPoint.rotation;

                enemy.SetActive(true);

                return enemy;
            }
        }

        /*
        //plan B
        GameObject newEnemy = Instantiate(_enemyPrefab);
        newEnemy.transform.parent = _enemyParentContainer.transform;
        newEnemy.transform.position = _enemySpawnPoint.position;
        newEnemy.transform.rotation = _enemySpawnPoint.rotation;
        newEnemy.SetActive(true);
        _enemyList.Add(newEnemy);
        return newEnemy;
        */

        Debug.LogWarning("SpawnManager, Enemy pool exhausted! Current size: " + _enemyList.Count);
        return null;
    }

    public void StopSpawning()
    {
        _canSpawn = false;
    }

    /*
    //plan C
    public void ReturnEnemyToPool(GameObject enemy)
    {
        if (enemy == null)
        return;

        enemy.transform.SetParent(_enemyParentContainer.transform);
        enemy.SetActive(false);

        //enemy.GetComponent<Enemy>().ResetState();
    }
    */

    /*
    //plan D
    public int GetActiveEnemyCount()
    {
        int count = 0;
        foreach (var enemy in _enemyList)
        {
            if (enemy.activeInHierarchy) count++;
        }
        return count;
    }
    */

    /*
    //plan E
    public void ClearAllEnemies()
    {
        foreach (var enemy in _enemyList)
        {
            if (enemy != null) 
            Destroy(enemy);
        }
        _enemyList.Clear();
    }
    */

    public void DestroyEnemyByOne(GameObject enemyDestroy)
    {
        if (enemyDestroy == null) return;

        if (_enemyList.Contains(enemyDestroy))
        {
            enemyDestroy.SetActive(false);
            Destroy(enemyDestroy);
            _enemyList.Remove(enemyDestroy);

            Debug.Log($"Enemy destroyed! Pool size now: {_enemyList.Count}");
        }
        else
        {
            Debug.Log("Enemy not found in pool!");
        }
    }

    /*
    //plan F
    public void ResetAllEnemies()
    {
       _enemyList = GenereateEnemy(_enemyMaxAmount.GetMaxEnemies());
    }
    */

    //plan G
    public void GetEnemiesMax(int amount)
    {
       _enemyMaxAmount = amount;
    }
}
