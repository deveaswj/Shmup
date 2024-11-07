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
    int boostCount = 0;

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

    PowerUpType lastWeaponType = PowerUpType.None;
    int duplicateWeaponCount = 0;

    // Main camera bounds
    Vector2 minBounds, maxBounds;

    // How we shoot
    Shooter shooter;
    bool canFire = true;

    // Health & Energy
    Health health;
    PlayerEnergy energy;

    AudioManager audioManager;

    void Awake()
    {
        shooter = GetComponent<Shooter>();
        droneGroupController = GetComponent<DroneGroupController>();
        health = GetComponent<Health>();
        energy = GetComponent<PlayerEnergy>();
        audioManager = FindObjectOfType<AudioManager>();
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
        CheckEnergy();
        // Roll();
        Move();
    }

    void CheckEnergy()
    {
        bool couldFire = canFire;
        if (energy != null)
        {
            if (couldFire)
            {
                // if we could, stop if we're at empty
                canFire = !energy.IsEmpty();
            }
            else
            {
                // if we couldn't, don't restart until we're above critical
                canFire = !energy.IsBelowCritical();
            }
        }
        else
        {
            canFire = true;
        }
        // if we could before, and can't now, stop firing
        if (couldFire && !canFire)
        {
            SetFiring(false);
        }
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

    void OnBoost(InputValue value)
    {
        if (value.isPressed)
        {
            if (!boosted)
            {
                if (boostCount < 1)
                {
                    audioManager.PlayErrorClip();
                    return;
                }
                else
                // Add speed
                {
                    // start boost coroutine
                    StartCoroutine(BoostSpeed());
                    boostCount--;
                }
            }
        }
    }

    void OnFire(InputValue value)
    {
        if (canFire)
        {
            bool isFiring = value.isPressed;
            if (energy != null)
            {
                if (energy.IsEmpty())
                {
                    Debug.Log("OnFire: No energy left!");
                    isFiring = false;
                }
            }
            SetFiring(isFiring);
        }
    }
    
    void SetFiring(bool value)
    {
        if (shooter != null)
        {
            shooter.SetFiring(value);
        }
        // let any drones know the firing state
        if (fireEventChannel != null)
        {
            fireEventChannel.RaiseEvent(value);
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
            case PowerUpType.Weapon2:
            case PowerUpType.Weapon3:
                Debug.Log("Power up: Weapon");
                WeaponPowerUp(powerUpType);
                break;
            case PowerUpType.Drone:
                Debug.Log("Power up: Drone");
                DronePowerUp();
                break;
            default:
                break;
        }
    }

    void WeaponPowerUp(PowerUpType powerUpType)
    {
        audioManager.PlayPowerUpClip();
        // figure out speed bonus if collected same weapon twice
        if (lastWeaponType == powerUpType)
        {
            duplicateWeaponCount++;
        }
        else
        {
            duplicateWeaponCount = 0;
        }
        // #TODO: alternate increasing speed (velocity) or firing rate
        // based on whether the count is even or odd
        // increase speed first, then rate, then speed, then rate ...
        //
        float speedBonus = duplicateWeaponCount * 0.5f;
        // float rateBonus = duplicateWeaponCount * 0.5f;
        // Add weapon
        switch (powerUpType)
        {
            case PowerUpType.Weapon1:
                Debug.Log("Power up: Weapon 1 (DoubleSpeed)");
                Weapon_DoubleSpeed(speedBonus);
                break;
            case PowerUpType.Weapon2:
                Debug.Log("Power up: Weapon 2 (DoubleShot)");
                Weapon_DoubleShot(speedBonus);
                break;
            case PowerUpType.Weapon3:
                Debug.Log("Power up: Weapon 3 (Photon)");
                Weapon_Photon(speedBonus);
                break;
        }
        lastWeaponType = powerUpType;
    }


    void HealthPowerUp(PowerUpType powerUpType)
    {
        // Add health
        if (health != null)
        {
            audioManager.PlayPowerUpClip();
            switch (powerUpType)
            {
                case PowerUpType.MinorHeal:
                    health.MinorHealPlus();
                    break;
                case PowerUpType.MajorHeal:
                    health.MajorHealPlus();
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
            audioManager.PlayShieldOnClip();
            shieldScript.TurnOn();
        }
        else
        {
            Debug.LogError("Shield script is null??");
        }
    }

    void SpeedPowerUp()
    {
        // Add to boost count
        audioManager.PlayPowerUpClip();
        boostCount++;
   }

    void Weapon_Default()   // called at startup and when player resets
    {
        // Add weapon
        SetWeapon(ProjectileType.SingleShot);
        lastWeaponType = PowerUpType.None;
    }

    void Weapon_DoubleSpeed(float speedBonus = 0.0f)
    {
        // Add weapon
        SetWeapon(ProjectileType.SingleShot, speedBonus + 2.0f);
    }

    void Weapon_DoubleShot(float speedBonus = 0.0f)
    {
        // Add weapon
        SetWeapon(ProjectileType.DoubleShot, speedBonus + 1.0f);
    }

    void Weapon_Photon(float speedBonus = 0.0f)
    {
        // Add weapon
        SetWeapon(ProjectileType.Photon, speedBonus + 1.0f);
    }

    void SetWeapon(ProjectileType type, float speed = 1.0f)
    {
        shooter.SetProjectileType(type);
        shooter.SetSpeedMultiplier(speed);
        ammoEventChannel.RaiseTypeEvent(type);
        ammoEventChannel.RaiseSpeedEvent(speed);
    }

    void DronePowerUp()
    {
        // Add drone
        if (droneGroupController != null)
        {
            audioManager.PlayPowerUpClip();
            droneGroupController.AddDrone();
        }
    }

    IEnumerator BoostSpeed()
    {
        boosted = true;
        audioManager.PlayBoosterClip();
        yield return new WaitForSeconds(boostDuration);
        boosted = false;
    }

}