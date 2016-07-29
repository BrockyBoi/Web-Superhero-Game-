using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour {
	public static MainMenu controller;
	public List<GameObject> screens;
	int currentScreen;

	public Text fxText;
	public Text musicText;

	public enum Screens{Title, Option_Achievement_HeroSelect, HeroSelect, Achievements, Options}

	public List<GameObject> heros = new List<GameObject>();


	void Awake()
	{
		controller = this;
	}
	// Use this for initialization
	void Start () {
		currentScreen = ScreensToInt (Screens.Title);
		TurnOffScreens ();
		ActivateScreen (currentScreen);

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
}
