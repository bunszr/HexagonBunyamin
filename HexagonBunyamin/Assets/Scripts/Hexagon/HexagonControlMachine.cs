using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HexagonControlMachine : MonoBehaviour, IHexagon, IAbstractHexagon
{
    Camera cam;
    GridManager gridManager;
    [SerializeField] RotateState rotateState;

    [SerializeField] UnityEngine.Object inputObject;
    IInput _input;

    [SerializeField] LayerMask hexagonMask;
    [SerializeField] Sprite bombSprite;
    [HideInInspector] public AbstractHexagon bombHexagon;

    public SelectionInfo Info { get; private set; }
    AnimatorMachineState state;
 
    [SerializeField] AnimatorMachineState[] animatorMachineStates;
    public Dictionary<int, Hexagon> AllHexagon { get; private set; } = new Dictionary<int, Hexagon>();
    int stateIndex = 0;

    [Header("Animation")]
    [Range(1, 3)] public float easeAmountFall = 2;
    [Range(0, 2)] public float hexFallTime = .32f;
    float hexFallSpeed;
    bool skipOneTime = false; // Bomba geldiğinden aynı renkte oluyorsa komşularıyla hemen yok oluyor ilk seferi atlamak için

    [Header("Begin Of Game Animation")]
    [Range(1, 3)] public float easeAmountCreating = 2;
    [Range(0, 2)] public float hexCreatingTime;
    float hexCreatingSpeed;

    private void Start()
    {
        cam = Camera.main;
        gridManager = FindObjectOfType<GridManager>();
        rotateState = FindObjectOfType<RotateState>();
        hexFallSpeed = 1f / hexFallTime;
        hexCreatingSpeed = 1f / hexCreatingTime;
        Info = new SelectionInfo();
        _input = inputObject as IInput;
        state = null;
    }

    private void Update()
    {
        if (state != null)
            return;

        if (_input.IsClick() && gridManager.IsMapAreaFrom(_input.MousePosition))
        {
            Info.oldMousePos = _input.MousePosition;
            SetClosestAndSideTripleHexagon(_input.MousePosition);
            rotateState.SetPosAndRot(Info.selectedTripleHexagons);
        }
        else if (_input.IsRotate() && Info.selectedTripleHexagons[0] != null)
        { 
            SetClosestAndSideTripleHexagon(Info.oldMousePos);
            ChangeParentOfSelectedHexagons(rotateState.transform);
            state = rotateState;
            HandleState();
        }

        if (Input.GetKeyDown(KeyCode.H))
        {
            for (int i = 0; i < AllHexagon.Count; i++)
            {
                Hexagon hex = AllHexagon.ElementAt(i).Value;
                hex.RequestMakeFallAnim(hex.transform.position +  Vector3.down * 10);
            }
        }
    }

    public void AddDictionary(Hexagon hexagon) {
        AllHexagon.Add(hexagon.circleCollider.GetInstanceID(), hexagon);
    }

    public void HandleState() {
        state.HandleState();
    }

    public void SetNextState() {
        state = animatorMachineStates[(stateIndex % (animatorMachineStates.Length - 1)) + 1];
        stateIndex++;
        state.enabled = true;
        HandleState();
    }

    public void SetStateNull() {
        state = null;
    }

    public void SetClosestAndSideTripleHexagon(Vector2 mousePosition)
    {
        Collider2D[] coliders = Physics2D.OverlapCircleAll(mousePosition, HexInfo.overlapRadius, hexagonMask);
        coliders = coliders.OrderBy(x => ((Vector2)x.transform.position - mousePosition).sqrMagnitude).ToArray();
        for (int i = 2; i < coliders.Length; i++)
        {
            if (Utility.SelectedTripleHexIsSideBySide(coliders[0].transform.position, coliders[1].transform.position, coliders[i].transform.position)) {
                Info.selectedTripleHexagons[0] = AllHexagon[coliders[0].GetInstanceID()];
                Info.selectedTripleHexagons[1] = AllHexagon[coliders[1].GetInstanceID()];
                Info.selectedTripleHexagons[2] = AllHexagon[coliders[i].GetInstanceID()];
                break;
            }
        }
    }

    public Hexagon[] GetSameColorHexagon(Hexagon middleHex) //middleHex dahil yanındaki 3 veya daha fazla hex döndürüyor 
    {
        List<Hexagon> neigbersSameColorHex = new List<Hexagon>();
        neigbersSameColorHex.Add(middleHex);
        RaycastHit2D hit2D;
        for (int i = 0; i < 6; i++)
        {
            hit2D = Physics2D.Raycast(middleHex.transform.position, HexInfo.neighberDir[i] * 2 * HexInfo.innerRadius, hexagonMask);

            if (hit2D.collider != null) {
                Hexagon hex = AllHexagon[hit2D.collider.GetInstanceID()];
                if (middleHex.CompareTo(hex) == 0) {
                    neigbersSameColorHex.Add(hex);
                }
            }
        }
        if (neigbersSameColorHex.Count >= 3) {
            return neigbersSameColorHex.ToArray();
        }
        return null;
    }

    Hexagon[] GetSideBySideSameColorHexagon(Hexagon[] sameColorHexagons)
    {
        Hexagon middleHexagon = sameColorHexagons[0];
        List<Hexagon> destroyedHexagons = new List<Hexagon>();
        for (int i = 1; i < sameColorHexagons.Length; i++)
        {
            Vector2 dir1 = sameColorHexagons[i].transform.position - middleHexagon.transform.position;
            int newI = i == sameColorHexagons.Length - 1 ? 1 : i + 1; // En son olarak son hex ile ilk hex arasındaki açıyıda kontrol etmek gerekiyor
            Vector2 dir2 = sameColorHexagons[newI].transform.position - middleHexagon.transform.position;
            float angle = Vector2.Angle(dir1, dir2);
            if (55 < angle && angle < 65)
            {
                destroyedHexagons.Add(middleHexagon);
                destroyedHexagons.Add(sameColorHexagons[i]);
                destroyedHexagons.Add(sameColorHexagons[newI]);
            }
        }
        if (destroyedHexagons.Count != 0)
            return destroyedHexagons.ToArray();
        return null;
    }

    public int GetRotateDirection() // 3'lü altıgen hangi yönde döneceğini...
    {
        return _input.RotateDir;
    }

    public bool HasDestroyHexagons(Hexagon[] hexagons) //120 derecelik dönme sonrası kontrol ediyoruz yok edilen var mı diye
    {
        Info.Clear();
        bool hasDestroy = false;
        for (int i = 0; i < hexagons.Length; i++)
        {
            Hexagon[] sameColorHexagons = GetSameColorHexagon(hexagons[i]);
            if (sameColorHexagons != null)
            {
                Hexagon[] SideBySideAndSameColorHexagons = GetSideBySideSameColorHexagon(sameColorHexagons);
                if (SideBySideAndSameColorHexagons != null)
                {
                    // Deactive(SideBySideAndSameColorHexagons);
                    Info.AddDestroyedHexagons(SideBySideAndSameColorHexagons);
                    hasDestroy = true;
                }
            }
        }
        ToggleBomb(Info.destroyedAllHexagons.ToArray());
        Info.DistinctDestroyedHexList(); // Bütün koşumşu altıgenleri gezerken aynı olanları atıyoruz
        Deactive(Info.destroyedAllHexagons.ToArray());
        Info.SetBottomRayPositions(); // Yok edilen altıgenlerin tabana en yakın olan pozisyonları alıyoruz
        return hasDestroy;
    }

    public void Active(Hexagon hexagon)
    {
        hexagon.gameObject.SetActive(true);
    }

    public void Deactive(params Hexagon[] hexagon)
    {
        for (int i = 0; i < hexagon.Length; i++)
        {
            UIManager.ins.SetScore();
            hexagon[i].gameObject.SetActive(false);
        }
    }

    void ToggleBomb(Hexagon[] destroyedHexagons) {
        
        if (UIManager.ins.IsBombCount) {
            if (bombHexagon == null) {
                bombHexagon = destroyedHexagons[0];
                bombHexagon.transform.rotation = Quaternion.Euler(Vector3.zero);
                UIManager.ins.bombHexagonT = bombHexagon.transform;
                bombHexagon.render.sprite = bombSprite;
            }
            else {
                if (Array.IndexOf(destroyedHexagons, bombHexagon) > -1 && skipOneTime) {
                    bombHexagon.render.sprite = gridManager.DefaultSprite;
                    UIManager.ins.IsBombCount = false;
                }
                skipOneTime = true;
            }
        } 
        else {
            bombHexagon = null;
            UIManager.ins.bombHexagonT = null;
        }
    }

    public void ChangeParentOfSelectedHexagons(Transform parent)
    {
        for (int i = 0; i < Info.selectedTripleHexagons.Length; i++)
        {
            Info.selectedTripleHexagons[i].transform.SetParent(parent);
        }
    }

    public IEnumerator FallAnim(Hexagon hex, Vector2 endPos)
    {
        Vector2 startPos = hex.transform.position;
        float percent = 0;
        float easedPercent;
        while (percent < 1)
        {
            percent += hexFallSpeed * Time.deltaTime;
            percent = Mathf.Clamp01(percent);
            easedPercent = Utility.Ease(percent, easeAmountFall);
            
            hex.transform.localPosition = Vector2.Lerp(startPos, endPos, easedPercent);
            yield return null;
        }
    }
    public void MakeFallAnim(Hexagon hex, Vector2 endPos)
    {
        Info.allMovedHexagons.Add(hex);
        StartCoroutine(FallAnim(hex, endPos));
    }


    public IEnumerator CreatingAnim(AbstractHexagon absHex, Vector3 finalScale)
    {
        float percent = 0;
        float easedPercent;
        while (percent <= 1)
        {
            percent += hexCreatingSpeed * Time.deltaTime;
            percent = Mathf.Clamp01(percent);
            easedPercent = Utility.Ease(percent, easeAmountCreating);
            absHex.transform.localScale = Vector3.Lerp(Vector3.zero, finalScale, easedPercent);
            yield return null;
        }
    }
    public void MakeCreatingAnim(AbstractHexagon absHex, Vector2 finalScale)
    {
        StartCoroutine(CreatingAnim(absHex, finalScale));
    }
    
#if UNITY_EDITOR
    private void OnDrawGizmos() {
        if (Application.isPlaying)
            Gizmos.DrawWireSphere(_input.MousePosition, HexInfo.overlapRadius);
    }
#endif
}