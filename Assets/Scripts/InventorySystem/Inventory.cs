﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour {

    public static Inventory instance;

    public int maxAmount = 99;

    [System.Serializable]
    public class InventoryItem {
        public Item item;
        public int currentAmount;
    }
     public List<InventoryItem> inventoryList = new List<InventoryItem>();

    

    private void Awake()
    {
        instance = this;
    }

    public void AddItem(Item itemToAdd) {
        InventoryItem invItem = new InventoryItem();
        invItem.item = itemToAdd;
        bool stackIsFull = false;

        for (int i = 0; i < inventoryList.Count; i++)
        {
            if (inventoryList[i].item == itemToAdd && inventoryList[i].currentAmount < maxAmount)
            {
                inventoryList[i].currentAmount++;
                return;
            }
            else {
                stackIsFull = true;
            }
        }
        if (!stackIsFull) {
            invItem.currentAmount++;
            inventoryList.Add(invItem);
        }
 
    }

    public void RemoveItem(Item itemToRemove) {
        for (int i = 0; i < inventoryList.Count; i++)
        {
            if (inventoryList[i].item == itemToRemove && inventoryList[i].currentAmount > 0)
            {
                inventoryList[i].currentAmount--;

                if (inventoryList[i].currentAmount == 0) {
                    inventoryList.Remove(inventoryList[i]);
                    return;
                }
            }
        }
    }

    public void SwapItems(int item1, int item2) {
        InventoryItem tempItem = inventoryList[item1];
        inventoryList[item1] = inventoryList[item2];
        inventoryList[item2] = tempItem;
        InventoryUI.instance.RecreateList();
    }
}