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

    private void StoreNewPlayerInfo() {
        GameInformation.PlayerName = newPlayer.theName;
        GameInformation.PlayerLevel = newPlayer.level;
        GameInformation.Health = newPlayer.baseHP;
        GameInformation.Strength = newPlayer.baseSTR;
        GameInformation.Dexterity = newPlayer.baseDEX;
        GameInformation.Intelligence = newPlayer.baseINT;
        GameInformation.Speed = newPlayer.baseSPD;
        GameInformation.Mana = newPlayer.baseMP;

    }
}
