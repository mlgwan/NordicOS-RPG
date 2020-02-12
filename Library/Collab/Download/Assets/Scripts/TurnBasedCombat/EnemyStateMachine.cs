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
    public GameObject blood;
    private Popups popups;
    private Animator animator;
    private Animator cameraAnimator;
   

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

    public int specialAttackCharges = 0;

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
    private bool isBleeding;

    private void Awake()
    {
        animator = gameObject.GetComponent<Animator>();
        cameraAnimator = GameObject.Find("Main Camera").GetComponent<Animator>();
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
                        for (int i = 0; i < BSM.performList.Count; i++)
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

            if (gameObject.name == "Warg 1")
            {
                if (specialAttackCharges > 1)
                {
                    myAttack.chosenAttack = enemy.abilities[1];
                    specialAttackCharges -= 1;
                }

                else if (specialAttackCharges == 1)
                {
                    myAttack.chosenAttack = enemy.abilities[0];
                    specialAttackCharges -= 1;
                }

                else if (specialAttackCharges == 0) {
                    if (num < 40)
                    {
                        myAttack.chosenAttack = enemy.basicAttack;
                    }
                    else 
                    {
                        myAttack.chosenAttack = enemy.abilities[Random.Range(0, enemy.abilities.Count)];
                    }
                }
            }

            else if (num < 30 || enemy.abilities.Count == 0 || specialAttackCharges == 0)
            {
                myAttack.chosenAttack = enemy.basicAttack;
            }


        if (myAttack.chosenAttack.name == "Howl") {
            myAttack.attackersTarget = this.gameObject;

        }


            BSM.CollectAction(myAttack);
            hasChosenAnAction = true;
    }

    private IEnumerator TimeForAction() {

        bool cameraCanMove = false;

        if (actionStarted) {
            yield break;
        }
        actionStarted = true;

        if (BSM.performList[0].chosenAttack.attackName == "Howl")
        {
            transform.Find("call_pack_ability_animation").gameObject.GetComponent<Animator>().Play("call pack", 0, 0.25f);
            DealDamage(BSM.performList[0].chosenAttack.isMagic);
            yield return new WaitForSeconds(0.53f);
        }

        else {

       

            bool statusCheck = CheckWhetherStatusAffectsTurn();
            Vector3 enemyPosition;
            if (!statusCheck)
            {
                enemyPosition = new Vector3(playerToAttack.transform.position.x - 1.5f, playerToAttack.transform.position.y, playerToAttack.transform.position.z);
                cameraCanMove = true;
            }
            else {
                enemyPosition = startPosition;
            }
            //animate the enemy near the hero to attack      
            animator.SetBool("shouldTriggerUpDown", false);
            animator.SetBool("shouldTriggerOpenMouth", false);
            animator.SetBool("shouldTriggerWalk", true);
            if (cameraCanMove) { cameraAnimator.SetBool("isEnemyAttack", true);}   
            while (MoveTowardsTarget(enemyPosition))
            {
                yield return null;
            }

            //wait a bit
            animator.SetBool("shouldTriggerWalk", false);
            animator.SetBool("shouldTriggerAttack", true);
            yield return new WaitForSeconds(0.75f);
       
            //do damage
            if (!statusCheck)
            {
                DealDamage(BSM.performList[0].chosenAttack.isMagic);
            }
            //animate back to start position
            Vector3 firstPosition = startPosition;
            cameraAnimator.SetBool("isEnemyAttack", false);
            while (MoveTowardsTarget(firstPosition))
            {
                yield return null;
            }
            animator.SetBool("shouldTriggerAttack", false);
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

    void DealDamage(bool isMagic)
    {
        if (BSM.performList[0].chosenAttack.attackName == "Howl")
        {
            if ( BSM.enemiesInBattle.Count < BSM.enemySpawnPoints.Count) {
            GameObject newEnemy = Instantiate(GameManager.instance.wolfPrefab, BSM.enemySpawnPoints[BSM.enemiesInBattle.Count].position, Quaternion.identity) as GameObject;
            newEnemy.name = newEnemy.GetComponent<EnemyStateMachine>().enemy.theName += " " + (BSM.enemiesInBattle.Count + 1);
            newEnemy.GetComponent<EnemyStateMachine>().enemy.theName = newEnemy.name;
            newEnemy.GetComponent<EnemyStateMachine>().enemy.SetupStats();

            BSM.enemiesInBattle.Add(newEnemy);
            BSM.totalEnemiesInBattle.Add(newEnemy);
            }
        }

        else
        {


            int attackScalingDamage;

            switch (BSM.performList[0].chosenAttack.scalesWith) //position 0 will always hold the current performer
            {
                case (BaseAttack.ScalesWith.NOTHING):
                    attackScalingDamage = 0;
                    break;

                case (BaseAttack.ScalesWith.STRENGTH):
                    attackScalingDamage = enemy.curSTR;
                    break;

                case (BaseAttack.ScalesWith.DEXTERITY):
                    attackScalingDamage = enemy.curDEX;
                    break;

                case (BaseAttack.ScalesWith.INT):
                    attackScalingDamage = enemy.curINT;
                    break;


                default:
                    attackScalingDamage = 0;
                    break;
            }

            int calculatedDamage = 0;

            if (!isMagic)
            {
                calculatedDamage = (int)(((enemy.curATK + attackScalingDamage + 1 / (playerToAttack.GetComponent<PlayerStateMachine>().player.curDEF + 1)) * BSM.performList[0].chosenAttack.attackDamage / 50) * Random.Range(BSM.performList[0].chosenAttack.lowerRandomBound, BSM.performList[0].chosenAttack.upperRandomBound));

                int n = Random.Range(0, 100);
                if (n < BSM.performList[0].chosenAttack.critChance)
                {
                    calculatedDamage = (int)(((enemy.curATK + attackScalingDamage + 1 / (playerToAttack.GetComponent<PlayerStateMachine>().player.curDEF + 1)) * BSM.performList[0].chosenAttack.attackDamage / 50) * BSM.performList[0].chosenAttack.upperRandomBound) * 2;

                    hasCritted = true;
                }
            }
            else
            {
                calculatedDamage = (int)(((enemy.curATK + attackScalingDamage + 1 / (playerToAttack.GetComponent<PlayerStateMachine>().player.curMR + 1)) * BSM.performList[0].chosenAttack.attackDamage / 50) * Random.Range(BSM.performList[0].chosenAttack.lowerRandomBound, BSM.performList[0].chosenAttack.upperRandomBound));
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
    }

    public void TakeDamage(int damageAmount, bool statusDamage)
    {
        GameObject bloodObject = Instantiate(blood);
        bloodObject.transform.position = new Vector3(gameObject.transform.position.x + 0.6f, gameObject.transform.position.y, gameObject.transform.position.z);
        bloodObject.GetComponent<ParticleSystem>().Play();
        animator.SetBool("shouldTakeDamage", true);
        animator.SetBool("shouldTriggerUpDown", false);
        animator.SetBool("shouldTriggerOpenMouth", false);

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
            enemy.curHP -= damageAmount;
            popups.Create(gameObject, damageAmount, true, hasBeenCritted);
        }
 


        if (enemy.curHP <= 0)
        {
            animator.SetBool("isDead", true);
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

                    if (num < BSM.performList[0].chosenAttack.applicationChance - attackedPlayer.GetComponent<PlayerStateMachine>().player.curParalysisResist)
                    {
                        attackedPlayer.GetComponent<PlayerStateMachine>().player.paralysed = true;
                        popups.CreateStatusText(attackedPlayer, BaseAttack.StatusEffects.PARALYSIS);

                    }
                    break;

                case (BaseAttack.StatusEffects.POISON):
                    if (num < BSM.performList[0].chosenAttack.applicationChance - attackedPlayer.GetComponent<PlayerStateMachine>().player.curPoisonResist)
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
                    if (num < BSM.performList[0].chosenAttack.applicationChance - attackedPlayer.GetComponent<PlayerStateMachine>().player.curBurnResist) { 
                        if (!attackedPlayer.GetComponent<PlayerStateMachine>().player.burned)
                        {
                            attackedPlayer.GetComponent<PlayerStateMachine>().player.burned = true;
                            popups.CreateStatusText(attackedPlayer, BaseAttack.StatusEffects.BURN);
                            attackedPlayer.GetComponent<PlayerStateMachine>().player.dotToTake += BSM.performList[0].chosenAttack.burnDamage;
                        }
                     }
                    break;

                case (BaseAttack.StatusEffects.FROSTBURN):
                    if (num < BSM.performList[0].chosenAttack.applicationChance - attackedPlayer.GetComponent<PlayerStateMachine>().player.curFrostburnResist)
                    {
                        if (!attackedPlayer.GetComponent<PlayerStateMachine>().player.frostburned)
                        {
                            attackedPlayer.GetComponent<PlayerStateMachine>().player.frostburned = true;
                            popups.CreateStatusText(attackedPlayer, BaseAttack.StatusEffects.FROSTBURN);
                            attackedPlayer.GetComponent<PlayerStateMachine>().player.dotToTake += BSM.performList[0].chosenAttack.frostBurnDamage;
                        }
                    }
                    break;

                case (BaseAttack.StatusEffects.STUN):
                    if (num < BSM.performList[0].chosenAttack.applicationChance - attackedPlayer.GetComponent<PlayerStateMachine>().player.curStunResist)
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
    public void resetTakeDamageAnimation()
    {
        animator.SetBool("shouldTakeDamage", false);
    }
}