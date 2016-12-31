using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemySpawner : MonoBehaviour {
    public static EnemySpawner controller;
    public Transform spawn1;
    public Transform spawn2;
    bool spawn1WasLastSpawn;

    public List<GameObject> enemyPrefabs;
    int[] currentEnemies = new int[4];
    public float[] spawnRates;
    public int[] maxEnemies;
	public int[] enemyRounds;

    int round;

	public bool finalRound;

    void Awake()
    {
        controller = this;
    }

    // Use this for initialization
    void Start()
    {
        StartCoroutine(SpawnCoroutine(0));

		if (finalRound)
			GoToLastRound ();

    }
	
	// Update is called once per frame
	void Update () {
	
	}

	void GoToLastRound()
	{
		round = enemyRounds[0];
		CheckRound ();
		round = enemyRounds[1];
		CheckRound ();
		round = enemyRounds[2];
		CheckRound ();

		for (int j = 0; j < 1000; j++) {
			for (int i = 0; i < enemyPrefabs.Count; i++) {
				AppendSpawnRate (i);
			}
		}
	}

    void SpawnEnemy(GameObject enemy)
    {
        if (spawn1WasLastSpawn)
        {
            Instantiate(enemy, spawn2.position, Quaternion.identity);
        }
        else
        {
            Instantiate(enemy, spawn1.position, Quaternion.identity);
        }

        spawn1WasLastSpawn = !spawn1WasLastSpawn;
        round++;

        CheckRound();
    }

    void CheckRound()
    {
		if (round == enemyRounds [0]) {
			StartCoroutine (SpawnCoroutine (1));
		} else if (round == enemyRounds [1]) {
			StartCoroutine (SpawnCoroutine (2));
		} else if (round == enemyRounds [2]) {
			StartCoroutine (SpawnCoroutine (3));
		}
    }

    void AppendSpawnRate(int enemyNum)
    {
       
        //Original rates are 3,6,10,30
        if ((enemyNum == 0 && spawnRates[0] < .35f) || 
            (enemyNum == 1 && spawnRates[1] < .8f) || 
            (enemyNum == 2 && spawnRates[2] < 1.6f) ||
            (enemyNum == 3 && spawnRates[3] < 5))
            return;

        spawnRates[enemyNum] *= .999f;
    }

    IEnumerator SpawnCoroutine(int enemyNum)
    {

        float time = 0;
        while (Player.playerSingleton.CheckAlive())
        {
            time += Time.deltaTime;

            if(time > spawnRates[enemyNum] && currentEnemies[enemyNum] < maxEnemies[enemyNum])
            {
                SpawnEnemy(enemyPrefabs[enemyNum]);

                AppendSpawnRate(enemyNum);
                time = 0;
            }
            yield return null;
        }
    }

    public void AddEnemy(int enemyNum)
    {
        currentEnemies[enemyNum]++;
    }

    public void SubtractEnemy(int enemyNum)
    {
        currentEnemies[enemyNum]--;
    }
}
