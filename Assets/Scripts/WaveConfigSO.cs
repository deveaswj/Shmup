using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Wave Config", menuName = "ScriptableObjects/Wave Config")]
public class WaveConfigSO : ScriptableObject
{
    [Header("Enemies")]
    [SerializeField] List<GameObject> enemyPrefabs;
    [Header("Path")]
    [SerializeField] Transform pathPrefab;
    [SerializeField] bool flipX;
    [SerializeField] bool flipY;
    [SerializeField] [Range(0, 180)] float rotationAngle;
    [Header("Movement")]
    [SerializeField] float moveSpeed = 5f;
    [SerializeField] bool reverseOrder;
    [Header("Spawning")]
    [SerializeField] float timeBetweenSpawns = 0.5f;
    [SerializeField] float spawnTimeVariance = 0.3f;
    [SerializeField] float minimumSpawnTime = 0.2f;
    [Header("Looping")]
    [SerializeField] [Range(0, 3)] int _loops = 0;

    List<Vector3> waypoints = new();

    public int Loops { get { return _loops; } }
    public int GetEnemyCount() => enemyPrefabs.Count;
    public GameObject GetEnemyPrefab(int index) => enemyPrefabs[index];
    public Vector3 GetStartingWaypoint() => waypoints.Count > 0 ? waypoints[0] : Vector3.zero;
    public List<Vector3> GetWaypoints() => waypoints;
    public float GetMoveSpeed() => moveSpeed;

    void OnEnable()
    {
        CreateWaypoints();
    }

    void OnDisable()
    {
        ClearWaypoints();
    }

    private void ClearWaypoints()
    {
        waypoints.Clear();
    }

    private void CreateWaypoints()
    {
        // note: better to precalculate rotation outside of the loop
        Quaternion quatRotation = Quaternion.identity;
        if (rotationAngle != 0) quatRotation = Quaternion.Euler(0, 0, rotationAngle);

        ClearWaypoints();
        foreach (Transform child in pathPrefab.transform)
        {
            Vector3 newPosition = ApplyTransform(child.position, quatRotation);

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

}
