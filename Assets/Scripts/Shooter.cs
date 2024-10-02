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

    Coroutine firingCoroutine;

    public void SetFiring(bool value) => isFiring = value;

    void Start()
    {
        SetFiring(enemyAI);
    }

    void Update()
    {
        Fire();
    }

    void Fire()
    {
        if (isFiring && firingCoroutine == null)
        {
            firingCoroutine = StartCoroutine(FireContinuously());
        }
        else if (!isFiring && firingCoroutine != null)
        {
            StopCoroutine(firingCoroutine);
            firingCoroutine = null;
        }
    }

    IEnumerator FireContinuously()
    {
        while (true)
        {
            if (isFiring)
            {
                // --- Formerly: ---
                //    GameObject projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
                //    if (projectile.TryGetComponent<Rigidbody2D>(out var rb))
                //    {
                //        // player fires up, enemies fire down
                //        rb.velocity = (enemyAI ? -transform.up : transform.up) * projectileSpeed;
                //    }
                //    Destroy(projectile, projectileLifetime);

                Projectile projectile = projectilePool.GetProjectile();
                Vector2 velocity = (enemyAI ? -transform.up : transform.up) * projectileSpeed;
                projectile.Fire(transform.position, velocity);
                StartCoroutine(ReturnProjectileAfterLifetime(projectile, projectileLifetime));
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

        // Deactivate the projectile after its lifetime expires
        projectile.Deactivate();

        // Return the projectile to the pool
        projectilePool.ReturnProjectile(projectile.gameObject);
    }

}
