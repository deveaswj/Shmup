using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shooter : MonoBehaviour
{
    [Header("General")]
    [SerializeField] GameObject projectilePrefab;
    [SerializeField] ProjectilePool projectilePool;
    [SerializeField] float projectileSpeed = 10f;
    [SerializeField] float projectileLifetime = 5f;
    [SerializeField] float baseFiringRate = 0.2f;

    [Header("AI")]
    [SerializeField] bool enemyAI = false;
    [SerializeField] float firingRateVariance = 0f;
    [SerializeField] float minimumFiringRate = 0.1f;

    bool isFiring = false;

    string debugPrefix = "Shooter: ";

    Coroutine firingCoroutine;

    public void SetFiring(bool value) => isFiring = value;

    void Start()
    {
        debugPrefix = (enemyAI ? "Enemy " : "Player ") + "Shooter: ";
        SetFiring(enemyAI);

        if (projectilePool == null && enemyAI)
        {
            GameObject enemyProjectilePoolObject = GameObject.FindWithTag("EnemyProjectilePool");
            if (enemyProjectilePoolObject != null)
            {
                Debug.Log(debugPrefix + "Enemy Projectile Pool object found");
                projectilePool = enemyProjectilePoolObject.GetComponent<ProjectilePool>();
                if (projectilePool == null)
                {
                    Debug.LogError(debugPrefix + "... But ProjectilePool component not found!");
                }
            }
            else
            {
                Debug.LogError(debugPrefix + "Enemy Projectile Pool not found in the scene!");
            }
        }
    }

    void Update()
    {
        Fire();
    }

    void Fire()
    {
        if (isFiring && firingCoroutine == null)
        {
            Debug.Log(debugPrefix + "firing, start coroutine");
            firingCoroutine = StartCoroutine(FireContinuously());
        }
        else if (!isFiring && firingCoroutine != null)
        {
            Debug.Log(debugPrefix + "not firing, stop coroutine");
            StopCoroutine(firingCoroutine);
            firingCoroutine = null;
        }
        else
        {
            string coroutineState = firingCoroutine == null ? "null" : "not null";
            Debug.Log(debugPrefix + "firing is " + isFiring + ", coroutine is " + coroutineState);
        }
    }

    IEnumerator FireContinuously()
    {
        bool errorState = false;
        Debug.Log(debugPrefix + "Entered FireContinuously coroutine");

        // Ensure the pool is assigned before starting firing
        while (projectilePool == null)
        {
            Debug.Log(debugPrefix + "Waiting for projectilePool to be assigned...");
            yield return null;
        }

        while (true)
        {
            Debug.Log(debugPrefix + "Firing? " + isFiring);
            if (isFiring)
            {
                if (projectilePool == null)
                {
                    errorState = true;
                    Debug.LogError(debugPrefix + "ProjectilePool is null while trying to fire a projectile");
                }
                Projectile projectile = projectilePool.GetProjectile();
                if (projectile == null)
                {
                    errorState = true;
                    Debug.LogError(debugPrefix + "Projectile is null while trying to fire a projectile");
                }
                if (!errorState)
                {
                    // player fires up, enemies fire down
                    Vector2 velocity = (enemyAI ? -transform.up : transform.up) * projectileSpeed;
                    projectile.Fire(transform.position, velocity);
                    Debug.Log(debugPrefix + "Fired projectile");
                    StartCoroutine(ReturnProjectileAfterLifetime(projectile, projectileLifetime));
                }
            }
            float firingRate = baseFiringRate;
            if (enemyAI)
            {
                firingRate += Random.Range(-firingRateVariance, firingRateVariance);
                firingRate = Mathf.Max(firingRate, minimumFiringRate);
            }
            yield return new WaitForSeconds(firingRate);
        }
    }

    private IEnumerator ReturnProjectileAfterLifetime(Projectile projectile, float lifetime)
    {
        yield return new WaitForSeconds(lifetime);

        if (projectilePool != null)
        {
            // Deactivate the projectile after its lifetime expires
            projectile.Deactivate();

            // Return the projectile to the pool
            projectilePool.ReturnProjectile(projectile.gameObject);
        }
        else
        {
            Debug.LogError(debugPrefix + "ProjectilePool is null while trying to return a projectile");
        }
    }

}
