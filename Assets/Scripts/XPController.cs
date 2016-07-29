using UnityEngine;
using System.Collections;

public class XPController : MonoBehaviour {
	public static XPController controller;


	public int currentXp = 0;


	void Awake()
	{
		controller = this;
	}

	// Use this for initialization
	void Start () {
	
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
		bool canLevelUp = false;

		switch (level) {
		case(1):
			if (currentXp > 25)
				canLevelUp = true;
			break;
		case(2):
			if (currentXp > 50)
				canLevelUp = true;
			break;
		case(3):
			if (currentXp > 100)
				canLevelUp = true;
			break;
		case(4):
			if (currentXp > 200)
				canLevelUp = true;
			break;
		case(5):
			if (currentXp > 400)
				canLevelUp = true;
			break;
		case(6):
			if (currentXp > 800)
				canLevelUp = true;
			break;
		case(7):
			if (currentXp > 1600)
				canLevelUp = true;
			break;
		case(8):
			if (currentXp > 3200)
				canLevelUp = true;
			break;
		case(9):
			if (currentXp > 6400)
				canLevelUp = true;
			break;
		default:
			break;
		}

		if (canLevelUp)
			Player.playerSingleton.LevelUp ();
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
}
