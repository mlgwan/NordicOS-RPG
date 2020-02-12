using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseMageClass : BaseCharacterClass {

    public BaseMageClass()
    {
        CharacterClassName = "Mage";
        CharacterClassDescription = "A Powerful Spellcasterr";
        Health = 11;
        Strength = 8;
        Dexterity = 14;
        Intelligence = 17;
        Speed = 12;
        Mana = 50;
    }
}
