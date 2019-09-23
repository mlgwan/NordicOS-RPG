using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitialSceneCamera : MonoBehaviour {
    private bool followingPlayer;
    private GameObject player;
    private Vector3 offset;
    private void Awake()
    {
        followingPlayer = false;
        player = GameObject.Find("PlayerCharacter");
        

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
