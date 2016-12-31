using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine.SceneManagement;

public class PlayerInfo : MonoBehaviour {
	public static PlayerInfo controller; 

	float musicLevel;
	float fxLevel;

	int[] killCounts;

	public bool newPlayer = false;
	public bool DontLoadOnStart;

	int hitsInOneAttack;
	int fullHealthKills;
	int totalDamageDone;
	int totalKillsAllHeroes;

	void Awake()
	{
		if (controller == null)
			controller = this;
		else if(controller != this)
			Destroy (this);

		musicLevel = fxLevel = .5f;
		//newPlayer = true;
	}
		
	void Start () {

		if (!DontLoadOnStart)
			Load ();
	}


	public void SetFXLevel(float f)
	{
		fxLevel = f;
	}

	public float GetFXLevel()
	{
		return fxLevel;
	}

	public void SetMusicLevel(float f)
	{
		musicLevel = f;
	}

	public float GetMusic()
	{
		return musicLevel;
	}

	public bool CheckIfNewPlayer()
	{
		return newPlayer;
	}

	public void NoLongerNewPlayer()
	{
		newPlayer = false;
	}

	public void Save()
	{
		BinaryFormatter bf = new BinaryFormatter ();
		FileStream file = File.Create (Application.persistentDataPath + "/playerInfo.dat");

		PlayerData data = new PlayerData ();
		//place data here
		data.fxLevel = fxLevel;
		data.musicLevel = musicLevel;

		data.killList = AchievementSystem.controller.GetKillList ();
		data.enemyKillList = AchievementSystem.controller.GetEnemyKillList ();

		data.playerDeathList = AchievementSystem.controller.GetPlayerDeathList ();
		data.totalPlayerDeaths = AchievementSystem.controller.GetTotalPlayerDeaths ();

		data.unlockedHeros = AchievementSystem.controller.GetUnlockedHerosList ();

		data.newPlayer = newPlayer;

		data.totalXP = AchievementSystem.controller.GetTotalXP ();
		data.hitsInOneAttack = AchievementSystem.controller.GetHitsInOneAttack ();
		data.fullHealthKills = AchievementSystem.controller.GetFullHealthKills ();
		data.totalDamageDone = AchievementSystem.controller.GetTotalDamageDone ();
		data.totalKillsAllHeroes = AchievementSystem.controller.GetTotalKillsAllHeroes();

		bf.Serialize (file, data);
		file.Close ();

	}

	public void Load()
	{
		if (File.Exists (Application.persistentDataPath + "/playerInfo.dat")) {
			BinaryFormatter bf = new BinaryFormatter ();
			FileStream file = File.Open (Application.persistentDataPath + "/playerInfo.dat", FileMode.Open);
			PlayerData data = (PlayerData)bf.Deserialize (file);
			file.Close ();

			//give data back here
			fxLevel = data.fxLevel;
			musicLevel = data.musicLevel;

			SoundController.controller.SetFX (fxLevel);
			SoundController.controller.SetMusic (musicLevel);

			AchievementSystem.controller.SetKillList (data.killList);
			AchievementSystem.controller.SetEnemyKillList (data.enemyKillList);

			AchievementSystem.controller.SetPlayerDeathList (data.playerDeathList);
			AchievementSystem.controller.SetTotalPlayerDeaths (data.totalPlayerDeaths);

			AchievementSystem.controller.SetUnlockedHerosList (data.unlockedHeros);
			if (SceneManager.GetActiveScene () == SceneManager.GetSceneByName ("Main Menu")) {
				MainMenu.controller.CheckUnlockedHeroes ();
			}

			AchievementSystem.controller.SetTotalXP (data.totalXP);
			AchievementSystem.controller.SetHitsInOneAttack (data.hitsInOneAttack);
			AchievementSystem.controller.SetKillsAtFullHealth (data.fullHealthKills);
			AchievementSystem.controller.SetTotalDamageDone (data.totalDamageDone);
			AchievementSystem.controller.SetTotalKillsAllHeroes (data.totalKillsAllHeroes);

			newPlayer = data.newPlayer;

			//For example: health = data.health;
		}
	}

	[Serializable]
	class PlayerData
	{
		public float musicLevel, fxLevel;

		public Dictionary<int, int> killList = new Dictionary<int, int>();
		public Dictionary<int,int> enemyKillList = new Dictionary<int, int>();

		public Dictionary<int,int> playerDeathList = new Dictionary<int, int>();
		public int totalPlayerDeaths;

		public Dictionary<int, bool> unlockedHeros = new Dictionary<int, bool>();

		public bool newPlayer;

		public int totalXP;
		public int hitsInOneAttack;
		public int fullHealthKills;
		public int totalDamageDone;
		public int totalKillsAllHeroes;
	}


}
 