using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeController : MonoBehaviour {
	public static UpgradeController controller;
	public enum Upgrades{Damage, MaxHealth, HealthRegen, PowerRegen, Speed, NextHero, UPGRADE_COUNT};

	int[] xpAmounts = new int[(int)Upgrades.UPGRADE_COUNT]; 
	int[] xpLevels = new int[(int)Upgrades.UPGRADE_COUNT];

	[SerializeField]
	int[] xpNeeds;

	void Awake()
	{
		controller = this;

		xpNeeds = new int[5]{ 100, 500, 1500, 5000, 15000 };

		for (int i = 0; i < (int)Upgrades.UPGRADE_COUNT; i++) {
			xpLevels [i] = 1;
		}
	}
	// Use this for initialization
	void Start () {
		UpdatePowers ();
	}

	public void SpendXP(int upgrade)
	{
		xpAmounts[upgrade] += 2;

		if (xpAmounts [upgrade] >= xpNeeds [xpLevels[upgrade]]) {
			xpAmounts [upgrade] = 0;
			xpLevels [upgrade]++;
			UpdatePowers ();
		}
	}

	public int UpgradeToInt(Upgrades upgrade)
	{
		return (int)upgrade;
	}

	public int GetUpgradeLevel(Upgrades upgrade)
	{
		return xpLevels [UpgradeToInt (upgrade)];
	}

	void UpdatePowers()
	{
		Player.playerSingleton.UpdatePowers (xpLevels [(int)Upgrades.Damage], xpLevels [(int)Upgrades.HealthRegen], 
			xpLevels [(int)Upgrades.HealthRegen], xpLevels [(int)Upgrades.PowerRegen], xpLevels [(int)Upgrades.Speed]);
	}
}
