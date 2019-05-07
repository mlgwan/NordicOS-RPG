using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CreateNewCharacter : MonoBehaviour {

    private string playerName;
    private BasePlayer newPlayer;

	void Start () {
        newPlayer = new BasePlayer();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnGUI()
    {
        newPlayer.PlayerClass = new BaseWarriorClass();
        newPlayer.PlayerLevel = 1;
        newPlayer.Health = newPlayer.PlayerClass.Health;
        newPlayer.Strength = newPlayer.PlayerClass.Strength;
        newPlayer.Dexterity = newPlayer.PlayerClass.Dexterity;
        newPlayer.Intelligence = newPlayer.PlayerClass.Intelligence;
        newPlayer.Speed = newPlayer.PlayerClass.Speed;
        newPlayer.Mana = newPlayer.PlayerClass.Mana;
        playerName = GUILayout.TextArea(playerName, 15);
        newPlayer.PlayerName = playerName;
        StoreNewPlayerInfo();
        SaveInformation.SaveAllInformation();

        if (GUILayout.Button("Load")) {
            SceneManager.LoadScene(0);
        }
    }
    
   


    private void StoreNewPlayerInfo() {
        GameInformation.PlayerName = newPlayer.PlayerName;
        GameInformation.PlayerLevel = newPlayer.PlayerLevel;
        GameInformation.PlayerClass = newPlayer.PlayerClass;
        GameInformation.Health = newPlayer.Health;
        GameInformation.Strength = newPlayer.Strength;
        GameInformation.Dexterity = newPlayer.Dexterity;
        GameInformation.Intelligence = newPlayer.Intelligence;
        GameInformation.Speed = newPlayer.Speed;
        GameInformation.Mana = newPlayer.Mana;

    }
}
