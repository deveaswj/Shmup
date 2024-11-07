using System.Collections.Generic;
using UnityEngine;

public class PlayerMagnet : MonoBehaviour
{
    public float magnetRadius = 5f;          // Radius within which power-ups are attracted
    public float attractionSpeed = 3f;       // Speed at which power-ups are pulled toward the player
    public float magnetEffectDuration = 2f;  // Duration of the magnet effect on each power-up
    public LayerMask powerUpLayer;           // Layer mask to specify the "PowerUps" layer

    private List<Collider2D> powerUpColliders = new List<Collider2D>();  // Reusable list for results
    private ContactFilter2D contactFilter;    // Filter for the "PowerUps" layer

    private void Awake()
    {
        // Set up the ContactFilter2D to use the specified layer mask
        contactFilter = new ContactFilter2D();
        contactFilter.SetLayerMask(powerUpLayer);
        contactFilter.useTriggers = true;  // Enable triggers if power-ups are triggers
    }

    private void Update()
    {
        AttractNearbyPowerUps();
    }

    private void AttractNearbyPowerUps()
    {
        // Clear the list before each use to avoid accumulating old results
        powerUpColliders.Clear();

        // Populate the list with results using OverlapCircle
        Physics2D.OverlapCircle(transform.position, magnetRadius, contactFilter, powerUpColliders);

        foreach (Collider2D collider in powerUpColliders)
        {
            PowerUp powerUp = collider.GetComponent<PowerUp>();
            if (powerUp != null)
            {
                // Trigger the magnet effect on each power-up within range
                powerUp.ActivateMagnet(transform, attractionSpeed, magnetEffectDuration);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        // Visualize the magnet radius in the editor
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, magnetRadius);
    }
}
