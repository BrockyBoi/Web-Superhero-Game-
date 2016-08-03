using UnityEngine;
using System.Collections;

public class XPController : MonoBehaviour {
	public static XPController controller;


	int currentXp = 0;
	public int[] xpCaps;


	void Awake()
	{
		controller = this;
	}

	// Use this for initialization
	void Start () {
		GameCanvas.controller.LevelUp (xpCaps[1]);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void AddXP(int enemyType)
	{
		currentXp += CheckEnemyType (enemyType);

		CheckXP ();
	}

	void CheckXP()
	{
		int level = Player.playerSingleton.GetLevel ();

		if (currentXp >= xpCaps [level]) {
			currentXp = 0;
			Player.playerSingleton.LevelUp ();
			GameCanvas.controller.LevelUp (xpCaps [level + 1]);
		}	
	}

	public void SetLevel(int num)
	{
		Player.playerSingleton.SetLevel (num);
		currentXp = 0; 
		GameCanvas.controller.LevelUp(xpCaps[num]);
	}

	int CheckEnemyType(int enemyType)
	{
		switch (enemyType)
		{
		case (0):
			return 1;
		case (1):
			return 5;
		case (2):
			return 10;
		case (3):
			return 50;
		default:
			Debug.Log("You probably did something wrong");
			return 0;
		}
	}

	public int GetCurrentXP()
	{
		return currentXp;
	}
}
