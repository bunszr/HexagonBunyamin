using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public static class Utility
{
    static System.Random random = new System.Random();

    public static T[] ShuffleArray<T> (T[] array, int seed)
    {
        System.Random prng = new System.Random (seed);

        for (int i = 0; i < array.Length; i++)
        {
            int randomIndex = prng.Next (i , array.Length);
            T tempItem = array[randomIndex];
            array[randomIndex] = array[i];
            array[i] = tempItem;
        }
        return array;
    }

    public static void SetHexagonRandomColor(List<AbstractHexagon> absHex, Color[] colors, int seed)
    {
        System.Random prng = new System.Random(seed);

        for (int i = 0; i < absHex.Count; i++)
        {
            int randomIndex = prng.Next(0, colors.Length);
            absHex[i].Color = colors[randomIndex];
            absHex[i].id = randomIndex;
        }
    }

    public static void SetHexagonRandomColor(List<Hexagon> hexagons, Color[] colors)
    {
        for (int i = 0; i < hexagons.Count; i++)
        {
            int randomIndex = random.Next(0, colors.Length);
            hexagons[i].Color = colors[randomIndex];
            hexagons[i].id = randomIndex;
        }
    }

    // 3'lü altıgende seçilmiş en üstteki altıgeni alıp ona outline rotasonunu belirmek için
    public static Hexagon GetAboveHexagonInsideGroup(Hexagon[] hexCells)
    {
        int bestIndex = 0;
        float higherYValue = 0;
        for (int i = 0; i < hexCells.Length; i++)
        {
            if (hexCells[i].transform.position.y > higherYValue)
            {
                higherYValue = hexCells[i].transform.position.y;
                bestIndex = i;
            }
        }
        return hexCells[bestIndex];
    }

    // 0-1 aralığında, parametrelerin değişmesiyle beraber farklı değerler döndüren çok güzel bir fonk.
    public static float Ease(float x, float easeAmount)
    {
        return Mathf.Pow(x, easeAmount) / (Mathf.Pow(x, easeAmount) + Mathf.Pow(1 - x, easeAmount));
    }

    // Yok edilenler arasında en alltaki altıgen pozisyonlarını alıp ray göndermek için
    public static List<Vector2> GetBottomRayPositions(List<Vector2> allPositions)
    {
        List<Vector2> copyAllPos = new List<Vector2>(allPositions);
        List<Vector2> bestVectorList = new List<Vector2>();
        List<Vector2> onlyXList = GetDistinctList(copyAllPos);
        for (int i = 0; i < onlyXList.Count; i++)
        {
            List<Vector2> sameXList = new List<Vector2>();
            sameXList.Add(new Vector2(onlyXList[i].x, 1000));
            for (int j = 0; j < copyAllPos.Count; j++)
            {
                if (Mathf.Abs(onlyXList[i].x - copyAllPos[j].x) < .2f)
                {
                    sameXList.Add(copyAllPos[j]);
                }
            }
            // Karşılaştırıp en küçüğü bulacaz
            Vector2 smallestYVector = GetSmallYValue(sameXList);
            // Vector2 finalYSmallVector = ySmallvec == Vector2.zero ? onlyXList[i] : ySmallvec;
            bestVectorList.Add(smallestYVector);
        }
        return bestVectorList;
    }

    public static List<Vector2> GetDistinctList(List<Vector2> positions)
    {
        List<Vector2> myDistinctList = new List<Vector2>(positions);
        for (int i = 0; i < myDistinctList.Count; i++)
        {
            for (int j = 0; j < myDistinctList.Count; j++)
            {
                if (Mathf.Abs(myDistinctList[i].x - myDistinctList[j].x) < .2f && i != j)
                {
                    myDistinctList.Remove(myDistinctList[j]);
                    j--;
                }
            }
        }
        return myDistinctList;
    }

    public static Vector2 GetSmallYValue(List<Vector2> sameXList)
    {
        Vector2 smallYVector = Vector2.zero;
        float dst = Mathf.Infinity;
        for (int i = 0; i < sameXList.Count; i++)
        {
            if (sameXList[i].y < dst)
            {
                dst = sameXList[i].y;
                smallYVector = sameXList[i];
            }
        }
        return smallYVector;
    }

    public static List<Vector2> GetPositionFromThisObject<T>(this List<T> tip) where T : AbstractHexagon
    {
        List<Vector2> list = tip.Select(x => (Vector2)x.transform.position).ToList();
        return list;
    }

    public static bool SelectedTripleHexIsSideBySide(params Vector3[] closestThreeHexagons)
    {
        float totalDistance = 0;
        totalDistance += Vector2.Distance(closestThreeHexagons[0], closestThreeHexagons[1]);
        totalDistance += Vector2.Distance(closestThreeHexagons[1], closestThreeHexagons[2]);
        totalDistance += Vector2.Distance(closestThreeHexagons[2], closestThreeHexagons[0]);

        Vector2 dirA = closestThreeHexagons[1] - closestThreeHexagons[0];
        Vector2 dirB = closestThreeHexagons[2] - closestThreeHexagons[0];
        float angle = Vector2.Angle(dirA, dirB);
        if (totalDistance <= 6 * HexInfo.innerRadius + .08f && 57 < angle && angle < 63)
            return true;
        return false;
    }
}