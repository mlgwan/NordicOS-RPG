using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Equipment")]
public class Equipment : Item
{


    public int bonusHP;
    public int bonusMP;
    public int damage;
    public int armor;
    public int bonusSTR;
    public int bonusDEX;
    public int bonusINT;
    public int bonusSPD;

    public bool isEquipped;
    
    public override void UseItem(GameObject user)
    {
        base.UseItem(user);

        // equip item
        if (!isEquipped)
        {
            user.GetComponent<PlayerStateMachine>().player.curHP += bonusHP;
            user.GetComponent<PlayerStateMachine>().player.curMP += bonusMP;
            user.GetComponent<PlayerStateMachine>().player.maxHP += bonusHP;
            user.GetComponent<PlayerStateMachine>().player.maxMP += bonusMP;
            user.GetComponent<PlayerStateMachine>().player.maxATK += damage;
            user.GetComponent<PlayerStateMachine>().player.curATK += damage;
            user.GetComponent<PlayerStateMachine>().player.maxDEF += armor;
            user.GetComponent<PlayerStateMachine>().player.curDEF += armor;
            user.GetComponent<PlayerStateMachine>().player.maxSTR += bonusSTR;
            user.GetComponent<PlayerStateMachine>().player.curSTR += bonusSTR;
            user.GetComponent<PlayerStateMachine>().player.maxDEX += bonusDEX;
            user.GetComponent<PlayerStateMachine>().player.curDEX += bonusDEX;
            user.GetComponent<PlayerStateMachine>().player.maxINT += bonusINT;
            user.GetComponent<PlayerStateMachine>().player.curINT += bonusINT;
            user.GetComponent<PlayerStateMachine>().player.maxSPD += bonusSPD;
            user.GetComponent<PlayerStateMachine>().player.curSPD += bonusSPD;

            Inventory.instance.RemoveItem(this);
        }

        // remove item from inventory temporarily

        else if (isEquipped) {
            user.GetComponent<PlayerStateMachine>().player.maxHP -= bonusHP;
            user.GetComponent<PlayerStateMachine>().player.maxMP -= bonusMP;
            user.GetComponent<PlayerStateMachine>().player.maxATK -= damage;
            user.GetComponent<PlayerStateMachine>().player.curATK -= damage;
            user.GetComponent<PlayerStateMachine>().player.maxDEF -= armor;
            user.GetComponent<PlayerStateMachine>().player.curDEF -= armor;
            user.GetComponent<PlayerStateMachine>().player.maxSTR -= bonusSTR;
            user.GetComponent<PlayerStateMachine>().player.curSTR -= bonusSTR;
            user.GetComponent<PlayerStateMachine>().player.maxDEX -= bonusDEX;
            user.GetComponent<PlayerStateMachine>().player.curDEX -= bonusDEX;
            user.GetComponent<PlayerStateMachine>().player.maxINT -= bonusINT;
            user.GetComponent<PlayerStateMachine>().player.curINT -= bonusINT;
            user.GetComponent<PlayerStateMachine>().player.maxSPD -= bonusSPD;
            user.GetComponent<PlayerStateMachine>().player.curSPD -= bonusSPD;
            Inventory.instance.AddItem(this);
        }


        isEquipped = !isEquipped;

   }
}
