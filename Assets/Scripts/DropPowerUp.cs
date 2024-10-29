using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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

    string debugPrefix = "";

    void Awake()
    {
        // Register for shutdown events
        Application.quitting += OnApplicationQuit;

        debugPrefix = "DropPowerUp (" + gameObject.name + ") - ";
    }

    public void DisableDrop() => canDrop = false;
    public void EnableDrop() => canDrop = true;

    void OnApplicationQuit()
    {
        isQuitting = true;
        DisableDrop();
    }

    void OnDestroy()
    {
        Debug.Log(debugPrefix + "OnDestroy");

        // Skip the logic if the game is quitting
        if (isQuitting || !Application.isPlaying) DisableDrop();

        if (!canDrop)
        {
            Debug.Log(debugPrefix + "OnDestroy - don't drop");
            return;
        }

        Debug.Log(debugPrefix + "OnDestroy - roll to drop");

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

    // Make sure we can't drop when a scene is unloaded

    void OnEnable()
    {
        // SceneManager.sceneUnloaded += OnSceneUnloaded;
        EnableDrop();
    }

    void OnDisable()
    {
        // don't unsubscribe
        DisableDrop();
    }

    void OnSceneUnloaded(Scene current)
    {
        Debug.Log(debugPrefix + "OnSceneUnloaded");
        DisableDrop();
    }
}
