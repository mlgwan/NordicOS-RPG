using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeakerSprites : MonoBehaviour {

    public static SpeakerSprites instance;
    public Sprite ulfNormal;
    public Sprite ulfTired;
    public Sprite ulfAngry;
    public Sprite ulfHappy;

    private void Awake()
    {
        instance = this;
    }
}
