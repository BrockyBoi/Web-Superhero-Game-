using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeController : MonoBehaviour
{
    public static UpgradeController controller;
    public enum Upgrades { Damage, MaxHealth, HealthRegen, PowerRegen, Speed, NextHero, UPGRADE_COUNT };

    public bool maxLevel;

    int[] xpAmounts = new int[(int)Upgrades.UPGRADE_COUNT];
    int[] xpLevels = new int[(int)Upgrades.UPGRADE_COUNT];

    [SerializeField]
    int[] xpNeeds;

    void Awake()
    {
        controller = this;

        xpNeeds = new int[5] { 100, 500, 1000, 2500, 5000 };

        if (!maxLevel)
        {
            for (int i = 0; i < (int)Upgrades.UPGRADE_COUNT; i++)
            {
                xpLevels[i] = 0;
            }
        }
        else
        {
            for (int i = 0; i < (int)Upgrades.UPGRADE_COUNT; i++)
            {
                xpLevels[i] = 5;
            }
        }
    }
    // Use this for initialization
    void Start()
    {
        UpdatePowers();
    }

    public void SpendXP(int upgrade, int amount)
    {
        if (xpLevels[upgrade] < 5)
            return;
        xpAmounts[upgrade] += amount;

        if (xpAmounts[upgrade] >= xpNeeds[xpLevels[upgrade] - 1])
        {
            xpAmounts[upgrade] = 0;
            xpLevels[upgrade]++;
            UpdatePowers();
            Debug.Log("Leveled up power: " + upgrade + " to level " + xpLevels[upgrade]);
        }
    }

    public int UpgradeToInt(Upgrades upgrade)
    {
        return (int)upgrade;
    }

    public int GetUpgradeLevel(Upgrades upgrade)
    {
        return xpLevels[UpgradeToInt(upgrade)];
    }

    public int GetUpgradeLevel(int upgrade)
    {
        return xpLevels[upgrade];
    }

    void UpdatePowers()
    {
        Player.playerSingleton.UpdatePowers(xpLevels[(int)Upgrades.Damage], xpLevels[(int)Upgrades.MaxHealth],
            xpLevels[(int)Upgrades.HealthRegen], xpLevels[(int)Upgrades.PowerRegen], xpLevels[(int)Upgrades.Speed]);
    }
}
