using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AnimatorMachineState : MonoBehaviour {

    protected HexagonControlMachine machine;
    public abstract void HandleState();

    protected virtual void Awake() {
        machine = FindObjectOfType<HexagonControlMachine>();
    }
}