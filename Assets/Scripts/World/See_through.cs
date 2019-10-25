using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class See_through : MonoBehaviour {
    private Renderer rend;
    private float threshold = 0f;
    private GameObject player;
    private Vector3 playerPosition, position, difference;

    public float seeThroughValue = 1f, distanceMultiplier = 15; 
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
            if (differenceX < 0.3f) differenceX = 0.3f;
          
            //print(differenceX);
            threshold = seeThroughValue - differenceX;
            Color tmp = rend.material.color;
            tmp.a = differenceX; 
            rend.material.color = tmp;
        
           // rend.material.SetFloat("_Threshold", threshold);
        }
    }
}
