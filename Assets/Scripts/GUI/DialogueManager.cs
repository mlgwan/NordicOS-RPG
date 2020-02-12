using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogueManager : MonoBehaviour {

    public GameObject dialogueBox;
    public TextMeshProUGUI dialogueText;
    public Image speakerImage; //Who is speaking right now and how they look/feel

    public bool dialogueActive;

    public string[] dialogueLines;
    public Sprite[] speakerSprites;
    public int currentLine;

    // Update is called once per frame
    void Update () {
        
        if (dialogueActive && Input.GetKeyDown(ControlScript.instance.acceptButton))
        {
            currentLine++;           
        }

        else if ((currentLine >= dialogueLines.Length && dialogueLines.Length > 0) || (Input.GetKeyDown(ControlScript.instance.escapeButton) && dialogueActive))
        {
            CloseDialogue();
        }

        else if (currentLine <= dialogueLines.Length && dialogueLines.Length > 0)
        {
            speakerImage.sprite = speakerSprites[currentLine];
            dialogueText.text = dialogueLines[currentLine];
           
        }
       

	}


    public void ShowDialogue() {
        dialogueActive = true;
        
        if (!(GameManager.instance.gameState == GameManager.GameStates.IDLE))
        {
            GameObject.Find("PlayerCharacter").GetComponent<Movement>().canMove = false;
            GameObject.FindWithTag("MenuCanvas").GetComponent<InventoryManager>().isInspecting = true;
        }

        dialogueBox.SetActive(true);
    }

    public void CloseDialogue() {
      
        dialogueBox.SetActive(false);
        dialogueActive = false;

        currentLine = 0;
        if (!(GameManager.instance.gameState == GameManager.GameStates.IDLE))
        {
            if (!GameObject.Find("PlayerCharacter").GetComponent<Movement>().inventoryIsOpen) { 
            GameObject.Find("PlayerCharacter").GetComponent<Movement>().canMove = true;}
            GameObject.FindWithTag("MenuCanvas").GetComponent<InventoryManager>().isInspecting = false;
        }
    }


}
