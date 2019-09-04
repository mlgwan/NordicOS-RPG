using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[System.Serializable]
public class EnemyStateMachine : MonoBehaviour {

    private BattleStateMachine BSM;
    public BaseEnemy enemy;
    public GameObject enemySelector;

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
    public float animSpeed = 12f;

    public bool hasChosenAnAction;

    //death
    private bool dead;

    //status effects
    private bool isPoisoned;
    private bool isBurned;
    private bool isFrostBurned;
    private bool isStunned;
    public bool tickApplied;

    void Start () {
        currentState = TurnState.WAITING;
        enemySelector.SetActive(false);
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
                if (dead)
                {
                    return;
                }
                else {
                    //change tag
                    this.gameObject.tag = "DeadEnemy";

                    //set to not attackable by enemies
                    BSM.enemiesInBattle.Remove(this.gameObject);

                    //deactivate the selector
                    enemySelector.SetActive(false);

                    //remove item from performList
                    if (BSM.enemiesInBattle.Count > 0) {
                        for (int i = 1; i < BSM.performList.Count; i++)
                        {
                            if (BSM.performList[i].attackersGameObject == this.gameObject)
                            {
                                BSM.performList.Remove(BSM.performList[i]);
                            }
                            if (BSM.performList[i].attackersTarget == this.gameObject)
                            {
                                BSM.performList[i].attackersTarget = BSM.enemiesInBattle[UnityEngine.Random.Range(0, BSM.enemiesInBattle.Count)];
                            }
                        }

                    }


                    //change color / player death-animation
                    this.gameObject.GetComponent<SpriteRenderer>().material.color = new Color32(102, 102, 102, 120);
                    this.gameObject.GetComponent<Animator>().enabled = false;
                    //reset playerInput
   

                    dead = true;
                    //reset the buttons for selecting enemies
                    BSM.EnemyButtons();
                    //chek for win/loss
                    BSM.currentState = BattleStateMachine.BattleStates.CHECKALIVE;
                }
                break;

        }
    }

    void ChooseAction() { // randomly selecting, very simple AI

        int num = Random.Range(1, 101);        
            HandleTurn myAttack = new HandleTurn
            {
                attackersName = enemy.theName,
                attackersType = "Enemy",
                attackersGameObject = this.gameObject,
                attackersTarget = BSM.playersInBattle[Random.Range(0, BSM.playersInBattle.Count)],
            };
            if (num < 70 || enemy.abilities.Count == 0)
            {
                myAttack.chosenAttack = enemy.basicAttack;
            }
            else
            {
                myAttack.chosenAttack = enemy.abilities[Random.Range(0, enemy.abilities.Count)];
            }
        

        


    BSM.CollectAction(myAttack);
            hasChosenAnAction = true;
    }

    private IEnumerator TimeForAction() {

        if (actionStarted) {
            yield break;
        }
        actionStarted = true;

        bool statusCheck = CheckWhetherStatusAffectsTurn(enemy.currentStatus);
        Vector3 enemyPosition;
        if (!statusCheck)
        {
            enemyPosition = new Vector3(playerToAttack.transform.position.x - 1.5f, playerToAttack.transform.position.y, playerToAttack.transform.position.z);

        }
        else {
            enemyPosition = startPosition;
        }
        //animate the enemy near the hero to attack       
        while (MoveTowardsTarget(enemyPosition))
        {
            yield return null;
        }

        //wait a bit
        yield return new WaitForSeconds(0.5f);
        //do damage
        if (!statusCheck)
        {
            DealDamage();
        }
        
        //animate back to start position
        Vector3 firstPosition = startPosition;
        while (MoveTowardsTarget(firstPosition))
        {
            yield return null;
        }

        //remove this performer from the list in BSM
        BSM.performList.RemoveAt(0);

        //reset the BSM -> WAIT
        BSM.currentState = BattleStateMachine.BattleStates.WAITING;
        //end coroutine
        actionStarted = false;
        //reset this enemy state
        hasChosenAnAction = false;
        tickApplied = false;

        if (isStunned) {
            isStunned = false;
            enemy.currentStatus = BaseCharacter.Status.UNHARMED;
        }
        currentState = TurnState.WAITING;

    }

    private bool MoveTowardsTarget(Vector3 target) {
        return target != (transform.position = Vector3.MoveTowards(transform.position, target, animSpeed * Time.deltaTime));
    }

    void DealDamage()
    {
        int calculatedDamage = (int)((enemy.curATK / 10 + 1) *  BSM.performList[0].chosenAttack.attackDamage); //simple damage formula, position 0 will always hold the current performer

        if (BSM.performList[0].chosenAttack.isAOE)
        {
            for (int i = 0; i < BSM.GetComponent<BattleStateMachine>().playersInBattle.Count; i++)
            {
                CheckForStatusToApply(BSM.GetComponent<BattleStateMachine>().playersInBattle[i]);
                BSM.GetComponent<BattleStateMachine>().playersInBattle[i].GetComponent<PlayerStateMachine>().TakeDamage(calculatedDamage);
            }
        }
        else
        {
            CheckForStatusToApply(playerToAttack);
            playerToAttack.GetComponent<PlayerStateMachine>().TakeDamage(calculatedDamage);
        }

    }

    public void TakeDamage(int damageAmount)
    {

        if (damageAmount <= 1)
        {
            enemy.curHP -= 1;
        }

        enemy.curHP -= (int)(damageAmount - (enemy.curDEF / 10)); //simple armor calculation


        if (enemy.curHP <= 0)
        {
            enemy.curHP = 0;
            currentState = TurnState.DEAD;
        }
    }


    public void CheckForStatusToApply(GameObject attackedPlayer) {

        if (BSM.performList[0].chosenAttack.statusEffectToApply == BaseAttack.StatusEffects.NONE)
        {
            return;
        }

        else { 

        int num = Random.Range(1, 101);

            switch (BSM.performList[0].chosenAttack.statusEffectToApply)
            {

                case (BaseAttack.StatusEffects.PARALYSIS):

                    if (num < BSM.performList[0].chosenAttack.applicationChance)
                    {
                        attackedPlayer.GetComponent<PlayerStateMachine>().player.currentStatus = BaseCharacter.Status.PARALYSED;
                    }
                    break;

                case (BaseAttack.StatusEffects.POISON):
                    if (num < BSM.performList[0].chosenAttack.applicationChance)
                    {
                        attackedPlayer.GetComponent<PlayerStateMachine>().player.currentStatus = BaseCharacter.Status.POISONED;
                    }
                    break;

                case (BaseAttack.StatusEffects.BURN):
                    if (num < BSM.performList[0].chosenAttack.applicationChance)
                    {
                        attackedPlayer.GetComponent<PlayerStateMachine>().player.currentStatus = BaseCharacter.Status.BURNED;
                    }
                    break;

                case (BaseAttack.StatusEffects.FROSTBURN):
                    if (num < BSM.performList[0].chosenAttack.applicationChance)
                    {
                        attackedPlayer.GetComponent<PlayerStateMachine>().player.currentStatus = BaseCharacter.Status.FROSTBURNED;
                    }
                    break;

                case (BaseAttack.StatusEffects.STUN):
                    if (num < BSM.performList[0].chosenAttack.applicationChance)
                    {
                        attackedPlayer.GetComponent<PlayerStateMachine>().player.currentStatus = BaseCharacter.Status.STUNNED;
                    }
                    break;
            }
        }
    }

    bool CheckWhetherStatusAffectsTurn(BaseCharacter.Status status) {
        switch (status)
        {

            case (BaseCharacter.Status.UNHARMED):
                return false;

            case (BaseCharacter.Status.PARALYSED):
                return true;

            case (BaseCharacter.Status.POISONED):
                if (!isPoisoned)
                {
                    enemy.dotToTake += BSM.performList[0].chosenAttack.poisonDamage;
                    isPoisoned = true;
                }

                if (!tickApplied)
                {
                    TakeDamage(enemy.dotToTake);
                    tickApplied = true;
                }


                if (currentState == TurnState.DEAD)
                {
                    return true;
                }

                else
                {
                    return false;
                }
    
            case (BaseCharacter.Status.BURNED):
                return false;

            case (BaseCharacter.Status.FROSTBURNED):
                return false;
            case (BaseCharacter.Status.STUNNED):
                isStunned = true;
                return true;

            default:

                return false;
        }

    }

}
