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
		round = 20;
		CheckRound ();
		round = 40;
		CheckRound ();
		round = 60;
		CheckRound ();
	}

    void SpawnEnemy(GameObject enemy)
    {
        if (spawn1WasLastSpawn)
        {
            GameObject spanwedEnemy = Instantiate(enemy, spawn2.position, Quaternion.identity) as GameObject;
        }
        else
        {
            GameObject spanwedEnemy = Instantiate(enemy, spawn1.position, Quaternion.identity) as GameObject;
        }

        spawn1WasLastSpawn = !spawn1WasLastSpawn;
        round++;

        CheckRound();
    }

    void CheckRound()
    {
        switch (round)
        {
            case (20):
                StartCoroutine(SpawnCoroutine(1));
                break;
            case (40):
                StartCoroutine(SpawnCoroutine(2));
                break;
            case (60):
                StartCoroutine(SpawnCoroutine(3));
                break;
            default:
                break;
        }
    }

    void AppendSpawnRate(int enemyNum)
    {
       
        //Original rates are 3,6,10,30
        if ((enemyNum == 0 && spawnRates[0] < 1.5f) || 
            (enemyNum == 1 && spawnRates[1] < 3) || 
            (enemyNum == 2 && spawnRates[2] < 5) ||
            (enemyNum == 3 && spawnRates[3] < 15))
            return;

        spawnRates[enemyNum] *= .975f;
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
