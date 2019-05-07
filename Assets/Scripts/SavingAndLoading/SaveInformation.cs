using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveInformation : MonoBehaviour {

    public static void SaveAllInformation() {
        PlayerPrefs.SetInt("PLAYERLEVEL", GameInformation.PlayerLevel);
        PlayerPrefs.SetString("PLAYERNAME", GameInformation.PlayerName);
        PlayerPrefs.SetInt("HEALTH", GameInformation.Health);
        PlayerPrefs.SetInt("STRENGTH", GameInformation.Strength);
        PlayerPrefs.SetInt("DEXTERITY", GameInformation.Dexterity);
        PlayerPrefs.SetInt("INTELLIGENCE", GameInformation.Intelligence);
        PlayerPrefs.SetInt("SPEED", GameInformation.Speed);
        PlayerPrefs.SetInt("MANA", GameInformation.Mana);
        
    }
}
