using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetButton : MonoBehaviour {

    public GameObject targetPrefab;

    public void SelectTarget() {
        GameObject.Find("BattleManager").GetComponent<BattleStateMachine>(); 
    }
}
