using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class HandleTurn {

    public string attackersName;
    public string attackersType; //player or enemy
    public GameObject attackersGameObject; //who attacks
    public GameObject attackersTarget;  //who gets attacked


    //which attack is performed
    public BaseAttack chosenAttack;
}
