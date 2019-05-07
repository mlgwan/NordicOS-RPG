using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseCharacterClass : MonoBehaviour {

    private string characterClassName;
    private string characterClassDescription;
    //stats
    private int health;
    private int strength;
    private int dexterity;
    private int intelligence;
    private int speed;
    private int mana;

    public string CharacterClassName {
        get{ return characterClassName;}
        set { characterClassName = value; }
    }

    public string CharacterClassDescription
    {
        get { return characterClassDescription; }
        set { characterClassDescription = value; }
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
}
