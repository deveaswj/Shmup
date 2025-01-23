using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Add this to Enemy prefabs

public class Pathfinder : MonoBehaviour
{
    [SerializeField] EnemyBehaviorType behaviorWhenShot;
    [SerializeField] [Range(0f, 1f)] float behaviorOdds = 0.5f;
    // courage: odds of diving or fleeing (0 = always flee, 1 = always dive)
    [SerializeField] [Range(0f, 1f)] float courage = 0.5f;

    EnemySpawner enemySpawner;
    EnemyCommander commander;
    WaveConfigSO waveConfig;
    List<Vector3> waypoints;
    int waypointIndex = 0;
    Health health;
    bool hasHealth;
    bool isOffScreen;
    Vector3 v3up = Vector3.up;
    int damage = 0;
    int loopCount = 0;
    bool isBossWave = false;
    bool actOnOdds = false;

    bool isCalm = false;

    void Awake()
    {
        enemySpawner = FindObjectOfType<EnemySpawner>();
        commander = FindObjectOfType<EnemyCommander>();
    }

    void Start()
    {
        waveConfig = enemySpawner.GetCurrentWave();
        waypoints = waveConfig.GetWaypoints();
        isBossWave = waveConfig.IsBossWave();
        transform.position = waypoints[waypointIndex];
        hasHealth = TryGetComponent(out health);

        float random = Random.Range(0f, 1f);
        actOnOdds = random < behaviorOdds;

        if (behaviorWhenShot == EnemyBehaviorType.Courage)
        {
            // roll a new random number between 0 and 1
            // if within courage threshold, dive, else flee
            // ex: if 80% courage threshold, up through 79% dives, 80% and above flees

            float courageCheck = Random.Range(0f, 1f);
            if (courageCheck < courage)
            {
                behaviorWhenShot = EnemyBehaviorType.Dive;
            }
            else
            {
                behaviorWhenShot = EnemyBehaviorType.Flee;
            }
        }

        if (behaviorWhenShot == EnemyBehaviorType.Either)
        {
            int newBehavior = Random.Range(0, 2);   // 0 or 1 and never 2
            switch (newBehavior)
            {
                case 0:
                    behaviorWhenShot = EnemyBehaviorType.Dive;
                    break;
                case 1:
                    behaviorWhenShot = EnemyBehaviorType.Flee;
                    break;
                default:
                    break;
            }
        }
    }

    void Update()
    {
        // once calmed, stay calm for life
        if (commander.IsCalm)
        {
            isCalm = true;
            behaviorWhenShot = EnemyBehaviorType.None;
        }

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

    float GetDeltaSpeed()
    {
        return commander.GetSpeedMultiplier() * waveConfig.GetMoveSpeed() * Time.deltaTime;
    }

    void FollowPath()
    {
        if (waypointIndex < waypoints.Count)
        {
            Vector3 targetPosition = waypoints[waypointIndex];
            float deltaMove = GetDeltaSpeed();
            transform.position = Vector2.MoveTowards(transform.position, targetPosition, deltaMove);
            if (transform.position == targetPosition)
            {
                waypointIndex++;
            }
        }
        else
        {
			if (isBossWave || loopCount < waveConfig.Loops)
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
        if (isCalm) return;

        // move upwards, destroy when off screen   
        float deltaMove = GetDeltaSpeed();
        transform.position += v3up * deltaMove;

        if (isOffScreen) DestroyShip();
    }

    void Dive()
    {
        if (isCalm) return;

        // move downwards, destroy when off screen
        float deltaMove = GetDeltaSpeed();
        transform.position -= v3up * deltaMove;

        if (isOffScreen) DestroyShip();
    }

    void DestroyShip()
    {
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
