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
    [SerializeField] float projectileLifetime = 5f; // maximum lifetime
    [SerializeField] float baseFiringRate = 0.2f;
    float speedMultiplier;
    float adjustedSpeed;
    float adjustedLifetime;
    float rateMultiplier = 1f;
    Vector2 velocity;
    Vector3 direction;
    float cameraHeight;

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

    AudioManager audioManager;
    bool useAudio = true;
    float projectilePitch = 1.0f;
    float enemyProjectilePitchVariance = 0.25f;

    int counter = 0;

    void Awake()
    {
        audioManager = FindObjectOfType<AudioManager>();
    }

    public bool IsFiring() => isFiring;
    public void SetFiring(bool value) => isFiring = value;
    public void SetProjectileType(ProjectileType newType) => projectileType = newType;

    public void SetSpeedMultiplier(float newMultiplier)
    {
        speedMultiplier = newMultiplier;
        // pre-calculate these so we don't have to do it every frame
        adjustedSpeed = speedMultiplier * projectileSpeed;
        adjustedLifetime = Mathf.Clamp(cameraHeight / adjustedSpeed, 0f, 2f + projectileLifetime);
        velocity = direction * adjustedSpeed;
    }

    public void SetSpeedMultiplier() => SetSpeedMultiplier(1.0f);

    public void SetRateMultiplier(float multiplier)
    {
        rateMultiplier = multiplier;
    }

    public void SetRateMultiplier() => SetRateMultiplier(1.0f);

    void Start()
    {
        debugPrefix = (enemyAI ? "Enemy " : (droneAI ? "Drone " : "Player ")) + "Shooter: ";

        InitializeBounds();
        cameraHeight = 2 * Camera.main.orthographicSize;

        // player & drones fire up, enemies fire down
        direction = enemyAI ? -transform.up : transform.up;

        FindProjectilePool();
        SetFiring(enemyAI);
        SetSpeedMultiplier();   // call in Start to precalculate some variables

        useAudio = !droneAI;    // drones don't need audio

        if (enemyAI)
        {
            projectilePitch = 1f + Random.Range(-enemyProjectilePitchVariance, enemyProjectilePitchVariance);
        }
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
            else
            {
                // player and drones fire at the same rate
                firingRate *= rateMultiplier;
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
            // float finalSpeed = speedMultiplier * projectileSpeed;
            // float finalLifetime = cameraHeight / finalSpeed;
            // Vector2 velocity = direction * finalSpeed;
            projectile.Fire(transform.position, velocity, counter);
            counter++;

            if (useAudio) audioManager.PlayShootingClip(projectilePitch);

            StartCoroutine(ReturnProjectileAfterLifetime(projectile, adjustedLifetime));
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
