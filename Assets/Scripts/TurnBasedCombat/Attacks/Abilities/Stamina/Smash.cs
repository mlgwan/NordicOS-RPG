using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Smash : BaseAttack {

    public Smash()
    {
        isAOE = true;
        attackName = "Smash";
        attackDescription = "A heavy smash";
        attackDamage = 40;
        attackCost = 10;
    }
}
