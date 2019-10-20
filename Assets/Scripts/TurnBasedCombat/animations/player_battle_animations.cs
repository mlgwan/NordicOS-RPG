using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class player_battle_animations : MonoBehaviour {
    private Animator animator;
    // Use this for initialization
    void Awake()
    {
        animator = this.gameObject.GetComponent<Animator>();
        animator.SetBool("isBattle", true);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
