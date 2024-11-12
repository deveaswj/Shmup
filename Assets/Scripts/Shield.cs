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
    [SerializeField] LayerMask bossLayer;              // Layer for detecting boss enemies
    [Header("Effects")]
    [SerializeField] Color shieldHitColor = Color.red;    // Color of the shield
    [SerializeField] float shieldHitFXDuration = 0.1f;

    LayerMask combinedLayerMask;
    Color originalColor;
    SpriteRenderer sr;
    CircleCollider2D cc;
    Rigidbody2D playerRB;
    bool shieldOn = false;
    bool hitShield = false;
    Transform shipTransform;
    int startingHitPoints;
    Vector2 positionOffset;
    Vector2 localScale;

    float shieldHitFXTimer = 0f;

    AudioManager audioManager;

    public void SetPlayerShip(GameObject obj) { playerShip = obj; }
    public void SetPositionOffset(Vector2 offset) { positionOffset = offset; }
    public void SetLocalScale(Vector2 scale) { localScale = scale; }

    void Awake()
    {
        startingHitPoints = shieldHitPoints;
        sr = GetComponent<SpriteRenderer>();
        cc = GetComponent<CircleCollider2D>();
        audioManager = FindObjectOfType<AudioManager>();
    }

    private void Start()
    {
        // TurnOn();
        if (playerShip == null)
        {
            playerShip = GameObject.FindWithTag("Player");
        }
        if (playerShip.TryGetComponent<Health>(out var playerHealth))
        {
            playerHealth.OnDefeat += HandlePlayerDefeat;
        }
        playerRB = playerShip.GetComponent<Rigidbody2D>();
        shipTransform = playerShip.transform;
        originalColor = sr.color;
        // don't combine bossLayer - we handle bosses separately
        combinedLayerMask = enemyLayer | projectileLayer;
    }

    private void HandlePlayerDefeat(Health playerHealth)
    {
        TurnOff();
        playerHealth.OnDefeat -= HandlePlayerDefeat;
    }

    private void OnDisable()
    {
        // when player dies, don't let this linger on the screen
        Debug.Log("Shield OnDisable");
        sr.enabled = false;
        TurnOff();
    }

    private void Update()
    {
        sr.enabled = shieldOn;
        if (shieldOn)
        {
            float newX, newY;
            newX = shipTransform.position.x + positionOffset.x;
            newY = shipTransform.position.y + positionOffset.y;
            transform.position = new Vector2(newX, newY);
            transform.localScale = new Vector2(localScale.x, localScale.y);
        }

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
        if (shieldOn)
        {
            if (IsInLayerMask(other.gameObject, bossLayer))    // Boss
            {
                Debug.Log("Shield hit by Boss: " + other.gameObject.name);

                // Calculate initial knockback direction based on relative position
                Vector2 knockbackDirection = (transform.position - other.transform.position).normalized;

                // Apply edge trap adjustment only if vertical knockback component is minimal
                knockbackDirection = AdjustForEdgeTrap(knockbackDirection);

                // Perform the knockback by moving the player position
                if (playerRB != null)
                {
                    float knockbackDistance = 1.0f;  // Customize this based on desired knockback effect
                    Vector2 newPosition = (Vector2)transform.position + knockbackDirection * knockbackDistance;
                    playerRB.MovePosition(newPosition);  // Smoothly moves player to new position
                }
                else
                {
                    Debug.LogWarning("Shield: Player has no Rigidbody2D component.");
                }
                TakeShieldHit();
            }
            else if (IsInLayerMask(other.gameObject, combinedLayerMask))
            {
                if (other.TryGetComponent<Health>(out var otherHealth))     // Enemy
                {
                    Debug.Log("Shield hit by Enemy: " + otherHealth.gameObject.name);
                    //
                    // 2024/11/11 - let Health call DamageDealer to damage the enemy instead
                    //
                    // otherHealth.TakeDamage(int.MaxValue, true);    // damage the enemy to the max!
                }
                else if (other.TryGetComponent<DamageDealer>(out var damageDealer))     // Projectile
                {
                    Debug.Log("Shield hit by Projectile: " + damageDealer.gameObject.name);
                    damageDealer.Hit();
                }
                TakeShieldHit();
            }
        }
    }

    private Vector2 AdjustForEdgeTrap(Vector2 knockbackDirection)
    {
        float minVerticalThreshold = 0.2f;  // Set threshold for minimal vertical movement

        // Check if vertical component of knockbackDirection is negligible
        if (Mathf.Abs(knockbackDirection.y) < minVerticalThreshold)
        {
            // Apply a small vertical adjustment to prevent edge trapping
            float verticalAdjustment = Random.Range(0.5f, 1.0f) * (Random.value > 0.5f ? 1 : -1);
            knockbackDirection.y += verticalAdjustment;
        }

        return knockbackDirection.normalized;  // Re-normalize to maintain consistent force
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
            audioManager.PlayShieldOffClip();
            TurnOff();  // Trigger shield destruction or deactivation
        }
    }

    public void TurnOff()
    {
        // Deactivate the shield
        shieldOn = false;
        Debug.Log("Shield turned off");
    }

    public void TurnOn()
    {
        // Activate the shield
        shieldOn = true;
        shieldHitPoints = startingHitPoints;
        Debug.Log("Shield turned on");
    }
}
