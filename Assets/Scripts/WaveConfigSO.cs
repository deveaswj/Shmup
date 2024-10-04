using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Wave Config", menuName = "ScriptableObjects/Wave Config")]
public class WaveConfigSO : ScriptableObject
{
    [SerializeField] List<GameObject> enemyPrefabs;
    [SerializeField] Transform pathPrefab;
    [SerializeField] float moveSpeed = 5f;
    [SerializeField] float timeBetweenSpawns = 0.5f;
    [SerializeField] float spawnTimeVariance = 0.3f;
    [SerializeField] float minimumSpawnTime = 0.2f;

    // [SerializeField] float timeBetweenSpawns = 0.5f;
    // [SerializeField] float spawnRandomFactor = 0.3f;

    public int GetEnemyCount() => enemyPrefabs.Count;
    public GameObject GetEnemyPrefab(int index) => enemyPrefabs[index];
    public Transform GetStartingWaypoint() => pathPrefab.GetChild(0);
    public float GetMoveSpeed() => moveSpeed;

    public List<Transform> GetWaypoints()
    {
        List<Transform> waypoints = new();
        foreach (Transform child in pathPrefab)
        {
            waypoints.Add(child);
        }
        return waypoints;
    }

    public float GetRandomSpawnTime()
    {
        float spawnTime = timeBetweenSpawns + Random.Range(-spawnTimeVariance, spawnTimeVariance);
        // float spawnTime = Random.Range(
        //    timeBetweenSpawns - spawnTimeVariance,
        //    timeBetweenSpawns + spawnTimeVariance
        // );
        return Mathf.Max(spawnTime, minimumSpawnTime);
    }

}
