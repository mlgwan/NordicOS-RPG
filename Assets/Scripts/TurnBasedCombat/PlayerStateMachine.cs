using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStateMachine : MonoBehaviour {

    private BattleStateMachine BSM;
    public BasePlayer player;



    public enum TurnState {
        ADDTOLIST,
        WAITING,
        ACTION,
        SELECTING,
        DEAD

    }

    public TurnState currentState;

    public Image healthBar;
    public Image manaBar;

    private void Start()
    {
        BSM = GameObject.Find("BattleManager").GetComponent<BattleStateMachine>();
        currentState = TurnState.ADDTOLIST;
    }



    void Update() {
        switch (currentState) {
            case (TurnState.ADDTOLIST):
                BSM.playersToManage.Add(this.gameObject);
                currentState = TurnState.WAITING;
                break;

            case (TurnState.WAITING):
                break;

            case (TurnState.ACTION):
                break;

            case (TurnState.DEAD):
                break;

        }
    }

    public void UpdateResourceBars() {
        healthBar.transform.localScale = new Vector2(Mathf.Clamp(((float)(player.curHP) / (float)(player.baseHP)), 0, 1), healthBar.transform.localScale.y);
        manaBar.transform.localScale = new Vector2(Mathf.Clamp(((float)(player.curMP) / (float)(player.baseMP)), 0, 1), healthBar.transform.localScale.y);
        

    }
}
