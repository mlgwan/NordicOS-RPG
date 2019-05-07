using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStats : MonoBehaviour {

    public int speedPoints;
    public int hitPoints;
    public int manaPoints;

    public int attackPoints;
    public int defensePoints;

    public int lowerAttackBound;
    public int higherAttackBound;

    public int experienceRewardPoints;




    public void basicAttack(int targetHP) {
        targetHP -= attackPoints + Random.Range(lowerAttackBound, higherAttackBound);
    }

    public void checkForDeath() {

        if (hitPoints <= 0) {

            GameObject.FindWithTag("Player").GetComponent<CharacterStat>().experiencePoints += experienceRewardPoints;
            Destroy(gameObject);
        }
    }
}
