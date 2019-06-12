using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeLayerForMesh : MonoBehaviour {
    public string layerToPushTo;
    void Start()
    {
        GetComponent<Renderer>().sortingLayerName = layerToPushTo;
    }
}
