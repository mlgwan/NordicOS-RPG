﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneWarp : MonoBehaviour {
    public int targetSceneIndex;

    private void OnTriggerEnter(Collider collision)
    {
        SceneManager.LoadScene(targetSceneIndex);
    }
}
