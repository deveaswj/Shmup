using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectilePool : MonoBehaviour
{
    public GameObject projectilePrefab;
    public int poolSize = 10;

    private Queue<Projectile> pool;

    void Start()
    {
        if (projectilePrefab == null)
        {
            Debug.LogError("Projectile prefab is null at Start");
        }

        pool = new Queue<Projectile>();

        // Pre-instantiate the projectiles in the pool
        for (int i = 0; i < poolSize; i++)
        {
            Projectile projectile = NewProjectileComponent();
            pool.Enqueue(projectile);
        }
    }

    Projectile NewProjectileComponent()
    {
        if (projectilePrefab == null)
        {
            Debug.LogError("Projectile prefab is null at NewProjectileComponent");
            return null;
        }

        GameObject obj = Instantiate(projectilePrefab);
        if (obj == null)
        {
            Debug.LogError("Failed to Instantiate Projectile");
            return null;
        }
        obj.SetActive(false);
        if (!obj.TryGetComponent<Projectile>(out var projectile))
        {
            Debug.LogError("Projectile component not found on instantiated object.");
            return null;
        }
        return projectile;
    }

    public Projectile GetProjectile()
    {
        Projectile projectile;
        if (pool.Count > 0)
        {
            projectile = pool.Dequeue();
        }
        else
        {
            projectile = NewProjectileComponent();
        }
        if (projectile == null)
        {
            Debug.LogError("Failed to GetProjectile");
        }
        return projectile;
    }

    public void ReturnProjectile(Projectile projectile)
    {
        pool.Enqueue(projectile);
    }
}
