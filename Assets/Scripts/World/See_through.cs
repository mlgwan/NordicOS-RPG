using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class See_through : MonoBehaviour {
    private Renderer rend;
    private float threshold = 0f;
    private GameObject player;
    private Vector3 playerPosition, position, difference;

    private float seeThroughValue = 1f, distanceMultiplier = 5; 
	// Use this for initialization
	void Start () {
        rend = GetComponent<Renderer>();
        player = GameObject.Find("PlayerCharacter");
        
    }
	
	// Update is called once per frame
	void Update () {
        playerPosition = player.transform.position;
        position = this.transform.position;
        difference = position - playerPosition;
        if(difference.z < 0)
        {
            float differenceX = Mathf.Abs(difference.x) * distanceMultiplier;
            if (differenceX > seeThroughValue) differenceX = seeThroughValue;
            if (differenceX < 0.0f) differenceX = 0.0f;
          
           
            threshold = seeThroughValue - differenceX;
            Color tmp = rend.material.color;
            tmp.a = differenceX; 
            rend.material.color = tmp;
       
        }
    }
}
