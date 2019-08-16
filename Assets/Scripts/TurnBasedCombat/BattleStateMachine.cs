using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class BattleStateMachine : MonoBehaviour {

	public enum BattleStates
    {
        WAITING,
        TAKEACTION,
        PERFORMACTION
    }

    public BattleStates currentState;

    public List<HandleTurn> performList = new List<HandleTurn>();

    public List<GameObject> enemiesInBattle = new List<GameObject>();
    public List<GameObject> playersInBattle = new List<GameObject>();

    //GUI stuff
    public enum PlayerGUI {
        ACTIVATE,
        WAITING, //idle
        INPUT1, //for now: basic attack
        INPUT2, //for now: selecting
        DONE

    }

    public PlayerGUI playerInput;
    public List<GameObject> playersToManage = new List<GameObject>();
    private HandleTurn playerChoice;
    public GameObject enemyButton;
    public Transform spacer;



    private void Start()
    {
        currentState = BattleStates.WAITING;

        enemiesInBattle.AddRange(GameObject.FindGameObjectsWithTag("Enemy"));
        playersInBattle.AddRange(GameObject.FindGameObjectsWithTag("Player"));

        EnemyButtons();
    }

    private void Update()
    {
        switch (currentState) {
            case (BattleStates.WAITING):
                if (performList.Count > 0) {
                    currentState = BattleStates.TAKEACTION;
                }
                break;

            case (BattleStates.TAKEACTION):
                GameObject performer = GameObject.Find(performList[0].attackersName);
                if (performList[0].attackerType == "Enemy") {
                    EnemyStateMachine ESM = performer.GetComponent<EnemyStateMachine>();
                    ESM.playerToAttack = performList[0].attackersTarget;
                    ESM.currentState = EnemyStateMachine.TurnState.ACTION;
                }

                if (performList[0].attackerType == "Player")
                {

                }

                currentState = BattleStates.PERFORMACTION;

                break;

            case (BattleStates.PERFORMACTION):
                break;
                /*
            case (BattleStates.LOSE):
                break;
            case (BattleStates.WIN):
                break;*/
        }
    }

    public void CollectAction(HandleTurn input) {
        performList.Add(input);
    }

    void EnemyButtons() {
        foreach (GameObject enemy in enemiesInBattle) {
            GameObject newButton = Instantiate(enemyButton) as GameObject;
            TargetButton button = newButton.GetComponent<TargetButton>();

            EnemyStateMachine curEnemy = enemy.GetComponent<EnemyStateMachine>();

            Text buttonText = newButton.transform.FindChild("Text").gameObject.GetComponent<Text>();
            buttonText.text = curEnemy.enemy.theName;

            button.targetPrefab = enemy;

            newButton.transform.SetParent(spacer,false);
        }
    }
}
