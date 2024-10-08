using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPartsManager : MonoBehaviour
{
    private GameObject shieldInstance;
    private GameObject boosterInstance;

    public GameObject shieldPrefab;
    public GameObject boosterPrefab;

    // Method to get the shield instance, instantiate if needed
    public GameObject GetShield()
    {
        if (shieldInstance == null)
        {
            shieldInstance = Instantiate(shieldPrefab);
        }
        shieldInstance.SetActive(true);
        return shieldInstance;
    }

    // Method to deactivate the shield when no longer in use
    public void DeactivateShield()
    {
        if (shieldInstance != null)
        {
            shieldInstance.SetActive(false);
        }
    }

    // Method to get the booster instance, instantiate if needed
    public GameObject GetBooster()
    {
        if (boosterInstance == null)
        {
            boosterInstance = Instantiate(boosterPrefab);
        }
        boosterInstance.SetActive(true);
        return boosterInstance;
    }

    // Method to deactivate the booster when no longer in use
    public void DeactivateBooster()
    {
        if (boosterInstance != null)
        {
            boosterInstance.SetActive(false);
        }
    }
}
