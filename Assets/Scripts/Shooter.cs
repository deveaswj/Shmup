using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shooter : MonoBehaviour
{
    [Header("General")]
    // [SerializeField] GameObject projectilePrefab;
    [SerializeField] ProjectilePool projectilePool;
    [SerializeField] ProjectileType projectileType;
    [SerializeField] float projectileSpeed = 10f;
    [SerializeField] float projectileLifetime = 5f;
    [SerializeField] float baseFiringRate = 0.2f;
    float speedMultiplier = 1.0f;

    [Header("AI")]
    [SerializeField] bool droneAI = false;
    [SerializeField] string droneAIProjectilePoolTag = "PlayerProjectilePool";
    [SerializeField] bool enemyAI = false;
    [SerializeField] string enemyAIProjectilePoolTag = "EnemyProjectilePool";
    [SerializeField] float firingRateVariance = 0f;
    [SerializeField] float minimumFiringRate = 0.1f;

    Vector2 minBounds, maxBounds;

    bool isFiring = false;

    string debugPrefix = "Shooter: ";

    Coroutine firingCoroutine;

    public void SetFiring(bool value) => isFiring = value;
    public void SetProjectileType(ProjectileType newType) => projectileType = newType;

    public void SetSpeedMultiplier(float newMultiplier) => speedMultiplier = newMultiplier;
    public void SetSpeedMultiplier() => speedMultiplier = 1.0f;

    void Start()
    {
        InitializeBounds();
        debugPrefix = (enemyAI ? "Enemy " : (droneAI ? "Drone " : "Player ")) + "Shooter: ";
        FindProjectilePool();
        SetFiring(enemyAI);
    }

    void FindProjectilePool()
    {
        if (projectilePool == null)
        {
            if (enemyAI || droneAI)
            {
                string externalProjectilePoolObjectTag = enemyAI ? enemyAIProjectilePoolTag : droneAIProjectilePoolTag;
                GameObject externalProjectilePoolObject = GameObject.FindWithTag(externalProjectilePoolObjectTag);
                if (externalProjectilePoolObject != null)
                {
                    // Debug.Log(debugPrefix + "External Projectile Pool object found");
                    projectilePool = externalProjectilePoolObject.GetComponent<ProjectilePool>();
                    if (projectilePool == null)
                    {
                        Debug.LogError(debugPrefix + "... But ProjectilePool component not found!");
                    }
                }
                else
                {
                    Debug.LogError(debugPrefix + "External Projectile Pool not found in the scene!");
                }
            }
            else
            {
                Debug.LogError(debugPrefix + "ProjectilePool is null!");
            }
        }
    }


    void InitializeBounds()
    {
        Camera mainCamera = Camera.main;
        minBounds = mainCamera.ViewportToWorldPoint(new Vector2(0, 0));
        maxBounds = mainCamera.ViewportToWorldPoint(new Vector2(1, 1));
    }

    bool OutOfBounds() => transform.position.x < minBounds.x || transform.position.x > maxBounds.x || transform.position.y < minBounds.y || transform.position.y > maxBounds.y;

    void Update()
    {
        HandleFiring();
    }

    void HandleFiring()
    {
        if (isFiring && firingCoroutine == null)
        {
            // Debug.Log(debugPrefix + "firing, start coroutine");
            firingCoroutine = StartCoroutine(FireContinuously());
        }
        else if (!isFiring && firingCoroutine != null)
        {
            // Debug.Log(debugPrefix + "not firing, stop coroutine");
            StopCoroutine(firingCoroutine);
            firingCoroutine = null;
        }
        else
        {
            string coroutineState = firingCoroutine == null ? "null" : "not null";
            // Debug.LogError(debugPrefix + "firing is " + isFiring + ", coroutine is " + coroutineState);
        }
    }

    IEnumerator FireContinuously()
    {
        bool errorState;
        float firingRate;

        // Ensure the pool is assigned before starting firing
        while (projectilePool == null)
        {
            yield return null;
        }

        while (true)
        {
            errorState = false;
            if (OutOfBounds())
            {
                // Debug.Log(debugPrefix + "Shooter: Out of bounds");
            }
            else if (isFiring)
            {
                errorState = FireOnce();
            }

            // determine when we can fire again
            firingRate = baseFiringRate;
            if (enemyAI)
            {
                firingRate += Random.Range(-firingRateVariance, firingRateVariance);
                firingRate = Mathf.Max(firingRate, minimumFiringRate);
            }

            yield return new WaitForSeconds(firingRate);
        }
    }

    bool FireOnce()
    {
        bool errorState = false;
        if (projectilePool == null)
        {
            errorState = true;
            Debug.LogError(debugPrefix + "ProjectilePool is null while trying to fire a projectile");
        }
        Projectile projectile = projectilePool.GetProjectile(projectileType);
        if (projectile == null)
        {
            errorState = true;
            Debug.LogError(debugPrefix + "Projectile is null while trying to fire a projectile");
        }
        if (!errorState)
        {
            // player & drones fire up, enemies fire down
            float finalSpeed = speedMultiplier * projectileSpeed;
            Vector2 velocity = (enemyAI ? -transform.up : transform.up) * finalSpeed;
            projectile.Fire(transform.position, velocity);
            StartCoroutine(ReturnProjectileAfterLifetime(projectile, projectileLifetime));
        }
        return errorState;
    }


    private IEnumerator ReturnProjectileAfterLifetime(Projectile projectile, float lifetime)
    {
        yield return new WaitForSeconds(lifetime);

        if (projectilePool != null)
        {
            // Deactivate the projectile after its lifetime expires
            projectile.Deactivate();

            // Return the projectile to the pool
            projectilePool.ReturnProjectile(projectile);
        }
        else
        {
            Debug.LogError(debugPrefix + "ProjectilePool is null while trying to return a projectile");
        }
    }

}
