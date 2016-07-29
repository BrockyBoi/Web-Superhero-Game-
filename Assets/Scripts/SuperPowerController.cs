using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SuperPowerController : MonoBehaviour {
    public static SuperPowerController controller;

    //Superheros are Tank, Elementalist, Paragon, Speedster, Vigilantee
    //Tank = High damage/health/knockback BUT long attack cooldowns
    //Elementalist = Low health/attack cooldowns
    //Paragon = Medium everything
    //Speedster = super low cooldowns, can move around quick, low health, lots of stuns, longest AOE attack
    public enum PowerNames { TankMelee, SonicClap, ShoulderCharge, GroundSmash,
                            Fire, Wave, RockThrow, Lightning,
                            ParagonMelee, FreezeBreath, HeatVision, Jump,
                            SpeedMelee, WindGust, DashAttack, MapDash,
                            Pistol, Shotgun, Sniper, Grenades
                           };
    public enum SuperHero { Tank, Elementalist, Paragon, Speedster, Vigilantee, HERO_COUNT}
    SuperHero currentHero;
    //First int is simply which power it is, and the second int is how powerful it is
    List<int> superPowers = new List<int>();

    int[] availablePowers = new int[4];

    void Awake()
    {
		controller = this;
        InitializePowers();

		//By default the superhero will be a tank
		SetSuperHero (SuperHero.Paragon);
    }

    // Use this for initialization
    void Start () {

	}
	
	// Update is called once per frame
	void Update () {
	
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
        superPowers.Add(PowerToInt(PowerNames.TankMelee));
        superPowers.Add(PowerToInt(PowerNames.SonicClap));
        superPowers.Add(PowerToInt(PowerNames.ShoulderCharge));
        superPowers.Add(PowerToInt(PowerNames.GroundSmash));

        superPowers.Add(PowerToInt(PowerNames.Fire));
        superPowers.Add(PowerToInt(PowerNames.Wave));
        superPowers.Add(PowerToInt(PowerNames.RockThrow));
        superPowers.Add(PowerToInt(PowerNames.Lightning));

        superPowers.Add(PowerToInt(PowerNames.ParagonMelee));
        superPowers.Add(PowerToInt(PowerNames.FreezeBreath));
        superPowers.Add(PowerToInt(PowerNames.HeatVision));
        superPowers.Add(PowerToInt(PowerNames.Jump));

        superPowers.Add(PowerToInt(PowerNames.SpeedMelee));
        superPowers.Add(PowerToInt(PowerNames.WindGust));
        superPowers.Add(PowerToInt(PowerNames.DashAttack));
        superPowers.Add(PowerToInt(PowerNames.MapDash));      

        superPowers.Add(PowerToInt(PowerNames.Pistol));
        superPowers.Add(PowerToInt(PowerNames.Shotgun));
        superPowers.Add(PowerToInt(PowerNames.Sniper));
        superPowers.Add(PowerToInt(PowerNames.Grenades));
    }

    public int PowerToInt(PowerNames power)
    {
        return (int)power;
    }

    public PowerNames IntToPower(int num)
    {
        return (PowerNames)num;
    }

	public SuperHero IntToHero(int num)
	{
		return (SuperHero)num;
	}

	public int HeroToInt(SuperHero hero)
	{
		return (int)hero;
	}

    public int GetPower(PowerNames power)
    {
        return superPowers[PowerToInt(power)];
    }
}
