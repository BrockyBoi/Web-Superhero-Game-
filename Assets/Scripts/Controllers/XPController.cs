using UnityEngine;
using System.Collections;

public class XPController : MonoBehaviour {
	public static XPController controller;


	int currentXp = 0;
	[SerializeField]
	[Range(1,10)]
	int level;
	public int[] xpCaps;

	void Awake()
	{
		controller = this;
		level = 1;
	}

	// Use this for initialization
	void Start () {
		GameCanvas.controller.LevelUp (xpCaps[1]);
	}

	public void AddXP(int enemyType)
	{
		currentXp += CheckEnemyType (enemyType);

		CheckXP ();
	}

	void CheckXP()
	{
		if (currentXp >= xpCaps [level]) {
			//currentXp = 0;
			GameCanvas.controller.LevelUp (xpCaps [level + 1]);
		}	
	}

	void SpendXP(int power)
	{
		if (currentXp <= 0)
			return;
		
		currentXp -= 2;

		if (currentXp < xpCaps [level])
			level--;

		UpgradeController.controller.SpendXP (power);
	}

	public void SetLevel(int num)
	{
		level = num;
		currentXp = 0; 
		GameCanvas.controller.LevelUp(xpCaps[num]);
	}

	public int GetLevel()
	{
		return level;
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
	
