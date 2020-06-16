using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

using UnityEngine.SceneManagement;

public class GridManager : MonoBehaviour
{
    [Range(0, 15)]
    public int width = 6;
    [Range(0, 15)]
    public int height = 6;
    [Range(0, 1)]
    public float hexagonScale = 1;

    [SerializeField]
    GameObject hexagonPrefab;

    [Header("Color")]
    public Color[] hexColors;
    public int colorSeed;

    [Header("Animation")]
    public int suffleSeed;
    [Range(0, 1)] public float creatingDuration;
    WaitForSeconds creatingWaitForSecond;
    bool gridAnimIsRunning = true;

    [SerializeField]
    HexagonControlMachine machine;
    [SerializeField]
    Bounds boundsMap;
    public Sprite DefaultSprite { get; private set; }

    private void Awake()
    {
        creatingWaitForSecond = new WaitForSeconds(creatingDuration);
        CreateMap();
    }
    
    public void CreateMap()
    {
// #if UNITY_EDITOR
//         foreach (Transform child in transform)
//             StartCoroutine(Destroy(child.gameObject));
// #endif
        List<AbstractHexagon> tempAbstractHexagons = new List<AbstractHexagon>();

        float startPosX = ((width - 1) * 2f * HexInfo.outerRadius * .75f) / -2f;
        float startPosY = ((height - 1) * 2f * HexInfo.innerRadius + HexInfo.innerRadius) / -2f;
        float newStartPosY = startPosY;

        Vector2 position = Vector2.zero;
        for (int x = 0; x < width; x++)
        {
            position.x = startPosX + 2 * HexInfo.outerRadius * x * .75f;
            for (int y = 0; y < height; y++)
            {
                newStartPosY = x % 2 == 1 ? startPosY + HexInfo.innerRadius : startPosY;
                position.y = newStartPosY + HexInfo.innerRadius * 2f * y;
                AbstractHexagon abshexagon = Instantiate(hexagonPrefab).GetComponent<AbstractHexagon>();
                abshexagon.transform.position = position;
                abshexagon.transform.SetParent(transform);
                abshexagon.transform.localScale = Vector3.zero;
                tempAbstractHexagons.Add(abshexagon);
                abshexagon.name = "Hexagon " + (tempAbstractHexagons.Count - 1);
                abshexagon.Register(machine);
                machine.AddDictionary((Hexagon)abshexagon);
            }
        }
        DefaultSprite = tempAbstractHexagons[0].render.sprite;
        Utility.SetHexagonRandomColor(tempAbstractHexagons, hexColors, colorSeed);
        
        boundsMap.size = new Vector2(-startPosX, -startPosY) * 2f;
        boundsMap.Expand(.6f);
        Camera.main.orthographicSize = (boundsMap.size.x + .8f) / (2f * Camera.main.aspect);

        StartCoroutine(MakeGridAnim(tempAbstractHexagons.ToArray()));
    }

    IEnumerator MakeGridAnim(AbstractHexagon[] allAbstractHexagons)
    {
        AbstractHexagon[] absHexagons = Utility.ShuffleArray<AbstractHexagon>(allAbstractHexagons, suffleSeed);
        for (int i = 0; i < absHexagons.Length; i++)
        {
            absHexagons[i].RequestMakeCreatingAnim(Vector3.one * hexagonScale);
            yield return creatingWaitForSecond;
        }
        gridAnimIsRunning = false;
    }

    public bool IsMapAreaFrom(Vector2 mousePos)
    {
        if (boundsMap.Contains(mousePos) && !gridAnimIsRunning)
        {
            return true;
        }
        return false;
    }

    public Vector2[] GetPossibleNewHexagonPositions(List<Vector2> bottomRayPos)
    {
        List<Vector2> possiblePos = new List<Vector2>();
        for (int i = 0; i < bottomRayPos.Count; i++)
        {
            Vector2 pos = bottomRayPos[i];
            possiblePos.Add(pos);
            while (true)
            {
                pos.y += 2f * HexInfo.innerRadius;
                if (pos.y < boundsMap.max.y)
                    possiblePos.Add(pos);
                else
                    break;
            }
        }
        return possiblePos.ToArray();
    }

#if UNITY_EDITOR
    IEnumerator Destroy(GameObject go)
    {
        yield return new WaitForEndOfFrame();
        DestroyImmediate(go);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(boundsMap.center, boundsMap.size);
    }
#endif
}

/*
 public void CreateMap()
    {
#if UNITY_EDITOR
        foreach (Transform child in transform)
            StartCoroutine(Destroy(child.gameObject));
#endif
        // Harita'yı ortalamak için başlangıç pozisyonu
        Vector2 addedPosition = -new Vector2(((width - 1) * 3 * HexInfo.outerRadius) / 2f, (height - 1) * HexInfo.innerRadius / 2f);
        List<AbstractHexagon> tempAbstractHexagons = new List<AbstractHexagon>();

        for (int y = 0; y < height; y++)
        {
            Vector2 newAddedPosition = addedPosition;
            int hexagonToDecrease = 0;
            if (y % 2 == 1)
            {
                hexagonToDecrease = 1;
                newAddedPosition = addedPosition + new Vector2(HexInfo.outerRadius + (HexInfo.outerRadius / 2f), 0);
            }
            for (int x = 0; x < width - hexagonToDecrease; x++)
            {
                Vector2 position = (newAddedPosition + new Vector2(x * 3 * HexInfo.outerRadius, y * HexInfo.innerRadius));
                AbstractHexagon abshexagon = Instantiate(hexagonPrefab).GetComponent<Hexagon>();
                abshexagon.transform.localScale = Vector2.one * hexagonScale;
                abshexagon.transform.SetParent(transform, false);
                abshexagon.transform.localPosition = position;
                tempAbstractHexagons.Add(abshexagon);
                abshexagon.name = "Hexagon " + (tempAbstractHexagons.Count - 1);
                abshexagon.Register(controlTower);
            }
        }
        DefaultMaterial = tempAbstractHexagons[0].meshRenderer.material;
        Utility.SetHexagonRandomColor(tempAbstractHexagons, hexColors, colorSeed);

        // boundsMap.size = -addedPosition * 2f;
        // boundsMap.Expand(.6f);
        // Camera.main.orthographicSize = (boundsMap.size.x + .8f) / (2f * Camera.main.aspect);
    }
*/





/*
float startPosX = ((width - 1) * 2f * HexInfo.outerRadius + (width - 1) * HexInfo.outerRadius) / -2f;
        float startPosY = (height - 1) * HexInfo.innerRadius / -2f;
        for (int x = 0; x < width; x++)
        {
            Vector2 position = Vector2.zero;
            position.x = startPosX + HexInfo.outerRadius * x * 3f;
            for (int y = 0; y < height; y++)
            {
                position.y = startPosY + HexInfo.innerRadius * y;
                AbstractHexagon abshexagon = Instantiate(hexagonPrefab).GetComponent<Hexagon>();
                abshexagon.transform.position = position;
                abshexagon.transform.SetParent(transform);

            }
*/
// int hexagonToDecrease = 0;
// if (x % 2 == 1) {
//     hexagonToDecrease = 1;
//     newStartPosX = startPosX + .75f * HexInfo.outerRadius * 2f;
// } else {
//     newStartPosX = startPosX;
// }