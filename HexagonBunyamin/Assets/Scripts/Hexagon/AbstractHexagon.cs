using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbstractHexagon : MonoBehaviour, IComparable<AbstractHexagon>  {
    
    public CircleCollider2D circleCollider;
    public SpriteRenderer render;
    public int id;
    public Vector2 oldPosition;
    public Vector2 finalPosition;

    public IAbstractHexagon _AbstractHexagon;
    
    public Color Color
    {
        get { return render.color; }
        set { render.color = value; }
    }

    public int CompareTo(AbstractHexagon other)
    {
        if (id == other.id)
            return 0;
        return -1;
    }

    public void RequestMakeCreatingAnim(Vector2 finalScale)
    {
        _AbstractHexagon.MakeCreatingAnim(this, finalScale);
    }

    public abstract void Register(HexagonControlMachine machine);
}