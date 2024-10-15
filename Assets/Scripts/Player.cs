using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [SerializeField] FireEventChannel fireEventChannel;
    [SerializeField] AmmoEventChannel ammoEventChannel;
    DroneGroupController droneGroupController;

    [Header("Movement")]
    [SerializeField] float moveSpeed = 5f;
    [SerializeField] float boostSpeed = 10f;
    [SerializeField] float boostDuration = 5f;
    [SerializeField] bool boosted = false;

    Vector2 rawInput;

    [Header("Padding")]
    [SerializeField] Vector2 padding0 = new(0.5f, 2f);  // left, bottom
    [SerializeField] Vector2 padding1 = new(0.5f, 5f);  // right, top

    [Header("Shield")]
    [SerializeField] GameObject shieldPrefab;
    [SerializeField] Vector2 shieldPosition;
    [SerializeField] Vector2 shieldScale = new(1f, 1f);
    GameObject shieldObject;
    Shield shieldScript;
    bool shieldCreated = false;

    // [Header("Roll")]
    // [SerializeField] float rollAmount = 15f;
    // [SerializeField] float rollSpeed = 5f;

    // Main camera bounds
    Vector2 minBounds, maxBounds;

    // How we shoot
    Shooter shooter;

    // Health
    Health health;

    void Awake()
    {
        shooter = GetComponent<Shooter>();
        droneGroupController = GetComponent<DroneGroupController>();
        health = GetComponent<Health>();
    }

    void Start()
    {
        InitShield();
        InitializeBounds();
        Weapon_Default();
    }

    void InitShield()
    {
        // Try to find an existing Shield in the scene
        shieldObject = GameObject.FindWithTag("PlayerShield");
        // If not found, try to create one
        if (shieldObject == null)
        {
            if (shieldPrefab != null)
            {
                // instantiate from the prefab
                shieldObject = Instantiate(shieldPrefab);
                shieldCreated = true;
            }
        }
        // Get a reference to the shield script
        if (shieldObject != null)
        {
            shieldScript = shieldObject.GetComponent<Shield>();
            if (shieldScript != null)
            {
                shieldScript.SetPositionOffset(shieldPosition);
                shieldScript.SetLocalScale(shieldScale);
                if (shieldCreated)
                {
                    // assign the player ship reference
                    shieldScript.SetPlayerShip(gameObject);
                }
            }
            else
            {
                Debug.LogError("Shield script component not found on Shield object");
            }
        }
        else
        {
            Debug.LogError("Shield object could not be found or created");
        }
        // debug verification
        if (shieldScript != null)
        {
            Debug.Log("Shield script ready!");
            shieldScript.TurnOff();
        }
        else
        {
            Debug.LogError("Shield script not ready!");
        }
    }


    void InitializeBounds()
    {
        Camera mainCamera = Camera.main;
        minBounds = mainCamera.ViewportToWorldPoint(new Vector2(0, 0));
        maxBounds = mainCamera.ViewportToWorldPoint(new Vector2(1, 1));
    }

    void Update()
    {
        // Roll();
        Move();
    }

    void Roll()
    {
        // TODO
    }

    void Move()
    {
        float speed = boosted ? boostSpeed : moveSpeed;
        Vector2 delta = speed * Time.deltaTime * rawInput;
        Vector2 newPos = new(transform.position.x + delta.x, transform.position.y + delta.y);
        newPos.x = Mathf.Clamp(newPos.x, minBounds.x + padding0.x, maxBounds.x - padding1.x);
        newPos.y = Mathf.Clamp(newPos.y, minBounds.y + padding0.y, maxBounds.y - padding1.y);
        transform.position = newPos;
    }

    void OnMove(InputValue value)
    {
        rawInput = value.Get<Vector2>();
    }

    void OnFire(InputValue value)
    {
        if (shooter != null)
        {
            shooter.SetFiring(value.isPressed);
        }

        // let any drones know we're firing
        if (fireEventChannel != null)
        {
            fireEventChannel.RaiseEvent(value.isPressed);
        }
    }
    
    public void PowerUp(PowerUpType powerUpType)
    {
        switch (powerUpType)
        {
            case PowerUpType.MinorHeal:
            case PowerUpType.MajorHeal:
            case PowerUpType.FullHeal:
                Debug.Log("Power up: Health");
                HealthPowerUp(powerUpType);
                break;
            case PowerUpType.Shield:
                Debug.Log("Power up: Shield");
                ShieldPowerUp();
                break;
            case PowerUpType.Speed:
                Debug.Log("Power up: Speed");
                SpeedPowerUp();
                break;
            case PowerUpType.Weapon1:
                Debug.Log("Power up: Weapon 1 (DoubleSpeed)");
                Weapon_DoubleSpeed();
                break;
            case PowerUpType.Weapon2:
                Debug.Log("Power up: Weapon 2 (DoubleShot)");
                Weapon_DoubleShot();
                break;
            case PowerUpType.Weapon3:
                Debug.Log("Power up: Weapon 3 (Photon)");
                Weapon_Photon();
                break;
            case PowerUpType.Drone:
                Debug.Log("Power up: Drone");
                DronePowerUp();
                break;
            default:
                break;
        }
    }

    void HealthPowerUp(PowerUpType powerUpType)
    {
        // Add health
        if (health != null)
        {
            switch (powerUpType)
            {
                case PowerUpType.MinorHeal:
                    health.MinorHeal();
                    break;
                case PowerUpType.MajorHeal:
                    health.MajorHeal();
                    break;
                case PowerUpType.FullHeal:
                    health.FullHeal();
                    break;
                default:
                    break;
            }
        }
        else
        {
            Debug.LogError("Health script is null??");
        }
    }

    void ShieldPowerUp()
    {
        // Add shield
        if (shieldScript != null)
        {
            Debug.Log("Shield ready to turn on");
            shieldScript.TurnOn();
        }
        else
        {
            Debug.LogError("Shield script is null??");
        }
    }

    void SpeedPowerUp()
    {
        // Add speed
        if (!boosted)
        {
            // start boost coroutine
            StartCoroutine(BoostSpeed());
        }
    }

    void Weapon_Default()
    {
        // Add weapon
        shooter.SetProjectileType(ProjectileType.SingleShot);
        shooter.SetSpeedMultiplier();
        ammoEventChannel.RaiseTypeEvent(ProjectileType.SingleShot);
        ammoEventChannel.RaiseSpeedEvent();
    }

    void Weapon_DoubleSpeed()
    {
        // Add weapon
        shooter.SetProjectileType(ProjectileType.SingleShot);
        shooter.SetSpeedMultiplier(2.0f);
        ammoEventChannel.RaiseTypeEvent(ProjectileType.SingleShot);
        ammoEventChannel.RaiseSpeedEvent(2.0f);
    }

    void Weapon_DoubleShot()
    {
        // Add weapon
        shooter.SetProjectileType(ProjectileType.DoubleShot);
        shooter.SetSpeedMultiplier();
        ammoEventChannel.RaiseTypeEvent(ProjectileType.DoubleShot);
        ammoEventChannel.RaiseSpeedEvent();
    }

    void Weapon_Photon()
    {
        // Add weapon
        shooter.SetProjectileType(ProjectileType.Photon);
        shooter.SetSpeedMultiplier();
        ammoEventChannel.RaiseTypeEvent(ProjectileType.Photon);
        ammoEventChannel.RaiseSpeedEvent();
    }

    void DronePowerUp()
    {
        // Add drone
        if (droneGroupController != null)
        {
            droneGroupController.AddDrone();
        }
    }

    IEnumerator BoostSpeed()
    {
        boosted = true;
        yield return new WaitForSeconds(boostDuration);
        boosted = false;
    }

}