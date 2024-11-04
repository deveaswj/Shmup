using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectilePool : MonoBehaviour
{
    [SerializeField] GameObject[] projectilePrefabs;
    [SerializeField] int poolSize = 10;

    private Dictionary<ProjectileType, Queue<Projectile>> poolDictionary;
	private Dictionary<ProjectileType, GameObject> prefabDictionary;
    private Dictionary<ProjectileType, int> serialNumbersDictionary;

    void Start()
    {
        poolDictionary = new();
        prefabDictionary = new();
        serialNumbersDictionary = new();

        // Populate the pool with projectiles from each prefab
        foreach (GameObject prefab in projectilePrefabs)
        {
            if (prefab == null)
            {
                Debug.LogError("Cannot instantiate - Projectile prefab is null");
                continue;
            }
            ProjectileType type = prefab.GetComponent<Projectile>().GetProjectileType();
			prefabDictionary[type] = prefab;
            Queue<Projectile> pool = new Queue<Projectile>();

            for (int i = 0; i < poolSize; i++)
            {
                Projectile newProjectile = NewProjectileComponent(prefab, type);
                pool.Enqueue(newProjectile);
            }

            poolDictionary[type] = pool;
        }
    }

    Projectile NewProjectileComponent(GameObject prefab, ProjectileType type)
    {
        if (prefab == null)
        {
            Debug.LogError("Cannot instantiate - Projectile prefab is null");
            return null;
        }
        GameObject obj = Instantiate(prefab);
        if (obj == null)
        {
            Debug.LogError("Failed to instantiate projectile");
            return null;
        }

        // Check serialNumbersDictionary for this projectile type
        // If not found, add an entry with a serial number of 0
        // If found, add 1 to the serial number
        int serialNumber = 0;
        if (!serialNumbersDictionary.ContainsKey(type))
        {
            serialNumbersDictionary.Add(type, serialNumber);
        }
        else
        {
            serialNumber = serialNumbersDictionary[type] + 1;
            serialNumbersDictionary[type] = serialNumber;
        }
        // rename this object to include the type and serial number
        obj.name = "(" + type + ") " + serialNumber;

        obj.SetActive(false); // Deactivate the projectile
        if (!obj.TryGetComponent<Projectile>(out var projectile))
        {
            Debug.LogError("Projectile component not found on instantiated object.");
            return null;
        }
        return projectile;
    }


    // Get a projectile from the pool based on its type
    public Projectile GetProjectile(ProjectileType type)
    {
        Projectile projectile = null;
        if (poolDictionary.ContainsKey(type))
        {
            if (poolDictionary[type].Count > 0)
            {
                projectile = poolDictionary[type].Dequeue();
            }
            else
            {
				GameObject prefab = prefabDictionary[type];
                projectile = NewProjectileComponent(prefab, type);
            }
        }
        else
        {
            Debug.LogError("No pool found for getting projectile type: " + type);
        }
        if (projectile == null)
        {
            Debug.LogError("Failed to GetProjectile");
        }
		return projectile;
    }


    public void ReturnProjectile(Projectile projectile)
    {
        // pool.Enqueue(projectile);
        ProjectileType type = projectile.GetProjectileType();
        if (poolDictionary.ContainsKey(type))
        {
            projectile.Deactivate();
            poolDictionary[type].Enqueue(projectile);
        }
        else
        {
            Debug.LogError("No pool found for returned projectile type: " + type);
        }
    }
}
