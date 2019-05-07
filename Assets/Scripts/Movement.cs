using UnityEngine;
using System.Collections;

public class Movement : MonoBehaviour
{
    // Normal Movements Variables
    public float walkSpeed;
    private float curSpeed;
    private float maxSpeed;
    private float sprintSpeed;
    private Vector3 moveForwardBackward;
    private Vector3 moveLeftRight;

    Animator player;
    Transform body;
    float horizontal;
    float vertical;
    float moveLimiter = 0.8f;


    float hitLength = 100f;
  



private CharacterStat plStat;

    void Start()
    {
        moveForwardBackward = new Vector3(0, 0, walkSpeed);
        moveLeftRight = new Vector3(walkSpeed, 0, 0);
        player = GetComponent<Animator>();
        body = GetComponent<Transform>();
       
    }

    void FixedUpdate()
    {

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
            Vector3 targetLocation = hit.point;
            /*
             * Modify the target location so that the object is being perfectly aligned with the ground (if it's flat).
             */
            targetLocation += new Vector3(0, transform.localScale.y / 2, 0);
            /*
             * Move the object to the target location.
             */
            transform.position = targetLocation;
        }
     }
}