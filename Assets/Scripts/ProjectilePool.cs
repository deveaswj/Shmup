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
        pool = new Queue<Projectile>();

        // Pre-instantiate the projectiles in the pool
        for (int i = 0; i < poolSize; i++)
        {
            GameObject obj = Instantiate(projectilePrefab);
            obj.SetActive(false);

            Projectile projectile = obj.GetComponent<Projectile>();
            pool.Enqueue(projectile);
        }
    }

    public Projectile GetProjectile()
    {
        if (pool.Count > 0)
        {
            Projectile projectile = pool.Dequeue();
            return projectile;
        }
        else
        {
            GameObject newObj = Instantiate(projectilePrefab);
            newObj.SetActive(false);
            return newObj.GetComponent<Projectile>();
        }
    }

    public void ReturnProjectile(GameObject projectileObj)
    {
        Projectile projectile = projectileObj.GetComponent<Projectile>();
        pool.Enqueue(projectile);
    }
}
