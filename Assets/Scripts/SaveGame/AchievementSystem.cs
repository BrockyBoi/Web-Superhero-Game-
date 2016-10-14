using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class AchievementSystem : MonoBehaviour {
	public static AchievementSystem controller = null;

	Dictionary<int, int> totalKills = new Dictionary<int, int>();
	Dictionary<int, int> enemyTypeKills = new Dictionary<int, int>();

	Dictionary<int,int> playerDeaths = new Dictionary<int, int>();
	int totalPlayerDeaths;

	Dictionary<int, bool> unlockedHeros = new Dictionary<int, bool>();

	//ADD THIS LATER
	/*
	 * TOTAL ENEMIES HIT IN ONE ATTACK
	 * ENEMIES KILLED AT FULL HEALTH
	 * TOTAL XP
	 * TOTAL DAMAGE DONE
	 * TOTAL KILLS (ACROSS ALL HEROES)
	 * */
	int hitsInOneAttack;
	int totalXP;
	int fullHealthKills;
	int totalDamageDone;
	int totalKillsAllHeroes;

	public static int[] totalXPAmounts = new int[10]{ 1000, 5000, 10000, 50000, 100000, 500000, 1000000, 5000000, 10000000, 50000000 };
	public static int[] killAmounts = new int[10]{ 25, 50, 100, 250, 1000, 5000, 25000, 100000, 500000, 1000000 };
	public static int[] playerDeathAmounts = new int[10]{ 1, 10, 25, 50, 100, 250, 500, 1000, 2500, 5000 };
	public static int[] hitsAmounts = new int[10]{100, 500, 1000, 5000, 10000, 50000, 100000, 500000, 1000000,5000000 };
	public static int[] fullHealthKillAmounts = new int[10]{25,75,150,500,1000,2500,5000,7500,10000,15000};
	public static int[] totalDamageAmounts = new int[10]{100,500,2500, 7500, 15000, 75000, 150000, 750000, 1500000, 10000000};
	public static int[] totalKillsAllHeroesAmounts = new int[10]{100, 500, 1000, 5000, 10000, 50000, 100000, 500000, 1000000, 5000000};

	public bool UnlockAllHeroes;



	void Awake()
	{
		InitializeDictionaries();
		DontDestroyOnLoad (this);

		if (controller == null)
			controller = this;
		else if (controller != this) {
			Destroy (gameObject);
		}
	}

	// Use this for initialization
	void Start () {
		if (UnlockAllHeroes)
			for (int i = 0; i < unlockedHeros.Count; i++) {
				unlockedHeros [i] = true;
			}
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void InitializeDictionaries()
	{
		totalKills.Add ((int)SuperPowerController.SuperHero.Tank, 0);
		totalKills.Add ((int)SuperPowerController.SuperHero.Elementalist, 0);
		totalKills.Add ((int)SuperPowerController.SuperHero.Vigilantee, 0);
		totalKills.Add ((int)SuperPowerController.SuperHero.Paragon, 0);
		totalKills.Add ((int)SuperPowerController.SuperHero.Speedster, 0);

		enemyTypeKills.Add (0, 0);
		enemyTypeKills.Add (1, 0);
		enemyTypeKills.Add (2, 0);
		enemyTypeKills.Add (3, 0);
		enemyTypeKills.Add (4, 0);

		playerDeaths.Add ((int)SuperPowerController.SuperHero.Tank, 0);
		playerDeaths.Add ((int)SuperPowerController.SuperHero.Elementalist, 0);
		playerDeaths.Add ((int)SuperPowerController.SuperHero.Vigilantee, 0);
		playerDeaths.Add ((int)SuperPowerController.SuperHero.Paragon, 0);
		playerDeaths.Add ((int)SuperPowerController.SuperHero.Speedster, 0);

		unlockedHeros.Add ((int)SuperPowerController.SuperHero.Tank, true);
		unlockedHeros.Add ((int)SuperPowerController.SuperHero.Elementalist, false);
		unlockedHeros.Add ((int)SuperPowerController.SuperHero.Vigilantee, false);
		unlockedHeros.Add ((int)SuperPowerController.SuperHero.Paragon, false);
		unlockedHeros.Add ((int)SuperPowerController.SuperHero.Speedster, false);
	}

	public void KilledEnemy(int dmg, int enemyType)
	{
		AddKill (enemyType);
		AddToTotalDamageDone (dmg);
		AddToTotalKillsAllHeroes ();

		if (Player.playerSingleton.AtFullHealth ())
			AddKillAtFullHealth ();
	}

	public int GetKillCount(int hero)
	{
		return totalKills [hero];
	}

	public int GetDeathCount(int hero)
	{
		return playerDeaths [hero];
	}

	void SetKillCount(int hero, int killCount)
	{
		totalKills [hero] = killCount;
	}

	public void AddKill(int enemyType)
	{
		int hero = SuperPowerController.controller.GetSuperHero ();
		totalKills [hero]++;	
		enemyTypeKills [enemyType]++;

		CheckKills (hero);
	}

	public Dictionary<int,int> GetKillList()
	{
		return totalKills;
	}

	public Dictionary<int,int> GetEnemyKillList()
	{
		return enemyTypeKills;
	}

	public Dictionary<int,int> GetPlayerDeathList()
	{
		return playerDeaths;
	}

	public int GetTotalKillsAllHeroes()
	{
		return totalKillsAllHeroes;
	}

	public int GetTotalXP()
	{
		return totalXP;
	}

	public int GetHitsInOneAttack()
	{
		return hitsInOneAttack;
	}

	public int GetFullHealthKills()
	{
		return fullHealthKills;
	}

	public int GetTotalDamageDone()
	{
		return totalDamageDone;
	}

	public int GetTotalPlayerDeaths()
	{
		return totalPlayerDeaths;
	}

	public Dictionary<int, bool> GetUnlockedHerosList()
	{
		return unlockedHeros;
	}

	public void SetKillList(Dictionary<int,int> list)
	{
		totalKills = list;
	}

	public void SetEnemyKillList(Dictionary<int,int> list)
	{
		enemyTypeKills = list;
	}

	public void SetPlayerDeathList(Dictionary<int,int> list)
	{
		playerDeaths = list;
	}

	public void SetTotalPlayerDeaths(int num)
	{
		totalPlayerDeaths = num;
	}

	public void SetTotalXP(int num)
	{
		totalXP = num;
	}

	public void SetUnlockedHerosList(Dictionary<int,bool> list)
	{
		unlockedHeros = list;
	}

	public void AddToTotalDamageDone(int num)
	{
		totalDamageDone += num;
	}

	public void SetTotalDamageDone(int num)
	{
		totalDamageDone = num;
	}

	public void AddKillAtFullHealth()
	{
		fullHealthKills++;
	}

	public void SetKillsAtFullHealth(int num)
	{
		fullHealthKills = num;
	}

	public void SetHitsInOneAttack(int num)
	{
		hitsInOneAttack = num;
	}

	public void CheckMaxHits(int num)
	{
		if (num > hitsInOneAttack) {
			hitsInOneAttack = num;
		}
	}

	public void AddToTotalKillsAllHeroes()
	{
		totalKillsAllHeroes++;
	}

	public void SetTotalKillsAllHeroes(int num)
	{
		totalKillsAllHeroes = num;
	}

	public void PlayerDeath(int hero)
	{
		playerDeaths [hero]++;
		totalPlayerDeaths++;
	}

	void CheckKills(int hero)
	{
		switch (totalKills [hero]) 
		{
		case(1):
			break;
		case(50):
			if (hero < unlockedHeros.Count - 1) {
				Debug.Log ("UNLOCKED NEW HERO");
				UnlockHero (hero + 1);
			}
			break;
		case(100):
			break;
		case(250):
			break;
		case(1000):
			break;
		case(5000):
			break;
		case(25000):
			break;
		case(100000):
			break;
		case(500000):
			break;
		case(10000000):
			break;
		default:
			return;
		}
		Debug.Log ("Kill achievement: " + totalKills [hero]);
		UnlockAchievement(hero, GetKillCount(hero));
	}

	void UnlockAchievement(int hero, int killCount)
	{
		Debug.Log ("Unlocked Achievement");
	}

	public void UnlockHero(SuperPowerController.SuperHero hero)
	{
		unlockedHeros [(int)hero] = true;
	}

	public void UnlockHero(int num)
	{
		unlockedHeros [num] = true;
	}

	public bool CheckIfUnlocked(int num)
	{
		return unlockedHeros [num];
	}

	public int CheckAchievements(int[] array, int amount)
	{
		int counter = 0;
		while (amount > array [counter]) {
			counter++;
		}

		return counter;
	}
}
