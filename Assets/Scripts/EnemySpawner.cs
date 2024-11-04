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

    [SerializeField] List<WaveSchedule> schedules;
    // [SerializeField] float timeBetweenWaves = 5f;
    [SerializeField] bool isLooping = false;

    WaveConfigSO currentWave;
    float currentCooldown;
    private int activeEnemies;

    void Start()
    {
        activeEnemies = 0;
        StartCoroutine(SpawnEnemyWaves());
    }

    public WaveConfigSO GetCurrentWave() => currentWave;

    IEnumerator SpawnEnemyWaves()
    {
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
                            health.OnEnemyDeath += HandleEnemyDefeated;
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
        } while (isLooping);
    }

    void HandleEnemyDefeated() => activeEnemies--;
}
