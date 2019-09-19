using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BaseEnemy : BaseCharacter
{


    public enum Type
    {
        BEAST,
        MAGIC,
        HUMAN,
        GOD
    }

    public enum Rarity
    {
        COMMON,
        UNCOMMON,
        RARE,
        SUPERRARE
    }

    public Type EnemyType;
    public Rarity rarity;
    public int experienceToReward;
    public int level;

    public void SetupStats() {  // the enemmie's level will be set in the inspector, this is just for testing purposes, as we will likely end up hand-picking the stats for each monster carefully and individually

        maxHP = DetermineBaseStat(baseHP, "HP");
        maxMP = DetermineBaseStat(baseMP, "MP");
        maxSTR = DetermineBaseStat(baseSTR);
        maxDEX = DetermineBaseStat(baseDEX);
        maxINT = DetermineBaseStat(baseINT);
        maxSPD = DetermineBaseStat(baseSPD, "SPD");

        curHP = maxHP;
        curMP = maxMP;
        curATK = maxATK;
        curDEF = maxDEF;
        curSTR = maxSTR;
        curDEX = maxDEX;
        curINT = maxINT;
        curSPD = maxSPD;;
    }

    int DetermineBaseStat(int baseStat, string statName = "")
    {  //Formulas to determine the individual base stats, does need a little tweeking

        if (statName == "HP")
        {
            return (int)(((baseStat * 1.9f * level) / 50) + 8 + maxSTR / 3);
        }

        else if (statName == "MP")
        {
            return (int)(((baseStat * 1.9f * level) / 50) + 8 + maxINT / 2);
        }

        else if (statName == "SPD")
        {
            return (int)(((baseStat * 1.9f * level) / 50) + 8 + maxDEX / 1.2f);
        }

        return (int)((baseStat * 1.4f * level) / 50) + 3;
    }
}
