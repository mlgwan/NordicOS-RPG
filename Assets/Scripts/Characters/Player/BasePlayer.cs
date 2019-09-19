using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BasePlayer : BaseCharacter
{

    public int level;
    public int[] toLevelUp;
    private int baseXP;
    public int currentExperiencePoints;
    public int totalExperiencePoints;




    public void reset()
    {
        curHP = maxHP;
        curMP = maxMP;
        curATK = maxATK;
        curDEF = maxDEF;
        curSTR = maxSTR;
        curDEX = maxDEX;
        curINT = maxINT;
        curSPD = maxSPD;
        level = 0;
        currentExperiencePoints = 0;
        totalExperiencePoints = 0;
    }

    public void restore()
    {
        curHP = maxHP;
        curMP = maxMP;
    }

    public void restoreFully()
    {
        curHP = maxHP;
        curMP = maxMP;
        curATK = maxATK;
        curDEF = maxDEF;
        curSTR = maxSTR;
        curDEX = maxDEX;
        curINT = maxINT;
        curSPD = maxSPD;
    }

    public void LevelUp()
    {
        if (currentExperiencePoints > toLevelUp[level]) {
            currentExperiencePoints -= toLevelUp[level];
        }
        level += 1;
        

        maxHP = DetermineBaseStat(baseHP, "HP");
        maxMP = DetermineBaseStat(baseMP, "MP");
        maxSTR = DetermineBaseStat(baseSTR);
        maxDEX = DetermineBaseStat(baseDEX);
        maxINT = DetermineBaseStat(baseINT);
        maxSPD = DetermineBaseStat(baseSPD, "SPD");
        restoreFully();
    }

    public void LevelXPSetUp()
    {
        toLevelUp = new int[100]; //100 is the maximum level
        baseXP  = 2000; //xp needed for level 2
        for (int i = 1; i < toLevelUp.Length; i++)
        {
            toLevelUp[i] = (int)Mathf.Floor(baseXP * Mathf.Pow(i, 1.5f));
        }
    }

    int DetermineBaseStat(int baseStat, string statName = "") {  //Formulas to determine the individual base stats, does need a little tweeking

        if (statName == "HP")
        {
            return (int)(((baseStat * 1.9f * level) / 50) + 8 + maxSTR / 3);
        }

        else if (statName == "MP") {
            return (int)(((baseStat * 1.9f * level) / 50) + 8 + maxINT / 2);
        }
        
        else if (statName == "SPD") {
            return (int)(((baseStat * 1.9f * level) / 50) + 8 + maxDEX / 1.2f);
        }

        return (int) ((baseStat * 1.4f * level) / 50) + 3;
    }

}