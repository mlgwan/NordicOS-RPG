using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Equipment")]
public class Equipment : Item
{
    public int damage;
    public int armor;
    
    public override void UseItem()
    {
        base.UseItem();

        // equip item

        // remove item from inventory temporarily

   }
}
