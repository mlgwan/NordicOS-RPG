using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Warp : MonoBehaviour {

    public string sceneToLoad;

    public string spawnPointName;
    //GameState 

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnTriggerEnter(Collider other)
    {
         //other.GetComponent<Transform>().position = warpPoint.GetComponent<Transform>().position + offset;
    }
}
