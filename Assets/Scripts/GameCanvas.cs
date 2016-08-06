using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameCanvas : MonoBehaviour {

	public static GameCanvas controller;

	public List<Slider> sliders;

	public Image ESCImage;
	public Image Options;

	public enum SliderNumbers{ShortAttack,MediumAttack,LargeAttack,AOEAttack,XP}
	float xpValue;
	public Text xpText;


	//Tutorial stuff
	bool tutorialMode;
	public Image tutorialImage;
	public Text tutorialText;
	List<string> tutorialStrings = new List<string>();
	int currentString;
	bool waiting;
	public enum TutorialPosition{PressQ = 4,PressW,PressE,PressR,Lvl1AOE = 9,Lvl10AOE = 10,FinalSlide = 11}
	public GameObject enemyPrefab;

	void Awake()
	{
		controller = this;

		if (SceneManager.GetActiveScene().name == "Tutorial Scene")
			tutorialMode = true;
	}

	// Use this for initialization
	void Start () {
		if (tutorialMode) {
			InitializeStrings ();
			Activate (tutorialImage);
			SetTutorialText (tutorialStrings [0]);
			Player.playerSingleton.SetLevel (1);
		} else
			//Disable (tutorialImage);

		StartCoroutine (UpdateXPSlider ());

		Disable (ESCImage);
		Disable (Options);
	}
	
	// Update is called once per frame
	void Update () {
		CheckTutorial ();
		CheckESC ();
			
	}

	public void CheckESC()
	{
		if (Input.GetKeyDown (KeyCode.Escape)) {
			PressEsc ();
		}
	}

	public void PressSave()
	{
		PlayerInfo.controller.Save ();
	}

	public void PressLoad()
	{
		PlayerInfo.controller.Load ();
	}

	public void InitializeSlider(int whichSlider, float value)
	{
		sliders [whichSlider].maxValue = value;
		sliders [whichSlider].value = value;
	}

	public void UpdateAttackSlider(int num)
	{
		StartCoroutine (TurnOnSlider (num));
	}

	IEnumerator TurnOnSlider(int num)
	{
		float time = 0;

		while (time < sliders [num].maxValue)
		{
			time += Time.deltaTime;
			sliders [num].value = time;
			yield return null;
		}
	}

	void FinishSlider(int num)
	{
		StopAllCoroutines ();

		for(int i =0; i < 4; i++)
			sliders [i].value = sliders [i].maxValue;

		StartCoroutine (UpdateXPSlider ());
	}

	public int SliderToInt(SliderNumbers num)
	{
		return (int)num;
	}

	public void LevelUp(int max)
	{
		sliders [(int)SliderNumbers.XP].maxValue = max;
		sliders [(int)SliderNumbers.XP].value = 0;
		UpdateXPText ();
		xpValue = 0;
	}

	IEnumerator UpdateXPSlider()
	{
		while (true)
		{
			while (xpValue < XPController.controller.GetCurrentXP ()) {
				xpValue += .3f;
				sliders [(int)SliderNumbers.XP].value = xpValue;
				UpdateXPText ();
				yield return null;
			}

			yield return null;
		}
	}

	void UpdateXPText()
	{
		xpText.text = ((int)xpValue).ToString() + " / " + sliders [(int)SliderNumbers.XP].maxValue;
	}

	void Activate(Image i)
	{
		i.gameObject.SetActive (true);
	}
	void Activate(Text i)
	{
		i.gameObject.SetActive (true);
	}
	void Activate(Canvas i)
	{
		i.gameObject.SetActive (true);
	}

	void Disable(Image i)
	{
		i.gameObject.SetActive (false);
	}
	void Disable(Canvas i)
	{
		i.gameObject.SetActive (false);
	}	
	void Disable(Text i)
	{
		i.gameObject.SetActive (false);
	}

	bool PressedNextPage()
	{
		if (Input.GetMouseButtonDown (0) || Input.GetAxisRaw ("Submit") == 1)
			return true;
		return false;
	}

	void InitializeStrings()
	{
		tutorialStrings.Add ("In this game you have two main goals: Level up and stay alive (Click to proceed)...");
		tutorialStrings.Add ("The top left shows your health...");
		tutorialStrings.Add ("While the top right shows your experience points...");
		tutorialStrings.Add ("Each superhero has four different attacks...");
		tutorialStrings.Add ("A short range attack (Press Q)");
		tutorialStrings.Add ("A medium range attack (Press W)");
		tutorialStrings.Add ("A long range attack (Press E)");
		tutorialStrings.Add ("And an area of effect attack (Press R)");
		tutorialStrings.Add ("Each kill gives you XP to level up your hero...");
		tutorialStrings.Add ("Use your AOE attack (R) while level 1");
		tutorialStrings.Add ("Now use it again while level 10 (fully leveled)");
		tutorialStrings.Add ("You've completed the tutorial, click to start the game.");
	}

	void CheckTutorial()
	{
		if (!tutorialMode || waiting)
			return;
		
		switch (currentString) 
		{
		case((int)TutorialPosition.PressQ):
			if (Input.GetKeyDown (KeyCode.Q))
				NextString (2);
			break;
		case((int)TutorialPosition.PressW):
			if (Input.GetKeyDown (KeyCode.W))
				NextString (2);
			break;
		case((int)TutorialPosition.PressE):
			if (Input.GetKeyDown (KeyCode.E))
				NextString (2);
			break;
		case((int)TutorialPosition.PressR):
			if (Input.GetKeyDown (KeyCode.R))
				NextString (2);
			break;
		case((int)TutorialPosition.Lvl1AOE):
			if (Input.GetKeyDown (KeyCode.R))
				NextString (2);
			break;
		case((int)TutorialPosition.Lvl10AOE):
			if (Input.GetKeyDown (KeyCode.R))
				NextString (2);
			break;
		default:
			if (PressedNextPage () && currentString < tutorialStrings.Count - 1) {
				NextString ();
			} else if (PressedNextPage () && currentString < tutorialStrings.Count) {
				PlayerInfo.controller.NoLongerNewPlayer ();
				SceneManager.LoadScene ("Test Scene");
			}
			break;
		}
	}

	void NextString()
	{
		waiting = false;
		currentString++;
		SetTutorialText (tutorialStrings [currentString]);

		switch (currentString) {
		case((int)TutorialPosition.PressQ):
			SpawnEnemy (Player.ShortAttackDistance ());
			break;
		case((int)TutorialPosition.PressW):
			SpawnEnemy (Player.MediumAttackDistance ());
			break;
		case((int)TutorialPosition.PressE):
			SpawnEnemy (Player.LargeAttackDistance ());
			break;
		case((int)TutorialPosition.PressR):
			SpawnEnemy (Player.ShortAttackDistance ());
			SpawnEnemy (-Player.ShortAttackDistance ());
			break;
		case((int)TutorialPosition.Lvl1AOE):
			Player.playerSingleton.FinishRechargingAttack (3);
			FinishSlider (3);
			XPController.controller.SetLevel (1);
			SpawnEnemy (Player.ShortAttackDistance ());
			SpawnEnemy (-Player.ShortAttackDistance ());
			break;
		case((int)TutorialPosition.Lvl10AOE):
			Player.playerSingleton.FinishRechargingAttack (3);
			FinishSlider (3);
			XPController.controller.SetLevel (10);
			SpawnEnemy (Player.ShortAttackDistance ());
			SpawnEnemy (-Player.ShortAttackDistance ());
			break;
		default:
			break;
		}
	}

	void SpawnEnemy(float distance)
	{
		GameObject enemy = Instantiate (enemyPrefab, new Vector3 (Player.playerSingleton.GetXPos () + distance, Player.playerSingleton.transform.position.y), Quaternion.identity) as GameObject;
		enemy.GetComponent<Enemy> ().SetTutorialMode (true);
	}

	void NextString(float time)
	{
	    waiting = true;
		Invoke ("NextString", time);
	}

	void SetTutorialText(string s)
	{
		tutorialText.text = s;
	}

	public void SetTutorialMode(bool b)
	{
		tutorialMode = b;
	}

	public bool GetTutorialMode()
	{
		return tutorialMode;
	}

	public int TutorialToInt(TutorialPosition t)
	{
		return (int)t;
	}

	public bool CheckIfOnSpecificPosition(TutorialPosition t)
	{
		if (currentString == (int)t)
			return true;
		return false;
	}

	public void PressSaveAndQuit()
	{
		PlayerInfo.controller.Save ();
		SceneManager.LoadScene ("Main Menu");
	}

	public void PressReturnToGame()
	{
		Disable (ESCImage);
		Disable (Options);
		Time.timeScale = 1;
	}

	public void PressOptions()
	{
		Activate (Options);
		Disable (ESCImage);
	}

	public void PressBack()
	{
		Disable (Options);
		Activate (ESCImage);
	}

	public void ToggleMusic()
	{
		PlayerInfo.controller.ClickMusic ();
	}

	public void ToggleFX()
	{
		PlayerInfo.controller.ClickFX ();
	}

	public void PressEsc()
	{
		if (!ESCImage.IsActive ()) {
			Activate (ESCImage);
			Time.timeScale = 0;
		} else {
			PressReturnToGame ();
		}
	}
}
