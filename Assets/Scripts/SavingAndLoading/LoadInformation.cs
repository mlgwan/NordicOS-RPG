using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadInformation : MonoBehaviour {

    public static void LoadAllInformation() {
        GameInformation.PlayerName = PlayerPrefs.GetString("PLAYERNAME");
        GameInformation.PlayerLevel = PlayerPrefs.GetInt("PLAYERLEVEL");
        GameInformation.Health = PlayerPrefs.GetInt("HEALTH");
        GameInformation.Strength = PlayerPrefs.GetInt("STRENGTH");
        GameInformation.Dexterity = PlayerPrefs.GetInt("DEXTERITY");
        GameInformation.Intelligence = PlayerPrefs.GetInt("INTELLIGENCE");
        GameInformation.Speed = PlayerPrefs.GetInt("SPEED");
        GameInformation.Mana = PlayerPrefs.GetInt("MANA");
    }
}
