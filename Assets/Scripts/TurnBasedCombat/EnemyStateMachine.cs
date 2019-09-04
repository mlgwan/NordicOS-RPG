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
    private Popups popups;

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
    private bool hasCritted;
    public bool hasBeenCritted;
    //status effects
    //private bool isPoisoned;
    private bool isBurned;
    private bool isFrostBurned;
    private bool isStunned;
    public bool tickApplied;

    private void Awake()
    {
        popups = GameObject.Find("PopupManager").GetComponent<Popups>();
    }

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

        bool statusCheck = CheckWhetherStatusAffectsTurn();
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
            enemy.stunned = false;
        }
        currentState = TurnState.WAITING;

    }

    private bool MoveTowardsTarget(Vector3 target) {
        return target != (transform.position = Vector3.MoveTowards(transform.position, target, animSpeed * Time.deltaTime));
    }

    void DealDamage()
    {
        int calculatedDamage = (int)((enemy.curATK / 10 + 1) *  Random.Range(BSM.performList[0].chosenAttack.attackDamageLowerBound, BSM.performList[0].chosenAttack.attackDamageHigherBound + 1)); //simple damage formula, position 0 will always hold the current performer

        int n = Random.Range(0, 100);
        if (n < BSM.performList[0].chosenAttack.critChance)
        {
            calculatedDamage *= 2;
            hasCritted = true;
        }
        if (BSM.performList[0].chosenAttack.isAOE)
        {
            for (int i = 0; i < BSM.GetComponent<BattleStateMachine>().playersInBattle.Count; i++)
            {
                CheckForStatusToApply(BSM.GetComponent<BattleStateMachine>().playersInBattle[i]);
                BSM.GetComponent<BattleStateMachine>().playersInBattle[i].GetComponent<PlayerStateMachine>().hasBeenCritted = hasCritted;
                BSM.GetComponent<BattleStateMachine>().playersInBattle[i].GetComponent<PlayerStateMachine>().TakeDamage(calculatedDamage, false);
            }
        }
        else
        {
            CheckForStatusToApply(playerToAttack);
            playerToAttack.GetComponent<PlayerStateMachine>().hasBeenCritted = hasCritted;
            playerToAttack.GetComponent<PlayerStateMachine>().TakeDamage(calculatedDamage, false);
        }

    }

    public void TakeDamage(int damageAmount, bool statusDamage)
    {

        if (damageAmount <= 1)
        {
            damageAmount = 1;
        }

        if (statusDamage)
        {
            enemy.curHP -= damageAmount;
            popups.Create(gameObject, damageAmount, true, false);
        }
        else {
            enemy.curHP -= (int)(damageAmount - (enemy.curDEF / 10)); //simple armor calculation
            popups.Create(gameObject, (int)(damageAmount - (enemy.curDEF / 10)), true, hasBeenCritted);
        }
 


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
                        attackedPlayer.GetComponent<PlayerStateMachine>().player.paralysed = true;
                        popups.CreateStatusText(attackedPlayer, BaseAttack.StatusEffects.PARALYSIS);

                    }
                    break;

                case (BaseAttack.StatusEffects.POISON):
                    if (num < BSM.performList[0].chosenAttack.applicationChance)
                    {
                        if (!attackedPlayer.GetComponent<PlayerStateMachine>().player.poisoned)
                        {
                            attackedPlayer.GetComponent<PlayerStateMachine>().player.poisoned = true;
                            popups.CreateStatusText(attackedPlayer, BaseAttack.StatusEffects.POISON);
                            attackedPlayer.GetComponent<PlayerStateMachine>().player.dotToTake += BSM.performList[0].chosenAttack.poisonDamage;
                        }
                        
                    }
                    break;

                case (BaseAttack.StatusEffects.BURN):
                    if (!attackedPlayer.GetComponent<PlayerStateMachine>().player.burned)
                    {
                        attackedPlayer.GetComponent<PlayerStateMachine>().player.burned = true;
                        popups.CreateStatusText(attackedPlayer, BaseAttack.StatusEffects.BURN);
                        attackedPlayer.GetComponent<PlayerStateMachine>().player.dotToTake += BSM.performList[0].chosenAttack.burnDamage;
                    }
                    break;

                case (BaseAttack.StatusEffects.FROSTBURN):
                    if (!attackedPlayer.GetComponent<PlayerStateMachine>().player.frostburned)
                    {
                        attackedPlayer.GetComponent<PlayerStateMachine>().player.frostburned = true;
                        popups.CreateStatusText(attackedPlayer, BaseAttack.StatusEffects.FROSTBURN);
                        attackedPlayer.GetComponent<PlayerStateMachine>().player.dotToTake += BSM.performList[0].chosenAttack.frostBurnDamage;
                    }
                    break;

                case (BaseAttack.StatusEffects.STUN):
                    if (num < BSM.performList[0].chosenAttack.applicationChance)
                    {
                        attackedPlayer.GetComponent<PlayerStateMachine>().player.stunned = true;
                        popups.CreateStatusText(attackedPlayer, BaseAttack.StatusEffects.STUN);
                    }
                    break;
            }
        }
    }

    bool CheckWhetherStatusAffectsTurn() {

        if (enemy.dotToTake > 0) {
            TakeDamage(enemy.dotToTake, true);
        }
        

        if (currentState == TurnState.DEAD)
        {
            return true;
        }

        if (enemy.paralysed)
        {
            return true;
        }

        else if (enemy.stunned) {

            isStunned = true;
            return true;
        }

        else
        {
            return false;
        }

    }

}