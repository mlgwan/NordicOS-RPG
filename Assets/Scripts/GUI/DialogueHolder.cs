using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueHolder : MonoBehaviour {

    public string dialogue;
    private DialogueManager dialogueManager;

    public string[] dialogueLines;

    private void Start()
    {
        dialogueManager = DialogueManager.instance;
    }


    public void DisplayBox()
    {

        if (!dialogueManager.dialogueActive) {
            dialogueManager.dialogueLines = dialogueLines;
            dialogueManager.currentLine = 0;
            dialogueManager.ShowDialogue();
        }
    }
}
