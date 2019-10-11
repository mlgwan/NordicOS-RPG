using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EquipmenttUI : MonoBehaviour {

    private Popups popups;

    private string originalText; //because stamina and MP are handled the same way this is used to reduce work
    private Image healthBar;
    private Image manaBar;
    public Sprite playerIcon;

    private PlayerPanelStats stats;
    public GameObject playerPanel;
    private Transform playerPanelSpacer;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void CreatePlayerPanel()
    {
        playerPanel = Instantiate(playerPanel) as GameObject;
        stats = playerPanel.GetComponent<PlayerPanelStats>();
        stats.PlayerName.text = GameManager.instance.heroes[0].GetComponent<PlayerStateMachine>().player.theName;

        stats.PlayerHP.text = "HP: " + GameManager.instance.heroes[0].GetComponent<PlayerStateMachine>().player.curHP + " / " + GameManager.instance.heroes[0].GetComponent<PlayerStateMachine>().player.maxHP;
        originalText = stats.PlayerMP.text;
        stats.PlayerMP.text = originalText + GameManager.instance.heroes[0].GetComponent<PlayerStateMachine>().player.curMP + " / " + GameManager.instance.heroes[0].GetComponent<PlayerStateMachine>().player.maxMP; //Because of the visual distinction between mana and stamina
        stats.PlayerLevel.text = GameManager.instance.heroes[0].GetComponent<PlayerStateMachine>().player.level.ToString();

        healthBar = stats.PlayerHPBar;
        manaBar = stats.PlayerMPBar;
        stats.PlayerIcon = playerIcon;
        playerPanel.transform.SetParent(playerPanelSpacer, false);
        UpdateResourceBars();
    }

    public void UpdateResourceBars()
    {
        healthBar.transform.localScale = new Vector2(Mathf.Clamp(((float)(GameManager.instance.heroes[0].GetComponent<PlayerStateMachine>().player.curHP) / (float)(GameManager.instance.heroes[0].GetComponent<PlayerStateMachine>().player.maxHP)), 0, 1), healthBar.transform.localScale.y);
        stats.PlayerHP.text = "HP: " + GameManager.instance.heroes[0].GetComponent<PlayerStateMachine>().player.curHP + " / " + GameManager.instance.heroes[0].GetComponent<PlayerStateMachine>().player.maxHP;

        manaBar.transform.localScale = new Vector2(Mathf.Clamp(((float)(GameManager.instance.heroes[0].GetComponent<PlayerStateMachine>().player.curMP) / (float)(GameManager.instance.heroes[0].GetComponent<PlayerStateMachine>().player.maxMP)), 0, 1), healthBar.transform.localScale.y);
        stats.PlayerMP.text = originalText + GameManager.instance.heroes[0].GetComponent<PlayerStateMachine>().player.curMP + " / " + GameManager.instance.heroes[0].GetComponent<PlayerStateMachine>().player.maxMP;
    }
}
