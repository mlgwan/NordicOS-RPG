using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

    public static GameManager instance;

    public RegionData currentRegion;

    //spawnpoints
    public string nextSpawnPoint;

    //hero
    public GameObject playerCharacter;

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
        //to make sure there is only one instance of our GameManager
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this) {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);
        if (!GameObject.Find("Player")) {
            GameObject player = Instantiate(playerCharacter, nextPlayerPosition, Quaternion.identity) as GameObject;
            player.name = "PlayerCharacter";
        }
    }

    private void Update()
    {
        switch (gameState)
        {
            case (GameStates.WORLD_STATE):
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

                gameState = GameStates.IDLE;

                break;

            case (GameStates.IDLE):

                break;       
        }
    }

    public void LoadNextScene() {
        SceneManager.LoadScene(sceneToLoad);
    }

    public void LoadSceneAfterBattle() {
        SceneManager.LoadScene(lastScene);
    }

    void RandomEncounter() {
        if (isWalking && canGetEncountered) {
            if (Random.Range(0, 1000) < 10) {
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
            enemiesToBattle.Add(currentRegion.possibleEnemies[Random.Range(0, currentRegion.maxAmountEnemies)]);
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
}
