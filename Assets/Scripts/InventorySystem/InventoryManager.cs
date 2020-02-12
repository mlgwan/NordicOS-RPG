using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using TMPro;

public class InventoryManager : MonoBehaviour
{

    ControlScript controls;

    public enum InventoryStates
    {
        DISABLED,
        OPTIONS,
        EQUIPMENT,
        EQUIPMENT_SLOTS,
        EQUIPMENT_USE,
        INVENTORY,
        INVENTORY_USE,
        INVENTORY_SORT
    }

    public InventoryStates currentState;

    public bool inSubMenu;
    public bool inSubSubMenu; //used for equipping items in the equip screen, terrible naming i know
    public bool isInspecting;

    public GameObject finger;
    public GameObject blinkingFinger;

    [Header("Panels")]
    public GameObject equipmentPanel;
    public GameObject optionsPanel;
    public GameObject inventoryPanel;
    public GameObject itemOptionsPopupPanel;
    public GameObject EquipmentSlotsOptionsPopupPanel;
    public GameObject itemDescriptionPanel;
    public GameObject equipmentDescriptionPanel;
    public GameObject EquippableItemsPanel;

    public List<GameObject> equipItemHolderList = new List<GameObject>();
    public GameObject equipItemHolderPrefab;
    public Transform equipItemSpacer;

    private GameObject tempPopupItem;
    private GameObject tempPopupEquip;
    private string tempItemName;

    public List<GameObject> optionsList = new List<GameObject>();
    public List<GameObject> inventoryOptionsList = new List<GameObject>();
    public List<GameObject> equipmentOptionsList = new List<GameObject>();
    public List<GameObject> equipmentSelectList = new List<GameObject>();
    public List<Transform> itemOptionsPopupList = new List<Transform>();
    public List<Transform> EquipmentSlotsOptionssPopupList = new List<Transform>();

    private List<Equipment> tempEquipInventory;

    private int currentOption;
    private int currentInventoryOption;
    private int currentItemOption;

    private int currentSelectedEquipmentSLot;

    private int selectedItem = -1; //placeholder number 

    [Header("Scrolling")]
    public ScrollRect itemScrollRect;
    public ScrollRect equipItemScrollRect;
    private int fingerCounter;
    public int maxAmountOfItemsToDisplay = 5;


    private void Start()
    {
        controls = ControlScript.instance;
    }

    private void Update()
    {
        if (!GameObject.FindWithTag("MenuCanvas").transform.Find("DialogueManager").GetComponent<DialogueManager>().dialogueActive)
        {
            switch (currentState)
            {
                case InventoryStates.DISABLED:
                    break;

                case InventoryStates.OPTIONS:

                    OptionsSelect();

                    break;

                case InventoryStates.EQUIPMENT:

                    EquipmentSelect();

                    break;
                case InventoryStates.EQUIPMENT_SLOTS:

                    EquipmentSlotsSelect();

                    break;

                case InventoryStates.EQUIPMENT_USE:

                    EquipmentUse();

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
    }
    void OptionsSelect()
    {
        blinkingFinger.SetActive(false);
        finger.transform.position = optionsList[currentOption].transform.position;
        finger.SetActive(true);
        inventoryPanel.SetActive(false);
        equipmentPanel.SetActive(false);
        optionsPanel.SetActive(true);

        if (Input.GetKeyDown(controls.upButton) && currentOption > 0)
        {
            currentOption--;
        }

        else if (Input.GetKeyDown(controls.downButton) && currentOption < optionsList.Count - 1)
        {
            currentOption++;
        }

        if (Input.GetKeyDown(controls.acceptButton) && currentOption == 0)
        {
            currentState = InventoryStates.EQUIPMENT;
            EquipmentUI.instance.UpdateResourceBars(currentOption);
            // equipment panel
        }

        else if (Input.GetKeyDown(controls.acceptButton) && currentOption == 1)
        {
            currentState = InventoryStates.INVENTORY;
            InventoryUI.instance.RecreateList();
            // inventory panel
        }
        else if (Input.GetKeyDown(controls.acceptButton) && currentOption == 2)
        {
            Application.Quit();
        }

        else if (Input.GetKeyDown(controls.acceptButton) && currentOption == 3)
        {
            currentOption = 0;
            GameObject.FindWithTag("Player").transform.Find("PlayerCharacter").GetComponent<Movement>().closeMenu();
        }



    }

    void InventorySelect()
    {

        blinkingFinger.SetActive(false);
        inventoryPanel.SetActive(true);
        equipmentPanel.SetActive(false);
        optionsPanel.SetActive(false);

        if (Input.GetKeyDown(controls.leftButton) && currentInventoryOption > 0)
        {
            currentInventoryOption--;
        }

        else if (Input.GetKeyDown(controls.rightButton) && currentInventoryOption < inventoryOptionsList.Count - 1)
        {
            currentInventoryOption++;
        }

        if (Input.GetKeyUp(controls.escapeButton))
        {
            currentInventoryOption = 0;
            currentState = InventoryStates.OPTIONS;
            finger.SetActive(false);
        }


        if (Input.GetKeyDown(controls.acceptButton) && currentInventoryOption == 0)
        {
            currentState = InventoryStates.INVENTORY_USE;
            InventoryUI.instance.descriptionTextField.text = Inventory.instance.inventoryList[currentInventoryOption].item.description;
            fingerCounter = 1;
        }
        else if (Input.GetKeyDown(controls.acceptButton) && currentInventoryOption == 1)
        {
            //sort items
            //Inventory.instance.inventoryList = Inventory.instance.inventoryList.OrderBy(x => x.item.itemID).ToList(); // By ID
            Inventory.instance.inventoryList = Inventory.instance.inventoryList.OrderBy(x => x.item.type).ThenBy(x => x.item.name).ToList(); // By Type first and then by name
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
        EquippableItemsPanel.SetActive(false);

        if (Input.GetKeyDown(controls.upButton) && currentInventoryOption > 0)
        {
            currentInventoryOption--;
            EquipmentUI.instance.selectedHeroIndex--;
            EquipmentUI.instance.updateInformationBox(currentInventoryOption);
        }

        else if (Input.GetKeyDown(controls.downButton) && currentInventoryOption < equipmentOptionsList.Count - 1)
        {
            currentInventoryOption++;
            EquipmentUI.instance.selectedHeroIndex++;
            EquipmentUI.instance.updateInformationBox(currentInventoryOption);
        }


        if (Input.GetKeyUp(controls.escapeButton))
        {
           
            currentInventoryOption = 0;
            EquipmentUI.instance.selectedHeroIndex = 0;
            currentState = InventoryStates.OPTIONS;
            finger.SetActive(false);
        }


        if (Input.GetKeyDown(controls.acceptButton) && currentInventoryOption == 0)
        {
            currentState = InventoryStates.EQUIPMENT_SLOTS;
            fingerCounter = 1;
        }

        finger.transform.position = equipmentOptionsList[currentInventoryOption].transform.position;

    }
    void EquipmentSlotsSelect()
    {

        finger.transform.position = equipmentSelectList[currentItemOption].transform.position;
        if (Input.GetKeyDown(controls.acceptButton) && currentItemOption == 0) // Weapon
        {
            currentSelectedEquipmentSLot = 0;
            currentState = InventoryStates.EQUIPMENT_USE;
        }

        if (Input.GetKeyDown(controls.acceptButton) && currentItemOption == 1) // Helmet
        {
            currentSelectedEquipmentSLot = 1;
            currentState = InventoryStates.EQUIPMENT_USE;
        }

        if (Input.GetKeyDown(controls.acceptButton) && currentItemOption == 2) // Armor
        {
            currentSelectedEquipmentSLot = 2;
            currentState = InventoryStates.EQUIPMENT_USE;
        }

        if (Input.GetKeyDown(controls.acceptButton) && currentItemOption == 3) // Boots
        {
            currentSelectedEquipmentSLot = 3;
            currentState = InventoryStates.EQUIPMENT_USE;
        }

        if (Input.GetKeyUp(controls.escapeButton))
        {
            GameObject.Find("PlayerCharacter").GetComponent<Movement>().canMove = false;
            currentItemOption = 0;
            currentState = InventoryStates.EQUIPMENT;
        }

        if (Input.GetKeyDown(controls.upButton) && currentItemOption > 0)
        {
            currentItemOption--;
        }

        else if (Input.GetKeyDown(controls.downButton) && currentItemOption < equipmentSelectList.Count - 1)
        {
            currentItemOption++;
        }

    }

    void UseAndOrder()
    {

        if (currentInventoryOption + 1 > Inventory.instance.inventoryList.Count)
        {
            currentInventoryOption = Inventory.instance.inventoryList.Count - 1;
        }

        if (Input.GetKeyUp(controls.escapeButton) && !inSubMenu && !isInspecting)
        {
            selectedItem = -1;
            currentInventoryOption = 0;
            currentState = InventoryStates.INVENTORY;
        }

        if (InventoryUI.instance.itemHolderList.Count <= 0)
        {
            return;
        }

        if (Input.GetKeyDown(controls.upButton) && currentInventoryOption > 0 && !inSubMenu && !isInspecting)
        {
            currentInventoryOption--;

            if (fingerCounter > 1)
            {
                fingerCounter--;
            }
            else if (fingerCounter == 1)
            {
                itemScrollRect.verticalNormalizedPosition += (float)1 / (InventoryUI.instance.itemHolderList.Count - maxAmountOfItemsToDisplay);
            }

            InventoryUI.instance.descriptionTextField.text = Inventory.instance.inventoryList[currentInventoryOption].item.description;

        }

        else if (Input.GetKeyDown(controls.downButton) && currentInventoryOption < InventoryUI.instance.itemHolderList.Count - 1 && !inSubMenu && !isInspecting)
        {
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



        if (!inSubMenu && !isInspecting)
        {
            //Use


            if (Input.GetKeyDown(controls.acceptButton))
            {

                tempPopupItem = Instantiate(itemOptionsPopupPanel, GameObject.Find("ItemOptionsPopupPanelHolder").transform.Find("Holder").position, Quaternion.identity, GameObject.Find("ItemOptionsPopupPanelHolder").transform.Find("Holder")) as GameObject;
                foreach (Transform point in tempPopupItem.transform.Find("SelectPoints").GetComponentInChildren<Transform>())
                {
                    itemOptionsPopupList.Add(point);
                }
                tempItemName = Inventory.instance.inventoryList[currentInventoryOption].item.name;
                inSubMenu = true;
            }

            //Order
            if (Input.GetKeyDown(controls.selectButton) && selectedItem == -1)
            {
                blinkingFinger.SetActive(true);
                blinkingFinger.transform.position = finger.transform.position;
                selectedItem = currentInventoryOption;
            }

            else if (Input.GetKeyDown(controls.selectButton) && (selectedItem != -1) && (selectedItem != currentInventoryOption))
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

            if (Input.GetKeyDown(controls.acceptButton) && currentItemOption == 0) // USE
            {
                Inventory.instance.inventoryList[currentInventoryOption].item.UseItem(GameManager.instance.ulf);
                InventoryUI.instance.RecreateList();
            }

            if (Input.GetKeyDown(controls.acceptButton) && currentItemOption == 1) // INSPECT
            {
                itemDescriptionPanel.SetActive(true);
                finger.SetActive(false);
                itemDescriptionPanel.transform.Find("DescriptionText").GetComponent<TextMeshProUGUI>().text = Inventory.instance.inventoryList[currentInventoryOption].item.description;
                isInspecting = true;
            }

            if (Input.GetKeyDown(controls.acceptButton) && currentItemOption == 2) // DELETE
            {
                Inventory.instance.RemoveItem(Inventory.instance.inventoryList[currentInventoryOption].item);
                InventoryUI.instance.RecreateList();
            }

            if (Input.GetKeyUp(controls.escapeButton) || tempItemName != Inventory.instance.inventoryList[currentInventoryOption].item.name || (Input.GetKeyDown(controls.acceptButton) && currentItemOption == 3)) //Escape or one runs out of item or CANCEL
            {
                GameObject.Find("PlayerCharacter").GetComponent<Movement>().canMove = false;
                currentItemOption = 0;
                itemOptionsPopupList.Clear();
                Destroy(tempPopupItem);
                inSubMenu = false;
            }

            if (Input.GetKeyDown(controls.upButton) && currentItemOption > 0)
            {
                currentItemOption--;
            }

            else if (Input.GetKeyDown(controls.downButton) && currentItemOption < itemOptionsPopupList.Count - 1)
            {
                currentItemOption++;
            }


        }

        else if (isInspecting)
        {
            if (Input.GetKeyDown(controls.acceptButton))
            {
                itemDescriptionPanel.SetActive(false);
                finger.SetActive(true);
                isInspecting = false;
            }
        }

    }



    void EquipmentUse()
    {

        if (!inSubMenu)
        {
            inSubMenu = true;
            tempPopupEquip = Instantiate(EquipmentSlotsOptionsPopupPanel, EquipmentUI.instance.parents[1].position + new Vector3(385, 0, 0), Quaternion.identity, GameObject.Find("EquipmentOptionsPopupPanelHolder").transform.Find("Holder")) as GameObject;// the positioning of the wolf helmet fits nicely and I was too lazy to do it properly
            foreach (Transform point in tempPopupEquip.transform.Find("SelectPoints").GetComponentInChildren<Transform>())
            {
                EquipmentSlotsOptionssPopupList.Add(point);
            }
            currentInventoryOption = 0;

        }

        else if (inSubMenu && !isInspecting && !inSubSubMenu)
        {

            finger.transform.position = EquipmentSlotsOptionssPopupList[currentInventoryOption].position;

            if (Input.GetKeyDown(controls.acceptButton) && currentInventoryOption == 0) // EQUIP
            {
                EquippableItemsPanel.SetActive(true);
                Item.ItemTypes type = Item.ItemTypes.AMULET; // could be anything, will be set in the following switch statement
                switch (currentSelectedEquipmentSLot)
                { //hard-coded because of time pressure
                    case (0):
                        type = Item.ItemTypes.WEAPON;
                        break;
                    case (1):
                        type = Item.ItemTypes.HELMET;
                        break;
                    case (2):
                        type = Item.ItemTypes.ARMOR;
                        break;
                    case (3):
                        type = Item.ItemTypes.BOOTS;
                        break;
                    default:
                        break;
                }
                tempEquipInventory = new List<Equipment>(); //temporary list of all equipment-items of the same type as the currently selected one (currentSelectedEquipmentSlot)

                foreach (Inventory.InventoryItem item in Inventory.instance.inventoryList)
                {
                    if (item.item.GetType() == typeof(Equipment) && item.item.type == type)
                    {
                        tempEquipInventory.Add((Equipment)item.item);
                    }
                }
                if (GameManager.instance.heroes[EquipmentUI.instance.selectedHeroIndex].GetComponent<PlayerStateMachine>().player.equipmentSlots[currentSelectedEquipmentSLot] != null) {
                    tempEquipInventory.Reverse();                                                   //adding the already equipped item to first position of the list, if there is something equipped
                    tempEquipInventory.Add((GameManager.instance.heroes[EquipmentUI.instance.selectedHeroIndex].GetComponent<PlayerStateMachine>().player.equipmentSlots[currentSelectedEquipmentSLot]));
                    tempEquipInventory.Reverse();
                }
                

                tempEquipInventory = tempEquipInventory.Distinct().ToList(); //removes duplicates

                for (int i = 0; i < tempEquipInventory.Count; i++)
                {

                    GameObject newItemHolder = Instantiate(equipItemHolderPrefab, equipItemSpacer, false) as GameObject;
                    EquipItemHolder holderScript = newItemHolder.GetComponent<EquipItemHolder>();
                    holderScript.icon.sprite = tempEquipInventory[i].icon;
                    holderScript.damageText.text = "DMG: " + tempEquipInventory[i].damage.ToString();
                    holderScript.armorText.text = "ARM: " + tempEquipInventory[i].armor.ToString();
                    holderScript.nameOfItemText.text = tempEquipInventory[i].name;
                    holderScript.typeText.text = tempEquipInventory[i].type.ToString();

                    equipItemHolderList.Add(newItemHolder);


                }
                currentInventoryOption = 0;
                tempPopupEquip.SetActive(false);
                inSubSubMenu = true;
            }

            if (Input.GetKeyDown(controls.acceptButton) && currentInventoryOption == 1) // UNEQUIP
            {
                if (GameManager.instance.heroes[EquipmentUI.instance.selectedHeroIndex].GetComponent<PlayerStateMachine>().player.equipmentSlots[currentSelectedEquipmentSLot] != null)
                {
                    GameManager.instance.heroes[EquipmentUI.instance.selectedHeroIndex].GetComponent<PlayerStateMachine>().player.equipmentSlots[currentSelectedEquipmentSLot].UseItem(GameManager.instance.heroes[EquipmentUI.instance.selectedHeroIndex]);
                    EquipmentUI.instance.UpdateResourceBars(EquipmentUI.instance.selectedHeroIndex);
                }

            }

            if (Input.GetKeyDown(controls.acceptButton) && currentInventoryOption == 2) // INSPECT
            {
                if (GameManager.instance.heroes[EquipmentUI.instance.selectedHeroIndex].GetComponent<PlayerStateMachine>().player.equipmentSlots[currentSelectedEquipmentSLot] != null)
                {
                    equipmentDescriptionPanel.SetActive(true);
                    finger.SetActive(false);
                    equipmentDescriptionPanel.transform.Find("DescriptionText").GetComponent<TextMeshProUGUI>().text = GameManager.instance.heroes[EquipmentUI.instance.selectedHeroIndex].GetComponent<PlayerStateMachine>().player.equipmentSlots[currentSelectedEquipmentSLot].description;
                    isInspecting = true;
                }
            }

            if (Input.GetKeyUp(controls.escapeButton) || (Input.GetKeyDown(controls.acceptButton) && currentInventoryOption == 3)) //Escape or CANCEL
            {
                GameObject.Find("PlayerCharacter").GetComponent<Movement>().canMove = false;
                currentState = InventoryStates.EQUIPMENT_SLOTS;
                EquipmentSlotsOptionssPopupList.Clear();
                Destroy(tempPopupEquip);
                inSubMenu = false;
                currentInventoryOption = 0;
                
            }

            if (Input.GetKeyDown(controls.upButton) && currentInventoryOption > 0)
            {
                currentInventoryOption--;
            }

            else if (Input.GetKeyDown(controls.downButton) && currentInventoryOption < EquipmentSlotsOptionssPopupList.Count - 1)
            {
                currentInventoryOption++;
            }
        }

        else if (inSubSubMenu)
        {
            finger.transform.position = equipItemHolderList[currentInventoryOption].GetComponent<EquipItemHolder>().itemSelectPoint.transform.position;
            if (Input.GetKeyDown(controls.upButton) && currentInventoryOption > 0 && !isInspecting)
            {
                currentInventoryOption--;

                if (fingerCounter > 1)
                {
                    fingerCounter--;
                }
                else if (fingerCounter == 1)
                {
                    equipItemScrollRect.verticalNormalizedPosition += (float)1 / (equipItemHolderList.Count - maxAmountOfItemsToDisplay);
                }

            }

            else if (Input.GetKeyDown(controls.downButton) && currentInventoryOption < equipItemHolderList.Count - 1 && !isInspecting)
            {
                currentInventoryOption++;

                if (fingerCounter < maxAmountOfItemsToDisplay)
                {
                    fingerCounter++;
                }
                else if (fingerCounter == maxAmountOfItemsToDisplay)
                {
                    equipItemScrollRect.verticalNormalizedPosition -= (float)1 / (equipItemHolderList.Count - maxAmountOfItemsToDisplay);
                }

                InventoryUI.instance.descriptionTextField.text = Inventory.instance.inventoryList[currentInventoryOption].item.description;

            }

            else if (Input.GetKeyUp(controls.escapeButton))
            {
                EquippableItemsPanel.SetActive(false);
                inSubSubMenu = false;
                currentInventoryOption = 0;
                foreach (GameObject itemholder in equipItemHolderList)
                {
                    Destroy(itemholder);
                }
                tempPopupEquip.SetActive(true);
                equipItemHolderList.Clear();

                GameObject.Find("PlayerCharacter").GetComponent<Movement>().canMove = false;
                currentState = InventoryStates.EQUIPMENT_SLOTS;
                EquipmentSlotsOptionssPopupList.Clear();
                Destroy(tempPopupEquip);
                inSubMenu = false;
                currentInventoryOption = 0;
            }

            else if (Input.GetKeyDown(controls.acceptButton))
            {

                tempEquipInventory[currentInventoryOption].UseItem(GameManager.instance.heroes[EquipmentUI.instance.selectedHeroIndex]);
                EquipmentUI.instance.UpdateResourceBars(EquipmentUI.instance.selectedHeroIndex);
            }

            


        }

        else if (isInspecting)
        {
            if (Input.GetKeyDown(controls.acceptButton))
            {
                equipmentDescriptionPanel.SetActive(false);
                finger.SetActive(true);
                isInspecting = false;
            }
        }
    }
}
