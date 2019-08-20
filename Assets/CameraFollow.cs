using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;            // The position that that camera will be following.
    public float smoothing = 5f;        // The speed with which the camera will be following.

    Vector3 offset;                     // The initial offset from the target.

    void Start()
    {
        // Calculate the initial offset.
        StartCoroutine(waitForAnimationToFinish());

        
    }

    void FixedUpdate()
    {
        if (target != null) {
            Debug.Log("hey");
        // Create a postion the camera is aiming for based on the offset from the target.
        Vector3 targetCamPos = target.position + offset;

        // Smoothly interpolate between the camera's current position and it's target position.
        transform.position = Vector3.Lerp(transform.position, targetCamPos, smoothing * Time.deltaTime);

        }

    }

    private IEnumerator waitForAnimationToFinish()
    {

        yield return new WaitForSeconds(gameObject.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).length + gameObject.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).normalizedTime);
        while (target == null) {
            target = GameObject.FindWithTag("Player").transform;
        }
        
        transform.position = new Vector3(target.position.x, target.position.y + 6, target.position.z - 5);
        offset = transform.position - target.position;
    }
}