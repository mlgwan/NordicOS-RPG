using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetButton : MonoBehaviour {

    public GameObject targetPrefab;


    public void SelectTarget() {
        GameObject.Find("BattleManager").GetComponent<BattleStateMachine>().Input2(targetPrefab);
        targetPrefab.transform.Find("enemySelector").gameObject.SetActive(false);
    }

    public void ShowSelector() {

            targetPrefab.transform.Find("enemySelector").gameObject.SetActive(true);

        }

    public void HideSelector()
    {

        targetPrefab.transform.Find("enemySelector").gameObject.SetActive(false);

    }
}
