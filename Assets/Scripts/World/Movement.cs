using UnityEngine;
using System.Collections;

public class Movement : MonoBehaviour
{
    // Normal Movements Variables
    public float walkSpeed;
    private float curSpeed;
    private float maxSpeed;
    private float sprintSpeed;
    private Vector3 moveForwardBackward,  moveLeftRight;

    Animator player;
    Transform body;
    Rigidbody rb;
    float horizontal;
    float vertical;
    float moveLimiter = 0.8f;

    public GameObject startMenu;
    private GameObject startMenuHolder;

    private bool inventoryIsOpen;


    float hitLength = 100f;

    Vector3 currentPosition, lastPosition;




    void Start()
    {
        if (GameManager.instance.nextSpawnPoint != "")
        {
            GameObject spawnPoint = GameObject.Find(GameManager.instance.nextSpawnPoint);
            transform.position = spawnPoint.transform.position;

            GameManager.instance.nextSpawnPoint = "";
        }
        else if (GameManager.instance.lastPlayerPosition != Vector3.zero) {
            transform.position = GameManager.instance.lastPlayerPosition;
            GameManager.instance.lastPlayerPosition = Vector3.zero;
        }


        moveForwardBackward = new Vector3(0, 0, walkSpeed);
        moveLeftRight = new Vector3(walkSpeed, 0, 0);
        player = GetComponent<Animator>();
        body = GetComponent<Transform>();

        startMenu = GameObject.Find("MenuCanvas");
        startMenuHolder = startMenu.transform.Find("MenuHolder").gameObject;


    }

    void FixedUpdate()
    {
        if (!inventoryIsOpen) {
            if (Input.GetKey(KeyCode.W)) {
                player.Play("backwards");
                body.position += moveForwardBackward;
            
            }

            else if (Input.GetKey(KeyCode.S)) {
                player.Play("forwards");
                body.position += -moveForwardBackward;
            }
            if (Input.GetKey(KeyCode.A))
            {
                player.Play("left");
                body.position += -moveLeftRight;
            }
            else if (Input.GetKey(KeyCode.D))
            {
                player.Play("right");
                body.position += moveLeftRight;
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape) && !inventoryIsOpen) {
            inventoryIsOpen = true;
            startMenuHolder.SetActive(true);
            startMenu.GetComponent<InventoryManager>().currentState = InventoryManager.InventoryStates.OPTIONS;
        }
        
        else if (Input.GetKeyDown(KeyCode.Escape) && inventoryIsOpen && startMenu.GetComponent<InventoryManager>().currentState == InventoryManager.InventoryStates.OPTIONS) {
            inventoryIsOpen = false;
            startMenuHolder.SetActive(false);
            startMenu.GetComponent<InventoryManager>().currentState = InventoryManager.InventoryStates.DISABLED;
        }

       


        RaycastHit hit;
        /*
         * Cast a Raycast.
         * If it hits something:
         */
        if (Physics.Raycast(transform.position, Vector3.down, out hit, hitLength))
        {
            /*
             * Set the target location to the location of the hit.
             */

        }

        currentPosition = transform.position;
        if (currentPosition == lastPosition)
        {
            GameManager.instance.isWalking = false;
        }
        else {
            GameManager.instance.isWalking = true;
        }
        lastPosition = currentPosition;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Warp")
        {
            Warp warp = other.gameObject.GetComponent<Warp>();
            GameManager.instance.nextSpawnPoint = warp.spawnPointName;
            GameManager.instance.sceneToLoad = warp.sceneToLoad;
            GameManager.instance.LoadNextScene();
        }
        if (other.tag == "EncounterZone")
        {
            RegionData region = other.gameObject.GetComponent<RegionData>();
            GameManager.instance.currentRegion = region;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "EncounterZone") {
            GameManager.instance.canGetEncountered = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "EncounterZone")
        {
            GameManager.instance.canGetEncountered = false;
        }
    }
}