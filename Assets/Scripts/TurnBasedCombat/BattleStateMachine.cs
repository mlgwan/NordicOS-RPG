using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using TMPro;
using UnityEngine.SceneManagement;


public class BattleStateMachine : MonoBehaviour {
    public GameObject popupManager;
    DialogueHolder dHolder;
    SpeakerSprites speakerSprites;
    private bool pickedAnAbility;
    public List<Inventory.InventoryItem> consumableItemsCopy;

    int currentMenuOption = 0;
    public GameObject finger;
    public List<Transform> targetSelectorsList; // contains the positions where the cursor jumps to when menu-ing
    public List<Transform> actionSelectorsList; // contains the positions where the cursor jumps to when menu-ing
    public List<Transform> abilitySelectorsList; // contains the positions where the cursor jumps to when menu-ing
    public List<Transform> itemSelectorsList; // contains the positions where the cursor jumps to when menu-ing

	public enum BattleStates
    {
        STARTOFTURN,
        WAITING,
        TAKEACTION,
        PERFORMACTION,
        CHECKALIVE,
        WIN,
        LOSE,
        ESCAPED
    }

    public BattleStates currentState;

    public List<HandleTurn> performList = new List<HandleTurn>();

    public List<GameObject> enemiesInBattle = new List<GameObject>();
    public List<GameObject> totalEnemiesInBattle = new List<GameObject>();
    public List<GameObject> playersInBattle = new List<GameObject>();

    private bool enemiesHaveChosen;

    public GameObject itemAttack;

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
    public Transform itemsSpacer;
    public GameObject itemsPanel;

    public GameObject actionButton;
    private List<GameObject> actionButtons = new List<GameObject>();
    public GameObject abilityButton;
    public GameObject itemButton;

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

        foreach (Inventory.InventoryItem item in Inventory.instance.inventoryList) {
            
            if (item.item.GetType() == typeof(Consumable)) {
                consumableItemsCopy.Add(item);
            }
        }

        dHolder = GameObject.FindWithTag("MenuCanvas").GetComponent<DialogueHolder>();
        speakerSprites = SpeakerSprites.instance;
    }

    private void Start()
    {
        currentState = BattleStates.STARTOFTURN;

        playerInput = PlayerGUI.ACTIVATE;

        actionPanel.SetActive(false);
        selectPanel.SetActive(false);
        abilitiesPanel.SetActive(false);
        itemsPanel.SetActive(false);

        EnemyButtons();

        if (enemiesInBattle[0].name.Contains("Wolf"))
        {
            displayDialogue(new string[] { "A wolf...", "I will have to kill it..." }, new Sprite[] {speakerSprites.ulfNormal, speakerSprites.ulfAngry });
        }
        else if (enemiesInBattle[0].name.Contains("Warg")) {
            displayDialogue(new string[] { "A Warg!", "May Odins wisdom guide me." }, new Sprite[] { speakerSprites.ulfNormal, speakerSprites.ulfAngry });
        }
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
                if (!enemiesHaveChosen)
                            {
                                if (playersInBattle.Count == 0) {
                                    break;
                                }
                   
                                SetToChooseAction(playersInBattle, enemiesInBattle);
                                enemiesHaveChosen = true;
                            }


                else if (performList.Count == (enemiesInBattle.Count + playersInBattle.Count))
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

           

                break;


            case (BattleStates.WAITING):
                if (performList.Count > 0)
                {
                    currentState = BattleStates.TAKEACTION;
                }

                else {
                    enemiesHaveChosen = false;
                    currentState = BattleStates.CHECKALIVE;
                   
                }


                break;

            case (BattleStates.TAKEACTION):
                GameObject performer = GameObject.Find(performList[0].attackersName);



                if (performList[0].attackersType == "Enemy") {
                    
                    EnemyStateMachine ESM = performer.GetComponent<EnemyStateMachine>();


                    if (performList[0].chosenAttack.attackName == "Howl")
                    {
                        Debug.Log("HI");
                        ESM.playerToAttack = ESM.gameObject;
                        ESM.currentState = EnemyStateMachine.TurnState.ACTION;
                    }
                    else {

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
                if (enemiesInBattle.Count == 0)
                {
                    currentState = BattleStates.WIN;
                }

                else if (playersInBattle.Count == 0)
                {
                    currentState = BattleStates.LOSE;
                }

                break;

            case (BattleStates.CHECKALIVE):
                if (playersInBattle.Count < 1)
                {
                    displayDialogue(new string[] { "Hel..." }, new Sprite[] { speakerSprites.ulfTired });
                    currentState = BattleStates.LOSE;
                }
                else if (enemiesInBattle.Count < 1)
                {
                    displayDialogue(new string[] { "I survived..." }, new Sprite[] { speakerSprites.ulfAngry });
                    currentState = BattleStates.WIN;
                }
                else {
                    ClearActionPanel();
                    playerInput = PlayerGUI.ACTIVATE;
                    currentState = BattleStates.STARTOFTURN;
                }

                break;
                
            case (BattleStates.WIN):
                playerInput = PlayerGUI.WAITING;
                if (Input.GetKeyDown(ControlScript.instance.acceptButton)) {
                    GameObject.FindWithTag("MenuCanvas").transform.Find("DialogueManager").GetComponent<DialogueManager>().CloseDialogue();
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

                playerInput = PlayerGUI.WAITING;
                if (Input.GetKeyDown(ControlScript.instance.acceptButton))
                {
                    GameObject.FindWithTag("MenuCanvas").transform.Find("DialogueManager").GetComponent<DialogueManager>().CloseDialogue();
                    GameManager.instance.resetGame();
                    GameManager.instance.gameState = GameManager.GameStates.WORLD_STATE;
                    GameManager.instance.LoadSceneAfterBattle();
                    GameManager.instance.enemiesToBattle.Clear();
                    

                }

                

                break;

            case (BattleStates.ESCAPED):

                playerInput = PlayerGUI.WAITING;
                if (Input.GetKeyDown(ControlScript.instance.acceptButton))
                {
                    actionPanel.SetActive(false);
                    selectPanel.SetActive(false);
                    abilitiesPanel.SetActive(false);
                    itemsPanel.SetActive(false);
                    finger.SetActive(false);
                    GameObject.FindWithTag("MenuCanvas").transform.Find("DialogueManager").GetComponent<DialogueManager>().CloseDialogue();
                    for (int i = 0; i < playersInBattle.Count; i++) // Cowards don't get to keep their resources!
                    {
                        playersInBattle[i].GetComponent<PlayerStateMachine>().currentState = PlayerStateMachine.TurnState.WAITING;
                        GameManager.instance.heroes[i].GetComponent<PlayerStateMachine>().player = playersInBattle[i].GetComponent<PlayerStateMachine>().player;
                    }
                    GameManager.instance.LoadSceneAfterBattle();
                    GameManager.instance.gameState = GameManager.GameStates.WORLD_STATE;
                    GameManager.instance.enemiesToBattle.Clear();
                    
                }

                break;

      
        }
        if (!GameObject.FindWithTag("MenuCanvas").transform.Find("DialogueManager").GetComponent<DialogueManager>().dialogueActive) {
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
                    finger.transform.position = actionSelectorsList[currentMenuOption].position;
                    if (Input.GetKeyDown(ControlScript.instance.upButton) && currentMenuOption > 0)
                    {
                        currentMenuOption--;
                    }

                    if (Input.GetKeyDown(ControlScript.instance.downButton) && currentMenuOption < actionSelectorsList.Count -1)
                    {
                        currentMenuOption++;
                    }

                    if (Input.GetKeyDown(ControlScript.instance.acceptButton) && currentMenuOption == 0)
                    { // Basic Attack
                        Input1();
                        pickedAnAbility = false;
                        playerInput = PlayerGUI.SELECTTARGET;
                        currentMenuOption = 0;
                    }

                    if (Input.GetKeyDown(ControlScript.instance.acceptButton) && currentMenuOption == 1) // Abilities
                    {
                        Input3();
                        pickedAnAbility = true;
                        playerInput = PlayerGUI.ABILITIES;
                        currentMenuOption = 0;
                    }

                    if (Input.GetKeyDown(ControlScript.instance.acceptButton) && currentMenuOption == 2) // Items
                    {
                        Input5();
                        pickedAnAbility = false;
                        playerInput = PlayerGUI.ITEMS;
                        currentMenuOption = 0;
                    }
                    if (Input.GetKeyDown(ControlScript.instance.acceptButton) && currentMenuOption == 3) // Escape
                    {

                        Input7();
                    }


                    break;

                case (PlayerGUI.SELECTTARGET):
                    finger.transform.position = targetSelectorsList[currentMenuOption].position;

                    if (!enemiesInBattle[currentMenuOption].GetComponent<EnemyStateMachine>().enemySelector.activeInHierarchy) {
                        EnemyButtons();
                    }
                    enemiesInBattle[currentMenuOption].GetComponent<EnemyStateMachine>().enemySelector.SetActive(true);
                    if (Input.GetKeyDown(ControlScript.instance.upButton) && currentMenuOption > 0)
                    {
                        currentMenuOption--;
                        enemiesInBattle[currentMenuOption+1].GetComponent<EnemyStateMachine>().enemySelector.SetActive(false);
                        enemiesInBattle[currentMenuOption].GetComponent<EnemyStateMachine>().enemySelector.SetActive(true);
                    }

                    if (Input.GetKeyDown(ControlScript.instance.downButton) && currentMenuOption < enemiesInBattle.Count -1)
                    {
                        currentMenuOption++;
                        enemiesInBattle[currentMenuOption-1].GetComponent<EnemyStateMachine>().enemySelector.SetActive(false);
                        enemiesInBattle[currentMenuOption].GetComponent<EnemyStateMachine>().enemySelector.SetActive(true);
                    }

                    if (Input.GetKeyDown(ControlScript.instance.acceptButton))
                    {
                        foreach (GameObject enemy in enemiesInBattle)
                        {
                            enemy.GetComponent<EnemyStateMachine>().enemySelector.SetActive(false);
                        }
                        Input2(enemiesInBattle[currentMenuOption]);
                        playerInput = PlayerGUI.DONE;
                        currentMenuOption = 0;

                    }

                    if (Input.GetKeyDown(ControlScript.instance.escapeButton)) {
                        if (!pickedAnAbility)
                        {
                            playerInput = PlayerGUI.SELECTACTION;
                            actionPanel.SetActive(true);
                            selectPanel.SetActive(false);
                            foreach (GameObject enemy in enemiesInBattle) {
                                enemy.GetComponent<EnemyStateMachine>().enemySelector.SetActive(false);
                            }
                       

                        }
                        else
                        {
                            playerInput = PlayerGUI.ABILITIES;
                            abilitiesPanel.SetActive(true);
                            selectPanel.SetActive(false);
                        }
                        foreach (GameObject enemy in enemiesInBattle)
                        {
                            enemy.GetComponent<EnemyStateMachine>().enemySelector.SetActive(false);
                        }
                        currentMenuOption = 0;
                    }


                
                   
                    break;

                case (PlayerGUI.ABILITIES):

                    finger.transform.position = abilitySelectorsList[currentMenuOption].position;

                    if (Input.GetKeyDown(ControlScript.instance.upButton) && currentMenuOption > 0)
                    {
                        currentMenuOption-=1;
                    }

                    if (Input.GetKeyDown(ControlScript.instance.downButton) && currentMenuOption < abilitySelectorsList.Count -1)
                    {
                        currentMenuOption+=1;
                    }
                    /* for grid based menuing, 
                    if (Input.GetKeyDown(ControlScript.instance.leftButton) && currentMenuOption > 0 && currentMenuOption % 2 == 1 )
                    {
                        currentMenuOption-=1;
                    }

                    if (Input.GetKeyDown(ControlScript.instance.rightButton) && currentMenuOption < abilitySelectorsList.Count - 1 && currentMenuOption % 2 == 0)
                    {
                        currentMenuOption+=1;
                    }*/

                    if (Input.GetKeyDown(ControlScript.instance.acceptButton))
                    {
                        if (playersToManage[0].GetComponent<PlayerStateMachine>().player.curMP >= playersToManage[0].GetComponent<PlayerStateMachine>().player.abilities[currentMenuOption].attackCost)
                        {
                            Input4(playersToManage[0].GetComponent<PlayerStateMachine>().player.abilities[currentMenuOption]);
                            playerInput = PlayerGUI.SELECTTARGET;
                            currentMenuOption = 0;
                        }
                        else {
                            displayDialogue(new string[] { "I don't have enough stamina..." }, new Sprite[] { speakerSprites.ulfAngry });
                        }
                    }

                    if (Input.GetKeyDown(ControlScript.instance.escapeButton))
                    {
                        playerInput = PlayerGUI.SELECTACTION;
                        actionPanel.SetActive(true);
                        abilitiesPanel.SetActive(false);
                        itemsPanel.SetActive(false);
                        currentMenuOption = 1; // because the second button is the abilites button
                    }

               
                    break;

                case (PlayerGUI.ITEMS):

                    finger.transform.position = itemSelectorsList[currentMenuOption].position;



                        if (Input.GetKeyDown(ControlScript.instance.upButton) && currentMenuOption > 0)
                        {
                            currentMenuOption-=1;
                        }

                        if (Input.GetKeyDown(ControlScript.instance.downButton) && currentMenuOption < itemSelectorsList.Count -1)
                        {
                            currentMenuOption+=1;
                        }
                        /*
                        if (Input.GetKeyDown(ControlScript.instance.leftButton) && currentMenuOption > 0 && currentMenuOption % 2 == 1 )
                        {
                            currentMenuOption-=1;
                        }

                        if (Input.GetKeyDown(ControlScript.instance.rightButton) && currentMenuOption < itemSelectorsList.Count - 1 && currentMenuOption % 2 == 0)
                        {
                            currentMenuOption+=1;
                        }*/

                        if (Input.GetKeyDown(ControlScript.instance.acceptButton))
                        {
                        
                            Input6(consumableItemsCopy[currentMenuOption]);
                            playersToManage[0].GetComponent<PlayerStateMachine>().UpdateResourceBars();
                        }

                        if (Input.GetKeyDown(ControlScript.instance.escapeButton))
                        {
                            playerInput = PlayerGUI.SELECTACTION;
                            actionPanel.SetActive(true);
                            abilitiesPanel.SetActive(false);
                            itemsPanel.SetActive(false);
                            currentMenuOption = 2; // because the third button is the items button
                        }
                

                    break;
                case (PlayerGUI.DONE):
                    playerInputDone();
                    finger.SetActive(false);
                    break;
            }
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
 
        playerChoice.attackersName = playersToManage[0].GetComponent<PlayerStateMachine>().player.theName;
        playerChoice.attackersType = "Player";
        playerChoice.attackersGameObject = playersToManage[0];
        playerChoice.chosenAttack = chosenAbility;
        abilitiesPanel.SetActive(false);
        selectPanel.SetActive(true);

        

    }

    public void Input5() { // for opening up the items
        actionPanel.SetActive(false);
        itemsPanel.SetActive(true);
    }

    public void Input6(Inventory.InventoryItem item) {
        Consumable consumableItemsCopy = (Consumable) item.item;
        if (!consumableItemsCopy.isUsable(consumableItemsCopy, playersInBattle[0].GetComponent<PlayerStateMachine>().player))
        {
            item.item.UseItem(playersToManage[0]);
        }
        if (consumableItemsCopy.isUsable(consumableItemsCopy, playersInBattle[0].GetComponent<PlayerStateMachine>().player) && item.currentAmount > 0)
        {
            item.item.UseItem(playersToManage[0]);

            playerChoice.attackersName = playersToManage[0].GetComponent<PlayerStateMachine>().player.theName;
            playerChoice.attackersType = "Player";
            playerChoice.attackersGameObject = playersToManage[0];
            playerChoice.chosenAttack = itemAttack.GetComponent<ItemAttack>();
            playerChoice.chosenAttack.name = item.item.name;
            playerChoice.chosenAttack.attackDescription = item.item.description;
            playersInBattle[0].GetComponent<PlayerStateMachine>().player.stunned = true;
            itemsPanel.SetActive(false);
            selectPanel.SetActive(true);
            Input2(playersInBattle[0]);
        }

        else if (item.currentAmount == 0) {
            displayDialogue(new string[] { "I ran out of " + item.item.name + "s..." }, new Sprite[] { speakerSprites.ulfAngry });
        }
        
    }

    public void Input7() { // for escaping
        displayDialogue(new string[] { "I escaped..." }, new Sprite[] { speakerSprites.ulfTired });
        currentState = BattleStates.ESCAPED;
        for (int i = 0; i < GameManager.scriptedEncounters.Length; i++)
        {
            GameManager.scriptedEncounters[i] = true;
        }
        
        GameManager.instance.lastPlayerPosition -= new Vector3(1f,0.2f,0);
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
        itemSelectorsList.Clear();
    }

    void CreateActionButtons() {
        GameObject attackButton = Instantiate(actionButton) as GameObject;
        TextMeshProUGUI attackButtonText = attackButton.transform.Find("Text").gameObject.GetComponent<TextMeshProUGUI>();
        attackButtonText.text = "Attack";
        attackButton.GetComponent<Button>().onClick.AddListener(() => Input1());
        attackButton.transform.SetParent(actionSpacer, false);
        actionButtons.Add(attackButton);
        actionSelectorsList.Add(attackButton.transform.Find("Text").Find("ActionSelector"));

        GameObject abilitiesButton = Instantiate(actionButton) as GameObject; //This button is just the button for opening the abilties menu
        TextMeshProUGUI abilitiesButtonText = abilitiesButton.transform.Find("Text").gameObject.GetComponent<TextMeshProUGUI>();
        abilitiesButtonText.text = "Abilities";
        abilitiesButton.GetComponent<Button>().onClick.AddListener(() => Input3());
        abilitiesButton.transform.SetParent(actionSpacer, false);
        actionButtons.Add(abilitiesButton);
        actionSelectorsList.Add(abilitiesButton.transform.Find("Text").Find("ActionSelector"));

        GameObject itemsButton = Instantiate(actionButton) as GameObject;
        TextMeshProUGUI itemsButtonText = itemsButton.transform.Find("Text").gameObject.GetComponent<TextMeshProUGUI>();
        itemsButtonText.text = "Items";
        itemsButton.GetComponent<Button>().onClick.AddListener(() => Input5());
        itemsButton.transform.SetParent(actionSpacer, false);
        actionButtons.Add(itemsButton);
        actionSelectorsList.Add(itemsButton.transform.Find("Text").Find("ActionSelector"));

        GameObject escapeButton = Instantiate(actionButton) as GameObject;
        TextMeshProUGUI escapeButtonText = escapeButton.transform.Find("Text").gameObject.GetComponent<TextMeshProUGUI>();
        escapeButtonText.text = "Escape";
        escapeButton.GetComponent<Button>().onClick.AddListener(() => Input7());
        escapeButton.transform.SetParent(actionSpacer, false);
        actionButtons.Add(escapeButton);
        actionSelectorsList.Add(escapeButton.transform.Find("Text").Find("ActionSelector"));


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

        if (Inventory.instance.inventoryList.Count > 0) {
            foreach (Inventory.InventoryItem item in consumableItemsCopy) {                                 
                    GameObject item_Button = Instantiate(itemButton) as GameObject;
                    TextMeshProUGUI itemButtonText = item_Button.transform.Find("Text").gameObject.GetComponent<TextMeshProUGUI>();
                    TextMeshProUGUI itemButtonAmountText = item_Button.transform.Find("AmountText").gameObject.GetComponent<TextMeshProUGUI>();         
                    itemButtonText.text = item.item.name;
                    itemButtonAmountText.text = item.currentAmount.ToString();
                    ItemButton itemBtn = item_Button.GetComponent<ItemButton>();
                    itemBtn.itemToUse = item;
                    item_Button.transform.SetParent(itemsSpacer, false);
                    actionButtons.Add(item_Button);
                    itemSelectorsList.Add(item_Button.transform.Find("Text").Find("ItemSelector"));                
            }
        }
        else
        {
            itemsButton.GetComponent<Button>().interactable = false; // if the inventory is empty
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

    void displayDialogue(string[] lines, Sprite[] speakerSprites) {
        dHolder.dialogueLines = lines;
        dHolder.speakerSprites = speakerSprites;
        dHolder.DisplayBox();
    }

}
