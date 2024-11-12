using UnityEngine;

public class CameraBounds : MonoBehaviour
{
    private Vector2 minBounds, maxBounds;

    public float Left => minBounds.x;
    public float Bottom => minBounds.y;
    public float Right => maxBounds.x;
    public float Top => maxBounds.y;
    public Vector2 Min => minBounds;
    public Vector2 Max => maxBounds;

    // Start is called before the first frame update
    void Start()
    {
        InitializeBounds();
    }

    public bool OutOfBounds(Vector3 point) => point.x < minBounds.x || point.x > maxBounds.x || point.y < minBounds.y || point.y > maxBounds.y;
    public bool OutOfBounds(Vector2 point) => point.x < minBounds.x || point.x > maxBounds.x || point.y < minBounds.y || point.y > maxBounds.y;
    //public bool WithinBounds(Vector3 point) => point.x >= minBounds.x && point.x <= maxBounds.x && point.y >= minBounds.y && point.y <= maxBounds.y;
    //public bool WithinBounds(Vector2 point) => point.x >= minBounds.x && point.x <= maxBounds.x && point.y >= minBounds.y && point.y <= maxBounds.y;

    void InitializeBounds()
    {
        Camera mainCamera = Camera.main;
        minBounds = mainCamera.ViewportToWorldPoint(new Vector2(0, 0));
        maxBounds = mainCamera.ViewportToWorldPoint(new Vector2(1, 1));
        Debug.Log("Camera Bounds: " + minBounds + " to " + maxBounds);
    }


}
