using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [System.Serializable]
    public class WaveSchedule
    {
        public WaveConfigSO wave;
        public float cooldown = 5f;
    }

    [SerializeField] float initialDelay = 3f;
    [SerializeField] List<WaveSchedule> schedules;
    [SerializeField] bool isLooping = false;
    [SerializeField] int initialLevel = 1;
    int level;
    // the intent of "level" here is to level-up the ships after each full loop


    WaveConfigSO currentWave;
    float currentCooldown;
    private int activeEnemies;

    private List<Health> trackedEnemies = new();

    void Start()
    {
        activeEnemies = 0;
        level = initialLevel;
        StartCoroutine(SpawnEnemyWaves());
    }

    public WaveConfigSO GetCurrentWave() => currentWave;

    IEnumerator SpawnEnemyWaves()
    {
        if (initialDelay > 0)
        {
            yield return new WaitForSeconds(initialDelay);
        }
        int waveNumber = 0;
        do {
            foreach (var schedule in schedules)
            {
                currentWave = schedule.wave;
                currentCooldown = schedule.cooldown;

                bool isBossWave = currentWave.IsBossWave();
                for (int i = 0; i < currentWave.GetEnemyCount(); i++)
                {
                    var newEnemy = Instantiate(currentWave.GetEnemyPrefab(i), 
                                currentWave.GetStartingWaypoint(), 
                                Quaternion.identity,
                                transform);
                    newEnemy.name = "(W" + waveNumber + " E" + i + ") " + newEnemy.name;

                    // Debug.Log("Created Enemy: " + newEnemy.name);

                    if (isBossWave)
                    {
                        if (newEnemy.TryGetComponent<Health>(out var health))
                        {
                            activeEnemies++;
                            health.OnDefeat += HandleEnemyDefeated;
                            trackedEnemies.Add(health);
                        }
                    }

                    yield return new WaitForSeconds(currentWave.GetRandomSpawnTime());
                }

                // If it's a boss wave, wait until all enemies from the wave are defeated
                if (isBossWave)
                {
                    yield return new WaitUntil(() => activeEnemies == 0);
                }

                yield return new WaitForSeconds(currentCooldown);
                waveNumber++;
            }
            UnsubscribeFromAllEnemies();
            level++;
        } while (isLooping);
    }

    void HandleEnemyDefeated(Health defeatedEnemy)
    {
        activeEnemies--;
        defeatedEnemy.OnDefeat -= HandleEnemyDefeated;
        trackedEnemies.Remove(defeatedEnemy);
    }

    void UnsubscribeFromAllEnemies()
    {
        foreach (var health in trackedEnemies)
        {
            if (health != null)
            {
                health.OnDefeat -= HandleEnemyDefeated;
            }
        }
        trackedEnemies.Clear();
    }
}
