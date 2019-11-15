using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class BattleStateMachine : MonoBehaviour {

    ControlScript controls = ControlScript.instance;

    private bool pickedAnAbility;

    int currentMenuOption = 0;
    public GameObject finger;
    public List<Transform> targetSelectorsList; // contains the positions where the cursor jumps to when menu-ing
    public List<Transform> actionSelectorsList; // contains the positions where the cursor jumps to when menu-ing
    public List<Transform> abilitySelectorsList; // contains the positions where the cursor jumps to when menu-ing

	public enum BattleStates
    {
        STARTOFTURN,
        WAITING,
        TAKEACTION,
        PERFORMACTION,
        CHECKALIVE,
        WIN,
        LOSE
    }

    public BattleStates currentState;

    public List<HandleTurn> performList = new List<HandleTurn>();

    public List<GameObject> enemiesInBattle = new List<GameObject>();
    public List<GameObject> totalEnemiesInBattle = new List<GameObject>();
    public List<GameObject> playersInBattle = new List<GameObject>();

    private bool enemiesHaveChosen;

    //GUI stuff
    public enum PlayerGUI {
        ACTIVATE,
        WAITING, //idle
        SELECTTARGET,
        SELECTACTION, 
        ABILITIES, 
        ITEMS,
        DONE

    }

    public PlayerGUI playerInput;
    public List<GameObject> playersToManage = new List<GameObject>();
    private HandleTurn playerChoice;
    public GameObject enemyButton;


    public Transform selectSpacer;
    public GameObject selectPanel;
    public Transform actionSpacer;
    public GameObject actionPanel;
    public Transform abilitiesSpacer;
    public GameObject abilitiesPanel;

    public GameObject actionButton;
    private List<GameObject> actionButtons = new List<GameObject>();
    public GameObject abilityButton;

    //enemy buttons
    private List<GameObject> enemyButtons = new List<GameObject>();

    //spawnpoints
    public List<Transform> enemySpawnPoints = new List<Transform>();
    public List<Transform> playerSpawnPoints = new List<Transform>();

    private void Awake()
    {
        finger.SetActive(false);

        for (int i = 0; i < GameManager.instance.enemyAmount; i++) {
            GameObject newEnemy = Instantiate(GameManager.instance.enemiesToBattle[i], enemySpawnPoints[i].position, Quaternion.identity) as GameObject;
            newEnemy.name = newEnemy.GetComponent<EnemyStateMachine>().enemy.theName += " " + (i + 1);
            newEnemy.GetComponent<EnemyStateMachine>().enemy.theName = newEnemy.name;
            newEnemy.GetComponent<EnemyStateMachine>().enemy.SetupStats();
           
            enemiesInBattle.Add(newEnemy);
        }

        totalEnemiesInBattle = new List<GameObject>(enemiesInBattle);

        for (int i = 0; i < GameManager.instance.heroes.Count; i++) {
            GameObject newPlayer = Instantiate(GameManager.instance.heroes[i], playerSpawnPoints[i].position, Quaternion.identity) as GameObject;
            newPlayer.name = GameManager.instance.heroes[i].GetComponent<PlayerStateMachine>().player.theName;
            playersInBattle.Add(newPlayer);

        }
    }

    private void Start()
    {
        currentState = BattleStates.STARTOFTURN;

        playerInput = PlayerGUI.ACTIVATE;

        actionPanel.SetActive(false);
        selectPanel.SetActive(false);
        abilitiesPanel.SetActive(false);

        EnemyButtons();
    }

    private void Update()
    {
        switch (currentState) {
            case (BattleStates.STARTOFTURN):

                if (enemiesInBattle.Count == 0)
                {
                    currentState = BattleStates.WIN;
                }

                else if (playersInBattle.Count == 0) {
                    currentState = BattleStates.LOSE;
                }



                if (performList.Count == (enemiesInBattle.Count + playersInBattle.Count))
                {
                  
                    performList.Sort(delegate (HandleTurn performer1, HandleTurn performer2) //Sort the performlist by the performers speed
                    {
                        if (performer1.attackersType == "Player"&& performer2.attackersType == "Player")
                        {
                            return performer1.attackersGameObject.GetComponent<PlayerStateMachine>().player.curSPD.CompareTo(performer2.attackersGameObject.GetComponent<PlayerStateMachine>().player.curSPD);
                        }

                        else if (performer1.attackersType == "Player" && performer2.attackersType == "Enemy")
                        {
                            return performer1.attackersGameObject.GetComponent<PlayerStateMachine>().player.curSPD.CompareTo(performer2.attackersGameObject.GetComponent<EnemyStateMachine>().enemy.curSPD);
                        }

                        else if (performer1.attackersType == "Enemy" && performer2.attackersType == "Player")
                        {
                            return performer1.attackersGameObject.GetComponent<EnemyStateMachine>().enemy.curSPD.CompareTo(performer2.attackersGameObject.GetComponent<PlayerStateMachine>().player.curSPD);
                        }

                        else
                        {
                            return performer1.attackersGameObject.GetComponent<EnemyStateMachine>().enemy.curSPD.CompareTo(performer2.attackersGameObject.GetComponent<EnemyStateMachine>().enemy.curSPD);
                        }
                    });

                    performList.Reverse(); // so the fastest one is at index 0
                    currentState = BattleStates.WAITING;
                }

                if (!enemiesHaveChosen)
                {
                   
                    SetToChooseAction(playersInBattle, enemiesInBattle);
                    enemiesHaveChosen = true;
                }

                break;


            case (BattleStates.WAITING):
                if (performList.Count > 0)
                {
                    currentState = BattleStates.TAKEACTION;
                }

                else {
                    enemiesHaveChosen = false;
                    currentState = BattleStates.STARTOFTURN;
                   
                }


                break;

            case (BattleStates.TAKEACTION):
                GameObject performer = GameObject.Find(performList[0].attackersName);

 


                    if (performList[0].attackersType == "Enemy") {
                    
                        EnemyStateMachine ESM = performer.GetComponent<EnemyStateMachine>();


                                for (int i = 0; i < playersInBattle.Count; i++)
                                {
                                    if (performList[0].attackersTarget == playersInBattle[i])
                                    {

                                        ESM.playerToAttack = performList[0].attackersTarget;
                                        ESM.currentState = EnemyStateMachine.TurnState.ACTION;

                                    }
                                    else
                                    {

                                        performList[0].attackersTarget = playersInBattle[Random.Range(0, playersInBattle.Count)];
                                        ESM.playerToAttack = performList[0].attackersTarget;
                                        ESM.currentState = EnemyStateMachine.TurnState.ACTION;
                                    }

                        }        

                    }

                    if (performList[0].attackersType == "Player")
                    {
                        PlayerStateMachine PSM = performer.GetComponent<PlayerStateMachine>();
                        PSM.enemyToAttack = performList[0].attackersTarget;
                        PSM.currentState = PlayerStateMachine.TurnState.ACTION;
                    }

                    currentState = BattleStates.PERFORMACTION;
                


                break;

            case (BattleStates.PERFORMACTION):
   
                break;

            case (BattleStates.CHECKALIVE):
                if (playersInBattle.Count < 1)
                {
                    currentState = BattleStates.LOSE;
                }
                else if (enemiesInBattle.Count < 1)
                {
                    Debug.Log("You won! Press left mouse button to continue.");
                    currentState = BattleStates.WIN;
                }
                else {
                    ClearActionPanel();
                    playerInput = PlayerGUI.ACTIVATE;
                    currentState = BattleStates.WAITING;
                }

                break;
                
            case (BattleStates.WIN):
                playerInput = PlayerGUI.WAITING;
                if (Input.GetMouseButtonDown(0)) { //will change this to button press or something similar
                    for (int i = 0; i < playersInBattle.Count; i++) //Reward the players heroes with experience
                    {
                        playersInBattle[i].GetComponent<PlayerStateMachine>().currentState = PlayerStateMachine.TurnState.WAITING;
                        RewardExperience(playersInBattle[i], totalEnemiesInBattle);
                        GameManager.instance.heroes[i].GetComponent<PlayerStateMachine>().player = playersInBattle[i].GetComponent<PlayerStateMachine>().player;
                    }

                    GameManager.instance.LoadSceneAfterBattle();
                    GameManager.instance.gameState = GameManager.GameStates.WORLD_STATE;
                    GameManager.instance.enemiesToBattle.Clear();
                }

                break;

            case (BattleStates.LOSE):
                break;

      
        }

        switch (playerInput) {
            case (PlayerGUI.ACTIVATE):



                if (playersToManage.Count > 0) {
                    playersToManage[0].transform.Find("Selector").gameObject.SetActive(true);
                    playerChoice = new HandleTurn();

                    currentMenuOption = 0;

                    actionPanel.SetActive(true);
                    CreateActionButtons();
                    playerInput = PlayerGUI.SELECTACTION;
                    finger.SetActive(true);
                }

            break;

            case (PlayerGUI.WAITING):

                break;

            case (PlayerGUI.SELECTACTION):
                if (Input.GetKeyDown(controls.upButton) && currentMenuOption > 0)
                {
                    currentMenuOption--;
                }

                if (Input.GetKeyDown(controls.downButton) && currentMenuOption < actionSelectorsList.Count)
                {
                    currentMenuOption++;
                }

                if (Input.GetKeyDown(controls.acceptButton) && currentMenuOption == 0)
                { // Basic Attack
                    Input1();
                    pickedAnAbility = false;
                    playerInput = PlayerGUI.SELECTTARGET;
                    currentMenuOption = 0;
                }

                if (Input.GetKeyDown(controls.acceptButton) && currentMenuOption == 1) // Abilities
                {
                    Input3();
                    pickedAnAbility = true;
                    playerInput = PlayerGUI.ABILITIES;
                    currentMenuOption = 0;
                }

                finger.transform.position = actionSelectorsList[currentMenuOption].position;
                break;

            case (PlayerGUI.SELECTTARGET):

                if (Input.GetKeyDown(controls.upButton) && currentMenuOption > 0)
                {
                    currentMenuOption--;
                }

                if (Input.GetKeyDown(controls.downButton) && currentMenuOption < targetSelectorsList.Count)
                {
                    currentMenuOption++;
                }

                if (Input.GetKeyDown(controls.acceptButton))
                {
                    Input2(enemiesInBattle[currentMenuOption]);
                    playerInput = PlayerGUI.DONE;
                    currentMenuOption = 0;
                }

                if (Input.GetKeyDown(controls.escapeButton)) {
                    if (!pickedAnAbility)
                    {
                        playerInput = PlayerGUI.SELECTACTION;
                        actionPanel.SetActive(true);
                        selectPanel.SetActive(false);
                    }
                    else
                    {
                        playerInput = PlayerGUI.ABILITIES;
                        abilitiesPanel.SetActive(true);
                        selectPanel.SetActive(false);
                    }
                    currentMenuOption = 0;
                }

                finger.transform.position = targetSelectorsList[currentMenuOption].position;
                break;

            case (PlayerGUI.ABILITIES):
                if (Input.GetKeyDown(controls.upButton) && currentMenuOption > 0)
                {
                    currentMenuOption--;
                }

                if (Input.GetKeyDown(controls.downButton) && currentMenuOption < abilitySelectorsList.Count)
                {
                    currentMenuOption++;
                }

                if (Input.GetKeyDown(controls.acceptButton))
                {
                    Input4(playersToManage[0].GetComponent<PlayerStateMachine>().player.abilities[currentMenuOption]);
                    playerInput = PlayerGUI.SELECTTARGET;
                    currentMenuOption = 0;
                }

                if (Input.GetKeyDown(controls.escapeButton))
                {
                    playerInput = PlayerGUI.SELECTACTION;
                    actionPanel.SetActive(true);
                    abilitiesPanel.SetActive(false);
                    currentMenuOption = 1; // because the second button is the abilites button
                }

                finger.transform.position = abilitySelectorsList[currentMenuOption].position;
                break;
            case (PlayerGUI.DONE):
                playerInputDone();
                finger.SetActive(false);
                break;
        }
    }

    public void CollectAction(HandleTurn input) {
        performList.Add(input);
    }

    public void EnemyButtons() {

        //cleanup
        foreach (GameObject enemyBtn in enemyButtons) {
            Destroy(enemyBtn);
        }
        enemyButtons.Clear();
        targetSelectorsList.Clear();

        //create buttons
        foreach (GameObject enemy in enemiesInBattle) {
            GameObject newButton = Instantiate(enemyButton) as GameObject;
            TargetButton button = newButton.GetComponent<TargetButton>();
            targetSelectorsList.Add(newButton.transform.Find("Text").Find("TargetSelector"));
            EnemyStateMachine curEnemy = enemy.GetComponent<EnemyStateMachine>();

            TextMeshProUGUI buttonText = newButton.transform.Find("Text").gameObject.GetComponent<TextMeshProUGUI>();
            buttonText.text = curEnemy.enemy.theName;

            button.targetPrefab = enemy;

            newButton.transform.SetParent(selectSpacer, false);
            enemyButtons.Add(newButton);
        }
    }

    public void Input1() { //for the attack button, will be reworked
        playerChoice.attackersName = playersToManage[0].GetComponent<PlayerStateMachine>().player.theName;
        playerChoice.attackersType = "Player";
        playerChoice.attackersGameObject = playersToManage[0];
        playerChoice.chosenAttack = playersToManage[0].GetComponent<PlayerStateMachine>().player.basicAttack;
        actionPanel.SetActive(false);
        selectPanel.SetActive(true);

    }

    public void Input2(GameObject chosenTarget) { //for the target selection , will be reworked
        playerChoice.attackersTarget = chosenTarget;
        playerInput = PlayerGUI.DONE;
    }

    public void Input3() { //for opening the abilities up, will be reworked
        actionPanel.SetActive(false);
        abilitiesPanel.SetActive(true);

    }

    public void Input4(BaseAttack chosenAbility) { //for choosing an ability, will be reworked
        if(playersToManage[0].GetComponent<PlayerStateMachine>().player.curMP >= chosenAbility.attackCost)
        {
        playerChoice.attackersName = playersToManage[0].GetComponent<PlayerStateMachine>().player.theName;
        playerChoice.attackersType = "Player";
        playerChoice.attackersGameObject = playersToManage[0];
        playerChoice.chosenAttack = chosenAbility;
        abilitiesPanel.SetActive(false);
        selectPanel.SetActive(true);

        }

    }


    void playerInputDone() {
        performList.Add(playerChoice);
        ClearActionPanel();
        playersToManage[0].transform.Find("Selector").gameObject.SetActive(false);
        playersToManage.RemoveAt(0);
        playerInput = PlayerGUI.ACTIVATE;
    }

    void SetToChooseAction(List<GameObject> players, List<GameObject> enemies) {
        foreach (GameObject player in players) {
            player.GetComponent<PlayerStateMachine>().currentState = PlayerStateMachine.TurnState.CHOOSEACTION;
        }

        foreach (GameObject enemy in enemies) {
            if (!enemy.GetComponent<EnemyStateMachine>().hasChosenAnAction)
            {
                enemy.GetComponent<EnemyStateMachine>().currentState = EnemyStateMachine.TurnState.CHOOSEACTION;
                enemy.GetComponent<EnemyStateMachine>().hasChosenAnAction = true;
            }
       }
    }

    void ClearActionPanel() {
        selectPanel.SetActive(false);
        actionPanel.SetActive(false);
        abilitiesPanel.SetActive(false);

        foreach (GameObject actionButton in actionButtons)
        {
            Destroy(actionButton);
        }
        actionButtons.Clear();
        actionSelectorsList.Clear();
        abilitySelectorsList.Clear();
    }

    void CreateActionButtons() {
        GameObject attackButton = Instantiate(actionButton) as GameObject;
        TextMeshProUGUI attackButtonText = attackButton.transform.Find("Text").gameObject.GetComponent<TextMeshProUGUI>();
        attackButtonText.text = "Attack";
        attackButton.GetComponent<Button>().onClick.AddListener(() => Input1());
        attackButton.transform.SetParent(actionSpacer, false);
        actionButtons.Add(attackButton);
        actionSelectorsList.Add(attackButton.transform.Find("Text").Find("ActionSelector"));

        GameObject abilitiesButton = Instantiate(actionButton) as GameObject;
        TextMeshProUGUI abilitiesButtonText = abilitiesButton.transform.Find("Text").gameObject.GetComponent<TextMeshProUGUI>();
        abilitiesButtonText.text = "Abilities";
        abilitiesButton.GetComponent<Button>().onClick.AddListener(() => Input3());
        abilitiesButton.transform.SetParent(actionSpacer, false);
        actionButtons.Add(abilitiesButton);
        actionSelectorsList.Add(abilitiesButton.transform.Find("Text").Find("ActionSelector"));


        if (playersToManage[0].GetComponent<PlayerStateMachine>().player.abilities.Count > 0) {
            foreach (BaseAttack ability in playersToManage[0].GetComponent<PlayerStateMachine>().player.abilities) {
                GameObject ability_Button = Instantiate(abilityButton) as GameObject;
                TextMeshProUGUI abilityButtonText = ability_Button.transform.Find("Text").gameObject.GetComponent<TextMeshProUGUI>();
                abilityButtonText.text = ability.attackName;
                AbilityButton abBtn = ability_Button.GetComponent<AbilityButton>();
                abBtn.abilityToPerform = ability;
                ability_Button.transform.SetParent(abilitiesSpacer, false);
                actionButtons.Add(ability_Button);
                abilitySelectorsList.Add(ability_Button.transform.Find("Text").Find("AbilitySelector"));
            }

        }
        else
        {
            abilitiesButton.GetComponent<Button>().interactable = false; // if a player knows no abilties
        }
    }

    public void RewardExperience(GameObject player, List<GameObject> enemiesInBattle)
    {

        for (int i = 0; i < enemiesInBattle.Count; i++)
        {
            player.GetComponent<PlayerStateMachine>().player.currentExperiencePoints += enemiesInBattle[i].GetComponent<EnemyStateMachine>().enemy.experienceToReward;
            player.GetComponent<PlayerStateMachine>().player.totalExperiencePoints += enemiesInBattle[i].GetComponent<EnemyStateMachine>().enemy.experienceToReward;
            while (player.GetComponent<PlayerStateMachine>().player.currentExperiencePoints >= player.GetComponent<PlayerStateMachine>().player.toLevelUp[player.GetComponent<PlayerStateMachine>().player.level]) //level up
            {
                player.GetComponent<PlayerStateMachine>().player.LevelUp();
            }

          
        }
    }

}
