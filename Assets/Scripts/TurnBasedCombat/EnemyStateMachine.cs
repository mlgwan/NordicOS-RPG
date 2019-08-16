using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class EnemyStateMachine : MonoBehaviour {

    private BattleStateMachine BSM;
    public BaseEnemy enemy;

    public enum TurnState
    {
        CHOOSEACTION,
        WAITING, //idle
        ACTION,
        DEAD

    }

    public TurnState currentState;

    //positions for animations etc
    private Vector3 startPosition;
    public float targetOffset = 1.5f;

    //ienumerator stuff
    private bool actionStarted = false;
    public GameObject playerToAttack;
    public float animSpeed = 5f;



    void Start () {
        currentState = TurnState.CHOOSEACTION;
        BSM = GameObject.Find("BattleManager").GetComponent<BattleStateMachine>();
        startPosition = gameObject.transform.position;
	}
	

	void Update () {
        switch (currentState)
        {
            case (TurnState.CHOOSEACTION):
                ChooseAction();
                currentState = TurnState.WAITING;
                break;

            case (TurnState.WAITING):
                break;

            case (TurnState.ACTION):
                StartCoroutine(TimeForAction());
                break;


            case (TurnState.DEAD):
                break;

        }
    }

    void ChooseAction() { // randomly selecting, very simple AI 
        HandleTurn myAttack = new HandleTurn();
        myAttack.attackersName = enemy.theName;
        myAttack.attackerType = "Enemy";
        myAttack.attackersGameObject = this.gameObject;
        myAttack.attackersTarget = BSM.playersInBattle[Random.Range(0, BSM.playersInBattle.Count)];
        BSM.CollectAction(myAttack);
    }

    private IEnumerator TimeForAction() {

        if (actionStarted) {
            yield break;
        }

        actionStarted = true;

        //animate the enemy towards the hero
        Vector3 playerPosition = new Vector3(playerToAttack.transform.position.x - targetOffset, playerToAttack.transform.position.y, playerToAttack.transform.position.z);
        while (MoveTowardsTarget(playerPosition)) {
            yield return null;
        }

        //wait a bit
        yield return new WaitForSeconds(0.5f);

        //do damage

        //animate back to startPosition
        Vector3 firstPosition = startPosition;
        while(MoveTowardsTarget(firstPosition)) { yield return null; }

        //remove this performer from the performList in BSM to not perform the action twice
        BSM.performList.RemoveAt(0);

        //reset BSM to WAITING
        BSM.currentState = BattleStateMachine.BattleStates.WAITING;
        //end coroutine
        actionStarted = false;
        //reset this enemy's state
        currentState = TurnState.WAITING;
    }

    private bool MoveTowardsTarget(Vector3 target) {
        return target != (transform.position = Vector3.MoveTowards(transform.position, target, animSpeed * Time.deltaTime));
    }
}
