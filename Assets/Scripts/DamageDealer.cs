using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This class is for objects that cause damage (to things that can take damage) upon collision.
// Works with the Health() script.
// It can be attached to any object that has a collider (or a script that uses an Overlay function?).
// : Projectiles and Enemy Ships are DamageDealers with colliders.
// : Shield is a DamageDealer (as of 2024/11/11)
// : Player is not a DamageDealer. It just dies when it collides with anything.
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
    [SerializeField] bool isShield = false;
    [SerializeField] bool isBoss = false;

    public int GetDamage() => damage;
    public void SetDamage(int amount) => damage = amount;

    void Start()
    {
        if (isBoss) SetDamage(int.MaxValue);
    }

    public void Hit()
    {
        if (isShield) return;
        bool selfDestroy = !isProjectile;
        if (isProjectile)
        {
            if (TryGetComponent<Projectile>(out var projectile))
            {
                projectile.Deactivate();
            }
            else
            {
                selfDestroy = true;
            }
        }
        if (selfDestroy)
        {
            Destroy(gameObject);
        }
    }
}
