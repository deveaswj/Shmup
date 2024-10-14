using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    }

    void OnDisable()
    {
        // Unsubscribe from events
        fireEventChannel.OnFireEvent -= HandleFireEvent;
    }

    void HandleFireEvent(bool isFiring)
    {
        if (shooter != null)
        {
            shooter.SetFiring(isFiring);
        }

    }
}
