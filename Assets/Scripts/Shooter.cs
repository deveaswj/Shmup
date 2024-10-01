using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shooter : MonoBehaviour
{
    [Header("General")]
    [SerializeField] GameObject projectilePrefab;
    [SerializeField] float projectileSpeed = 10f;
    [SerializeField] float projectileLifetime = 5f;
    [SerializeField] float baseFiringRate = 0.2f;

    [Header("AI")]
    [SerializeField] bool useAI = false;
    [SerializeField] float firingRateVariance = 0f;
    [SerializeField] float minimumFiringRate = 0.1f;

    bool isFiring = false;

    Coroutine firingCoroutine;

    public void SetFiring(bool value) => isFiring = value;

    void Start()
    {
        SetFiring(useAI);
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
                GameObject projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
                if (projectile.TryGetComponent<Rigidbody2D>(out var rb))
                {
                    rb.velocity = (useAI ? -transform.up : transform.up) * projectileSpeed;
                }
                Destroy(projectile, projectileLifetime);
            }
            float firingRate = baseFiringRate;
            if (useAI)
            {
                firingRate += Random.Range(-firingRateVariance, firingRateVariance);
                firingRate = Mathf.Max(firingRate, minimumFiringRate);
            }
            yield return new WaitForSeconds(firingRate);
        }
    }
}
