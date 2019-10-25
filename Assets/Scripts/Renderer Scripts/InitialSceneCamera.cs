using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitialSceneCamera : MonoBehaviour {
    private bool followingPlayer;
    private GameObject player;
    private static Vector3 offset;
    private void Awake()
    {
        
        player = GameObject.Find("PlayerCharacter");

        if (!GameManager.scriptedEncounters[0]) // the first encounter, being the wolf ulf has to fight after leaving his camp
        {
            followingPlayer = false;
            GetComponent<Animator>().Play("Pan to Character");
            GameManager.scriptedEncounters[0] = true;
        }

        else {
            GetComponent<Animator>().enabled = false;
            transform.position = GameManager.instance.nextPlayerPosition + offset;
            followingPlayer = true;
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
}
