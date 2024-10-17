using UnityEngine;

public class AsteroidRotation : MonoBehaviour
{
    [Tooltip("Rotation speed and direction. Positive for clockwise, negative for counter-clockwise.")]
    [SerializeField] float rotationSpeed = 30f;
    [SerializeField] bool alternatesRotation = true;
    int rotationDirection = 1;
    Vector3 rotationAxis = Vector3.forward;

    void Update()
    {
        // Rotate the asteroid based on the rotationSpeed value
        transform.Rotate(rotationAxis, -rotationSpeed * rotationDirection * Time.deltaTime);
    }

    void OnBecameInvisible()
    {
        if (alternatesRotation)
        {
            rotationDirection *= -1;
        }
    }
}
