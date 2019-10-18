using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EquipmentUI : MonoBehaviour {

    public static EquipmentUI instance;

    public Transform character1Panel;
    public Transform character2Panel;
    public Transform character3Panel;
    public Transform character4Panel;

    public Transform[] characterPanels;

    private Popups popups;

    public GameObject statPanelHolder;
    private StatPanels statPanels;

    private string originalText; //because stamina and MP are handled the same way this is used to reduce work
    private Image healthBar;
    private Image manaBar;
    public Sprite playerIcon;

    private string[] originalTexts;

    private PlayerPanelStats stats;

    private void Awake()
    {
        instance = this;
    }
    // Use this for initialization
    void Start () {
        FillCharacterPanels();
        CreatePlayerPanel();
 
        statPanels = statPanelHolder.GetComponent<StatPanels>();
        Debug.Log(statPanelHolder);
        Debug.Log(statPanels);

        originalTexts = GetOriginalTexts();

    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void CreatePlayerPanel()
    {
        for (int i = 0; i < GameManager.instance.heroes.Count; i++) {
            GameObject equipPanel = Instantiate(GameManager.instance.heroes[i].GetComponent<PlayerStateMachine>().EquipscreenCharacterPanel, characterPanels[i], false) as GameObject;
            stats = equipPanel.GetComponent<PlayerPanelStats>();
            stats.PlayerName.text = GameManager.instance.heroes[i].GetComponent<PlayerStateMachine>().player.theName;

            stats.PlayerHP.text = "HP: " + GameManager.instance.heroes[i].GetComponent<PlayerStateMachine>().player.curHP + " / " + GameManager.instance.heroes[i].GetComponent<PlayerStateMachine>().player.maxHP;
            originalText = stats.PlayerMP.text;
            stats.PlayerMP.text = originalText + GameManager.instance.heroes[i].GetComponent<PlayerStateMachine>().player.curMP + " / " + GameManager.instance.heroes[i].GetComponent<PlayerStateMachine>().player.maxMP; //Because of the visual distinction between mana and stamina
            stats.PlayerLevel.text = GameManager.instance.heroes[i].GetComponent<PlayerStateMachine>().player.level.ToString();

            healthBar = stats.PlayerHPBar;
            manaBar = stats.PlayerMPBar;
            //stats.PlayerIcon = playerIcon;
            UpdateResourceBars();

        }
        
    }

    public void UpdateResourceBars()
    {
 
    }

    void FillCharacterPanels() {
        characterPanels = new Transform[] { character1Panel, character2Panel, character3Panel, character4Panel };
    }

    public void UpdateCharacterStats() {

        statPanels.hitPoints.text = originalTexts[0] + GameManager.instance.heroes[0].GetComponent<PlayerStateMachine>().player.curHP + " / " + GameManager.instance.heroes[0].GetComponent<PlayerStateMachine>().player.maxHP;
        statPanels.manaPoints.text = originalTexts[1] + GameManager.instance.heroes[0].GetComponent<PlayerStateMachine>().player.curMP + " / " + GameManager.instance.heroes[0].GetComponent<PlayerStateMachine>().player.maxMP;
        statPanels.defense.text = originalTexts[2] + GameManager.instance.heroes[0].GetComponent<PlayerStateMachine>().player.curDEF;
        statPanels.attack.text = originalTexts[3] + GameManager.instance.heroes[0].GetComponent<PlayerStateMachine>().player.curATK;
        statPanels.magicResist.text = originalTexts[4] + GameManager.instance.heroes[0].GetComponent<PlayerStateMachine>().player.curMR;
        statPanels.speed.text = originalTexts[5] + GameManager.instance.heroes[0].GetComponent<PlayerStateMachine>().player.curSPD;
        statPanels.strength.text = originalTexts[6] + GameManager.instance.heroes[0].GetComponent<PlayerStateMachine>().player.curSTR;
        statPanels.dexterity.text = originalTexts[7] + GameManager.instance.heroes[0].GetComponent<PlayerStateMachine>().player.curDEX;
        statPanels.intelligence.text = originalTexts[8] + GameManager.instance.heroes[0].GetComponent<PlayerStateMachine>().player.curINT;
        statPanels.level.text = originalTexts[9] + GameManager.instance.heroes[0].GetComponent<PlayerStateMachine>().player.level;
        statPanels.experience.text = originalTexts[10] + GameManager.instance.heroes[0].GetComponent<PlayerStateMachine>().player.currentExperiencePoints;

    }

    string[] GetOriginalTexts() {

        string[] results = new string[11];

        results[0] = statPanels.hitPoints.text;
        results[1] = statPanels.manaPoints.text;
        results[2] = statPanels.defense.text;
        results[3] = statPanels.attack.text;
        results[4] = statPanels.magicResist.text;
        results[5] = statPanels.speed.text;
        results[6] = statPanels.strength.text;
        results[7] = statPanels.dexterity.text;
        results[8] = statPanels.intelligence.text;
        results[9] = statPanels.level.text;
        results[10] = statPanels.experience.text;

        return results;
    }
}
