using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Item : ScriptableObject {

    new public string name;
    public string description;
    public int itemID = 0;      //Item IDs: 1-100 Consumables, 101-200 Weapons, 201-300 Armor, 301-400 Accesories
    public Sprite icon = null;

    public enum ItemTypes
    {
        CONSUMABLE,
        WEAPON,
        ARMOR,
        ACCESSORY
    }
    public ItemTypes type;

    public enum UsableBy {
        ALL,
        ULF,
        RIKKR
    }
    public UsableBy usableBy;

    public virtual void UseItem() { }
}
