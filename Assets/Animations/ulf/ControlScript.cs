using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlScript : MonoBehaviour {

    public static ControlScript instance;

    public KeyCode acceptButton;
    public KeyCode selectButton;
    public KeyCode upButton;
    public KeyCode downButton;
    public KeyCode leftButton;
    public KeyCode rightButton;
    public KeyCode escapeButton;

	// Use this for initialization
	void Awake () {
        instance = this;
	}
	
	// Update is called once per frame
	void Start () {
        acceptButton = KeyCode.Return;
        selectButton = KeyCode.Backspace;
        upButton = KeyCode.W;
        downButton = KeyCode.S;
        leftButton = KeyCode.A;
        rightButton = KeyCode.D;
        escapeButton = KeyCode.Escape;
	}
}
