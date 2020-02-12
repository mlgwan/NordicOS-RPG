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

    public int restorationType; //used to determine usability: 0 = HP, 1 = MP, 2 = HP and MP ...

    public override void UseItem(GameObject user)
    {
        base.UseItem(user);

        if (isUsable(this, user.GetComponent<PlayerStateMachine>().player))
        {
            // remove item from inventory
            Inventory.instance.RemoveItem(this);
            //add stats to selected character
            int restoredHP = 0; // these two are for the dialogue text
            int restoredMP = 0;
            if (user.GetComponent<PlayerStateMachine>().player.curHP + hpAmount > user.GetComponent<PlayerStateMachine>().player.maxHP)
            {
                restoredHP = user.GetComponent<PlayerStateMachine>().player.maxHP - user.GetComponent<PlayerStateMachine>().player.curHP;
                user.GetComponent<PlayerStateMachine>().player.curHP = user.GetComponent<PlayerStateMachine>().player.maxHP;
            }
            else {
                restoredHP = hpAmount;
                user.GetComponent<PlayerStateMachine>().player.curHP += hpAmount;
            }

            if (user.GetComponent<PlayerStateMachine>().player.curMP + mpAmount > user.GetComponent<PlayerStateMachine>().player.maxMP)
            {
                restoredMP = user.GetComponent<PlayerStateMachine>().player.maxMP - user.GetComponent<PlayerStateMachine>().player.curMP;
                user.GetComponent<PlayerStateMachine>().player.curMP = user.GetComponent<PlayerStateMachine>().player.maxMP;
            }
            else
            {
                restoredMP = mpAmount;
                user.GetComponent<PlayerStateMachine>().player.curMP += mpAmount;
            }
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

            if (restorationType == 2)
            {
                GameObject.FindWithTag("MenuCanvas").GetComponent<DialogueHolder>().dialogueLines = new string[] { "Restored " + restoredHP + " HP and " + restoredMP + " MP." };
            }
            else if (restorationType == 0)
            {
                GameObject.FindWithTag("MenuCanvas").GetComponent<DialogueHolder>().dialogueLines = new string[] { "Restored " + restoredHP + " HP." };
            }
            else if (restorationType == 1) {
                GameObject.FindWithTag("MenuCanvas").GetComponent<DialogueHolder>().dialogueLines = new string[] { "Restored " + restoredMP + " MP." };
            }
            GameObject.FindWithTag("MenuCanvas").GetComponent<DialogueHolder>().speakerSprites = new Sprite[] { SpeakerSprites.instance.ulfNormal };
            GameObject.FindWithTag("MenuCanvas").GetComponent<DialogueHolder>().DisplayBox();
        }

        else
        {
            
            GameObject.FindWithTag("MenuCanvas").GetComponent<DialogueHolder>().dialogueLines = new string[] { "I can't use this right now..." };
            GameObject.FindWithTag("MenuCanvas").GetComponent<DialogueHolder>().speakerSprites = new Sprite[] { SpeakerSprites.instance.ulfAngry };
            GameObject.FindWithTag("MenuCanvas").GetComponent<DialogueHolder>().DisplayBox();                    
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

    public bool isUsable(Consumable item, BasePlayer player) {

        switch (restorationType) {
            case (0):
                if (player.curHP == player.maxHP)
                {
                    return false;
                }
                else {
                    break;
                }
                
            case (1):
                if (player.curMP == player.maxMP)
                {
                    return false;
                }
                else
                {
                    break;
                }
            case (2):
                if (player.curMP == player.maxMP && player.curHP == player.maxHP)
                {
                    return false;
                }
                else
                {
                    break;
                }
            default:
                Debug.Log("Invalid restorationType");
                break;
        }
        return true;
    }
}
