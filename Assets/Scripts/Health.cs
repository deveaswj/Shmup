using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Track health and damage. Flash when hit, explode when health runs out.
// The OnTriggerEnter2D() handles Player-(any Enemy) collisions and Projectile-(any Ship) collisions.


public class Health : MonoBehaviour
{
    [SerializeField] int health = 50;
    [SerializeField] int maxHealth = 50;
    [SerializeField] ParticleSystem explodeEffect;
    [SerializeField] SimpleFlash flashEffect;

    public int GetHealth() => health;

    void OnTriggerEnter2D(Collider2D other)
    {
        // if what collided with us is a DamageDealer, take damage and destroy it
        if (other.TryGetComponent<DamageDealer>(out var damageDealer))
        {
            TakeDamage(damageDealer.GetDamage());
            damageDealer.Hit();
        }
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
            flashEffect.Flash();
        }
        else
        {
            PlayExplosion();
            Destroy(gameObject);
        }
    }

    void PlayExplosion()
    {
        if (explodeEffect != null)
        {
            ParticleSystem explosion = Instantiate(explodeEffect, transform.position, Quaternion.identity);
            Destroy(explosion.gameObject, explosion.main.duration + explosion.main.startLifetime.constantMax);
        }
    }
}
