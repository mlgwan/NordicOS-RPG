using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueHolder : MonoBehaviour {

    public string dialogue;
    public DialogueManager dialogueManager;

    public string[] dialogueLines;
    public Sprite[] speakerSprites;

    private void Start()
    {
        dialogueManager = GameObject.FindWithTag("MenuCanvas").transform.Find("DialogueManager").GetComponent<DialogueManager>();
    }


    public void DisplayBox(bool treasure = false)
    {
        refresh();
        if (!dialogueManager.dialogueActive) {
            dialogueManager.speakerSprites = speakerSprites;
            dialogueManager.dialogueLines = dialogueLines;

            if (treasure || GameManager.instance.gameState != GameManager.GameStates.WORLD_STATE)
            {
                dialogueManager.currentLine = 0;
            }
            else {
                dialogueManager.currentLine = -0; // SOMETIMES -1 WORKS SOMETIMES IT DOESN'T
            }
                

            dialogueManager.ShowDialogue();
        }
    }

    public void refresh() {
        dialogueManager = GameObject.FindWithTag("MenuCanvas").transform.Find("DialogueManager").GetComponent<DialogueManager>();
    }
}
