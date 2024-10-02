using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    //[SerializeField] float speed = 10f;
    //[SerializeField] float lifetime = 5f;
    //[SerializeField] float baseFiringRate = 0.2f;
    //[SerializeField] float firingRateVariance = 0f;
    //[SerializeField] float minimumFiringRate = 0.1f;

    private Rigidbody2D rb;

    void Awake()
    {
        // Cache the Rigidbody2D for setting velocity when the projectile is fired
        rb = GetComponent<Rigidbody2D>();
    }

    // Method to fire the projectile
    public void Fire(Vector3 position, Vector2 velocity)
    {
        // Set position and activate the projectile
        transform.position = position;
        gameObject.SetActive(true);

        // Apply velocity
        rb.velocity = velocity;
    }

    // Method to deactivate the projectile
    public void Deactivate()
    {
        // Deactivates the entire GameObject, which will disable all components
        gameObject.SetActive(false);
    }
}
