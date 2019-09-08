using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class BaseCharacter
{

    public string theName;

    public int maxHP;
    public int curHP;

    public int maxMP;
    public int curMP;

    public int maxATK;
    public int curATK;

    public int maxDEF;
    public int curDEF;

    public int maxSTR;
    public int curSTR;

    public int maxINT;
    public int curINT;

    public int maxDEX;
    public int curDEX;

    public int maxSPD;
    public int curSPD;

    // to calculate the maximum stats when leveling up
    [Header("Base stats")]
    public int baseHP;
    public int baseMP;
    public int baseSTR;
    public int baseDEX;
    public int baseINT;
    public int baseSPD;

    public BaseAttack basicAttack;
    public List<BaseAttack> abilities = new List<BaseAttack>();

    public bool paralysed;
    public bool burned;
    public bool poisoned;
    public bool frostburned;
    public bool stunned;

    public int dotToTake; //dot - damage over time
}
