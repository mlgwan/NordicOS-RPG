using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitialSceneCamera : MonoBehaviour {
    private bool followingPlayer;
    private GameObject player;
    private static Vector3 offset;
 

    private ControlScript controls;
    private void Awake()
    {
        controls = ControlScript.instance;
        
        player = GameObject.Find("PlayerCharacter");

        if (!GameManager.scriptedEncounters[0]) // not a real encounter, it's a placeholder used to ensure that the initial camera sequence plays only once and to set the other encounters to true at the start
        {
            followingPlayer = false;
            for (int i = 0; i < GameManager.scriptedEncounters.Length; i++) {
                GameManager.scriptedEncounters[i] = true;
            }
        }
        

        else {
            GetComponent<Animator>().enabled = false;
            transform.position = GameManager.instance.nextPlayerPosition + offset;
            followingPlayer = true;
            player.GetComponent<Movement>().canMove = true;
        }
        
        GameManager.instance.CheckScriptedEncounters();
    }
    private void Update()
    {
        if (Input.GetKey(controls.acceptButton)) {
            if (!GameManager.gameStarted) {
                StartCoroutine(enableMovementAfterInitialCameraAnimation());
            }
            GameManager.gameStarted = true;
            GetComponent<Animator>().SetBool("gameStarted", true);            
        }
        
    }

    private void LateUpdate()
    {
        if (followingPlayer)
        {
            this.transform.position = player.transform.position + offset;
        }
    }

    void changeTextActive()
    {
        if (GameObject.Find("Initial Text") != false)
        {
            GameObject.Find("Initial Text").SetActive(false);
        }
    }
    void toggleFollowingPlayer() {
        followingPlayer = !followingPlayer;
    }
    void calculateOffset() {
        offset = this.transform.position - player.transform.position;
    }

    private IEnumerator enableMovementAfterInitialCameraAnimation() {

        yield return new WaitForSeconds(25f); // rough length of the "Pan to Character" animation, TODO: should be retrieved dynamically
        player.GetComponent<Movement>().canMove = true;
    }
}
