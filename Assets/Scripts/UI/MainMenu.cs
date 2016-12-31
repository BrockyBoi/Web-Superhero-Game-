using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour {
	public static MainMenu controller;
	public List<GameObject> screens;
	int currentScreen;

	public GameObject heroSelect;

	public enum Screens{Title, Option_Achievement_HeroSelect, HeroSelect, Achievements, Options}

	public List<Button> heroButtons;

	public Text descriptionText;
	string[] heroDescriptions = new string[(int)SuperPowerController.SuperHero.HERO_COUNT];

	public Animator heroAnimator;




	void Awake()
	{
		controller = this;
	}

	void Start () {
		currentScreen = ScreensToInt (Screens.Title);
		TurnOffScreens ();
		ActivateScreen (currentScreen);
		SetHero ((int)SuperPowerController.SuperHero.Tank);

		CheckUnlockedHeroes ();

		InitializeStrings ();

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

		if (currentScreen == (int)Screens.HeroSelect) {
			SetHeroDescription (GetHeroString (SuperPowerController.SuperHero.Tank));
			CheckUnlockedHeroes ();
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

	void InitializeStrings()
	{
		heroDescriptions [(int)SuperPowerController.SuperHero.Tank] = "* Highest health\n*Takes no knockback\n*Slow speed/attack regen\n*Highest enemy knockback";
		heroDescriptions [(int)SuperPowerController.SuperHero.Elementalist] = "* Lowest health\n*Takes knockback\n*Medium speed/attack regen\n*High enemy knockback/damage";
		heroDescriptions [(int)SuperPowerController.SuperHero.Paragon] = "* High health\n*Takes no knockback\n*Medium speed, slow attack regen\n*Medium enemy knockback";
		heroDescriptions [(int)SuperPowerController.SuperHero.Vigilante] = "* Low health\n*Takes knockback\n*Medium speed/attack regen\n*High enemy knockback";
		heroDescriptions [(int)SuperPowerController.SuperHero.Speedster] = "* Medium health\n*Takes knockback\n*Highest speed/attack regen\n*Low enemy knockback";
	}

	void SetHeroDescription(string s)
	{
		descriptionText.text = s;
	}

	string GetHeroString(int num)
	{
		return heroDescriptions [num];
	}

	string GetHeroString(SuperPowerController.SuperHero hero)
	{
		return heroDescriptions [SuperPowerController.HeroToInt (hero)];
	}

	int ScreensToInt(Screens s)
	{
		return (int)s;
	}

	public void SetHero(int num)
	{
		switch (num) 
		{
		case((int)SuperPowerController.SuperHero.Tank):
			heroAnimator.SetTrigger ("Tank");
			break;
		case((int)SuperPowerController.SuperHero.Elementalist):
			heroAnimator.SetTrigger ("Elementalist");
			break;
		case((int)SuperPowerController.SuperHero.Paragon):
			heroAnimator.SetTrigger ("Paragon");
			break;
		case((int)SuperPowerController.SuperHero.Speedster):
			heroAnimator.SetTrigger ("Speedster");
			break;
		case((int)SuperPowerController.SuperHero.Vigilante):
			heroAnimator.SetTrigger ("Vigilante");
			break;
		default:
			break;
		}
		SuperPowerController.controller.SetSuperHero (num);
		SetHeroDescription (GetHeroString (num));
	}

	public void StartGame()
	{
		SceneManager.LoadScene ("Test Scene");
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

	public void SetMusicVolume(float f)
	{
		SoundController.controller.SetMusic (f);
	}

	public void SetFXVolume(float f)
	{
		SoundController.controller.SetFX (f);
	}
}
