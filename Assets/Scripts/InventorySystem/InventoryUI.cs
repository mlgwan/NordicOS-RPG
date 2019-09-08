using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryUI : MonoBehaviour {

    public static InventoryUI instance;

    Inventory inventory;

    public List<GameObject> itemHolderList = new List<GameObject>();
    public GameObject itemHolderPrefab;
    public Transform itemSpacer;

    public TextMeshProUGUI descriptionTextField;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        inventory = Inventory.instance;
        FillList();
    }

    void FillList() {
        for (int i = 0; i < inventory.inventoryList.Count; i++)
        {
            GameObject newItemHolder = Instantiate(itemHolderPrefab, itemSpacer, false) as GameObject;
            ItemHolder holderScript = newItemHolder.GetComponent<ItemHolder>();
            holderScript.icon.sprite = inventory.inventoryList[i].item.icon;
            holderScript.amountText.text = inventory.inventoryList[i].currentAmount.ToString();
            holderScript.nameOfItemText.text = inventory.inventoryList[i].item.name;
            holderScript.typeText.text = inventory.inventoryList[i].item.type.ToString();

            itemHolderList.Add(newItemHolder);
        }
    }

    public void RecreateList() {
        foreach (GameObject itemholder in itemHolderList) {
            Destroy(itemholder);
        }
        itemHolderList.Clear();
        FillList();
    }
}
