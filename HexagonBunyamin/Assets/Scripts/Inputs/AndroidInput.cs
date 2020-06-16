using UnityEngine;

public class AndroidInput : MonoBehaviour, IInput
{
    Camera cam;
    Touch touch;
    
    Vector2 beganTouchPos;
    Vector2 endTouchPos;
    float sqrDistance;

    const float threshold = .2f;
    public Vector2 MousePosition { get; private set; }
    public int RotateDir => (int)Mathf.Sign(Vector2.Dot(new Vector2(beganTouchPos.x, 1), MousePosition));

    private void Start()
    {
        cam = Camera.main;
    }

    public bool IsClick() {
        if (touch.phase == TouchPhase.Ended && sqrDistance < threshold) {
            touch.phase = TouchPhase.Canceled;
            return true;
        }
        return false;
    }

    public bool IsRotate() {

        if (touch.phase == TouchPhase.Ended  && sqrDistance > threshold) {
            touch.phase = TouchPhase.Canceled;
            return true;
        }
        return false;
    }

    private void Update() {
        if (Input.touchCount > 0) {

            touch = Input.GetTouch(0);
            MousePosition = cam.ScreenToWorldPoint(touch.position);

            if (touch.phase == TouchPhase.Began) {
                beganTouchPos = MousePosition;
                endTouchPos = beganTouchPos;
            }
            
            if (touch.phase == TouchPhase.Ended) {
                endTouchPos = MousePosition;
                sqrDistance = (endTouchPos - beganTouchPos).sqrMagnitude;
                beganTouchPos = MousePosition;
            }
        }
    }
}