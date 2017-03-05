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
		
	void Start () {
		GameCanvas.controller.NewLevel (1);

		if (startWithMaxXP) {
			currentXp = 4505;
			GameCanvas.controller.NewLevel (4);
			level = 4;
		}
	}

	void Update()
	{
		//currentXp += 1;
		CheckXP ();
	}

	public void AddXP(int enemyType)
	{
		currentXp += CheckEnemyType (enemyType);

		CheckXP ();
	}

	void CheckXP()
	{
		if (level < 4 && currentXp >= xpCaps [level - 1]) {
			level++;
			GameCanvas.controller.NewLevel (level);
		}	
	}

    IEnumerator SpendXP(int power)
    {
		int amount = 5;
		while (currentXp - amount >= 0 && UpgradeController.controller.GetUpgradeLevel(power) < 5)
        {
			currentXp = Mathf.Max(0, currentXp - amount);

			if (level > 1 && currentXp < xpCaps [level - 2]) {
				level--;
				GameCanvas.controller.NewLevel (level);
			}

			GameCanvas.controller.SetXP(currentXp);
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
            GameCanvas.controller.NewLevel(num);
        }
	}

	public int GetLevel()
	{
		return level;
	}

	public int GetCap(int level)
	{
		return xpCaps [level - 1];
	}

	int CheckEnemyType(int enemyType)
	{
		switch (enemyType)
		{
		case (0):
			return 5;
		case (1):
			return 10;
		case (2):
			return 25;
		case (3):
			return 100;
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
	
