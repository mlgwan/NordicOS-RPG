using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseConsumable : BaseStatItem
{

    public enum ConsumableTypes
    {
        HEALTH,
        STRENGTH,
        DEXTERITY,
        INTELLIGENCE,
        SPEED,
        RESISTANCE,
        MANA
    }

    private ConsumableTypes consumableType;

    public ConsumableTypes ConsumableType
    {
        get { return consumableType; }
        set { consumableType = value; }
    }


}
