using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class EnemySpawner : NetworkBehaviour
{
    //wave enum
    public enum SpawnState {SPAWNING, WAITING, COUNTING};
    
    //wave stats
    [SerializeField] private Wave[] wave;
    [SerializeField] private float timeBetweenWaves = 3f;
    [SerializeField] private float waveCountdown = 0;

    //bool
    [SerializeField] private bool isInfinite;

    SpawnState state = SpawnState.COUNTING;

    int currentWave;

    [SerializeField] private Text currentWaveText;
    
    //references
    [SerializeField] private Transform[] spawner;
    [SerializeField] private List<CharacterStats> enemyList;
    [SerializeField] private GameObject charachter;

    void Update()
    {
        if (state == SpawnState.WAITING)
        {
            if (!EnemiesDead())
                return;
            else
            {
                FinishWave();
                int currentWaveNumber = currentWave += 1;
                currentWaveText.text = currentWaveNumber.ToString();
            }
        }
        
        if (waveCountdown <= 0)
        {
            if (state != SpawnState.SPAWNING)
            {
                StartCoroutine(SpawnWave(wave[currentWave]));
            }
        }
        else
        {
            waveCountdown -= Time.deltaTime;
        }
    }

    void Start()
    {
        waveCountdown = timeBetweenWaves;
        currentWave = 0;
    }

    IEnumerator SpawnWave(Wave wave)
    {
        state = SpawnState.SPAWNING;
        if (!isInfinite)
        {
            for (int i = 0; i < wave.numberOfEnemies; i++)
            {
                SpawnEnemy(wave.enemy);
                yield return new WaitForSeconds(wave.delay);
            }

            state = SpawnState.WAITING;

            yield break;
        }
        
    }

    void SpawnEnemy(GameObject enemy)
    {
        int randomInt = Random.RandomRange(1, spawner.Length);
        Transform randomSpawner = spawner[randomInt];

        GameObject spawnedEnemy = Instantiate(enemy, randomSpawner.position, randomSpawner.rotation);
        CharacterStats spawnedEnemyStats = spawnedEnemy.GetComponent<CharacterStats>();

        enemyList.Add(spawnedEnemyStats);

        NetworkServer.Spawn(spawnedEnemy.gameObject);
    }

    bool EnemiesDead()
    {
        int i = 0;
        foreach(CharacterStats enemy in enemyList)
        {
            if (enemy.IsDead())
                i++;
            else
                return false;
        }
        return true;
    }

    void FinishWave()
    {
        if (!isInfinite)
        {
            Debug.Log("Wave Finished");

            state = SpawnState.COUNTING;
            waveCountdown = timeBetweenWaves;

            if (currentWave + 1 > wave.Length - 1)
            {
                currentWave = 0;
                Debug.Log("All waves finished");
            }

            else
            {
                currentWave++;
            }
        }
    }
}