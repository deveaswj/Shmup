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


    public int Loops { get { return _loops; } }
    public int GetEnemyCount() => enemyPrefabs.Count;
    public GameObject GetEnemyPrefab(int index) => enemyPrefabs[index];
    public Transform GetStartingWaypoint() => pathPrefab.GetChild(0);
    public float GetMoveSpeed() => moveSpeed;

    public List<Transform> GetWaypoints()
    {
        if (flipX || flipY) 
        {
            pathPrefab.transform.localScale = new Vector3(
                flipX ? -1 : 1,
                flipY ? -1 : 1,
                1
            );
        }
        if (rotationAngle != 0) pathPrefab.transform.rotation = Quaternion.Euler(0, 0, rotationAngle);

        List<Transform> waypoints = new();
        foreach (Transform child in pathPrefab)
        {
            //Vector3 position = child.position;
            // child.position = ApplyTransform(position);

            waypoints.Add(child);
        }
        if (reverseOrder) waypoints.Reverse();
        return waypoints;
    }

    private Vector3 ApplyTransform(Vector3 originalPosition)
    {
        // note: better to precalculate rotation outside of the loop
        Quaternion quatRotation = Quaternion.identity;
        if (rotationAngle != 0) quatRotation = Quaternion.Euler(0, 0, rotationAngle);
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
        // float spawnTime = Random.Range(
        //    timeBetweenSpawns - spawnTimeVariance,
        //    timeBetweenSpawns + spawnTimeVariance
        // );
        return Mathf.Max(spawnTime, minimumSpawnTime);
    }

}
