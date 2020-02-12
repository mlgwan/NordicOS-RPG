using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EquipmentUI : MonoBehaviour {

    private Popups popups;

    private string originalText; //because stamina and MP are handled the same way this is used to reduce work
    private Image healthBar;
    private Image manaBar;
    public Sprite playerIcon;

    public PlayerPanelStats stats;
    public GameObject playerPanelPrefab;
    public List<Transform> parents; // the gameobjects that hold the player information (Character1 - Character4)

    public GameObject informationBoxEquipmentHolder;
    public Image weaponIcon;
    public TextMeshProUGUI weaponText;
    public Image helmetIcon;
    public TextMeshProUGUI helmetText;
    public Image armorIcon;
    public TextMeshProUGUI armorText;
    public Image bootsIcon;
    public TextMeshProUGUI bootsText;

    public Sprite emptyWeaponIcon;
    public Sprite emptyHelmetIcon;
    public Sprite emptyArmorIcon;
    public Sprite emptyBootsIcon;


    public int selectedHeroIndex;



    public static EquipmentUI instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);

    }

    // Use this for initialization
    void Start () {
        playerPanelPrefab = GameManager.instance.ulf.GetComponent<PlayerStateMachine>().equipPlayerPanel;
        CreatePlayerPanel();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void CreatePlayerPanel()
    {
        for (int i = 0; i < GameManager.instance.heroes.Count; i++) {
            GameObject playerPanel = Instantiate(playerPanelPrefab, parents[i], false) as GameObject;
            stats = playerPanel.GetComponent<PlayerPanelStats>();
            stats.PlayerName.text = GameManager.instance.heroes[0].GetComponent<PlayerStateMachine>().player.theName;

            stats.PlayerHP.text = "HP: " + GameManager.instance.heroes[0].GetComponent<PlayerStateMachine>().player.curHP + "/" + GameManager.instance.heroes[0].GetComponent<PlayerStateMachine>().player.maxHP;
            originalText = stats.PlayerMP.text;
            stats.PlayerMP.text = originalText + GameManager.instance.heroes[0].GetComponent<PlayerStateMachine>().player.curMP + "/" + GameManager.instance.heroes[0].GetComponent<PlayerStateMachine>().player.maxMP; //Because of the visual distinction between mana and stamina
            stats.PlayerLevel.text = GameManager.instance.heroes[0].GetComponent<PlayerStateMachine>().player.level.ToString();

            healthBar = stats.PlayerHPBar;
            manaBar = stats.PlayerMPBar;
            stats.PlayerIcon = playerIcon;
        }
                
        UpdateResourceBars(selectedHeroIndex);
    }

    public void UpdateResourceBars(int indexOfHero)
    {
        healthBar.transform.localScale = new Vector2(Mathf.Clamp(((float)(GameManager.instance.heroes[indexOfHero].GetComponent<PlayerStateMachine>().player.curHP) / (float)(GameManager.instance.heroes[indexOfHero].GetComponent<PlayerStateMachine>().player.maxHP)), 0, 1), healthBar.transform.localScale.y);
        stats.PlayerHP.text = "HP: " + GameManager.instance.heroes[indexOfHero].GetComponent<PlayerStateMachine>().player.curHP + "/" + GameManager.instance.heroes[indexOfHero].GetComponent<PlayerStateMachine>().player.maxHP;

        manaBar.transform.localScale = new Vector2(Mathf.Clamp(((float)(GameManager.instance.heroes[0].GetComponent<PlayerStateMachine>().player.curMP) / (float)(GameManager.instance.heroes[0].GetComponent<PlayerStateMachine>().player.maxMP)), 0, 1), healthBar.transform.localScale.y);
        stats.PlayerMP.text = originalText + GameManager.instance.heroes[indexOfHero].GetComponent<PlayerStateMachine>().player.curMP + "/" + GameManager.instance.heroes[indexOfHero].GetComponent<PlayerStateMachine>().player.maxMP;

        updateInformationBox(selectedHeroIndex);

    }

    public void updateInformationBox(int indexOfHero) {

        if (indexOfHero+1 > GameManager.instance.heroes.Count)
        {
            informationBoxEquipmentHolder.SetActive(false);
        }
        else {
           
            if (!(GameManager.instance.heroes[indexOfHero].GetComponent<PlayerStateMachine>().player.weaponSlot == null))
            {
                weaponIcon.sprite = GameManager.instance.heroes[indexOfHero].GetComponent<PlayerStateMachine>().player.weaponSlot.icon;
                weaponText.text = GameManager.instance.heroes[indexOfHero].GetComponent<PlayerStateMachine>().player.weaponSlot.name;
            }
            else
            {
                weaponIcon.sprite = emptyWeaponIcon;
                weaponText.text = "No weapon equipped";
            }
            if (!(GameManager.instance.heroes[indexOfHero].GetComponent<PlayerStateMachine>().player.helmetSlot == null))
            {
                helmetIcon.sprite = GameManager.instance.heroes[indexOfHero].GetComponent<PlayerStateMachine>().player.helmetSlot.icon;
                helmetText.text = GameManager.instance.heroes[indexOfHero].GetComponent<PlayerStateMachine>().player.helmetSlot.name;
            }
            else
            {
                helmetIcon.sprite = emptyHelmetIcon;
                helmetText.text = "No helmet equipped";
            }
            if (!(GameManager.instance.heroes[indexOfHero].GetComponent<PlayerStateMachine>().player.armorSlot == null))
            {
                armorIcon.sprite = GameManager.instance.heroes[indexOfHero].GetComponent<PlayerStateMachine>().player.armorSlot.icon;
                armorText.text = GameManager.instance.heroes[indexOfHero].GetComponent<PlayerStateMachine>().player.armorSlot.name;
            }
            else
            {
                armorIcon.sprite = emptyArmorIcon;
                armorText.text = "No armor equipped";
            }
            if (!(GameManager.instance.heroes[indexOfHero].GetComponent<PlayerStateMachine>().player.bootsSlot == null))
            {
                bootsIcon.sprite = GameManager.instance.heroes[indexOfHero].GetComponent<PlayerStateMachine>().player.bootsSlot.icon;
                bootsText.text = GameManager.instance.heroes[indexOfHero].GetComponent<PlayerStateMachine>().player.bootsSlot.name;
            }
            else
            {
                bootsIcon.sprite = emptyBootsIcon;
                bootsText.text = "No boots equipped";
            }

            informationBoxEquipmentHolder.SetActive(true);
        }
       
    }
}
