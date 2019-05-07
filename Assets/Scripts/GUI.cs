using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GUI : MonoBehaviour {

    private BaseCharacterClass class1 = new BaseWarriorClass();
    private BaseCharacterClass class2 = new BaseMageClass();

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnGUI()
    {
        GUILayout.Label(class1.CharacterClassName);
        GUILayout.Label(class1.CharacterClassDescription);
        GUILayout.Label(class2.CharacterClassName);
        GUILayout.Label(class2.CharacterClassDescription);     
    }
}
