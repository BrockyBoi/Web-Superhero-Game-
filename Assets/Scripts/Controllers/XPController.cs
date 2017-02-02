using UnityEngine;
using System.Collections;

public class XPController : MonoBehaviour {
	public static XPController controller;

	bool buttonSpending;

    IEnumerator[] spenders = new IEnumerator[(int)UpgradeController.Upgrades.UPGRADE_COUNT];

	public bool startWithMaxXP;


	int currentXp = 0;
	[SerializeField]
	[Range(1,4)]
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

		if(startWithMaxXP)
        	currentXp = 5000;
	}

	public void AddXP(int enemyType)
	{
		currentXp += CheckEnemyType (enemyType);

		CheckXP ();
	}

	void CheckXP()
	{
		if (level < 4 && currentXp >= xpCaps [level]) {
			//currentXp = 0;
			GameCanvas.controller.LevelUp (xpCaps [level - 1]);
            level++;
		}	
	}

    IEnumerator SpendXP(int power)
    {
        while (currentXp > 0)
        {
			int amount = 2;
			currentXp -= amount;

            if (currentXp < xpCaps[level - 1] && level > 1)
                level--;

            UpgradeController.controller.SpendXP(power, amount);
            yield return null;
        }
    }


	public void PressSpendXP(int power)
	{
       spenders[power] = SpendXP(power);
       StartCoroutine(spenders[power]);
    }

    public void StopSpending(int power)
    {
        StopCoroutine(spenders[power]);
    }

    public void SetLevel(int num)
	{
        if (GameCanvas.controller.GetTutorialMode())
        {
            level = num;
            currentXp = 0;
            GameCanvas.controller.LevelUp(xpCaps[num]);
        }
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
	
