using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Designed for use with a pooling system, but not required
// Can fire itself, when provided with a position and velocity

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
    // [SerializeField] GameObject spriteSource;

    private Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void Fire(Vector2 position, Vector2 velocity, int counter = 0)
    {
        transform.position = position;
        gameObject.SetActive(true);
        rb.velocity = velocity;

        // special code for Photon type
        // if (projectileType == ProjectileType.Photon)
        // {
        // }
    }

    public void Deactivate()
    {
        gameObject.SetActive(false);
    }

    public ProjectileType GetProjectileType() => projectileType;
}
