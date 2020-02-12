using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour {

    public static GameManager instance;

    public GameObject wolfPrefab; // only because we want to get this over with

    public RegionData currentRegion;
    public static bool gameStarted = false;

    //dialogue boxes
    public GameObject dialogueBoxOverworld;
    public GameObject dialogueBoxBattle;

    //spawnpoints
    public string nextSpawnPoint;

    //player & heroes 
    public GameObject playerCharacter;

    public List<GameObject> heroes;
    public GameObject ulf;

    //Positions
    public Vector3 nextPlayerPosition;
    public Vector3 lastPlayerPosition;

    //Scenes
    public string sceneToLoad;
    public string lastScene;

    //bools
    public bool isWalking;
    public bool canGetEncountered;
    public bool gotAttacked;
    private static bool ulfWasAdded;
    public static bool firstAnimTriggered = false;

    public static bool[] scriptedEncounters = new bool[100];
    public List<GameObject> scriptedEncounterZones;
    

    public enum GameStates {
        WORLD_STATE,
        TOWN_STATE,
        DUNGEON_STATE,
        BATTLE_STATE,
        IDLE
    }

    //battle
    public List<GameObject> enemiesToBattle = new List<GameObject>();
    public int enemyAmount;

    public GameStates gameState;

    private void Awake()
    {

        if (!ulfWasAdded) { //ulf gets added once at the beginning of the game
            heroes.Clear();
            ulf.GetComponent<PlayerStateMachine>().player.reset();
            ulf.GetComponent<PlayerStateMachine>().player.LevelXPSetUp();
            ulf.GetComponent<PlayerStateMachine>().player.LevelUp();
            ulf.GetComponent<PlayerStateMachine>().player.restoreFully();
            heroes.Add(ulf);
            ulfWasAdded = true;

        }

        //to make sure there is only one instance of our GameManager
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this) {
            Destroy(gameObject);
        }

       
        if (!GameObject.FindWithTag("Player")) {
            GameObject player = Instantiate(playerCharacter, nextPlayerPosition, Quaternion.identity) as GameObject;
            player.name = "UlfWrapper";
        }
       
        
        
       
        DontDestroyOnLoad(gameObject);
    }



    private void Update()
    {
        switch (gameState)
        {
            case (GameStates.WORLD_STATE): FindEncounterZones();
                if (isWalking) {
                    RandomEncounter();
                }
                if (gotAttacked) {
           
                    gameState = GameStates.BATTLE_STATE;
 
                }
                break;

            case (GameStates.TOWN_STATE):

                break;

            case (GameStates.DUNGEON_STATE):

                break;

            case (GameStates.BATTLE_STATE):
                StartBattle();
                GameObject.FindWithTag("MenuCanvas").transform.Find("DialogueManager").GetComponent<DialogueManager>().dialogueBox = dialogueBoxBattle;
                GameObject.FindWithTag("MenuCanvas").transform.Find("DialogueManager").GetComponent<DialogueManager>().dialogueText = dialogueBoxBattle.transform.Find("DialogueText").GetComponent<TextMeshProUGUI>();
                GameObject.FindWithTag("MenuCanvas").transform.Find("DialogueManager").GetComponent<DialogueManager>().speakerImage = dialogueBoxBattle.transform.Find("SpeakerImage").GetComponent<Image>();
                GameObject.FindWithTag("MenuCanvas").GetComponent<DialogueHolder>().refresh();
                gameState = GameStates.IDLE;

                break;

            case (GameStates.IDLE):

                break;       
        }
    }

    public void CheckScriptedEncounters() {
        for (int i = 0; i < scriptedEncounterZones.Count; i++) {
            if (scriptedEncounters[i+1])
            {
                if (scriptedEncounterZones[i]!= null) {  scriptedEncounterZones[i].SetActive(true);}
               
            }
            else {
                if (scriptedEncounterZones[i] != null) { scriptedEncounterZones[i].SetActive(false);}
                
            }
        }
    }

    public void LoadNextScene() {
        SceneManager.LoadScene(sceneToLoad);
    }

    public void LoadSceneAfterBattle() {
        SceneManager.LoadScene(lastScene);
        GameObject.FindWithTag("MenuCanvas").transform.Find("DialogueManager").GetComponent<DialogueManager>().dialogueBox = dialogueBoxOverworld;
        GameObject.FindWithTag("MenuCanvas").transform.Find("DialogueManager").GetComponent<DialogueManager>().dialogueText = dialogueBoxOverworld.transform.Find("DialogueText").GetComponent<TextMeshProUGUI>();
        GameObject.FindWithTag("MenuCanvas").transform.Find("DialogueManager").GetComponent<DialogueManager>().speakerImage = dialogueBoxOverworld.transform.Find("SpeakerImage").GetComponent<Image>();
    }

    void RandomEncounter() {
        if (isWalking && canGetEncountered)
        {
            if (currentRegion != null && currentRegion.isScriptedEncounter) {
                
                gotAttacked = true;
                scriptedEncounters[currentRegion.regionId] = false;
            }
            else if (Random.Range(0, 1000) < 10)
            {
                Debug.Log("I got attacked");
                gotAttacked = true;
            }
         
        }

         
        
    }

    void StartBattle() {
        //Amount of enemies
        enemyAmount = Random.Range(1,currentRegion.maxAmountEnemies + 1);
        //which enemies
        for (int i = 0; i < enemyAmount; i++) {
            enemiesToBattle.Add(currentRegion.possibleEnemies[Random.Range(0, currentRegion.possibleEnemies.Count)]);
        }

        lastPlayerPosition = GameObject.Find("PlayerCharacter").gameObject.transform.position;
        nextPlayerPosition = lastPlayerPosition;
        lastScene = SceneManager.GetActiveScene().name;

        //load level
        SceneManager.LoadScene(currentRegion.battleScene);
        //reset hero
        isWalking = false;
        gotAttacked = false;
        canGetEncountered = false;
    }

    void FindEncounterZones() {
        scriptedEncounterZones[0] = GameObject.Find("ScriptedEncounterZone1");
        scriptedEncounterZones[1] = GameObject.Find("ScriptedEncounterZone2");
    }

    public void resetGame() {
        ulfWasAdded = false;
        gameStarted = false;
        isWalking = false;
        canGetEncountered = false;
        gotAttacked = false;
        for (int i = 0; i < scriptedEncounters.Length; i++) {
            scriptedEncounters[i] = false;
        }
        lastPlayerPosition = new Vector3(0, 0, 0);
        nextPlayerPosition = new Vector3(30.075f, 0f, 14.136f);
    }


    
}
