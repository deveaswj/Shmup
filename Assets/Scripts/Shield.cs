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
    CircleCollider2D cc;
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
        shipTransform = playerShip.transform;
        originalColor = sr.color;
        combinedLayerMask = enemyLayer | projectileLayer;
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
        if (shieldOn && IsInLayerMask(other.gameObject, combinedLayerMask))
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
