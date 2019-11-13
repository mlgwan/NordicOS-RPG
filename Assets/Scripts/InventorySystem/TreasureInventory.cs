using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreasureInventory : MonoBehaviour {

    public bool isLooted;
    public Sprite lootedSprite;
    public Sprite unlootedSprite;
    public List<Item> items;

	// Use this for initialization
	void Start () {
        UpdateSprite();
		
	}

    public void AddToPlayerInventory() {
        if (!isLooted) {
            for (int i = 0; i < items.Count; i++)
            {
                Inventory.instance.AddItem(items[i]);
            }
            GetComponent<DialogueHolder>().dialogueLines[1] = GetComponent<DialogueHolder>().dialogueLines[1].Replace("{itemName}", items[0].name);
            GetComponent<DialogueHolder>().DisplayBox();
        }
        isLooted = true;
        UpdateSprite();
        
    }

    void UpdateSprite() {
        if (!isLooted)
        {
            GetComponent<SpriteRenderer>().sprite = unlootedSprite;
        }
        else
        {
            GetComponent<SpriteRenderer>().sprite = lootedSprite;
        }
    }
}
