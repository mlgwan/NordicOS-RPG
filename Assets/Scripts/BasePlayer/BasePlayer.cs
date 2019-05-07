using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasePlayer : MonoBehaviour {

    private string playerName;
    private int playerLevel;
    private int health;
    private int strength;
    private int dexterity;
    private int intelligence;
    private int speed;
    private int mana;
    private BaseCharacterClass playerClass;

    public string PlayerName
    {
        get { return playerName; }
        set { playerName = value; }
    }

    public int PlayerLevel
    {
        get { return playerLevel; }
        set { playerLevel = value; }
    }

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


    public BaseCharacterClass PlayerClass
    {
        get { return playerClass; }
        set { playerClass = value; }
    }
}
