using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatingState : AnimatorMachineState
{
    GridManager gridManager;
    Collider2D col;

    float nextTime;
    int index;
    List<Vector2> newHexagonsPos = new List<Vector2>();

    [Range(0, 2)] 
    public float duration = .1f;
    float TotalDurationOfCreating;

    protected override void Awake() {
        base.Awake();

        gridManager = FindObjectOfType<GridManager>();
        this.enabled = false;
    }
    
    public override void HandleState()
    {
        CreateNewHexagonPos();
        index = 0;
    }

    private void Update() {
        
        if (Time.time > nextTime && index < machine.Info.destroyedAllHexagons.Count) {
            nextTime = Time.time + duration;

            Hexagon hex = machine.Info.destroyedAllHexagons[index];
            hex.transform.position = newHexagonsPos[index] + Vector2.up * 2;
            hex.RequestMakeFallAnim(newHexagonsPos[index]);
            hex.gameObject.SetActive(true);
            index++;
        }
        else {
            if (TotalDurationOfCreating < Time.time) {
                machine.SetNextState();
                this.enabled = false;
            }
        }
    }

    public void CreateNewHexagonPos()
    {
        newHexagonsPos.Clear();
        Utility.SetHexagonRandomColor(machine.Info.destroyedAllHexagons, gridManager.hexColors);
        Vector2[] possiblePos = gridManager.GetPossibleNewHexagonPositions(machine.Info.bottomRayPosList);
        for (int i = 0; i < possiblePos.Length; i++)
        {
            col = Physics2D.OverlapPoint(possiblePos[i]);
            if (col == null) {
                newHexagonsPos.Add(possiblePos[i]);
            }
        }
        TotalDurationOfCreating = Time.time + (newHexagonsPos.Count * duration) + machine.hexFallTime + HexInfo.animTimeSkinWidth;
    }

}