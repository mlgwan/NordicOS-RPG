using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RegionData : MonoBehaviour {

    public bool isScriptedEncounter;
    public int maxAmountEnemies = 4;
    public string battleScene;
    public List<GameObject> possibleEnemies = new List<GameObject>();
    public int regionId;

}


