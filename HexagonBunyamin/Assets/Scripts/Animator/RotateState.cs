using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RotateState : AnimatorMachineState
{
    [Range(1, 3)]
    public float easeAmount = 2; //Kolaylık miktarı
    public float timeRotation = 1;
    float turnSpeed;

    GridManager gridManager;

    Material material;
    float offsetTexture;
    public float textureSpeed = 2f;

    protected override void Awake() {
        base.Awake();
        material = GetComponent<LineRenderer>().material;
        turnSpeed = 1f / (timeRotation / 3f);
        gridManager = FindObjectOfType<GridManager>();
    }

    private void Update() {
        offsetTexture += textureSpeed * Time.deltaTime;
        material.SetTextureOffset("_MainTex", new Vector2(offsetTexture, 0));
    }

    Vector3 GetMidPointFromThripleHexagons(Hexagon[] selectedTripleHexagons)
    {
        Vector2 sumVector = Vector2.zero;
        for (int i = 0; i < selectedTripleHexagons.Length; i++)
        {
            sumVector += (Vector2)selectedTripleHexagons[i].transform.position;
        }
        sumVector = sumVector / 3f;
        return new Vector3(sumVector.x, sumVector.y, -.1f);
    }
    
    Quaternion GetOutlineRotation(Hexagon[] selectedTripleHexagons)
    {
        Hexagon aboveHexagon = Utility.GetAboveHexagonInsideGroup(selectedTripleHexagons);
        Vector2 dir = aboveHexagon.transform.position - transform.position;
        float angle = Vector2.Angle(Vector2.right, dir);
        return Quaternion.Euler(0, 0, angle);
    }

    public void SetPosAndRot(Hexagon[] selectedTripleHexagons)
    {
        transform.position = GetMidPointFromThripleHexagons(selectedTripleHexagons);
        transform.rotation = GetOutlineRotation(selectedTripleHexagons);
    }

    public override void HandleState()
    {
        StartCoroutine(RotateAnim());
    }

    public IEnumerator RotateAnim()
    {
        int dir = machine.GetRotateDirection();
        for (int i = 0; i < 3; i++)
        {
            Quaternion from = transform.rotation;
            Quaternion to = transform.rotation * Quaternion.Euler(0, 0, dir * 120);

            float percent = 0;
            while (percent < 1)
            {
                percent += turnSpeed * Time.deltaTime;
                percent = Mathf.Clamp01(percent);
                float easedPercent = Utility.Ease(percent, easeAmount);
                transform.rotation = Quaternion.Slerp(from, to, easedPercent);

                if (machine.bombHexagon != null)
                    machine.bombHexagon.transform.rotation = Quaternion.Euler(0, 0, 0);
                yield return null;
            }

            if (machine.HasDestroyHexagons(machine.Info.selectedTripleHexagons)) {
                machine.SetNextState();
                machine.ChangeParentOfSelectedHexagons(gridManager.transform);
                yield break;
            }
        }
        machine.ChangeParentOfSelectedHexagons(gridManager.transform);
        machine.SetStateNull();
    }
}

/*
public IEnumerator RotateAnim(HexagonControlMachine machine)
    {
        int dir = machine.GetRotateDirection();
        speed = 1f / (timeRotation / 3f);

        for (int i = 0; i < 3; i++)
        {
            float from = transform.localEulerAngles.z;
            float to = transform.localEulerAngles.z + 120f;
            float percent = 0;
            Vector3[] dirVec = new[] 
            { 
                machine.info.selectedTripleHexagons[0].transform.position - transform.position,
                machine.info.selectedTripleHexagons[1].transform.position - transform.position,
                machine.info.selectedTripleHexagons[2].transform.position - transform.position
            };
            while (percent <= 1000)
            {
                percent += speed * Time.deltaTime;
                float angle = Mathf.Lerp(from, to, percent);
                for (int j = 0; j < machine.info.selectedTripleHexagons.Length; j++)
                {
                    dirVec[j] = Quaternion.Euler(0,0,percent) * dirVec[j];
                    machine.info.selectedTripleHexagons[j].transform.position = transform.position + dirVec[j];
                }
                yield return null;
            }

            // if (machine.HasDestroyHexagons(machine.info.selectedTripleHexagons))
            // {
            //     machine.SetNextState();
            //     // machine.ChangeParentOfSelectedHexagons(gridManager.transform);
            //     print("break");
            //     yield break;
            // }
        }
        // machine.ChangeParentOfSelectedHexagons(gridManager.transform);
        machine.SetStateNull();
    }
*/