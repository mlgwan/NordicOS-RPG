using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleManager : MonoBehaviour {

    public bool isPlayersTurn;

    // Use this for initialization
    public void startBattle(GameObject enemy) {
        Instantiate(enemy, GameObject.Find("Enemies").GetComponent<Transform>());
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
