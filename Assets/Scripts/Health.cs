using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Track health and damage. Flash when hit, explode when health runs out.
// Used by Player ship and Enemy ships.
// Enemy ships and all projectiles are DamageDealers.
// The OnTriggerEnter2D() handles Player-(any Enemy) collisions and Projectile-(any Ship) collisions.


public class Health : MonoBehaviour
{
    // events for this specific Health script on this particular object
    // get the script's reference and subscribe to e.g. health.OnDefeat
    // (as used by EnemySpawner which only needs to be informed by *specific* enemies)
    public event Action<Health> OnDefeat;

    [SerializeField] bool isPlayer;
    [SerializeField] int score = 50;    // default score for enemies; ignored if isPlayer
    [SerializeField] int health;
    [SerializeField] int maxHealth = 50;

    [Header("Healing")]
    [SerializeField] [Range(0, 100)] int minorHealPercent = 20;
    [SerializeField] [Range(0, 100)] int majorHealPercent = 50;
    private float nerfFactor = 0.5f;

    [Header("Effects")]
    [SerializeField] ParticleSystem explodeEffect;
    [SerializeField] SimpleFlash flashEffect;
    [SerializeField] bool applyCameraShake;
    [SerializeField] ShakeSettings smallShake;
    [SerializeField] ShakeSettings largeShake;
    CameraShake cameraShake;
    ShakeSettings nextShake;
    ScoreKeeper scoreKeeper;
    AudioManager audioManager;
    LevelManager levelManager;

    AOEDealer aoeDealer = null;

    private static bool isQuitting = false;

    public int GetHealth() => health;
    public int GetMaxHealth() => maxHealth;
    public int GetDamage() => maxHealth - health;
    public int GetHealthPercentage() => Mathf.RoundToInt(100 * ((float)health / maxHealth));

    private bool invincible = false;

    public void SetInvincible(bool flag = true) => invincible = flag;

    public void MinorHeal()
    {
        AddHealthPercentage(minorHealPercent);
    }

    public void MajorHeal()
    {
        AddHealthPercentage(majorHealPercent);
    }

    public void MinorHealPlus()
    {
        // add a fraction to maxhealth if already at full
        if (health == maxHealth)
        {
            float nerfedValue = minorHealPercent * nerfFactor;
            AddMaxHealth(Mathf.RoundToInt(maxHealth * (nerfedValue / 100f)));
        }
        else
        {
            MinorHeal();
        }
    }

    public void MajorHealPlus()
    {
        // add a fraction to maxhealth if already at full
        if (health == maxHealth)
        {
            float nerfedValue = majorHealPercent * nerfFactor;
            AddMaxHealth(Mathf.RoundToInt(maxHealth * (nerfedValue / 100f)));
        }
        else
        {
            MajorHeal();
        }
    }

    public void FullHeal()
    {
        health = maxHealth;
    }

    void Awake()
    {
        cameraShake = Camera.main.GetComponent<CameraShake>();
        health = maxHealth;
        audioManager = FindObjectOfType<AudioManager>();
        scoreKeeper = FindObjectOfType<ScoreKeeper>();
        levelManager = FindObjectOfType<LevelManager>();

        // Register for shutdown events
        Application.quitting += OnApplicationQuit;
    }

    void OnApplicationQuit()
    {
        isQuitting = true;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // if the other object has an AOEDealer component, store its reference
        aoeDealer = other.GetComponent<AOEDealer>();

        // if the other object is a DamageDealer, we take damage, and it takes a hit
        if (other.TryGetComponent<DamageDealer>(out var damageDealer))
        {
            TakeDamage(damageDealer.GetDamage());
            ShakeCameraAsNeeded();
            damageDealer.Hit();
            DieAsNeeded();
        }
    }

    private void DieAsNeeded()
    {
        if (health <= 0) Die();
    }

    public void AddHealthPercentage(int percent)
    {
        AddHealth(Mathf.RoundToInt(maxHealth * (percent / 100f)));
    }

    public void AddHealth(int amount)
    {
        health = Mathf.Clamp(health + amount, 0, maxHealth);
    }

    public void AddMaxHealth(int amount)
    {
        maxHealth = Mathf.Clamp(maxHealth + amount, 0, int.MaxValue);
        AddHealth(amount);
    }

    public void TakeDamage(int damage, bool dieWhenZero = false)
    {
        if (invincible) return;

        health -= damage;
        if (health > 0)
        {
            nextShake = smallShake;
            audioManager.PlayDamageClip();
            PlayFlashEffect();
        }
        else
        {
            nextShake = largeShake;
            audioManager.PlayExplosionClip();
            PlayExplosion();
            if (dieWhenZero)
            {
                Die();
            }
        }
    }

    void PlayFlashEffect()
    {
        if (flashEffect != null)
        {
            flashEffect.Flash();
        }
    }

    void PlayExplosion()
    {
        if (explodeEffect != null)
        {
            ParticleSystem explosion = Instantiate(explodeEffect, transform.position, Quaternion.identity);
            float explosionDuration = explosion.main.duration + explosion.main.startLifetime.constantMax;
            Destroy(explosion.gameObject, explosionDuration);
        }
    }

    void ShakeCameraAsNeeded()
    {
        if (cameraShake != null && applyCameraShake)
        {
            cameraShake.Play(nextShake);
        }
    }

    void Die()
    {
        Debug.Log("Died: " + gameObject.name);

        if (!isPlayer)
        {
            if (!isQuitting)
            {
                OnDefeat?.Invoke(this);     // used in EnemySpawner
                if (aoeDealer != null)
                {
                    AOEDealer newAOEDealer = aoeDealer.CreateAOE(transform.position, gameObject.name);
                    if (newAOEDealer == null) return;
                    // explicitly call DestroyLater() because of Unity timing issues
                    newAOEDealer.DestroyLater();
                }
            }
            scoreKeeper.AddScore(score);
            gameObject.SetActive(false);
            Destroy(gameObject);
        }
        else
        {
            OnDefeat?.Invoke(this);
            gameObject.SetActive(false);
            levelManager.LoadGameOver();
       }
    }
}
