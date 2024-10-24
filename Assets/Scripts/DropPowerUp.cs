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
    [SerializeField] [Range(0f,1f)] float dropChance = 0.2f; // Chance to drop a power-up at all (20% in this case)

    private static bool isQuitting = false;
    private bool canDrop = true;

    void Awake()
    {
        // Register for shutdown events
        Application.quitting += OnApplicationQuit;
    }

    public void SetEnabled(bool value)
    {
        // Debug.Log("DropPowerUp set to " + (value ? "enabled" : "disabled"));
        canDrop = value;
    }

    void OnApplicationQuit()
    {
        isQuitting = true;
        SetEnabled(false);
    }

    void OnDestroy()
    {
        // Skip the logic if the game is quitting
        if (isQuitting || !Application.isPlaying) return;
        if (!canDrop)
        {
            // Debug.Log("DropPowerUp on destroy canceled");
            return;
        }

        // Random chance to drop a power-up
        if (powerUpEntries.Count > 0 && Random.value < dropChance)
        {
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
