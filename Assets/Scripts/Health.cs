using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] int health = 50;
    [SerializeField] ParticleSystem explodeEffect;
    [SerializeField] SimpleFlash flashEffect;

    void OnTriggerEnter2D(Collider2D other)
    {
        
        if (other.TryGetComponent<DamageDealer>(out var damageDealer))
        {
            TakeDamage(damageDealer.GetDamage());
            flashEffect.Flash();
            damageDealer.Hit();
        }
    }

    void TakeDamage(int damage)
    {
        health -= damage;
        if (health <= 0)
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
