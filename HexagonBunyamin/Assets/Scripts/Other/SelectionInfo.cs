using UnityEngine;
using System.Collections.Generic;
using System.Linq;

[System.Serializable]
public class SelectionInfo
{
    public SelectionInfo()
    {
        selectedTripleHexagons = new Hexagon[3];
    }
    
    public Hexagon[] selectedTripleHexagons;
    public List<Vector2> bottomRayPosList = new List<Vector2>();
    public List<Hexagon> destroyedAllHexagons = new List<Hexagon>();
    List<Vector2> destroyedAllHexagonsPos = new List<Vector2>();
    public List<Hexagon> allMovedHexagons = new List<Hexagon>();
    public Vector2 oldMousePos;


    public void DistinctDestroyedHexList()
    {
        destroyedAllHexagons = destroyedAllHexagons.Distinct().ToList();
    }

    public void AddDestroyedHexagons(Hexagon[] hexagons)
    {
        destroyedAllHexagons.AddRange(hexagons);
    }

    public void SetBottomRayPositions()
    {
        destroyedAllHexagonsPos = Utility.GetPositionFromThisObject(destroyedAllHexagons);
        bottomRayPosList = Utility.GetBottomRayPositions(destroyedAllHexagonsPos);
    }

    public List<Vector2> GetDestroyedHexPos()
    {
        return destroyedAllHexagonsPos;
    }

    public void Clear()
    {
        bottomRayPosList.Clear();
        destroyedAllHexagons.Clear();
        allMovedHexagons.Clear();
    }
}