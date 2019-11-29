using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Consumable")]
public class Consumable : Item {

    public int hpAmount;
    public int mpAmount;

    public int bonusHP;
    public int bonusMP;
    public int bonusATK;
    public int bonusDEF;
    public int bonusSTR;
    public int bonusDEX;
    public int bonusINT;
    public int bonusSPD;

    public int durationAmount; //how many battles/turns, used turns for now
    public float durationTime; //in seconds

    public override void UseItem(GameObject user)
    {
        base.UseItem(user);

        bool isUsable = false;

        //if (user.GetComponent<PlayerStateMachine>().player)

        if (isUsable)
        {
            // remove item from inventory
            Inventory.instance.RemoveItem(this);
            //add stats to selected character
            user.GetComponent<PlayerStateMachine>().player.curHP += hpAmount;
            user.GetComponent<PlayerStateMachine>().player.curMP += mpAmount;
            user.GetComponent<PlayerStateMachine>().player.maxHP += bonusHP;
            user.GetComponent<PlayerStateMachine>().player.maxMP += bonusMP;

            user.GetComponent<PlayerStateMachine>().player.maxATK += bonusATK;
            user.GetComponent<PlayerStateMachine>().player.curATK += bonusATK;
            user.GetComponent<PlayerStateMachine>().player.maxDEF += bonusDEF;
            user.GetComponent<PlayerStateMachine>().player.curDEF += bonusDEF;
            user.GetComponent<PlayerStateMachine>().player.maxSTR += bonusSTR;
            user.GetComponent<PlayerStateMachine>().player.curSTR += bonusSTR;
            user.GetComponent<PlayerStateMachine>().player.maxDEX += bonusDEX;
            user.GetComponent<PlayerStateMachine>().player.curDEX += bonusDEX;
            user.GetComponent<PlayerStateMachine>().player.maxINT += bonusINT;
            user.GetComponent<PlayerStateMachine>().player.curINT += bonusINT;
            user.GetComponent<PlayerStateMachine>().player.maxSPD += bonusSPD;
            user.GetComponent<PlayerStateMachine>().player.curSPD += bonusSPD;

            EndItemEffect(durationAmount, user); //ends the items effect after a given amount of time (might be better to do this after a certain amount of battles)

        }

        else {
            if (!GameObject.Find("MenuCanvas").GetComponent<InventoryManager>().isInspecting) {
                GameObject.Find("MenuCanvas").GetComponent<DialogueHolder>().DisplayBox();
                GameObject.Find("MenuCanvas").GetComponent<InventoryManager>().isInspecting = true;
            }
           
        }

        

      
    }

    void EndItemEffect(int turns, GameObject user) {
        Debug.Log("effect over");
        user.GetComponent<PlayerStateMachine>().player.maxHP -= bonusHP;
        user.GetComponent<PlayerStateMachine>().player.maxMP -= bonusMP;
        user.GetComponent<PlayerStateMachine>().player.maxATK -= bonusATK;
        user.GetComponent<PlayerStateMachine>().player.curATK -= bonusATK;
        user.GetComponent<PlayerStateMachine>().player.maxDEF -= bonusDEF;
        user.GetComponent<PlayerStateMachine>().player.curDEF -= bonusDEF;
        user.GetComponent<PlayerStateMachine>().player.maxSTR -= bonusSTR;
        user.GetComponent<PlayerStateMachine>().player.curSTR -= bonusSTR;
        user.GetComponent<PlayerStateMachine>().player.maxDEX -= bonusDEX;
        user.GetComponent<PlayerStateMachine>().player.curDEX -= bonusDEX;
        user.GetComponent<PlayerStateMachine>().player.maxINT -= bonusINT;
        user.GetComponent<PlayerStateMachine>().player.curINT -= bonusINT;
        user.GetComponent<PlayerStateMachine>().player.maxSPD -= bonusSPD;
        user.GetComponent<PlayerStateMachine>().player.curSPD -= bonusSPD;

    }
}
