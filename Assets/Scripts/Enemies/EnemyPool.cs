using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyPool : MonoBehaviour
{
    public static EnemyPool Instance { get; private set; }

    public int poolSize = 10;

    [Header("Enemy Prefabs")]
    public GameObject archerEnemyPrefab;
    public GameObject thugEnemyPrefab;
    public GameObject bossKnightPrefab;
    public GameObject blackWidowPrefab;
    public GameObject redWidowPrefab;
    public GameObject grayWidowPrefab;
    public GameObject bossWidowPrefab;

    private Queue<GameObject> archerEnemyPool = new Queue<GameObject>();
    private Queue<GameObject> thugEnemyPool = new Queue<GameObject>();
    private Queue<GameObject> bossKnightPool = new Queue<GameObject>();
    private Queue<GameObject> blackWidowPool = new Queue<GameObject>();
    private Queue<GameObject> redWidowPool = new Queue<GameObject>();
    private Queue<GameObject> grayWidowPool = new Queue<GameObject>();
    private Queue<GameObject> bossWidowPool = new Queue<GameObject>();

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        InitPool(archerEnemyPrefab, archerEnemyPool);
        InitPool(thugEnemyPrefab, thugEnemyPool);
        InitPool(bossKnightPrefab, bossKnightPool);
        InitPool(blackWidowPrefab, blackWidowPool);
        InitPool(redWidowPrefab, redWidowPool);
        InitPool(grayWidowPrefab, grayWidowPool);
        InitPool(bossWidowPrefab, bossWidowPool);
    }

    private void InitPool(GameObject prefab, Queue<GameObject> pool)
    {
        for (int i = 0; i < poolSize; i++)
        {
            GameObject enemy = Instantiate(prefab, this.transform);
            enemy.SetActive(false);
            pool.Enqueue(enemy);
        }
    }

    public GameObject GetArcherEnemy() => GetEnemyFromPool(archerEnemyPrefab, archerEnemyPool);
    public GameObject GetThugEnemy() => GetEnemyFromPool(thugEnemyPrefab, thugEnemyPool);
    public GameObject GetBossKnightEnemy() => GetEnemyFromPool(bossKnightPrefab, bossKnightPool);
    public GameObject GetBlackWidowEnemy() => GetEnemyFromPool(blackWidowPrefab, blackWidowPool);
    public GameObject GetRedWidowEnemy() => GetEnemyFromPool(redWidowPrefab, redWidowPool);
    public GameObject GetGrayWidowEnemy() => GetEnemyFromPool(grayWidowPrefab, grayWidowPool);
    public GameObject GetBossWidowEnemy() => GetEnemyFromPool(bossWidowPrefab, bossWidowPool);
    public void ResetEnemyComponents(GameObject enemy)
    {
        EnemyAI enemyAI = enemy.GetComponent<EnemyAI>();
        enemyAI.GetComponent<CapsuleCollider>().enabled = true;

        // GetComponent<BoxCollider>().enabled = true;
        enemyAI.GetComponent<Animator>().enabled = true;
        enemyAI.GetComponent<NavMeshAgent>().enabled = true;
        enemyAI.health = enemyAI.maxHealth;
    }
    private GameObject GetEnemyFromPool(GameObject prefab, Queue<GameObject> pool)
    {
        GameObject enemy;
        if (pool.Count > 0)
        {
            enemy = pool.Dequeue();
            ResetEnemyComponents(enemy);
            // enemy.SetActive(true);
        }
        else
        {
            enemy = Instantiate(prefab, this.transform);
        }
        // Optionally reset enemy state here
        var enemyAI = enemy.GetComponent<EnemyAI>();
        if (enemyAI != null)
        {
            enemyAI.ResetEnemy(enemy.transform.position, enemy.transform.rotation);
        }

        return enemy;
    }

    public void ReturnArcherEnemy(GameObject enemy) => ReturnEnemyToPool(enemy, archerEnemyPool);
    public void ReturnThugEnemy(GameObject enemy) => ReturnEnemyToPool(enemy, thugEnemyPool);
    public void ReturnBossKnightEnemy(GameObject enemy) => ReturnEnemyToPool(enemy, bossKnightPool);
    public void ReturnBlackWidowEnemy(GameObject enemy) => ReturnEnemyToPool(enemy, blackWidowPool);
    public void ReturnRedWidowEnemy(GameObject enemy) => ReturnEnemyToPool(enemy, redWidowPool);
    public void ReturnGrayWidowEnemy(GameObject enemy) => ReturnEnemyToPool(enemy, grayWidowPool);
    public void ReturnBossWidowEnemy(GameObject enemy) => ReturnEnemyToPool(enemy, bossWidowPool);

    private void ReturnEnemyToPool(GameObject enemy, Queue<GameObject> pool)
    {
        enemy.SetActive(false);
        enemy.transform.SetParent(this.transform);
        pool.Enqueue(enemy);
    }
    
}
