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
    Vector3 v3up = Vector3.up;
    int damage = 0;

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
        damage = hasHealth ? health.GetDamage() : 0;
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
            DestroyShip();
        }
    }

    void Flee()
    {
        // move upwards, destroy when off screen   
        float deltaMove = waveConfig.GetMoveSpeed() * Time.deltaTime;
        transform.position += v3up * deltaMove;

        if (isOffScreen) DestroyShip();
    }

    void Dive()
    {
        // move downwards, destroy when off screen
        float deltaMove = waveConfig.GetMoveSpeed() * Time.deltaTime;
        transform.position -= v3up * deltaMove;

        if (isOffScreen) DestroyShip();
    }

    void DestroyShip()
    {
        if (TryGetComponent<DropPowerUp>(out var dropPowerUp))
        {
            dropPowerUp.SetEnabled(false);
        }
        Destroy(gameObject);
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
