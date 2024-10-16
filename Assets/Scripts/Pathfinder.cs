using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Add this to Enemy prefabs

public class Pathfinder : MonoBehaviour
{
    [SerializeField] EnemyBehaviorType behaviorWhenShot;
    EnemySpawner enemySpawner;
    WaveConfigSO waveConfig;
    List<Transform> waypoints;
    int waypointIndex = 0;
    Health health;
    bool hasHealth;
    bool isOffScreen;

    void Awake()
    {
        enemySpawner = FindObjectOfType<EnemySpawner>();
    }

    void Start()
    {
        waveConfig = enemySpawner.GetCurrentWave();
        waypoints = waveConfig.GetWaypoints();
        transform.position = waypoints[waypointIndex].position;
        hasHealth = TryGetComponent(out health);
    }

    void Update()
    {
        int damage = hasHealth ? health.GetDamage() : 0;
        if (damage > 0)
        {
            ActWhenDamaged();
        }
        else
        {
            FollowPath();
        }
    }

    void ActWhenDamaged()
    {
        switch (behaviorWhenShot)
        {
            case EnemyBehaviorType.None:
                FollowPath();
                break;
            case EnemyBehaviorType.Flee:
                Flee();
                break;
            case EnemyBehaviorType.Dive:
                Dive();
                break;
            default:
                break;
        }
    }

    void FollowPath()
    {
        if (waypointIndex < waypoints.Count)
        {
            Vector3 targetPosition = waypoints[waypointIndex].position;
            float deltaMove = waveConfig.GetMoveSpeed() * Time.deltaTime;
            transform.position = Vector2.MoveTowards(transform.position, targetPosition, deltaMove);
            if (transform.position == targetPosition)
            {
                waypointIndex++;
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Flee()
    {
        // move upwards, destroy when off screen   
        float deltaMove = waveConfig.GetMoveSpeed() * Time.deltaTime;
        transform.position += Vector3.up * deltaMove;

        if (isOffScreen) Destroy(gameObject);
    }

    void Dive()
    {
        // move downwards, destroy when off screen
        float deltaMove = waveConfig.GetMoveSpeed() * Time.deltaTime;
        transform.position -= Vector3.up * deltaMove;

        if (isOffScreen) Destroy(gameObject);
    }

    void OnBecameInvisible()
    {
        isOffScreen = true;
    }

    void OnBecameVisible()
    {
        isOffScreen = false;
    }
}
