using UnityEngine;

public class AsteroidRotation : MonoBehaviour
{
    [Tooltip("Rotation speed and direction. Positive for clockwise, negative for counter-clockwise.")]
    [SerializeField] float rotationSpeed = 30f;

    void Update()
    {
        // Rotate the asteroid based on the rotationSpeed value
        transform.Rotate(Vector3.forward, -rotationSpeed * Time.deltaTime);
    }
}
