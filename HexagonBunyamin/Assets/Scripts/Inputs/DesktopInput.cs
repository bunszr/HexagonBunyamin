using UnityEngine;

public class DesktopInput : MonoBehaviour, IInput
{
    Camera cam;
    public Vector2 MousePosition { get; private set; }
    // public int RotateDir => (int)Mathf.Sign(Vector2.Dot(new Vector2(MousePosition.x, 1), MousePosition));
    public int RotateDir => (int)Mathf.Sign(Vector2.Dot(new Vector2(MousePosition.x, 1), MousePosition));

    private void Start()
    {
        cam = Camera.main;
    }

    public bool IsClick() {
        if (Input.GetMouseButtonDown(0)) {
            return true;
        }
        return false;
    }

    public bool IsRotate() {
        if (Input.GetMouseButtonDown(1)) {
            return true;
        }
        return false;
    }

    private void Update() {
        MousePosition = cam.ScreenToWorldPoint(Input.mousePosition);
    }
}