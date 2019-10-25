using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using TMPro;

public class InventoryManager : MonoBehaviour {

    public enum InventoryStates {
        DISABLED,
        OPTIONS,
        EQUIPMENT,
        INVENTORY,
        INVENTORY_USE,
        INVENTORY_SORT
    }

    public InventoryStates currentState;

    private bool inSubMenu;
    private bool isInspecting;

    public GameObject finger;
    public GameObject blinkingFinger;

    [Header("Panels")]
    public GameObject equipmentPanel;
    public GameObject optionsPanel;
    public GameObject inventoryPanel;
    public GameObject itemOptionsPopupPanel;
    public GameObject itemDescriptionPanel;

    private GameObject tempPopup;
    private string tempItemName;

    public List<GameObject> optionsList = new List<GameObject>();
    public List<GameObject> inventoryOptionsList = new List<GameObject>();
    public List<GameObject> equipmentOptionsList = new List<GameObject>();
    public List<Transform> itemOptionsPopupList = new List<Transform>();

    private int currentOption;
    private int currentInventoryOption;
    private int currentItemOption;

    private int selectedItem = -1; //placeholder number 

    [Header("Scrolling")]
    public ScrollRect itemScrollRect;
    private int fingerCounter;
    public int maxAmountOfItemsToDisplay = 5;

    private void Update()
    {
        switch (currentState) {
            case InventoryStates.DISABLED:
                break;

            case InventoryStates.OPTIONS:

                OptionsSelect();

                break;

            case InventoryStates.EQUIPMENT:

                EquipmentSelect();

                break;

            case InventoryStates.INVENTORY:
                InventorySelect();
                break;

            case InventoryStates.INVENTORY_USE:
                UseAndOrder();
                break;
            case InventoryStates.INVENTORY_SORT:
                
                break;


        }
    }

    void OptionsSelect() {
        blinkingFinger.SetActive(false);
        finger.transform.position = optionsList[currentOption].transform.position;

        inventoryPanel.SetActive(false);
        equipmentPanel.SetActive(false);
        optionsPanel.SetActive(true);

        if (Input.GetKeyDown(KeyCode.W) && currentOption > 0) {
            currentOption--;
        }

        else if (Input.GetKeyDown(KeyCode.S) && currentOption < optionsList.Count - 1){
            currentOption++;
        }

        if (Input.GetKeyDown(KeyCode.Return) && currentOption == 0)
        {
            currentState = InventoryStates.EQUIPMENT;
            // equipment panel
        }

        else if (Input.GetKeyDown(KeyCode.Return) && currentOption == 1) {
            currentState = InventoryStates.INVENTORY;
            // inventory panel
        }



    }

    void InventorySelect() {

        blinkingFinger.SetActive(false);
        inventoryPanel.SetActive(true);
        equipmentPanel.SetActive(false);
        optionsPanel.SetActive(false);

        if (Input.GetKeyDown(KeyCode.A) && currentInventoryOption > 0)
        {
            currentInventoryOption--;
        }

        else if (Input.GetKeyDown(KeyCode.D) && currentInventoryOption < inventoryOptionsList.Count - 1)
        {
            currentInventoryOption++;
        }

        if (Input.GetKeyDown(KeyCode.Escape)) {
            currentInventoryOption = 0;
            currentState = InventoryStates.OPTIONS;
        }


        if (Input.GetKeyDown(KeyCode.Return) && currentInventoryOption == 0)
        {
            currentState = InventoryStates.INVENTORY_USE;
            InventoryUI.instance.descriptionTextField.text = Inventory.instance.inventoryList[currentInventoryOption].item.description;
            fingerCounter = 1;
        }
        else if (Input.GetKeyDown(KeyCode.Return) && currentInventoryOption == 1)
        {
            //sort items
            //Inventory.instance.inventoryList = Inventory.instance.inventoryList.OrderBy(x => x.item.itemID).ToList(); // By ID
            Inventory.instance.inventoryList = Inventory.instance.inventoryList.OrderBy(x => x.item.type).ThenBy(x=>x.item.name).ToList(); // By Type first and then by name
            //recreate
            InventoryUI.instance.RecreateList();


        }

        finger.transform.position = inventoryOptionsList[currentInventoryOption].transform.position;

    }

    void EquipmentSelect()
    {

        blinkingFinger.SetActive(false);
        inventoryPanel.SetActive(false);
        equipmentPanel.SetActive(true);
        optionsPanel.SetActive(false);

        if (Input.GetKeyDown(KeyCode.W) && currentInventoryOption > 0)
        {
            currentInventoryOption--;
        }

        else if (Input.GetKeyDown(KeyCode.S) && currentInventoryOption < equipmentOptionsList.Count - 1)
        {
            currentInventoryOption++;
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            currentInventoryOption = 0;
            currentState = InventoryStates.OPTIONS;
        }


        if (Input.GetKeyDown(KeyCode.Return) && currentInventoryOption == 0)
        {
            currentState = InventoryStates.INVENTORY_USE;
            InventoryUI.instance.descriptionTextField.text = Inventory.instance.inventoryList[currentInventoryOption].item.description;
            fingerCounter = 1;
        }
        else if (Input.GetKeyDown(KeyCode.Return) && currentInventoryOption == 1)
        {
            //sort items
            //Inventory.instance.inventoryList = Inventory.instance.inventoryList.OrderBy(x => x.item.itemID).ToList(); // By ID
            Inventory.instance.inventoryList = Inventory.instance.inventoryList.OrderBy(x => x.item.type).ThenBy(x => x.item.name).ToList(); // By Type first and then by name
            //recreate
            InventoryUI.instance.RecreateList();


        }

        finger.transform.position = equipmentOptionsList[currentInventoryOption].transform.position;

    }


    void UseAndOrder() {

        if (Input.GetKeyDown(KeyCode.Escape) && !inSubMenu)
        {
            selectedItem = -1;
            currentInventoryOption = 0;
            currentState = InventoryStates.INVENTORY;
        }

        if(InventoryUI.instance.itemHolderList.Count <= 0)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.W) && currentInventoryOption > 0 && !inSubMenu) {
            currentInventoryOption--;

            if (fingerCounter > 1)
            {
                fingerCounter--;
            }
            else if (fingerCounter == 1) {
                itemScrollRect.verticalNormalizedPosition += (float)1 / (InventoryUI.instance.itemHolderList.Count - maxAmountOfItemsToDisplay);
            }

            InventoryUI.instance.descriptionTextField.text = Inventory.instance.inventoryList[currentInventoryOption].item.description;

        }

        else if (Input.GetKeyDown(KeyCode.S) && currentInventoryOption < InventoryUI.instance.itemHolderList.Count - 1 && !inSubMenu) {
            currentInventoryOption++;

            if (fingerCounter < maxAmountOfItemsToDisplay)
            {
                fingerCounter++;
            }
            else if (fingerCounter == maxAmountOfItemsToDisplay)
            {
                itemScrollRect.verticalNormalizedPosition -= (float)1 / (InventoryUI.instance.itemHolderList.Count - maxAmountOfItemsToDisplay);
            }

            InventoryUI.instance.descriptionTextField.text = Inventory.instance.inventoryList[currentInventoryOption].item.description;

        }



        if (!inSubMenu)
        {
            //Use

            if (Input.GetKeyDown(KeyCode.Return))
            {

                tempPopup = Instantiate(itemOptionsPopupPanel, InventoryUI.instance.itemHolderList[currentInventoryOption].transform.position + new Vector3(250, 0, 0), Quaternion.identity, GameObject.Find("ItemOptionsPopupPanelHolder").transform.Find("Holder")) as GameObject;
                foreach (Transform point in tempPopup.transform.Find("SelectPoints").GetComponentInChildren<Transform>())
                {
                    itemOptionsPopupList.Add(point);
                }
                tempItemName = Inventory.instance.inventoryList[currentInventoryOption].item.name;
                inSubMenu = true;
            }

            //Order
            if (Input.GetKeyDown(KeyCode.Backspace) && selectedItem == -1)
            {
                blinkingFinger.SetActive(true);
                blinkingFinger.transform.position = finger.transform.position;
                selectedItem = currentInventoryOption;
            }

            else if (Input.GetKeyDown(KeyCode.Backspace) && (selectedItem != -1) && (selectedItem != currentInventoryOption))
            {
                //Rearrange items
                Inventory.instance.SwapItems(selectedItem, currentInventoryOption);
                selectedItem = -1;
                blinkingFinger.SetActive(false);
                InventoryUI.instance.descriptionTextField.text = Inventory.instance.inventoryList[currentInventoryOption].item.description;
            }

            if (blinkingFinger.activeInHierarchy)
            {
                blinkingFinger.transform.position = InventoryUI.instance.itemHolderList[selectedItem].GetComponent<ItemHolder>().itemSelectPoint.transform.position;

            }

            finger.transform.position = InventoryUI.instance.itemHolderList[currentInventoryOption].GetComponent<ItemHolder>().itemSelectPoint.transform.position;

        }

        else if (inSubMenu && !isInspecting)
        {

            finger.transform.position = itemOptionsPopupList[currentItemOption].position;

            if (Input.GetKeyDown(KeyCode.Return) && currentItemOption == 0) // USE
            {
                Inventory.instance.inventoryList[currentInventoryOption].item.UseItem(GameManager.instance.ulf);
                InventoryUI.instance.RecreateList();
            }

            if (Input.GetKeyDown(KeyCode.Return) && currentItemOption == 1) // INSPECT
            {
                itemDescriptionPanel.SetActive(true);
                finger.SetActive(false);
                itemDescriptionPanel.transform.Find("DescriptionText").GetComponent<TextMeshProUGUI>().text = Inventory.instance.inventoryList[currentInventoryOption].item.description;
                isInspecting = true;
            }

            if (Input.GetKeyDown(KeyCode.Return) && currentItemOption == 2) // DELETE
            {
                Inventory.instance.RemoveItem(Inventory.instance.inventoryList[currentInventoryOption].item);
                InventoryUI.instance.RecreateList();
            }

            if (Input.GetKeyDown(KeyCode.Escape) || tempItemName != Inventory.instance.inventoryList[currentInventoryOption].item.name || (Input.GetKeyDown(KeyCode.Return) && currentItemOption == 3)) //Escape or one runs out of item or CANCEL
            {
                currentItemOption = 0;
                itemOptionsPopupList.Clear();
                Destroy(tempPopup);
                inSubMenu = false;
            }

            if (Input.GetKeyDown(KeyCode.W) && currentItemOption > 0)
            {
                currentItemOption--;
            }

            else if (Input.GetKeyDown(KeyCode.S) && currentItemOption < itemOptionsPopupList.Count - 1)
            {
                currentItemOption++;
            }


        }

        else if (isInspecting) {
            if (Input.anyKeyDown) {
                itemDescriptionPanel.SetActive(false);
                finger.SetActive(true);
                isInspecting = false;
            }
        }
     
    }
}
