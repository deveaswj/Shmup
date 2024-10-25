using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DroneBehavior : MonoBehaviour
{

    [SerializeField] FireEventChannel fireEventChannel;

    // How we shoot
    Shooter shooter;

    void Start()
    {
        shooter = GetComponent<Shooter>();
    }

    void OnEnable()
    {
        // Subscribe to events
        fireEventChannel.OnFireEvent += HandleFireEvent;
        SceneManager.sceneUnloaded += OnSceneUnloaded;
    }

    void OnDisable()
    {
        // Unsubscribe from events
        fireEventChannel.OnFireEvent -= HandleFireEvent;
        SceneManager.sceneUnloaded -= OnSceneUnloaded;
    }

    void HandleFireEvent(bool isFiring)
    {
        if (shooter != null)
        {
            shooter.SetFiring(isFiring);
        }
    }

    public void SetAmmoType(ProjectileType type)
    {
        if (shooter != null)
        {
            shooter.SetProjectileType(type);
        }
    }

    public void SetAmmoSpeed(float speed = 1.0f)
    {
        if (shooter != null)
        {
            shooter.SetSpeedMultiplier(speed);
        }
    }

    void OnSceneUnloaded(Scene scene)
    {
        Destroy(gameObject);
    }
}
