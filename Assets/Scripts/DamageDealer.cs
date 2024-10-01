using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This class is for objects that cause damage (to things that can take damage) upon collision.
// It can be attached to any object that has a collider.
// Another object can call GetDamage() from a script to determine how much to damage itself,
// and call Hit() to destroy the object that this is attached to.
// For example, in the other object's OnTriggerEnter2D():
//
//       if (other.TryGetComponent<DamageDealer>(out var damageDealer))
//        {
//            TakeDamage(damageDealer.GetDamage());
//            flashEffect.Flash();
//            damageDealer.Hit();
//        }

public class DamageDealer : MonoBehaviour
{
    [SerializeField] int damage = 25;
    public int GetDamage() => damage;

    public void Hit()
    {
        Destroy(gameObject);
    }
}
