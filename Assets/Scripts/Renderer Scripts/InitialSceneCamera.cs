using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitialSceneCamera : MonoBehaviour {
    void changeTextActive()
    {
        GameObject.Find("Initial Text").SetActive(false);
    }
}
