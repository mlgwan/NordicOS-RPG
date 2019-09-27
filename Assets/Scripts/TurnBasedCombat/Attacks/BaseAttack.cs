using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BaseAttack : MonoBehaviour {
    public string attackName;
    public string attackDescription;
    public int attackDamage;
    public int critChance;
    public int hitChance; //accuracy
    public int attackCost;
    public bool isAOE; //AOE - Area of Effect
    public bool isMagic;

    public float lowerRandomBound;  
    public float upperRandomBound;

    public int applicationChance;
    public enum StatusEffects {
        NONE,
        PARALYSIS,
        BURN,
        POISON,
        FROSTBURN,
        STUN
    }
    public StatusEffects statusEffectToApply;

    public int poisonDamage;
    public int burnDamage;
    public int frostBurnDamage;

    public enum ScalesWith {
        NOTHING,
        STRENGTH,
        DEXTERITY,
        INT
    }
    public ScalesWith scalesWith;
}
