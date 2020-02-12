using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemButton : MonoBehaviour
{

    public Inventory.InventoryItem itemToUse;

    public void UseItem()
    {
        GameObject.Find("BattleManager").GetComponent<BattleStateMachine>().Input6(itemToUse);
    }
}