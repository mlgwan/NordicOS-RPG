using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BaseAttack : MonoBehaviour {
    public string attackName;
    public string attackDescription;
    public int attackDamage;
    public int attackCost;
    public bool isAOE; //AOE - Area of Effect

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
}
