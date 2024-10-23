using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Add this to Enemy prefabs

public class Pathfinder : MonoBehaviour
{
    [SerializeField] EnemyBehaviorType behaviorWhenShot;
    [SerializeField] [Range(0f, 1f)] float behaviorOdds = 0.5f;
    EnemySpawner enemySpawner;
    WaveConfigSO waveConfig;
    List<Vector3> waypoints;
    int waypointIndex = 0;
    Health health;
    bool hasHealth;
    bool isOffScreen;
    Vector3 v3up = Vector3.up;
    int damage = 0;
    int loopCount = 0;
    float random;
    bool actOnOdds = false;

    void Awake()
    {
        enemySpawner = FindObjectOfType<EnemySpawner>();
    }

    void Start()
    {
        waveConfig = enemySpawner.GetCurrentWave();
        waypoints = waveConfig.GetWaypoints();
        transform.position = waypoints[waypointIndex];
        hasHealth = TryGetComponent(out health);
        random = Random.Range(0f, 1f);
        actOnOdds = random < behaviorOdds;
    }

    void Update()
    {
        damage = hasHealth ? health.GetDamage() : 0;
        if (damage > 0 && actOnOdds)
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
            Vector3 targetPosition = waypoints[waypointIndex];
            float deltaMove = waveConfig.GetMoveSpeed() * Time.deltaTime;
            transform.position = Vector2.MoveTowards(transform.position, targetPosition, deltaMove);
            if (transform.position == targetPosition)
            {
                waypointIndex++;
            }
        }
        else
        {
			if (loopCount < waveConfig.Loops)
			{
				loopCount++;
				waypointIndex = 0;
				transform.position = waypoints[waypointIndex];
			}
			else
			{
				DestroyShip();
			}
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
