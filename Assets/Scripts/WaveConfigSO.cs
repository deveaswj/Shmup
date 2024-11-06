using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Wave Config", menuName = "ScriptableObjects/Wave Config")]
public class WaveConfigSO : ScriptableObject
{
    // Note: we'll use WaveConfigEditor for Inspector
    // which will override Headers and Ranges in this script
    //[Header("Enemies")]
    [SerializeField] bool _isBossWave = false;
    [SerializeField] List<GameObject> enemyPrefabs = new();
    //[Header("Path")]
    [SerializeField] Transform pathPrefab;
    [SerializeField] bool flipX;
    [SerializeField] bool flipY;
    [SerializeField] [Range(0, 180)] float rotationAngle;
    [SerializeField] Vector2 offset;
    //[Header("Movement")]
    [SerializeField] float moveSpeed = 5f;
    [SerializeField] bool reverseOrder;
    //[Header("Spawning")]
    [SerializeField] float timeBetweenSpawns = 0.5f;
    [SerializeField] float spawnTimeVariance = 0.3f;
    [SerializeField] float minimumSpawnTime = 0.2f;
    //[Header("Looping")]
    [SerializeField] [Range(0, 3)] int _loops = 0;

    List<Vector3> waypoints = new();
    private float _totalDistance;

    public int Loops { get { return _loops; } }
    public int GetEnemyCount() => enemyPrefabs.Count;
    public GameObject GetEnemyPrefab(int index) => enemyPrefabs[index];
    public Vector3 GetStartingWaypoint() => waypoints.Count > 0 ? waypoints[0] : Vector3.zero;
    public List<Vector3> GetWaypoints() => waypoints;
    public float GetMoveSpeed() => moveSpeed;
    public float GetDistance() => _totalDistance;
    public bool IsBossWave() => _isBossWave;

    void OnEnable()
    {
        CreateWaypoints();
        _totalDistance = CalcTotalPathDistance();
    }

    void OnDisable()
    {
        ClearWaypoints();
        _totalDistance = 0f;
    }

    private void ClearWaypoints()
    {
        waypoints.Clear();
    }

    public void CreateWaypoints()
    {
        Quaternion quatRotation = Quaternion.identity;
        if (rotationAngle != 0) quatRotation = Quaternion.Euler(0, 0, rotationAngle);

        ClearWaypoints();
        foreach (Transform child in pathPrefab.transform)
        {
            Vector3 newPosition = ApplyTransform(child.position, quatRotation);

            newPosition += (Vector3)offset;

            waypoints.Add(newPosition);
        }
        if (reverseOrder) waypoints.Reverse();
    }

    private Vector3 ApplyTransform(Vector3 originalPosition, Quaternion quatRotation)
    {
        Vector3 newPosition = originalPosition;
        // flip and rotate
        if (flipX) newPosition.x = -newPosition.x;
        if (flipY) newPosition.y = -newPosition.y;
        if (rotationAngle != 0) newPosition = quatRotation * newPosition;
        return newPosition;
    }

    public float GetRandomSpawnTime()
    {
        float spawnTime = timeBetweenSpawns + Random.Range(-spawnTimeVariance, spawnTimeVariance);
        return Mathf.Max(spawnTime, minimumSpawnTime);
    }

    private float CalcTotalPathDistance()
    {
        float totalDistance = 0f;
        for (int i = 0; i < waypoints.Count - 1; i++)
        {
            totalDistance += Vector3.Distance(waypoints[i], waypoints[i + 1]);
        }
        totalDistance *= (_loops + 1);
        return totalDistance;
    }

    public (float minTime, float maxTime, float avgTime) CalculateWaveTimeRange()
    {
        // Number of enemies
        int enemyCount = enemyPrefabs.Count;

        // Step 1: Calculate total spawn time
        float minSpawnTimeTotal = (enemyCount - 1) * minimumSpawnTime;
        float maxSpawnTimeTotal = (enemyCount - 1) * (timeBetweenSpawns + spawnTimeVariance);
        float avgSpawnTimeTotal = (minSpawnTimeTotal + maxSpawnTimeTotal) / 2;

        // Step 2: Calculate traversal time (distance from first to last waypoint)
        float totalPathDistance = CalcTotalPathDistance();
        float traversalTime = totalPathDistance / moveSpeed;

        // Step 3: Calculate total time by combining spawn and traversal times
        float minTotalTime = minSpawnTimeTotal + traversalTime;
        float maxTotalTime = maxSpawnTimeTotal + traversalTime;
        float avgTotalTime = avgSpawnTimeTotal + traversalTime;

        return (minTotalTime, maxTotalTime, avgTotalTime);
    }
}
