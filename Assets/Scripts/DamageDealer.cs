using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This class is for objects that cause damage (to things that can take damage) upon collision.
// Works with the Health() script.
// It can be attached to any object that has a collider (or a script that uses an Overlay function?).
// : Projectiles and Enemy Ships are DamageDealers with colliders.
// : Shield is not a DamageDealer; it calls Health() functions directly.
// Another object can call GetDamage() from a script to determine how much to damage itself,
// and call Hit() to destroy the object that this is attached to.
// For example, say the other object has a Health script with an OnTriggerEnter2D():
//
//       if (other.TryGetComponent<DamageDealer>(out var damageDealer))
//        {
//            TakeDamage(damageDealer.GetDamage());     // another function in Health script
//            flashEffect.Flash();
//            damageDealer.Hit();
//        }

public class DamageDealer : MonoBehaviour
{
    [SerializeField] int damage = 25;
    [SerializeField] bool isProjectile = false;
    public int GetDamage() => damage;

    public void Hit()
    {
        if (isProjectile)
        {
            // return it to the pool
            if (TryGetComponent<Projectile>(out var projectile))
            {
                projectile.Deactivate();
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
