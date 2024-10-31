using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEnergy : MonoBehaviour
{
    [SerializeField] FireEventChannel fireEventChannel;

    [SerializeField] float maxEnergy = 100f;
    [SerializeField] float criticalThreshold = 20f;
    [SerializeField] float drainRate = 5f;
    [SerializeField] float regenRate = 10f;
    [SerializeField] float gracePeriod = 0.5f;

    private float currentEnergy;
    private float previousEnergy;

    private float fireHoldTime = 0f;
    private bool isDraining = false;

    private void Start()
    {
        currentEnergy = maxEnergy;
        previousEnergy = currentEnergy;
    }

    private void OnEnable()
    {
        // Subscribe to events
        fireEventChannel.OnFireEvent += HandleFireEvent;
    }

    private void OnDisable()
    {
        // Unsubscribe from events
        fireEventChannel.OnFireEvent -= HandleFireEvent;
    }

    private void HandleFireEvent(bool isFiring)
    {
        if (isFiring)
        {
            StartDraining();
        }
        else
        {
            StopDraining();
        }
    }

    public float GetCurrentEnergy() => currentEnergy;
    public float GetEnergyDifference() => currentEnergy - previousEnergy;
    public int GetEnergyPercentage() => Mathf.RoundToInt(100 * (currentEnergy / maxEnergy));
    public bool IsBelowCritical() => currentEnergy <= criticalThreshold;
    public bool IsEmpty() => currentEnergy <= 1;
    
    public void ResetEnergy() => currentEnergy = maxEnergy;

    public void StartDraining()
    {
        isDraining = true;
        fireHoldTime = 0f; // Reset hold time when firing begins
    }

    public void StopDraining()
    {
        isDraining = false;
    }

    private void Update()
    {
        if (isDraining)
        {
            fireHoldTime += Time.deltaTime;
            if (fireHoldTime > gracePeriod && currentEnergy > 0)
            {
                DrainEnergy();
            }
        }
        else
        {
            RegenerateEnergy();
        }
        previousEnergy = currentEnergy;
    }

    private void DrainEnergy()
    {
        currentEnergy -= drainRate * Time.deltaTime;
        currentEnergy = Mathf.Clamp(currentEnergy, 0, maxEnergy);
    }

    private void RegenerateEnergy()
    {
        currentEnergy += regenRate * Time.deltaTime;
        currentEnergy = Mathf.Clamp(currentEnergy, 0, maxEnergy);
    }
}
