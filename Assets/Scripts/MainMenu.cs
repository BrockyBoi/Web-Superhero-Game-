using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour {
	public static MainMenu controller;
	public List<GameObject> screens;
	int currentScreen;

	public Text fxText;
	public Text musicText;

	public GameObject heroSelect;

	public enum Screens{Title, Option_Achievement_HeroSelect, HeroSelect, Achievements, Options}

	public List<Button> heroButtons;




	void Awake()
	{
		controller = this;
	}
	// Use this for initialization
	void Start () {
		currentScreen = ScreensToInt (Screens.Title);
		TurnOffScreens ();
		ActivateScreen (currentScreen);
		SetHero ((int)SuperPowerController.SuperHero.Tank);

		CheckUnlockedHeroes ();

		PlayerInfo.controller.Load ();

	}
	
	// Update is called once per frame
	void Update () {

	}

	public void CheckAudioSettings()
	{
		bool varOn = PlayerInfo.controller.GetMusic ();
		string result;
		if (varOn)
			result = "On";
		else
			result = "Off";

		musicText.text = "Music: " + result;

		varOn = PlayerInfo.controller.GetFX ();
		if (varOn)
			result = "On";
		else
			result = "Off";

		fxText.text = "FX: " + result;
	}

	void TurnOffScreens()
	{
		for (int i = 0; i < screens.Count; i++) {
			DeactivateScreen (i);
		}
	}

	public void ActivateScreen(Screens s)
	{
		screens [ScreensToInt (s)].gameObject.SetActive (true);
		currentScreen = ScreensToInt (s);
	}

	public void ActivateScreen(int i)
	{
		DeactivateScreen (currentScreen);
		screens [i].gameObject.SetActive (true);
		currentScreen = i;

		if (currentScreen == (int)Screens.Options) {
			CheckAudioSettings ();
		}
	}

	void DeactivateScreen(Screens s)
	{
		screens [ScreensToInt (s)].gameObject.SetActive (false);
	}

	void DeactivateScreen(int i)
	{
		screens [i].gameObject.SetActive (false);
	}

	public void ToggleMusic()
	{
		bool varOn = PlayerInfo.controller.ClickMusic ();
		string result;
		if (varOn)
			result = "On";
		else
			result = "Off";
		
		musicText.text = "Music: " + result;
	}

	public void ToggleFX()
	{
		bool varOn = PlayerInfo.controller.ClickFX ();
		string result;
		if (varOn)
			result = "On";
		else
			result = "Off";
		fxText.text = "FX: " + result;
	}

	void CheckScreens ()
	{
		
	}

	int ScreensToInt(Screens s)
	{
		return (int)s;
	}

	void PreviousScreen()
	{
		screens [currentScreen].gameObject.SetActive (false);
		currentScreen--;
		screens [currentScreen].gameObject.SetActive (true);
	}

	void NextScreen()
	{
		screens [currentScreen].gameObject.SetActive (false);
		currentScreen++;
		screens [currentScreen].gameObject.SetActive (true);
	}

	public void SetHero(int num)
	{
		switch (num) 
		{
		case((int)SuperPowerController.SuperHero.Tank):
			//Bla bla bla
			heroSelect.GetComponent<SpriteRenderer>().color = Color.green;
			break;
		case((int)SuperPowerController.SuperHero.Elementalist):
			heroSelect.GetComponent<SpriteRenderer>().color = Color.blue;
			break;
		case((int)SuperPowerController.SuperHero.Paragon):
			heroSelect.GetComponent<SpriteRenderer>().color = Color.red;
			break;
		case((int)SuperPowerController.SuperHero.Speedster):
			heroSelect.GetComponent<SpriteRenderer>().color = Color.yellow;
			break;
		case((int)SuperPowerController.SuperHero.Vigilantee):
			heroSelect.GetComponent<SpriteRenderer>().color = Color.black;
			break;
		default:
			break;
		}
		SuperPowerController.controller.SetSuperHero (num);
	}

	public void StartGame()
	{
		if (!PlayerInfo.controller.CheckIfNewPlayer ())
			SceneManager.LoadScene ("Test Scene");
		else
			SceneManager.LoadScene ("Tutorial Scene");
	}

	public void CheckUnlockedHeroes()
	{
		for (int i = 0; i < heroButtons.Count; i++) {
			bool unlocked = AchievementSystem.controller.CheckIfUnlocked (i);
			if (!unlocked) {
				heroButtons [i].GetComponent<Button> ().enabled = false;
				heroButtons [i].GetComponent<Image> ().color = Color.gray;
			} else {
				heroButtons [i].GetComponent<Button> ().enabled = true;
				heroButtons [i].GetComponent<Image> ().color = Color.white;
			}
		}
	}
}
