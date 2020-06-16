using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections;

public class Hexagon : AbstractHexagon
{
    public LayerMask hexMask;

    public IHexagon _Controlable;

    public void RequestMakeFallAnim(Vector2 endPos)
    {
        oldPosition = transform.position;
        _Controlable.MakeFallAnim(this, endPos);
    }

    public override void Register(HexagonControlMachine machine)
    {
        _Controlable = machine;
        _AbstractHexagon = machine;
    }

    public Hexagon[] RequestNeigberHexagon()
    {
        return _Controlable.GetSameColorHexagon(this);
    }
}