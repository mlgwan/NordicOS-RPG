using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BaseStatItem : BaseItem {

    private int health;
    private int strength;
    private int dexterity;
    private int intelligence;
    private int speed;
    private int mana;
    private int spellEffectID;

    public int Health
    {
        get { return health; }
        set { health = value; }
    }
    public int Strength
    {
        get { return strength; }
        set { strength = value; }
    }
    public int Dexterity
    {
        get { return dexterity; }
        set { dexterity = value; }
    }
    public int Intelligence
    {
        get { return intelligence; }
        set { intelligence = value; }
    }
    public int Speed
    {
        get { return speed; }
        set { speed = value; }
    }
    public int Mana
    {
        get { return mana; }
        set { mana = value; }
    }
    public int SpellEffectID
    {
        get { return spellEffectID; }
        set { spellEffectID = value; }
    }
}
