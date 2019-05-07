using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterStat : MonoBehaviour {

    public int speed;
    public int attack;
    public int hitpoints;
    public int mana;
    public bool isDead;
    public GameObject hpDisplayText;

    public int experiencePoints;


    private void Awake()
    { 
    }

    public void basicAttack(GameObject enemy) {
        enemy.GetComponent<EnemyStats>().hitPoints -= attack + Random.Range(attack - (int)(attack / 10), attack + (int)(attack / 10));
        enemy.GetComponent<EnemyStats>().checkForDeath();
    }
    public void checkForDeath() {
        hpDisplayText.GetComponent<Text>().text = hitpoints.ToString();
        if (hitpoints <= 0) {
            isDead = true;
        }
    }
}
