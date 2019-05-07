using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BaseItem : MonoBehaviour {

    private string itemName;
    private string itemDescription;
    private int itemID;
    public enum ItemTypes {
        EQUIPMENT,
        WEAPONS,
        CONSUMABLE,
        CHEST
    }
    private ItemTypes itemType;

    public string ItemName {
        get { return itemName; }
        set { itemName = value; }
    }

    public string ItemDescription
    {
        get { return itemDescription; }
        set { itemDescription = value; }
    }

    public int ItemID
    {
        get { return itemID; }
        set { itemID = value; }
    }

    public ItemTypes ItemType {
        get { return itemType; }
        set { itemType = value; }
    }
}
