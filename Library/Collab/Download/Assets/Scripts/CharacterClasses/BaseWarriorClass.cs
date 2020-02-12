using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseWarriorClass : BaseCharacterClass {

    public BaseWarriorClass()
    {
        CharacterClassName = "Warrior";
        CharacterClassDescription = "A Fierce Axe-Wielder";
        Health = 20;
        Strength = 15;
        Dexterity = 12;
        Intelligence = 9;
        Speed = 10;
        Mana = 5;
    }
}
