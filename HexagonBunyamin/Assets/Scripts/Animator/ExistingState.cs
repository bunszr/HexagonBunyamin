using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExistingState : AnimatorMachineState
{
    List<int> bestIDs = new List<int>();
    RaycastHit2D[] raycastHit2Ds;
    GridManager gridManager;

    float nextTime;
    int index;
    LayerMask hexagonMask = 1 << 8;

    [Range(0, 2)] public float duration = .1f;
    float totalDurationOfCreating;

    protected override void Awake() {
        base.Awake();
        gridManager = FindObjectOfType<GridManager>();
        raycastHit2Ds = new RaycastHit2D[gridManager.height];
        this.enabled = false;
    }

    public override void HandleState()
    {
        index = 0;
        CreateNewHexagon(machine);
    }

    private void Update() {
        
        if (Time.time > nextTime && index < bestIDs.Count) {
            nextTime = Time.time + duration;
            Hexagon hex = machine.AllHexagon[bestIDs[index]];
            hex.RequestMakeFallAnim(hex.finalPosition);
            index++;
        }
        else {
            if (totalDurationOfCreating < Time.time) {
                machine.SetNextState();
                this.enabled = false;
            }
        }
    }

    public void CreateNewHexagon(HexagonControlMachine machine)
    {
        bestIDs.Clear();
        for (int i = 0; i < machine.Info.bottomRayPosList.Count; i++)
        {
            Vector2 rayOrgin = machine.Info.bottomRayPosList[i];
            int hitSize = Physics2D.RaycastNonAlloc(rayOrgin, Vector2.up, raycastHit2Ds, 10, hexagonMask);
            if (hitSize != 0) {
                float fallAmount = Mathf.Abs(rayOrgin.y - raycastHit2Ds[0].transform.position.y);
                for (int j = 0; j < hitSize; j++)
                {
                    int id = raycastHit2Ds[j].collider.GetInstanceID();
                    Hexagon hex = machine.AllHexagon[id];
                    bestIDs.Add(id);
                    hex.finalPosition = hex.transform.position + Vector3.down * fallAmount;
                }
            }
        }
        totalDurationOfCreating = Time.time + (bestIDs.Count * duration) + machine.hexFallTime + HexInfo.animTimeSkinWidth;
    }
}