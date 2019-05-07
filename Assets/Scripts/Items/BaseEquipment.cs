using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BaseEquipment : BaseStatItem {

    public enum EquipmentTypes {
        HELMET,
        ARMOR,
        GAUNTLETS,
        BOOTS,
        NECKLACE,
        RING
    }

    private EquipmentTypes equipmentType;


    public EquipmentTypes EquipmentType
    {
        get { return equipmentType; }
        set { equipmentType = value; }
    }


}
