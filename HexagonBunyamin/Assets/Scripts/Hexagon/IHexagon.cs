using UnityEngine;


public interface IHexagon {
    Hexagon[] GetSameColorHexagon(Hexagon hexagon);
    void MakeFallAnim(Hexagon Hexagon, Vector2 endPos);
}