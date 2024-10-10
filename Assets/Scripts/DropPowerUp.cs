using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Add this to Enemy prefabs

public class DropPowerUp : MonoBehaviour
{
    [System.Serializable]
    public class PowerUpEntry
    {
        public GameObject powerUpPrefab; // The power-up prefab
        public int dropWeight;           // The odds of dropping this prefab (weight)
    }

    [SerializeField] List<PowerUpEntry> powerUpEntries = new(); // List of power-up prefabs with weights
    [SerializeField] float dropChance = 0.2f; // Chance to drop a power-up at all (20% in this case)


    void OnDestroy()
    {
        // Random chance to drop a power-up
        if (Random.value < dropChance)
        {
            if (powerUpEntries.Count == 0) return; // Exit if no power-ups are defined

            // Calculate the total weight sum
            int totalWeight = 0;
            foreach (var entry in powerUpEntries)
            {
                totalWeight += entry.dropWeight;
            }

            // Roll a random number between 1 and the total weight (inclusive)
            int randomRoll = Random.Range(1, totalWeight + 1); // Random number from 1 to totalWeight (inclusive)

            // Determine which power-up to drop based on the random roll
            int cumulativeWeight = 0;
            foreach (var entry in powerUpEntries)
            {
                cumulativeWeight += entry.dropWeight;

                if (randomRoll <= cumulativeWeight)
                {
                    // Drop the power-up at the enemy's position
                    Instantiate(entry.powerUpPrefab, transform.position, Quaternion.identity);
                    return; // Exit once the power-up is dropped
                }
            }
        }
    }
}
