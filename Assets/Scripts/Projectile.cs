using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Designed for use with a pooling system, but not required
// Can fire itself, provided with a position and velocity

public enum ProjectileType
{
    SingleShot,
    DoubleShot,
    Photon,
    Enemy
}

public class Projectile : MonoBehaviour
{
    // these should probably be in a scriptable object instead
    //[SerializeField] float speed = 10f;
    //[SerializeField] float lifetime = 5f;
    //[SerializeField] float baseFiringRate = 0.2f;
    //[SerializeField] float firingRateVariance = 0f;
    //[SerializeField] float minimumFiringRate = 0.1f;

    [SerializeField] ProjectileType projectileType;

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
