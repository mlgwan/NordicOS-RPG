using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStateMachine : MonoBehaviour {

    private Animator animator;
    private BattleStateMachine BSM;
    public BasePlayer player;
    public GameObject selector;
    private Popups popups;

    private string originalText; //because stamina and MP are handled the same way this is used to reduce work


    public enum TurnState {
        CHOOSEACTION,
        WAITING,
        ACTION,
        SELECTING,
        DEAD

    }

    public TurnState currentState;

    private Image healthBar;
    private Image manaBar;
    public Sprite playerIcon;

    //positions for animations etc
    private Vector3 startPosition;
    public float targetOffset = 1.5f;

    //ienumerator stuff

    private bool actionStarted = false;
    public GameObject enemyToAttack;
    public float animSpeed = 12f;

    //death
    private bool dead;
    private bool hasCritted;
    public bool hasBeenCritted;

    //status effects
    private bool isPoisoned;
    private bool isBurned;
    private bool isFrostBurned;
    private bool isStunned;
    private bool isBleeding;
    public bool tickApplied;

    //GUI
    private PlayerPanelStats stats;
    public GameObject playerPanel;
    private Transform playerPanelSpacer;

    private void Awake()
    {
        popups = GameObject.Find("PopupManager").GetComponent<Popups>();
        animator = gameObject.GetComponent<Animator>();
    }

    private void Start()
    {
        playerPanelSpacer = GameObject.Find("BattleCanvas").transform.Find("PlayerPanel").transform.Find("PlayerBarSpacer");
        CreatePlayerPanel();
        startPosition = gameObject.transform.position;
        selector.SetActive(false);
        BSM = GameObject.Find("BattleManager").GetComponent<BattleStateMachine>();
        currentState = TurnState.WAITING;
    }



    void Update() {
        switch (currentState) {
            case (TurnState.CHOOSEACTION):
                BSM.playersToManage.Add(this.gameObject);
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
                    this.gameObject.tag = "DeadPlayer";

                    //set to not attackable by enemies
                    BSM.playersInBattle.Remove(this.gameObject);

                    //set to not manageable
                    BSM.playersToManage.Remove(this.gameObject);

                    //deactivate the selector
                    selector.SetActive(false);

                    //reset GUI
                    BSM.actionPanel.SetActive(false);
                    BSM.selectPanel.SetActive(false);

                    //remove item from performList
                    if (BSM.playersInBattle.Count > 0) {

                        for (int i = 1; i < BSM.performList.Count; i++) {
                            if (BSM.performList[i].attackersGameObject == this.gameObject) {
                                BSM.performList.Remove(BSM.performList[i]);
                                }
                            if (BSM.performList[i].attackersTarget == this.gameObject) {
                                BSM.performList[i].attackersTarget = BSM.playersInBattle[Random.Range(0, BSM.playersInBattle.Count)];
                                }
                        }
                    }
                   

                    //change color / player death-animation
                    this.gameObject.GetComponent<SpriteRenderer>().material.color = new Color32(102, 102, 102, 120);
                    this.gameObject.GetComponent<Animator>().enabled = false;
                    //reset playerInput
                    BSM.currentState = BattleStateMachine.BattleStates.CHECKALIVE;

                    dead = true;
                }

                break;

        }
    }

    public void UpdateResourceBars() {
        healthBar.transform.localScale = new Vector2(Mathf.Clamp(((float)(player.curHP) / (float)(player.maxHP)), 0, 1), healthBar.transform.localScale.y);
        stats.PlayerHP.text = "HP: " + player.curHP + " / " + player.maxHP;

        manaBar.transform.localScale = new Vector2(Mathf.Clamp(((float)(player.curMP) / (float)(player.maxMP)), 0, 1), healthBar.transform.localScale.y);
        stats.PlayerMP.text = originalText + player.curMP + " / " + player.maxMP;
    }

    private IEnumerator TimeForAction()
    {

        if (actionStarted)
        {
            yield break;
        }
        actionStarted = true;

        bool statusCheck = CheckWhetherStatusAffectsTurn();
        Vector3 playerPosition;
        if (!statusCheck)
        {
            playerPosition = new Vector3(enemyToAttack.transform.position.x + targetOffset, enemyToAttack.transform.position.y, enemyToAttack.transform.position.z);
        }
        else {
            playerPosition = startPosition;
        }
        //animate the player towards the enemy
        animator.SetBool("isWalkToAttack", true);
        while (MoveTowardsTarget(playerPosition))
        {
            yield return null;
        }

        //wait a bit
        animator.SetBool("isWalkToAttack", false);
        animator.SetBool("isAttack", true);
        yield return new WaitForSeconds(1.25f);
        animator.SetBool("isAttack", false);
        animator.SetBool("isWalkFromAttack", true);

        //do damage
        if (!statusCheck) {
            DealDamage(BSM.performList[0].chosenAttack.isMagic);
        }
       

        //animate back to startPosition
        Vector3 firstPosition = startPosition;
        while (MoveTowardsTarget(firstPosition)) {
            yield return null;
        }
        animator.SetBool("isWalkFromAttack", false);
        yield return new WaitForSeconds(1f);


        //remove this performer from the performList in BSM to not perform the action twice
        BSM.performList.RemoveAt(0);

        //reset BSM to WAITING
        if (BSM.currentState != BattleStateMachine.BattleStates.WIN && BSM.currentState != BattleStateMachine.BattleStates.LOSE)
        {
            BSM.currentState = BattleStateMachine.BattleStates.WAITING;
            currentState = TurnState.CHOOSEACTION;
        }
        else
        {
            currentState = TurnState.WAITING;
        }


        //end coroutine
        actionStarted = false;

        tickApplied = false;

        if (isStunned)
        {
            isStunned = false;
            player.stunned = false;
        }
        currentState = TurnState.WAITING;
   
    }

    private bool MoveTowardsTarget(Vector3 target)
    {
        return target != (transform.position = Vector3.MoveTowards(transform.position, target, animSpeed * Time.deltaTime));
    }

    void DealDamage(bool isMagic)
    {
        player.curMP -= BSM.performList[0].chosenAttack.attackCost;

        int attackScalingDamage;

        switch (BSM.performList[0].chosenAttack.scalesWith) //position 0 will always hold the current performer
        {
            case (BaseAttack.ScalesWith.NOTHING):
                attackScalingDamage = 0;
                break;

            case (BaseAttack.ScalesWith.STRENGTH):
                attackScalingDamage = player.curSTR;
                break;

            case (BaseAttack.ScalesWith.DEXTERITY):
                attackScalingDamage = player.curDEX;
                break;

            case (BaseAttack.ScalesWith.INT):
                attackScalingDamage = player.curINT;
                break;


            default:
                attackScalingDamage = 0;
                break;
        }

        int calculatedDamage = 0;

        if (!isMagic)
        {
            calculatedDamage = (int)(((player.curATK + attackScalingDamage + 1/ (enemyToAttack.GetComponent<EnemyStateMachine>().enemy.curDEF + 1)) * BSM.performList[0].chosenAttack.attackDamage / 50) * Random.Range(BSM.performList[0].chosenAttack.lowerRandomBound, BSM.performList[0].chosenAttack.upperRandomBound));


            int n = Random.Range(0, 100);
            if (n < BSM.performList[0].chosenAttack.critChance)
            {
                calculatedDamage = (int)(((player.curATK + attackScalingDamage + 1/ (enemyToAttack.GetComponent<EnemyStateMachine>().enemy.curDEF + 1)) * BSM.performList[0].chosenAttack.attackDamage / 50) * BSM.performList[0].chosenAttack.upperRandomBound) * 2;

                hasCritted = true;
            }

        }
        else {
            calculatedDamage = (int)(((player.curATK + attackScalingDamage + 1/ (enemyToAttack.GetComponent<EnemyStateMachine>().enemy.curMR + 1)) * BSM.performList[0].chosenAttack.attackDamage / 50) * Random.Range(BSM.performList[0].chosenAttack.lowerRandomBound, BSM.performList[0].chosenAttack.upperRandomBound));
        }


        if (BSM.performList[0].chosenAttack.isAOE)
        {
            for (int i = 0; i < BSM.GetComponent<BattleStateMachine>().enemiesInBattle.Count; i++)
            {
                CheckForStatusToApply(BSM.GetComponent<BattleStateMachine>().enemiesInBattle[i]);
                BSM.GetComponent<BattleStateMachine>().enemiesInBattle[i].GetComponent<EnemyStateMachine>().hasBeenCritted = hasCritted;
                BSM.GetComponent<BattleStateMachine>().enemiesInBattle[i].GetComponent<EnemyStateMachine>().TakeDamage(calculatedDamage, false);
            }
        }
        else {
            CheckForStatusToApply(enemyToAttack);
            enemyToAttack.GetComponent<EnemyStateMachine>().hasBeenCritted = hasCritted;
            enemyToAttack.GetComponent<EnemyStateMachine>().TakeDamage(calculatedDamage, false);
        }
        
        UpdateResourceBars();

    }

    public void TakeDamage(int damageAmount, bool statusDamage) {
        animator.SetBool("takeDamage", true);
        if (damageAmount <= 1) {
            damageAmount = 1;
        }

        if (statusDamage)
        {
            player.curHP -= damageAmount; //simple armor calculation
            popups.Create(gameObject, damageAmount, true, false);
        }

        else {
            player.curHP -= damageAmount; //simple armor calculation
            popups.Create(gameObject, damageAmount, true, hasBeenCritted);
        }
        

        if (player.curHP <= 0) {
            player.curHP = 0;
            currentState = TurnState.DEAD;
        }
        UpdateResourceBars();

    }

    void CreatePlayerPanel() {
        playerPanel = Instantiate(playerPanel) as GameObject;
        stats = playerPanel.GetComponent<PlayerPanelStats>();
        stats.PlayerName.text = player.theName;

        stats.PlayerHP.text = "HP: " +  player.curHP + " / " + player.maxHP;
        originalText = stats.PlayerMP.text;
        stats.PlayerMP.text = originalText + player.curMP + " / " + player.maxMP; //Because of the visual distinction between mana and stamina
        stats.PlayerLevel.text = player.level.ToString();
        
        healthBar = stats.PlayerHPBar;
        manaBar = stats.PlayerMPBar;
        stats.PlayerIcon = playerIcon;
        playerPanel.transform.SetParent(playerPanelSpacer, false);
        UpdateResourceBars();
    }

    public void CheckForStatusToApply(GameObject attackedEnemy)
    {

        if (BSM.performList[0].chosenAttack.statusEffectToApply == BaseAttack.StatusEffects.NONE)
        {
            return;
        }

        else
        {

            int num = Random.Range(1, 101);

            switch (BSM.performList[0].chosenAttack.statusEffectToApply)
            {

                case (BaseAttack.StatusEffects.PARALYSIS):

                    if (num < BSM.performList[0].chosenAttack.applicationChance - attackedEnemy.GetComponent<EnemyStateMachine>().enemy.curParalysisResist)
                    {
                        attackedEnemy.GetComponent<EnemyStateMachine>().enemy.paralysed = true;
                        popups.CreateStatusText(attackedEnemy, BaseAttack.StatusEffects.PARALYSIS);
                    }
                    break;

                case (BaseAttack.StatusEffects.POISON):
                    if (num < BSM.performList[0].chosenAttack.applicationChance - attackedEnemy.GetComponent<EnemyStateMachine>().enemy.curPoisonResist)
                    {
                        if (!attackedEnemy.GetComponent<EnemyStateMachine>().enemy.poisoned)
                        {
                            attackedEnemy.GetComponent<EnemyStateMachine>().enemy.poisoned = true;
                            popups.CreateStatusText(attackedEnemy, BaseAttack.StatusEffects.POISON);
                            attackedEnemy.GetComponent<EnemyStateMachine>().enemy.dotToTake += BSM.performList[0].chosenAttack.poisonDamage;
                        }
                    }
                    break;

                case (BaseAttack.StatusEffects.BURN):
                    if (num < BSM.performList[0].chosenAttack.applicationChance - attackedEnemy.GetComponent<EnemyStateMachine>().enemy.curBurnResist)
                    {
                        if (!attackedEnemy.GetComponent<EnemyStateMachine>().enemy.burned)
                        {
                            attackedEnemy.GetComponent<EnemyStateMachine>().enemy.burned = true;
                            popups.CreateStatusText(attackedEnemy, BaseAttack.StatusEffects.BURN);
                            attackedEnemy.GetComponent<EnemyStateMachine>().enemy.dotToTake += BSM.performList[0].chosenAttack.burnDamage;
                        }
                    }
                    break;

                case (BaseAttack.StatusEffects.FROSTBURN):
                    if (num < BSM.performList[0].chosenAttack.applicationChance - attackedEnemy.GetComponent<EnemyStateMachine>().enemy.curFrostburnResist)
                    {
                        if (!attackedEnemy.GetComponent<EnemyStateMachine>().enemy.frostburned)
                        {
                            attackedEnemy.GetComponent<EnemyStateMachine>().enemy.frostburned = true;
                            popups.CreateStatusText(attackedEnemy, BaseAttack.StatusEffects.FROSTBURN);
                            attackedEnemy.GetComponent<EnemyStateMachine>().enemy.dotToTake += BSM.performList[0].chosenAttack.frostBurnDamage;
                        }    
                    }
                    break;

                case (BaseAttack.StatusEffects.STUN):
                    if (num < BSM.performList[0].chosenAttack.applicationChance - attackedEnemy.GetComponent<EnemyStateMachine>().enemy.curStunResist)
                    {
                        attackedEnemy.GetComponent<EnemyStateMachine>().enemy.stunned = true;
                        popups.CreateStatusText(attackedEnemy, BaseAttack.StatusEffects.STUN);
                    }
                    break;
                case (BaseAttack.StatusEffects.BLEED):
                    if (num < BSM.performList[0].chosenAttack.applicationChance - attackedEnemy.GetComponent<EnemyStateMachine>().enemy.curFrostburnResist)
                    {
                        if (!attackedEnemy.GetComponent<EnemyStateMachine>().enemy.frostburned)
                        {
                            attackedEnemy.GetComponent<EnemyStateMachine>().enemy.frostburned = true;
                            popups.CreateStatusText(attackedEnemy, BaseAttack.StatusEffects.FROSTBURN);
                            attackedEnemy.GetComponent<EnemyStateMachine>().enemy.dotToTake += BSM.performList[0].chosenAttack.frostBurnDamage;
                        }
                    }
                    break;
            }
        }
    }

    bool CheckWhetherStatusAffectsTurn()
    {
        if (player.dotToTake != 0) {
            TakeDamage(player.dotToTake, true);
        }    

        if (currentState == TurnState.DEAD)
        {
            return true;
        }

        if (player.paralysed)
        {
            return true;
        }

        else if (player.stunned)
        {

            isStunned = true;
            return true;
        }

        else
        {
            return false;
        }

    }

    public void resetTakeDamageAnimation() {
        animator.SetBool("takeDamage", false);
    }
}
