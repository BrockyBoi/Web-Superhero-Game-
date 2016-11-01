using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class AchievementDataBase : MonoBehaviour
{
	public List<List<GameObject>> killLists = new List<List<GameObject>> ();
	//public List<List<Image>> hitLists = new List<List<Image>> ();
	public List<List<GameObject>> deathLists = new List<List<GameObject>> ();

	public GameObject tankPanel;
	public GameObject tankKillObject;
	List<GameObject> tankKills = new List<GameObject>();
	//public List<Image> tankHits;
	public GameObject tankDeathObject;
	List<GameObject> tankDeaths = new List<GameObject>();

	public GameObject elementalistPanel;
	public GameObject elementalistKillObject;
	List<GameObject> elementalistKills = new List<GameObject>();
	//List<Image> elementalistHits;
	public GameObject elementalistDeathObject;
	List<GameObject> elementalistDeaths = new List<GameObject>();

	public GameObject vigilantePanel;
	public GameObject vigilanteKillObject;
	List<GameObject> vigilanteKills = new List<GameObject>();
	//List<Image> vigilanteHits;
	public GameObject vigilanteDeathObject;
	List<GameObject> vigilanteDeaths = new List<GameObject>();

	public GameObject paragonPanel;
	public GameObject paragonKillObject;
	List<GameObject> paragonKills = new List<GameObject>();
	//List<Image> paragonHits;
	public GameObject paragonDeathObject;
	List<GameObject> paragonDeaths = new List<GameObject>();

	public GameObject speedsterPanel;
	public GameObject speedsterKillObject;
	List<GameObject> speedsterKills = new List<GameObject>();
	//List<Image> speedsterHits;
	public GameObject speedsterDeathObject;
	List<GameObject> speedsterDeaths = new List<GameObject>();

	public GameObject generalPanel;
	public GameObject totalKillObject;
	List<GameObject> totalKills = new List<GameObject>();

	public GameObject totalHitObject;
	List<GameObject> totalHits = new List<GameObject>();

	public GameObject totalDeathObject;
	List<GameObject> totalDeaths = new List<GameObject>();

	public GameObject totalXPObject;
	List<GameObject> totalXP = new List<GameObject>();

	public GameObject fullHealthObject;
	List<GameObject> fullHealthKills = new List<GameObject>();

	public GameObject totalDamageObject;
	List<GameObject> totalDamage = new List<GameObject>();

	//string[] names = new string[(int)SuperPowerController.SuperHero.HERO_COUNT]{"Tank", "Elementalist", "Paragon", "Speedster", "Vigilantee"}; 

	List<GameObject> panelList = new List<GameObject>();

	// Use this for initialization
	void Start ()
	{

		InitializePanelList ();

		InitializePanels ();

		CombineLists ();

		InitializeAchievements ();

		CheckAllAchievements ();
	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}

	void InitializePanelList()
	{
		panelList.Add (tankPanel);
		panelList.Add (elementalistPanel);
		panelList.Add (paragonPanel);
		panelList.Add (speedsterPanel);
		panelList.Add (vigilantePanel);
		panelList.Add (generalPanel);

		for (int i = 1; i < panelList.Count; i++) {
			panelList [i].gameObject.SetActive (false);
		}
	}

	void InitializePanels()
	{
		AddToPanel(tankPanel, tankKillObject, tankKills, tankDeathObject, tankDeaths);
		AddToPanel (elementalistPanel, elementalistKillObject, elementalistKills, elementalistDeathObject, elementalistDeaths);
		AddToPanel (paragonPanel, paragonKillObject, paragonKills, paragonDeathObject, paragonDeaths);
		AddToPanel (speedsterPanel, speedsterKillObject, speedsterKills, speedsterDeathObject, speedsterDeaths);
		AddToPanel (vigilantePanel, vigilanteKillObject, vigilanteKills, vigilanteDeathObject, vigilanteDeaths);

		AddToPanel (generalPanel, totalKillObject, totalKills, totalXPObject, totalXP);
		AddToPanel (generalPanel, totalDamageObject, totalDamage, totalDeathObject, totalDeaths);
		AddToPanel (generalPanel, totalHitObject, totalHits, fullHealthObject, fullHealthKills);

	}

	void CombineLists()
	{
		killLists.Add (tankKills);
		killLists.Add (elementalistKills);
		killLists.Add (paragonKills);
		killLists.Add (speedsterKills);
		killLists.Add (vigilanteKills);

		//		hitLists.Add (tankHits);
		//		hitLists.Add (elementalistHits);
		//		hitLists.Add (vigilanteHits);
		//		hitLists.Add (paragonHits);
		//		hitLists.Add (speedsterHits);

		deathLists.Add (tankDeaths);
		deathLists.Add (elementalistDeaths);
		deathLists.Add (paragonDeaths);
		deathLists.Add (speedsterDeaths);
		deathLists.Add (vigilanteDeaths);

	}

	public void ActivatePanel(int num)
	{
		for (int i = 0; i < panelList.Count; i++) {
			if (i == num)
				panelList [i].gameObject.SetActive (true);
			else
				panelList [i].gameObject.SetActive (false);
		}
	}
		
	void UnlockAchievement(List<GameObject> list, int spot)
	{
		list [spot].GetComponent<Image> ().color = Color.white;
	}

	void LockAchievement(List<GameObject> list, int spot)
	{
		list [spot].GetComponent<Image> ().color = Color.gray;
	}

	void GiveData(GameObject image, string name, string amount)
	{
		image.GetComponent<AchievementItem> ().GiveData (name, amount);
	}

	void InitializeList(GameObject g, List<GameObject> list)
	{
		for (int i = 0; i < g.transform.childCount; i++) {
			list.Add (g.transform.GetChild (i).gameObject);
		}
	}

	void AddToPanel(GameObject panel, GameObject holdingObject1, List<GameObject> list1, GameObject holdingObject2, List<GameObject> list2)
	{
		InitializeList (holdingObject1, list1);
		InitializeList (holdingObject2, list2);

		for (int i = 0; i < list1.Count; i++) {
			list1 [i].transform.SetParent (panel.transform);
		}

		for (int i = 0; i < list2.Count; i++) {
			list2 [i].transform.SetParent (panel.transform);
		}
	}

	public void InitializeAchievements()
	{
		for (int i = 0; i < (int)SuperPowerController.SuperHero.HERO_COUNT; i++) {
			for (int j = 0; j < killLists [i].Count; j++) {
				GiveData (killLists [i] [j], SuperPowerController.HeroToString (i) + " Kills", AchievementSystem.killAmounts [j].ToString());
				LockAchievement (killLists [i], j);
			}
			for (int j = 0; j < deathLists [i].Count; j++) {
				GiveData (deathLists [i] [j], SuperPowerController.HeroToString (i) + " Deaths", AchievementSystem.playerDeathAmounts [j].ToString());
				LockAchievement (deathLists [i], j);
			}
		}

		for (int i = 0; i < totalKills.Count; i++) {
			GiveData(totalKills[i],"Total Kills", AchievementSystem.totalKillsAllHeroesAmounts[i].ToString());
			LockAchievement (totalKills, i);
		}
		for (int i = 0; i < totalXP.Count; i++) {
			GiveData(totalXP[i],"Total XP", AchievementSystem.totalXPAmounts[i].ToString());
			LockAchievement (totalXP, i);
		}
		for (int i = 0; i < totalHits.Count; i++) {
			GiveData(totalHits[i],"Total Hits", AchievementSystem.hitsAmounts[i].ToString());
			LockAchievement (totalHits, i);
		}
		for (int i = 0; i < totalDeaths.Count; i++) {
			GiveData(totalDeaths[i],"Total Deaths", AchievementSystem.playerDeathAmounts[i].ToString());
			LockAchievement (totalDeaths, i);
		}
		for (int i = 0; i < totalDamage.Count; i++) {
			GiveData(totalDamage[i],"Total Damage", AchievementSystem.totalDamageAmounts[i].ToString());
			LockAchievement (totalDamage, i);
		}
		for (int i = 0; i < fullHealthKills.Count; i++) {
			GiveData(fullHealthKills[i],"Total Full Health Kills", AchievementSystem.fullHealthKillAmounts[i].ToString());
			LockAchievement (fullHealthKills, i);
		}
	}

	public void CheckAllAchievements()
	{
		for (int i = 0; i < (int)SuperPowerController.SuperHero.HERO_COUNT; i++) {
			int killAmounts = AchievementSystem.controller.CheckAchievements (AchievementSystem.killAmounts, AchievementSystem.controller.GetKillCount (i));
			int deathAmounts = AchievementSystem.controller.CheckAchievements (AchievementSystem.playerDeathAmounts, AchievementSystem.controller.GetDeathCount (i));

			for(int j = 0; j < killAmounts; j++)
			{
				UnlockAchievement (killLists[i], j);
			}
			for (int j = 0; j < deathAmounts; j++) {
				UnlockAchievement (deathLists[i], j);
			}
		}

		int kills = AchievementSystem.controller.CheckAchievements (AchievementSystem.totalKillsAllHeroesAmounts, AchievementSystem.controller.GetTotalKillsAllHeroes ());
		for (int i = 0; i < kills; i++) {
			UnlockAchievement (totalKills, i);
		}

		int xp = AchievementSystem.controller.CheckAchievements (AchievementSystem.totalXPAmounts, AchievementSystem.controller.GetTotalXP ());
		for(int i= 0; i < xp; i++)
		{
			UnlockAchievement (totalXP, i);
		}

		int deaths = AchievementSystem.controller.CheckAchievements (AchievementSystem.playerDeathAmounts, AchievementSystem.controller.GetTotalPlayerDeaths ());
		for (int i = 0; i < deaths; i++) {
			UnlockAchievement (totalDeaths, i);
		}

		int hits = AchievementSystem.controller.CheckAchievements (AchievementSystem.hitsAmounts, AchievementSystem.controller.GetHitsInOneAttack ());
		for (int i = 0; i < hits; i++) {
			UnlockAchievement (totalHits, i);
		}

		int fullHealth = AchievementSystem.controller.CheckAchievements (AchievementSystem.fullHealthKillAmounts, AchievementSystem.controller.GetFullHealthKills ());
		for (int i = 0; i < fullHealth; i++) {
			UnlockAchievement (fullHealthKills, i);
		}

		int damage = AchievementSystem.controller.CheckAchievements (AchievementSystem.totalDamageAmounts, AchievementSystem.controller.GetTotalDamageDone ());
		for (int i = 0; i < damage; i++) {
			UnlockAchievement (totalDamage, i);
		}
	}
}

