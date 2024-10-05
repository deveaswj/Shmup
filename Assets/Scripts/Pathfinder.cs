using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinder : MonoBehaviour
{
    [SerializeField] EnemyBehaviorType behaviorWhenShot;
    EnemySpawner enemySpawner;
    WaveConfigSO waveConfig;
    List<Transform> waypoints;
    int waypointIndex = 0;
    Health health;

    void Awake()
    {
        enemySpawner = FindObjectOfType<EnemySpawner>();
        health = GetComponent<Health>();
    }

    void Start()
    {
        waveConfig = enemySpawner.GetCurrentWave();
        waypoints = waveConfig.GetWaypoints();
        transform.position = waypoints[waypointIndex].position;
    }

    void Update()
    {
        int damage = health.GetDamage();
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
                break;
            case EnemyBehaviorType.Dive:
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

        // TODO: destroy when off screen
    }

    void Dive()
    {
        // move downwards, destroy when off screen
        float deltaMove = waveConfig.GetMoveSpeed() * Time.deltaTime;
        transform.position -= Vector3.up * deltaMove;

        // TODO: destroy when off screen
    }
}
