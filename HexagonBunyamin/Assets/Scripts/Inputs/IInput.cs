using UnityEngine;

public interface IInput {

    bool IsRotate();
    bool IsClick();
    Vector2 MousePosition { get; }
    int RotateDir { get; }
}