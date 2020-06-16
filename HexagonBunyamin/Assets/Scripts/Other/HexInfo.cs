using UnityEngine;

public static class HexInfo
{
    public const float outerRadius = .5f;
    public const float innerRadius = outerRadius * 0.86602540378f;
    public const float overlapRadius = innerRadius * 3f;

    public const float animTimeSkinWidth = .3f;

    // 30, 90, 150... derece
    public static readonly Vector2[] neighberDir = new [] { 
        new Vector2(0.9f, 0.5f), 
        new Vector2(0.0f, 1.0f),
        new Vector2(-0.9f, 0.5f),
        new Vector2(-0.9f, -0.5f),
        new Vector2(0.0f, -1.0f),
        new Vector2(0.9f, -0.5f)
    };

    public static readonly int[] upperVertexIndex = new [] { 2 ,3 };
    public static readonly int[] bottomVertexIndex = new [] { 5 ,6 };

}