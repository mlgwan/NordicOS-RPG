using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseWeapon : BaseStatItem {

    public enum WeaponTypes {
        AXE,
        SWORD,
        HAMMER,
        STAFF,
        BOW,
        SHIELD,
        SCEPTER
    }

    private WeaponTypes weaponType;

    public WeaponTypes WeaponType
    {
        get { return weaponType; }
        set { weaponType = value; }
    }


}
