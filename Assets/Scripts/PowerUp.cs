using System.Collections;
using System.Collections.Generic;
using UnityEditor.EditorTools;
using UnityEngine;

public class PowerUp : MonoBehaviour
{
    [Header("Falling")]
    [SerializeField] float fallSpeed = 1f;
    [SerializeField] PowerUpType powerUpType;

    [Header("Floating")]
    [SerializeField] float floatSpeed = 1f;
    [SerializeField] float maxFloatRadius = 1f;
    [SerializeField] float directionChangeInterval = 1f;
    Vector2 randomDirection;

    [Header("Expiration")]
    [Tooltip("Delay before the power-up starts blinking")]
    [SerializeField] float delayBeforeBlink = 5f;
    [SerializeField] float blinkOff = 0.2f;
    [SerializeField] float blinkOn = 0.3f;
    [SerializeField] float lifetime = 10f;
    bool isBlinking = false;

    float spawnTime;
    float blinkStartTime;
    float expireTime;
    SpriteRenderer spriteRenderer;
    Vector3 spawnPosition;
    float directionChangeTimer = 0f;

    void Start()
    {
        // Save the spawn position
        spawnPosition = transform.position;
        // Capture the spawn time
        spawnTime = Time.time;
        blinkStartTime = spawnTime + delayBeforeBlink;
        expireTime = spawnTime + lifetime;
        // Cache the sprite renderer for blinking effect
        spriteRenderer = GetComponent<SpriteRenderer>();
        // Start the random movement
        ChangeRandomDirection();             // Set the initial random direction
        randomDirection = Random.insideUnitCircle.normalized;
    }

    void Update()
    {
        FloatAround();
        ChangeDirectionOnTimer();
        BlinkAsNeeded();
        DestroyIfExpired();
    }

    void BlinkAsNeeded()
    {
        // Check if it's time to start blinking
        if (isBlinking) return;
        if (Time.time >= blinkStartTime)
        {
            StartCoroutine(BlinkEffect());
        }
    }

    void DestroyIfExpired()
    {
        // Destroy the power-up after its lifetime
        if (Time.time >= expireTime)
        {
            Destroy(gameObject);
        }
    }

    void FallDownward()
    {
        transform.Translate(fallSpeed * Time.deltaTime * Vector2.down);
    }

    void FloatAround()
    {
        // Move the power-up in the random direction
        transform.Translate(floatSpeed * Time.deltaTime * randomDirection);

        // If it floats too far from the spawn point, move it back in bounds
        // Keep the power-up within a floating radius around its spawn position
        if (Vector3.Distance(transform.position, spawnPosition) > maxFloatRadius)
        {
            Vector3 directionToSpawn = (spawnPosition - transform.position).normalized;
            randomDirection = directionToSpawn;  // Change direction back toward the spawn area
            directionChangeTimer = 0f;
        }
    }

    void ChangeDirectionOnTimer()
    {
        // Change the direction at set intervals to simulate "bubbling" or aimless movement
        directionChangeTimer += Time.deltaTime;
        if (directionChangeTimer > directionChangeInterval)
        {
            ChangeRandomDirection();
            directionChangeTimer = 0f;  // Reset the timer after changing direction
        }
    }

    void ChangeRandomDirection()
    {
        randomDirection = Random.insideUnitCircle.normalized;
    }

    IEnumerator BlinkEffect()
    {
        isBlinking = true;
        float blinkDuration;
        while (Time.time < spawnTime + lifetime)
        {
            spriteRenderer.enabled = !spriteRenderer.enabled;  // Toggle visibility
            blinkDuration = spriteRenderer.enabled ? blinkOn : blinkOff;
            yield return new WaitForSeconds(blinkDuration);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // assume the other object has a Player script with a PowerUp function
            if (other.TryGetComponent(out Player player))
            {
                player.PowerUp(powerUpType);
            }
            Destroy(gameObject);
        }
    }
}
