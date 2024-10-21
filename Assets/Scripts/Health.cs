using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Track health and damage. Flash when hit, explode when health runs out.
// Used by Player ship and Enemy ships.
// Enemy ships and all projectiles are DamageDealers.
// The OnTriggerEnter2D() handles Player-(any Enemy) collisions and Projectile-(any Ship) collisions.


public class Health : MonoBehaviour
{
    [SerializeField] int health;
    [SerializeField] int maxHealth = 50;
    [SerializeField] int minorHealPercent = 20;
    [SerializeField] int majorHealPercent = 50;
    [SerializeField] ParticleSystem explodeEffect;
    [SerializeField] SimpleFlash flashEffect;

    [SerializeField] bool applyCameraShake;
    [SerializeField] ShakeSettings smallShake;
    [SerializeField] ShakeSettings largeShake;
    CameraShake cameraShake;
    ShakeSettings nextShake;

    AudioPlayer audioPlayer;

    public int GetHealth() => health;
    public int GetDamage() => maxHealth - health;

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
        // add maxhealth if already at full
        if (health == maxHealth)
        {
            AddMaxHealth(maxHealth * (minorHealPercent / 100));
        }
        else
        {
            MinorHeal();
        }
    }

    public void MajorHealPlus()
    {
        // add maxhealth if already at full
        if (health == maxHealth)
        {
            AddMaxHealth(maxHealth * (majorHealPercent / 100));
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
        audioPlayer = FindObjectOfType<AudioPlayer>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // if what collided with us is a DamageDealer, take damage and destroy it
        if (other.TryGetComponent<DamageDealer>(out var damageDealer))
        {
            TakeDamage(damageDealer.GetDamage());
            ShakeCameraAsNeeded();
            damageDealer.Hit();
        }
    }

    public void AddHealthPercentage(int percent)
    {
        AddHealth(maxHealth * (percent / 100));
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

    public void TakeDamage(int damage)
    {
        health -= damage;
        if (health > 0)
        {
            nextShake = smallShake;
            audioPlayer.PlayDamageClip();
            PlayFlashEffect();
        }
        else
        {
            nextShake = largeShake;
            audioPlayer.PlayExplosionClip();
            PlayExplosion();
            Die();
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
        // return it if it's in an object pool (enemy), else destroy it (player)
        // enemy ships aren't in a pool yet, but they will be
        // for now, just destroy whatever this is attached to
        Destroy(gameObject);
    }

}
