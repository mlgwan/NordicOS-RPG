﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class BaseCharacter
{

    public string theName;

    public int baseHP;
    public int curHP;

    public int baseMP;
    public int curMP;

    public int baseATK;
    public int curATK;

    public int baseDEF;
    public int curDEF;

    public int baseSTR;
    public int curSTR;

    public int baseINT;
    public int curINT;

    public int baseDEX;
    public int curDEX;

    public int baseSPD;
    public int curSPD;

    public BaseAttack basicAttack;
    public List<BaseAttack> abilities = new List<BaseAttack>();

    public enum Status {
        UNHARMED,
        PARALYSED,
        BURNED,
        POISONED,
        FROSTBURNED,
        STUNNED
    }

    public Status currentStatus;

    public int dotToTake; //dot - damage over time
}
