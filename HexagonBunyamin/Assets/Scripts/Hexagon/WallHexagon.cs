using UnityEngine;

// Farklı bir çeşit hexagon olursa diye örnek amaçlı eklendi
public class WallHexagon : AbstractHexagon
{
    public override void Register(HexagonControlMachine machine)
    {
        _AbstractHexagon = machine;
    }
}