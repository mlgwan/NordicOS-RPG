using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartGameController : MonoBehaviour {

	// Update is called once per frame
	void Update () {
        GetComponent<Animator>().SetBool("gameStarted", GameManager.gameStarted);
	}
}
