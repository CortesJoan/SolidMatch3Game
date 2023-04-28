using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class DialogueState : GameState, IObserver
{
    public override void EnterState(GameController gameController)
    {
       }

    public override void UpdateState(GameController gameController)
    {
        // Handle dialogue input
    }

    public override void ExitState(GameController gameController)
    {
     }

    public void OnNotify()
    {
        // Handle dialogue events
    }
}

