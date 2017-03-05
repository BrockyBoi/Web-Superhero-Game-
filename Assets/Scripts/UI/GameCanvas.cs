using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameCanvas : MonoBehaviour
{

    public static GameCanvas controller;

    public List<Slider> sliders;
    float lastXPMax;
    float currentXPMax;
    float currentXPValue;

    public List<Image> attackImages;

    public Image ESCImage;
    public Image Options;
    public GameObject UpgradeUI;

    public enum SliderNumbers { ShortAttack, MediumAttack, LargeAttack, AOEAttack, XP, Health }
    float xpValue;
    public Text xpText;

    public Transform boundary1;
    public Transform boundary2;

    public List<GameObject> heroPrefabs = new List<GameObject>();

    public Transform startLocation;

    public GameObject Enemies;


    //Tutorial stuff
    bool tutorialMode;
    public Image tutorialImage;
    public Text tutorialText;
    List<string> tutorialStrings = new List<string>();
    int currentString;
    bool waiting;
    public enum TutorialPosition { PressQ = 4, PressW, PressE, PressR, Lvl1AOE = 9, Lvl10AOE = 10, FinalSlide = 11 }
    public GameObject enemyPrefab;

    public GameObject musicButton, fxButton;

    void Awake()
    {
        controller = this;
    }

    // Use this for initialization
    void Start()
    {
        if (PlayerInfo.controller.CheckIfNewPlayer())
            tutorialMode = true;

        SpawnHero();

        if (tutorialMode)
        {
            InitializeStrings();
            Activate(tutorialImage.gameObject);
            SetTutorialText(tutorialStrings[0]);
            XPController.controller.SetLevel(1);
            EnemySpawner.controller.gameObject.SetActive(false);
        }
        else
            Disable(tutorialImage.gameObject);

        StartCoroutine(UpdateXPSlider());
        StartCoroutine(UpdateHealthSlider());

        Disable(ESCImage.gameObject);
        Disable(Options.gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        CheckTutorial();
        CheckESC();
    }

    void SpawnHero()
    {
        Instantiate(heroPrefabs[SuperPowerController.controller.GetSuperHero()], startLocation.position, Quaternion.identity);
    }

    public void CheckESC()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            PressEsc();
        }
    }
	

    public void InitializeSlider(int whichSlider, float value)
    {
        sliders[whichSlider].maxValue = value;
        sliders[whichSlider].value = value;
    }

    void InitializeAttackImages()
    {
        for (int i = 0; i < attackImages.Count; i++)
        {
            attackImages[i].color = new Color(.3f, .3f, .3f, 1);
        }
    }

    void TurnOnAttackImage(int num)
    {
        attackImages[num].color = new Color(1, 1, 1, attackImages[num].color.a);
    }

    void TurnOffAttackImage(int num)
    {
        attackImages[num].color = new Color(.3f, .3f, .3f, attackImages[num].color.a);
    }

    public void UpdateAttackSlider(int num)
    {
        StartCoroutine(TurnOnSlider(num));
    }

    public Transform GrabBoundary1()
    {
        return boundary1;
    }

    public Transform GrabBoundary2()
    {
        return boundary2;
    }

    IEnumerator TurnOnSlider(int num)
    {
        float time = 0;

        while (time < sliders[num].maxValue)
        {
            time += Time.deltaTime;
            sliders[num].value = time;
            yield return null;
        }
    }

    public void UpdateAttackImage(int num)
    {
        StartCoroutine(ColorizeAttackImage(num));
    }

    IEnumerator ColorizeAttackImage(int num)
    {
        float time = 0;
        float endTime = Player.playerSingleton.GetAttackTime(num);
        attackImages[num].color = new Color(attackImages[num].color.r, attackImages[num].color.g, attackImages[num].color.b, 0);

        while (time < endTime)
        {
            time += Time.deltaTime;
            float a = time / endTime;
            attackImages[num].color = new Color(attackImages[num].color.r,
                                                attackImages[num].color.g,
                                                attackImages[num].color.b,
                                                                        a);
            yield return null;
        }
    }

    void FinishSlider(int num)
    {
        if (!tutorialMode)
            return;

        StopAllCoroutines();

        for (int i = 0; i < 4; i++)
            sliders[i].value = sliders[i].maxValue;

        StartCoroutine(UpdateXPSlider());
    }

    public int SliderToInt(SliderNumbers num)
    {
        return (int)num;
    }

    public void NewLevel(int level)
    {
        currentXPValue = XPController.controller.GetCurrentXP();
        if (level > 1 && level != 4)
            lastXPMax = XPController.controller.GetCap(level - 1);
        else if (level == 1)
            lastXPMax = 0;
        else if (level == 4)
            lastXPMax = XPController.controller.GetCap(level - 2);

        if (level == 4)
            currentXPMax = XPController.controller.GetCap(3);
        else currentXPMax = XPController.controller.GetCap(level);

        float newMax = currentXPMax - lastXPMax;

        for (int i = 1; i <= 4; i++)
        {
            if (i <= level)
            {
                TurnOnAttackImage(i - 1);
            }
            else
                TurnOffAttackImage(i - 1);
        }

        sliders[(int)SliderNumbers.XP].value = currentXPValue - lastXPMax;
        sliders[(int)SliderNumbers.XP].maxValue = newMax;

        UpdateXPText();
    }

    public void AssignSliderMaxValues(int slider, float max)
    {
        sliders[slider].maxValue = max;
    }

    IEnumerator UpdateXPSlider()
    {
        while (true)
        {
            int xpDiff = Mathf.Abs((int)xpValue - (XPController.controller.GetCurrentXP() - (int)lastXPMax));
            while (xpDiff > 0)
            {
                // while (xpValue != XPController.controller.GetCurrentXP () - lastXPMax) {
                // 	if (Time.timeScale > 0) {
                // 		xpValue = Mathf.Lerp (xpValue, XPController.controller.GetCurrentXP () - lastXPMax, time);
                // 		sliders [(int)SliderNumbers.XP].value = xpValue;
                // 		UpdateXPText ();

                // 		time += Time.deltaTime;
                // 		if (time >= 1) {
                // 			xpValue = XPController.controller.GetCurrentXP ();
                // 			UpdateXPText ();
                // 			sliders [(int)SliderNumbers.XP].value = xpValue;
                // 		}
                // 	} else {
                // 		xpValue = XPController.controller.GetCurrentXP ();
                // 		sliders [(int)SliderNumbers.XP].value = xpValue;
                // 		UpdateXPText ();
                // 	}
                if (Time.timeScale > 0)
                {
                    if (xpDiff < 10)
                    {
                        xpValue++;
                    }
                    else if (xpDiff < 40)
                    {
                        xpValue += 2;
                    }
                    else if (xpDiff < 100)
                    {
                        xpValue += 5;
                    }
                    else
                    {
                        xpValue += 10;
                    }

                    xpDiff = Mathf.Abs((int)xpValue - (XPController.controller.GetCurrentXP() - (int)lastXPMax));
                    sliders[(int)SliderNumbers.XP].value = xpValue;
                    UpdateXPText();

					Debug.Log(xpDiff);
                    yield return new WaitForSeconds(.1f);
                }
                else
                {
                    xpValue = XPController.controller.GetCurrentXP();
                    sliders[(int)SliderNumbers.XP].value = xpValue;
                    UpdateXPText();
                }
            }

            yield return null;
        }
    }

    IEnumerator UpdateHealthSlider()
    {
        float health = Player.playerSingleton.GetMaxHealth();
        sliders[(int)SliderNumbers.Health].maxValue = health;
        sliders[(int)SliderNumbers.Health].value = health;
        while (true)
        {
            float time = 0;
            while (Mathf.Abs(Player.playerSingleton.GetHealth() - health) > .05f)
            {
                health = Mathf.Lerp(health, Player.playerSingleton.GetHealth(), time);
                time += Time.deltaTime * .25f;
                sliders[(int)SliderNumbers.Health].value = health;

                if (time >= 1)
                {
                    health = Player.playerSingleton.GetHealth();
                }
                yield return null;
            }
            yield return null;
        }
    }

	public void SetXP(float xp)
	{
		xpValue = xp;
		sliders[(int)SliderNumbers.XP].value = xpValue;
        UpdateXPText();
	}

    void UpdateXPText()
    {
        xpText.text = (((int)(xpValue + lastXPMax)).ToString() + " / " + ((int)currentXPMax).ToString());
    }

    void Activate(GameObject i)
    {
        i.SetActive(true);
    }

    void Disable(GameObject i)
    {
        i.SetActive(false);
    }

    bool PressedNextPage()
    {
        if (Input.GetMouseButtonDown(0) || Input.GetAxisRaw("Submit") == 1)
            return true;
        return false;
    }

    void InitializeStrings()
    {
        tutorialStrings.Add("In this game you have two main goals: Level up and stay alive (Click to proceed)...");
        tutorialStrings.Add("The top left shows your health...");
        tutorialStrings.Add("While the top right shows your experience points...");
        tutorialStrings.Add("Each superhero has four different attacks...");
        tutorialStrings.Add("A short range attack (Press Q)");
        tutorialStrings.Add("A medium range attack (Press W)");
        tutorialStrings.Add("A long range attack (Press E)");
        tutorialStrings.Add("And an area of effect attack (Press R)");
        tutorialStrings.Add("Each kill gives you XP to level up your hero...");
        tutorialStrings.Add("Use your AOE attack (R) while level 1");
        tutorialStrings.Add("Now use it again while level 10 (fully leveled)");
        tutorialStrings.Add("You've completed the tutorial, click to start the game.");
    }

    void CheckTutorial()
    {
        if (!tutorialMode || waiting)
            return;

        switch (currentString)
        {
            case ((int)TutorialPosition.PressQ):
                if (Input.GetKeyDown(KeyCode.Q))
                    NextString(2);
                break;
            case ((int)TutorialPosition.PressW):
                if (Input.GetKeyDown(KeyCode.W))
                    NextString(2);
                break;
            case ((int)TutorialPosition.PressE):
                if (Input.GetKeyDown(KeyCode.E))
                    NextString(2);
                break;
            case ((int)TutorialPosition.PressR):
                if (Input.GetKeyDown(KeyCode.R))
                    NextString(2);
                break;
            case ((int)TutorialPosition.Lvl1AOE):
                if (Input.GetKeyDown(KeyCode.R))
                    NextString(2);
                break;
            case ((int)TutorialPosition.Lvl10AOE):
                if (Input.GetKeyDown(KeyCode.R))
                    NextString(2);
                break;
            default:
                if (PressedNextPage() && currentString < tutorialStrings.Count - 1)
                {
                    NextString();
                }
                else if (PressedNextPage() && currentString < tutorialStrings.Count)
                {
                    PlayerInfo.controller.NoLongerNewPlayer();
                    SceneManager.LoadScene("Test Scene");
                }
                break;
        }
    }

    void NextString()
    {
        waiting = false;
        currentString++;
        SetTutorialText(tutorialStrings[currentString]);

        switch (currentString)
        {
            case ((int)TutorialPosition.PressQ):
                SpawnEnemy(Player.ShortAttackDistance());
                break;
            case ((int)TutorialPosition.PressW):
                SpawnEnemy(Player.MediumAttackDistance());
                break;
            case ((int)TutorialPosition.PressE):
                SpawnEnemy(Player.LargeAttackDistance());
                break;
            case ((int)TutorialPosition.PressR):
                SpawnEnemy(Player.ShortAttackDistance());
                SpawnEnemy(-Player.ShortAttackDistance());
                break;
            case ((int)TutorialPosition.Lvl1AOE):
                Player.playerSingleton.FinishRechargingAttack(3);
                FinishSlider(3);
                XPController.controller.SetLevel(1);
                SpawnEnemy(Player.ShortAttackDistance());
                SpawnEnemy(-Player.ShortAttackDistance());
                break;
            case ((int)TutorialPosition.Lvl10AOE):
                Player.playerSingleton.FinishRechargingAttack(3);
                FinishSlider(3);
                XPController.controller.SetLevel(4);
                SpawnEnemy(Player.ShortAttackDistance());
                SpawnEnemy(-Player.ShortAttackDistance());
                break;
            default:
                break;
        }
    }

    void SpawnEnemy(float distance)
    {
        GameObject enemy = Instantiate(enemyPrefab, new Vector3(Player.playerSingleton.GetXPos() + distance, Player.playerSingleton.transform.position.y), Quaternion.identity) as GameObject;
        enemy.GetComponent<Enemy>().SetTutorialMode(true);
    }

    void NextString(float time)
    {
        waiting = true;
        Invoke("NextString", time);
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

    #region Press Buttons
    public void PressSaveAndQuit()
    {
        PlayerInfo.controller.Save();
        SceneManager.LoadScene("Main Menu");
    }

    public void PressReturnToGame()
    {
        Disable(ESCImage.gameObject);
        Disable(Options.gameObject);
        Time.timeScale = 1;
    }

    public void PressOptions()
    {
        Activate(Options.gameObject);
        Disable(ESCImage.gameObject);
    }

    public void PressBack()
    {
        Disable(Options.gameObject);
        Activate(ESCImage.gameObject);
    }


    public void PressEsc()
    {
        if (!ESCImage.IsActive())
        {
            Activate(ESCImage.gameObject);
            Time.timeScale = 0;
        }
        else
        {
            PressReturnToGame();
        }
    }

    public void PressSave()
    {
        PlayerInfo.controller.Save();
    }

    public void PressLoad()
    {
        PlayerInfo.controller.Load();
    }

    public void PressUpgradeButton()
    {
        Activate(UpgradeUI);
        Time.timeScale = 0;
    }

    public void PressBackUpgrade()
    {
        Time.timeScale = 1;
        Disable(UpgradeUI);
    }
    #endregion

    #region Audio
    public void SetMusicVolume(float f)
    {
        SoundController.controller.SetMusic(f);
    }

    public void SetFXVolume(float f)
    {
        SoundController.controller.SetFX(f);
    }
    #endregion
}
