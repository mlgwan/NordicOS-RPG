﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

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

    public GameObject finger;
    public GameObject blinkingFinger;

    [Header("Panels")]
    public GameObject equipmentPanel;
    public GameObject optionsPanel;
    public GameObject inventoryPanel;

    public List<GameObject> optionsList = new List<GameObject>();
    public List<GameObject> inventoryOptionsList = new List<GameObject>();

    private int currentOption;
    private int currentInventoryOption;

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

    void UseAndOrder() {

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            selectedItem = -1;
            currentInventoryOption = 0;
            currentState = InventoryStates.INVENTORY;
        }

        if(InventoryUI.instance.itemHolderList.Count <= 0)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.W) && currentInventoryOption > 0) {
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

        else if (Input.GetKeyDown(KeyCode.S) && currentInventoryOption < InventoryUI.instance.itemHolderList.Count - 1) {
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

        //Order
        if (Input.GetKeyDown(KeyCode.Backspace) && selectedItem == -1)
        {
            blinkingFinger.SetActive(true);
            blinkingFinger.transform.position = finger.transform.position;
            selectedItem = currentInventoryOption;
        }

        else if (Input.GetKeyDown(KeyCode.Backspace) && (selectedItem != -1) && (selectedItem != currentInventoryOption)) {
            //Rearrange items
            Inventory.instance.SwapItems(selectedItem, currentInventoryOption);
            selectedItem = -1;
            blinkingFinger.SetActive(false);
            InventoryUI.instance.descriptionTextField.text = Inventory.instance.inventoryList[currentInventoryOption].item.description;

        }

        if (blinkingFinger.activeInHierarchy) {
            blinkingFinger.transform.position = InventoryUI.instance.itemHolderList[selectedItem].GetComponent<ItemHolder>().itemSelectPoint.transform.position;
            
        }

        finger.transform.position = InventoryUI.instance.itemHolderList[currentInventoryOption].GetComponent<ItemHolder>().itemSelectPoint.transform.position;

    }
}