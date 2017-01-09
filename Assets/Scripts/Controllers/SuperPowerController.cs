using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SuperPowerController : MonoBehaviour {
	public static SuperPowerController controller = null;

    //Superheros are Tank, Elementalist, Paragon, Speedster, Vigilantee
    //Tank = High damage/health/knockback BUT long attack cooldowns
    //Elementalist = Low health/attack cooldowns
    //Paragon = Medium everything
    //Speedster = super low cooldowns, can move around quick, low health, lots of stuns, longest AOE attack
    public enum PowerNames { TankMelee, SonicClap, ShoulderCharge, GroundSmash,
                            Fire, Wave, RockThrow, Lightning,
                            ParagonMelee, FreezeBreath, HeatVision, Jump,
                            SpeedMelee, WindGust, DashAttack, MapDash,
                            Pistol, Shotgun, Sniper, Grenades,
							POWERS_COUNT
                           };
    public enum SuperHero { Tank, Elementalist, Paragon, Speedster, Vigilante, HERO_COUNT}
    SuperHero currentHero;
    //First int is simply which power it is, and the second int is how powerful it is
    List<int> superPowers = new List<int>();

    int[] availablePowers = new int[4];

    void Awake()
    {
		if (controller == null) {
			controller = this;
		} else if (controller != this) 
		{
			Destroy (this);
		}
		
        InitializePowers();
		DontDestroyOnLoad (this);

		//By default the superhero will be a tank
		SetSuperHero (SuperHero.Paragon);
    }

    public void SetSuperHero(SuperHero hero)
    {
        currentHero = hero;
		int firstPower = (int)hero * 4;
        for(int i = 0; i < 4; i++)
        {
            availablePowers[i] = firstPower + i;
        }
    }

	public void SetSuperHero(int hero)
	{
		currentHero = IntToHero (hero);
		int firstPower = hero * 4;
		for(int i = 0; i < 4; i++)
		{
			availablePowers[i] = firstPower + i;
		}
	}

	public int GetSuperHero()
	{
		return (int)currentHero;
	}

    public int[] GetAvailablePowers()
    {
        return availablePowers;
    }

    void InitializePowers()
    {
		for (int i = 0; i < (int) PowerNames.POWERS_COUNT; i++) {
			superPowers.Add (i);
		}
    }

    public static int PowerToInt(PowerNames power)
    {
        return (int)power;
    }

    public static PowerNames IntToPower(int num)
    {
        return (PowerNames)num;
    }

	public static SuperHero IntToHero(int num)
	{
		return (SuperHero)num;
	}

	public static int HeroToInt(SuperHero hero)
	{
		return (int)hero;
	}

	public static string HeroToString(SuperHero hero)
	{
		return hero.ToString();
	}

	public static string HeroToString(int hero)
	{
		return ((SuperHero)hero).ToString();
	}

    public int GetPower(PowerNames power)
    {
        return superPowers[PowerToInt(power)];
    }
}
