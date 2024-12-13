using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemiesManager : MonoBehaviour, IRestartable
{
    List<Transform> spawnPoints = new List<Transform>();

    [SerializeField] GameObject[] enemyPrefab;

    [SerializeField] List<Enemy> enemies = new List<Enemy>();

    [SerializeField] float timeToFirstSpawn = 60;

    [SerializeField] float timeToSpeedUpEnemies = 300;
    [SerializeField] float speedUpValue = .25f;
    [SerializeField] int maxSpeedUpTimes = 10;
    int speedUpTimes;

    [SerializeField] float timeToSpawnNewEnemy = 300;
    [SerializeField] int maxEnemies = 5;

    List<Coroutine> enemyRoutines = new List<Coroutine>();

    private void Awake()
    {
        List<Collider> spawnList = new List<Collider>();
        spawnList.AddRange(GetComponentsInChildren<Collider>());
        for (int i = 0; i < spawnList.Count; i++)
        {
            spawnPoints.Add(spawnList[i].transform);
        }
    }

    private void Start()
    {
        Initialize();
    }

    void Initialize()
    {
        enemyRoutines.Add(StartCoroutine(FirstSpawn()));
        enemyRoutines.Add(StartCoroutine(EnemiesSpeedUp()));
        enemyRoutines.Add(StartCoroutine(EnemiesSpawner()));
    }

    void SpawnEnemy(bool first = false)
    {
        Transform spawnPoint = first ? spawnPoints[0] : spawnPoints[Random.Range(0, spawnPoints.Count)];

        Enemy enemy = Instantiate(enemyPrefab[Random.Range(0, enemyPrefab.Length)], spawnPoint).GetComponent<Enemy>();
        enemy.SpeedUp(speedUpValue * speedUpTimes);

        enemies.Add(enemy);
    }

    IEnumerator FirstSpawn()
    {
        yield return new WaitForSeconds(timeToFirstSpawn);
        SpawnEnemy(true);
    }

    void ReplaceEnemy()
    {
        Enemy lastEnemy = enemies[maxEnemies - 1];
        enemies.RemoveAt(maxEnemies - 1);
        List<Enemy> updatedList = new List<Enemy>();
        updatedList.Add(lastEnemy);
        updatedList.AddRange(enemies);
        enemies.Clear();
        enemies.AddRange(updatedList);

        lastEnemy.Agent.enabled = false;

        lastEnemy.transform.SetParent(spawnPoints[Random.Range(0, spawnPoints.Count)]);
        lastEnemy.transform.localPosition = Vector3.zero;

        lastEnemy.Agent.enabled = true;
    }

    IEnumerator EnemiesSpeedUp()
    {
        while (speedUpTimes < maxSpeedUpTimes)
        {
            yield return new WaitForSeconds(timeToSpeedUpEnemies);

            foreach (Enemy e in enemies)
            {
                e.SpeedUp(speedUpValue);
            }
            speedUpTimes++;
        }
    }

    IEnumerator EnemiesSpawner()
    {
        yield return new WaitForSeconds(timeToFirstSpawn);
        while (true)
        {
            yield return new WaitForSeconds(timeToSpawnNewEnemy);
            if (enemies.Count < maxEnemies) SpawnEnemy();
            else
            {
                ReplaceEnemy();
            }
        }
    }

    public void Restart()
    {
        foreach (Coroutine routine in enemyRoutines)
        {
            StopCoroutine(routine);
        }
        speedUpTimes = 0;

        foreach (Enemy enemy in enemies)
        {
            Destroy(enemy.gameObject);
        }
        enemies.Clear();

        Initialize();
    }
}
