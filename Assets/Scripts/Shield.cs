using UnityEngine;
using System.Collections;

public class Shield : MonoBehaviour
{
    [SerializeField] float shieldRadius = 5f;          // Radius of the circular shield
    [SerializeField] Vector2 offset = new Vector2(0f, 0f); // Position offset of the shield
    [SerializeField] int shieldHitPoints = 10;         // Number of hits the shield can take
    [SerializeField] LayerMask enemyLayer;             // Layer for detecting enemies
    [SerializeField] LayerMask projectileLayer;        // Layer for detecting projectiles
    [SerializeField] float detectionInterval = 0.1f;   // How often to check for enemies/projectiles

    private void Start()
    {
        StartCoroutine(ShieldDetectionRoutine());
    }

    IEnumerator ShieldDetectionRoutine()
    {
        while (shieldHitPoints > 0)
        {
            DetectAndDestroyEnemiesAndProjectiles();
            yield return new WaitForSeconds(detectionInterval);  // Wait before checking again
        }

        // Shield is broken, deactivate or destroy
        DestroyShield();
    }

    private void DetectAndDestroyEnemiesAndProjectiles()
    {
        Vector2 offsetPosition = (Vector2)transform.position + offset;

        // For circular shield
        Collider2D[] detectedEnemies = Physics2D.OverlapCircleAll(offsetPosition, shieldRadius, enemyLayer);
        Collider2D[] detectedProjectiles = Physics2D.OverlapCircleAll(offsetPosition, shieldRadius, projectileLayer);

        // Handle enemies within the shield
        foreach (Collider2D enemy in detectedEnemies)
        {
            if (enemy.TryGetComponent<Health>(out var health))
            {
                health.TakeDamage(int.MaxValue);    // damage the enemy to the max!
            }
            else
            {
                Destroy(enemy.gameObject);  // Destroy the enemy as needed
            }
            TakeShieldHit();            // Register the hit on the shield
        }

        // Handle enemy projectiles within the shield
        foreach (Collider2D projectile in detectedProjectiles)
        {
            Destroy(projectile.gameObject);  // Destroy the projectile
            TakeShieldHit();                 // Register the hit on the shield
        }
    }

    private void TakeShieldHit()
    {
        shieldHitPoints--;

        if (shieldHitPoints <= 0)
        {
            DestroyShield();  // Trigger shield destruction or deactivation
        }
    }

    private void DestroyShield()
    {
        // Deactivate or destroy the shield
        Destroy(gameObject);
    }

    private void OnDrawGizmos()
    {
        // For visualization in the Unity Editor (optional)
        Gizmos.color = Color.red;
        Vector2 offsetPosition = (Vector2) transform.position + offset;
        Gizmos.DrawWireSphere(offsetPosition, shieldRadius);
     }
}
