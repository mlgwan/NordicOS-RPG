using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStateMachine : MonoBehaviour {

    private BattleStateMachine BSM;
    public BasePlayer player;
    public GameObject selector;

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

    //GUI
    private PlayerPanelStats stats;
    public GameObject playerPanel;
    private Transform playerPanelSpacer;

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
        healthBar.transform.localScale = new Vector2(Mathf.Clamp(((float)(player.curHP) / (float)(player.baseHP)), 0, 1), healthBar.transform.localScale.y);
        stats.PlayerHP.text = "HP: " + player.curHP + " / " + player.baseHP;

        manaBar.transform.localScale = new Vector2(Mathf.Clamp(((float)(player.curMP) / (float)(player.baseMP)), 0, 1), healthBar.transform.localScale.y);
        stats.PlayerMP.text = originalText + player.curMP + " / " + player.baseMP;
    }

    private IEnumerator TimeForAction()
    {

        if (actionStarted)
        {
            yield break;
        }

        actionStarted = true;

        //animate the enemy towards the enemy
        Vector3 playerPosition = new Vector3(enemyToAttack.transform.position.x + targetOffset, enemyToAttack.transform.position.y, enemyToAttack.transform.position.z);
        while (MoveTowardsTarget(playerPosition))
        {
            yield return null;
        }

        //wait a bit
        yield return new WaitForSeconds(0.5f);

        //do damage
        DealDamage();

        //animate back to startPosition
        Vector3 firstPosition = startPosition;
        while (MoveTowardsTarget(firstPosition)) { yield return null; }

        //remove this performer from the performList in BSM to not perform the action twice
        BSM.performList.RemoveAt(0);

        //reset BSM to WAITING
        if (BSM.currentState != BattleStateMachine.BattleStates.WIN && BSM.currentState != BattleStateMachine.BattleStates.LOSE)
        {
            BSM.currentState = BattleStateMachine.BattleStates.WAITING;
            currentState = TurnState.CHOOSEACTION;
        }
        else {
            currentState = TurnState.WAITING;
        }
        

        //end coroutine
        actionStarted = false;

        //reset this players state
        currentState = TurnState.WAITING;
    }

    private bool MoveTowardsTarget(Vector3 target)
    {
        return target != (transform.position = Vector3.MoveTowards(transform.position, target, animSpeed * Time.deltaTime));
    }

    void DealDamage()
    {
        int calculatedDamage = (int)((player.curATK / 10 + 1) * BSM.performList[0].chosenAttack.attackDamage); //simple damage formula, position 0 will always hold the current performer
        player.curMP -= BSM.performList[0].chosenAttack.attackCost;
        if (BSM.performList[0].chosenAttack.isAOE)
        {
            for (int i = 0; i < BSM.GetComponent<BattleStateMachine>().enemiesInBattle.Count; i++)
            {
                BSM.GetComponent<BattleStateMachine>().enemiesInBattle[i].GetComponent<EnemyStateMachine>().TakeDamage(calculatedDamage);
            }
        }
        else {
            enemyToAttack.GetComponent<EnemyStateMachine>().TakeDamage(calculatedDamage);
        }
        
        UpdateResourceBars();

    }

    public void TakeDamage(int damageAmount) {

        if (damageAmount <= 1) {
            player.curHP -= 1;
        }

        player.curHP -= (int)(damageAmount - (player.curDEF / 10)); //simple armor calculation
        

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

        stats.PlayerHP.text = "HP: " +  player.curHP + " / " + player.baseHP;
        originalText = stats.PlayerMP.text;
        stats.PlayerMP.text = originalText + player.curMP + " / " + player.baseMP; //Because of the visual distinction between mana and stamina
        stats.PlayerLevel.text = player.level.ToString();
        
        healthBar = stats.PlayerHPBar;
        manaBar = stats.PlayerMPBar;
        stats.PlayerIcon = playerIcon;
        playerPanel.transform.SetParent(playerPanelSpacer, false);
        UpdateResourceBars();
    }
}
