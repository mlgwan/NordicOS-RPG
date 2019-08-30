using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BasePlayer : BaseCharacter
{

    public int level;
    public int currentExperiencePoints;
    public int totalExperiencePoints;



    public void reset( ) {
        curHP = baseHP;
        curMP = baseMP;
        curATK = baseATK;
        curDEF = baseDEF;
        curSTR = baseSTR;
        curDEX = baseDEX;
        curINT = baseINT;
        curSPD = baseSPD;
        level = 1;
        currentExperiencePoints = 0;
        totalExperiencePoints = 0;
    }

    public void restore() {
        curHP = baseHP;
        curMP = baseMP;
    }

    public void restoreFully() {
        curHP = baseHP;
        curMP = baseMP;
        curATK = baseATK;
        curDEF = baseDEF;
        curSTR = baseSTR;
        curDEX = baseDEX;
        curINT = baseINT;
        curSPD = baseSPD;
    }


}
