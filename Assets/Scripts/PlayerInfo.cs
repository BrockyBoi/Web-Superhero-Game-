using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine.SceneManagement;

public class PlayerInfo : MonoBehaviour {
	public static PlayerInfo controller = null; 

	bool useMusic;
	bool useFX;

	int[] killCounts;

	bool newPlayer;

	void Awake()
	{
		if (controller == null)
			controller = this;
		else if(controller != this)
			Destroy (gameObject);
		
		DontDestroyOnLoad (this);
	}

	// Use this for initialization
	void Start () {
		useMusic = true;
		useFX = true;
		newPlayer = true;
	}
	
	// Update is called once per frame
	void Update () {

	}

	public bool ClickFX()
	{
		useFX = !useFX;
		return useFX;
	}

	public bool GetFX()
	{
		return useFX;
	}

	public bool ClickMusic()
	{
		useMusic = !useMusic;
		return useMusic;
	}

	public bool GetMusic()
	{
		return useMusic;
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
		data.audio = useMusic;
		data.fx = useFX;

		data.killList = AchievementSystem.controller.GetKillList ();
		data.enemyKillList = AchievementSystem.controller.GetEnemyKillList ();

		data.playerDeathList = AchievementSystem.controller.GetPlayerDeathList ();
		data.totalPlayerDeaths = AchievementSystem.controller.GetTotalPlayerDeaths ();

		data.unlockedHeros = AchievementSystem.controller.GetUnlockedHerosList ();

		data.newPlayer = newPlayer;
		//For example: data.health = health;

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
			useMusic = data.audio;
			useFX = data.fx;

			AchievementSystem.controller.SetKillList (data.killList);
			AchievementSystem.controller.SetEnemyKillList (data.enemyKillList);

			AchievementSystem.controller.SetPlayerDeathList (data.playerDeathList);
			AchievementSystem.controller.SetTotalPlayerDeaths (data.totalPlayerDeaths);

			AchievementSystem.controller.SetUnlockedHerosList (data.unlockedHeros);

			MainMenu.controller.CheckAudioSettings ();

			MainMenu.controller.CheckUnlockedHeroes ();

			newPlayer = data.newPlayer;

			//For example: health = data.health;
		}
	}

	[Serializable]
	class PlayerData
	{
		public bool audio;
		public bool fx;

		public Dictionary<int, int> killList = new Dictionary<int, int>();
		public Dictionary<int,int> enemyKillList = new Dictionary<int, int>();

		public Dictionary<int,int> playerDeathList = new Dictionary<int, int>();
		public int totalPlayerDeaths;

		public Dictionary<int, bool> unlockedHeros = new Dictionary<int, bool>();

		public bool newPlayer;
	}
}
 