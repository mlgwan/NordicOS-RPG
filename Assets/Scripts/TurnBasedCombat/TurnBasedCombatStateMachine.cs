using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnBasedCombatStateMachine : MonoBehaviour {

	public enum BattleStates
    {
        START,
        PLAYERCHOICE,
        ENEMYCHOICE,
        LOSE,
        WIN
    }

    private BattleStates currentState;

    private void Start()
    {
        currentState = BattleStates.START;
    }

    private void Update()
    {
        switch (currentState) {
            case (BattleStates.START):
                break;
            case (BattleStates.PLAYERCHOICE):
                break;
            case (BattleStates.ENEMYCHOICE):
                break;
            case (BattleStates.LOSE):
                break;
            case (BattleStates.WIN):
                break;
        }
    }
}
