using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinalState : AnimatorMachineState
{
    protected override void Awake() {
        base.Awake();
    }
    
    public override void HandleState()
    {
        if (machine.HasDestroyHexagons(machine.Info.allMovedHexagons.ToArray())) {
            machine.SetNextState();
        }
        else {
            machine.SetStateNull();
            UIManager.ins.SetMovesAndBombCount();
        }
        this.enabled = false;
    }

}