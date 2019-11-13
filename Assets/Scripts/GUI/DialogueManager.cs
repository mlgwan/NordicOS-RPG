using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DialogueManager : MonoBehaviour {

    public static DialogueManager instance;

    public GameObject dialogueBox;
    public TextMeshProUGUI dialogueText;

    public bool dialogueActive;

    public string[] dialogueLines;
    public int currentLine;

    private void Awake()
    {
        instance = this;
    }

    // Update is called once per frame
    void Update () {
        if (dialogueActive && Input.GetKeyDown(ControlScript.instance.acceptButton))
        {
            currentLine++;
        }

        if (currentLine >= dialogueLines.Length)
        {
            dialogueBox.SetActive(false);
            dialogueActive = false;

            currentLine = 0;
        }

        dialogueText.text = dialogueLines[currentLine];

	}


    public void ShowDialogue() {
        dialogueActive = true;
        dialogueBox.SetActive(true);

    }

}
