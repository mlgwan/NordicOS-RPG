using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemInBattle : BaseAttack {

    public Inventory.InventoryItem item;

    public ItemInBattle() {

    }

    public ItemInBattle(Inventory.InventoryItem item){
        this.item = item;
        attackName = item.item.name;
        attackDescription = item.item.description;
        attackDamage = 0;
        critChance = 0;
        hitChance = 100;
        attackCost = 0;
        isAOE = false;
        isMagic = false;

        lowerRandomBound = 0;
        upperRandomBound = 0;

        applicationChance = 100;

        statusEffectToApply = StatusEffects.STUN;

        poisonDamage = 0;
        burnDamage = 0;
        frostBurnDamage = 0;
        bleedDamage = 0;

        scalesWith = ScalesWith.NOTHING;
    }
}
