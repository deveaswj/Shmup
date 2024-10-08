using UnityEngine;
using System.Collections;

// revising to be separate from Player but follow its transform
// assign the object to the scene, but not as a child of Player

public class Shield : MonoBehaviour
{
    [SerializeField] GameObject playerShip;
    [SerializeField] int shieldHitPoints = 10;         // Number of hits the shield can take
    [SerializeField] LayerMask enemyLayer;             // Layer for detecting enemies
    [SerializeField] LayerMask projectileLayer;        // Layer for detecting projectiles
    [Header("Effects")]
    [SerializeField] Color shieldHitColor = Color.red;    // Color of the shield
    [SerializeField] float shieldHitFXDuration = 0.1f;

    LayerMask combinedLayerMask;
    Color originalColor;
    SpriteRenderer sr;
    bool hitShield = false;
    Transform shipTransform;

    float shieldHitFXTimer = 0f;

    private void Start()
    {
        shipTransform = playerShip.transform;
        sr = GetComponent<SpriteRenderer>();
        originalColor = sr.color;
        combinedLayerMask = enemyLayer | projectileLayer;
        TurnOn();
    }

    private void Update()
    {
        transform.position = shipTransform.position;

        if (shieldHitFXTimer > 0)
        {
            shieldHitFXTimer -= Time.deltaTime;
        }
        else
        {
            hitShield = false;
        }
        sr.color = hitShield ? shieldHitColor : originalColor;
    }

    private bool IsInLayerMask(GameObject obj, LayerMask layerMask)
    {
        // The object's layer is compared against the LayerMask using bitwise operations
        return layerMask == (layerMask | (1 << obj.layer));
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (IsInLayerMask(other.gameObject, combinedLayerMask))
        {
            if (other.TryGetComponent<Health>(out var otherHealth))
            {
                Debug.Log("Shield hit by " + otherHealth.gameObject.name);
                otherHealth.TakeDamage(int.MaxValue);    // damage the enemy to the max!
            }
            else if (other.TryGetComponent<DamageDealer>(out var damageDealer))
            {
                Debug.Log("Shield hit by " + damageDealer.gameObject.name);
                damageDealer.Hit();
            }
            TakeShieldHit();
        }
    }

    private void TakeShieldHit()
    {
        hitShield = true;
        shieldHitPoints--;
        if (shieldHitPoints > 0)
        {
            shieldHitFXTimer = shieldHitFXDuration;
        }
        else
        {
            TurnOff();  // Trigger shield destruction or deactivation
        }
    }

    private void TurnOff()
    {
        // Deactivate the shield and move ship to original layer
        gameObject.SetActive(false);
    }

    private void TurnOn()
    {
        // Activate the shield and move ship to Invulnerable layer
        gameObject.SetActive(true);
    }
}
