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
    public BaseAttack bonusAbility;

    public bool isEquipped;


    private Equipment slotToFill;
    
    public override void UseItem(GameObject user)
    {
        base.UseItem(user);

        // determine type
        switch (type)
        {
            case (ItemTypes.WEAPON):
                slotToFill = user.GetComponent<PlayerStateMachine>().player.weaponSlot;
                break;
            case (ItemTypes.HELMET):
                slotToFill = user.GetComponent<PlayerStateMachine>().player.helmetSlot;
                break;
                case (ItemTypes.ARMOR):
                slotToFill = user.GetComponent<PlayerStateMachine>().player.armorSlot;
                break;
            case (ItemTypes.BOOTS):
                slotToFill = user.GetComponent<PlayerStateMachine>().player.bootsSlot ;
                break;
            default:
                break;
        }
        // equip item
        if (slotToFill == null)
        {
            switch (type)
            {
                case (ItemTypes.WEAPON):
                    user.GetComponent<PlayerStateMachine>().player.weaponSlot = this;
                    break;
                case (ItemTypes.HELMET):
                    user.GetComponent<PlayerStateMachine>().player.helmetSlot = this;
                    break;
                case (ItemTypes.ARMOR):
                    user.GetComponent<PlayerStateMachine>().player.armorSlot = this;
                    break;
                case (ItemTypes.BOOTS):
                    user.GetComponent<PlayerStateMachine>().player.bootsSlot = this;
                    break;
                default:
                    break;
            }
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
            if (bonusAbility != null)
            {
                user.GetComponent<PlayerStateMachine>().player.abilities.Add(bonusAbility);
            }
            Inventory.instance.RemoveItem(this);
            isEquipped = true;

            GameObject.FindWithTag("MenuCanvas").GetComponent<DialogueHolder>().dialogueLines = new string[] { "Equipped the " + name + "."};
            GameObject.FindWithTag("MenuCanvas").GetComponent<DialogueHolder>().speakerSprites = new Sprite[] { SpeakerSprites.instance.ulfNormal };
            GameObject.FindWithTag("MenuCanvas").GetComponent<DialogueHolder>().DisplayBox();
    }

        // remove item from inventory temporarily

        else if (isEquipped)
        {
            switch (type)
            {
                case (ItemTypes.WEAPON):
                    user.GetComponent<PlayerStateMachine>().player.weaponSlot = null;
                    break;
                case (ItemTypes.HELMET):
                    user.GetComponent<PlayerStateMachine>().player.helmetSlot = null;
                    break;
                case (ItemTypes.ARMOR):
                    user.GetComponent<PlayerStateMachine>().player.armorSlot = null;
                    break;
                case (ItemTypes.BOOTS):
                    user.GetComponent<PlayerStateMachine>().player.bootsSlot = null;
                    break;
                default:
                    break;
            }
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
            if (bonusAbility != null)
            {
                user.GetComponent<PlayerStateMachine>().player.abilities.Remove(bonusAbility);
            }

            Inventory.instance.AddItem(this);
            isEquipped = false;

            GameObject.FindWithTag("MenuCanvas").GetComponent<DialogueHolder>().dialogueLines = new string[] { "Unequipped the " + name + "." };
            GameObject.FindWithTag("MenuCanvas").GetComponent<DialogueHolder>().speakerSprites = new Sprite[] { SpeakerSprites.instance.ulfNormal };
            GameObject.FindWithTag("MenuCanvas").GetComponent<DialogueHolder>().DisplayBox();
        }


        else if (slotToFill != null && slotToFill != this) { // if a different item is equipped than this one

            //remove the old item
            user.GetComponent<PlayerStateMachine>().player.maxHP -= slotToFill.bonusHP;
            user.GetComponent<PlayerStateMachine>().player.maxMP -= slotToFill.bonusMP;
            user.GetComponent<PlayerStateMachine>().player.maxATK -= slotToFill.damage;
            user.GetComponent<PlayerStateMachine>().player.curATK -= slotToFill.damage;
            user.GetComponent<PlayerStateMachine>().player.maxDEF -= slotToFill.armor;
            user.GetComponent<PlayerStateMachine>().player.curDEF -= slotToFill.armor;
            user.GetComponent<PlayerStateMachine>().player.maxSTR -= slotToFill.bonusSTR;
            user.GetComponent<PlayerStateMachine>().player.curSTR -= slotToFill.bonusSTR;
            user.GetComponent<PlayerStateMachine>().player.maxDEX -= slotToFill.bonusDEX;
            user.GetComponent<PlayerStateMachine>().player.curDEX -= slotToFill.bonusDEX;
            user.GetComponent<PlayerStateMachine>().player.maxINT -= slotToFill.bonusINT;
            user.GetComponent<PlayerStateMachine>().player.curINT -= slotToFill.bonusINT;
            user.GetComponent<PlayerStateMachine>().player.maxSPD -= slotToFill.bonusSPD;
            user.GetComponent<PlayerStateMachine>().player.curSPD -= slotToFill.bonusSPD;
            if (slotToFill.bonusAbility != null) {
                user.GetComponent<PlayerStateMachine>().player.abilities.Remove(slotToFill.bonusAbility);
            }
            Inventory.instance.AddItem(slotToFill);
            slotToFill.isEquipped = false;

            switch (slotToFill.type)
            {
                case (ItemTypes.WEAPON):
                    user.GetComponent<PlayerStateMachine>().player.weaponSlot = null;
                    break;
                case (ItemTypes.HELMET):
                    user.GetComponent<PlayerStateMachine>().player.helmetSlot = null;
                    break;
                case (ItemTypes.ARMOR):
                    user.GetComponent<PlayerStateMachine>().player.armorSlot = null;
                    break;
                case (ItemTypes.BOOTS):
                    user.GetComponent<PlayerStateMachine>().player.bootsSlot = null;
                    break;
                default:
                    break;
            }
            //equip the new one
            UseItem(user);
        }
        user.GetComponent<PlayerStateMachine>().player.resetEquipmentSlots();
   }
}
