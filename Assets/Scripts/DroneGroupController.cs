using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneGroupController : MonoBehaviour
{
    [SerializeField] Transform playerTransform;     // Reference to the player's ship
    [SerializeField] float movingFollowSpeed = 4.5f;  // Speed at which drones follow the player while moving
    [SerializeField] float idleFollowSpeed = 0.25f;  // Speed at which drones follow the player while idle
    [SerializeField] int bufferLength = 10;         // How many positions to buffer for delay
    [SerializeField] GameObject dronePrefab;        // The drone prefab to instantiate
    [SerializeField] int startingDrones = 0;        // How many drones to start with
    [SerializeField] int maxDrones = 3;             // Maximum number of drones in the group
    [SerializeField] AmmoEventChannel ammoEventChannel;

    private List<DroneBehavior> drones;             // List of active drones
    private List<Queue<Vector3>> droneBuffers;      // Buffers for staggered movement

    ProjectileType droneAmmoType = ProjectileType.SingleShot;
    float droneAmmoSpeed = 1.0f;
    float droneAmmoRate = 1.0f;

    Vector3 lastPlayerPosition;
    bool playerIsMoving;

    public int Count { get { return drones.Count; } }

    void OnEnable()
    {
        // Subscribe to events
        ammoEventChannel.OnAmmoTypeChange += HandleAmmoTypeChange;
        ammoEventChannel.OnAmmoSpeedChange += HandleAmmoSpeedChange;
        ammoEventChannel.OnAmmoRateChange += HandleAmmoRateChange;
    }

    void OnDisable()
    {
        // Unsubscribe from events
        ammoEventChannel.OnAmmoTypeChange -= HandleAmmoTypeChange;
        ammoEventChannel.OnAmmoSpeedChange -= HandleAmmoSpeedChange;
        ammoEventChannel.OnAmmoRateChange -= HandleAmmoRateChange;

        // Clear the list of active drones
        RemoveAllDrones();
    }


    void Start()
    {
        drones = new();
        droneBuffers = new();
        lastPlayerPosition = playerTransform.position;

        // Add the starting number of drones to the active list
        startingDrones = Mathf.Clamp(startingDrones, 0, maxDrones);
        for (int i = 0; i < startingDrones; i++)
        {
            AddDrone();
        }
    }

    void Update()
    {
        // Check if the player is moving
        playerIsMoving = playerTransform.position != lastPlayerPosition;
        lastPlayerPosition = playerTransform.position;

        float followSpeed = playerIsMoving ? movingFollowSpeed : idleFollowSpeed;

        if (playerIsMoving)
        // Update the buffer for the first drone (following the player)
        if (drones.Count > 0)
        {
            UpdateBuffer(droneBuffers[0], playerTransform.position);
        }

        // Handle movement and update buffers for each drone
        for (int i = 0; i < drones.Count; i++)
        {
            Vector3 targetPosition;

            if (i == 0)
            {
                // First drone follows the buffered positions of the player
                targetPosition = droneBuffers[i].Peek();
            }
            else
            {
                // Subsequent drones follow the buffered positions of the previous drone
                UpdateBuffer(droneBuffers[i], drones[i - 1].transform.position);
                targetPosition = droneBuffers[i].Peek();
            }

            // Smoothly move the drone to the target position
            drones[i].transform.position = Vector3.Lerp(
                drones[i].transform.position, 
                targetPosition, 
                followSpeed * Time.deltaTime
            );
        }
    }

    // Method to add a drone to the player's collection (e.g., when a power-up is collected)
    public void AddDrone()
    {
        if (drones.Count >= maxDrones)
        {
            return;  // No more drones can be added
        }

        // Instantiate a new drone object
        GameObject droneObject = Instantiate(dronePrefab);
        droneObject.transform.position = playerTransform.position;

        // Get the DroneBehavior component and set its properties
        DroneBehavior drone = droneObject.GetComponent<DroneBehavior>();

        drones.Add(drone);  // Add the drone to the active list
        droneBuffers.Add(new Queue<Vector3>(new Vector3[bufferLength])); // Add a new buffer for the drone

        SetAmmoType(droneAmmoType);
        SetAmmoSpeed(droneAmmoSpeed);
        SetAmmoRate(droneAmmoRate);
    }

    // Method to remove a drone (e.g., when destroyed)
    public void RemoveDrone(DroneBehavior drone)
    {
        int index = drones.IndexOf(drone);
        drones.RemoveAt(index);          // Remove from the list
        droneBuffers.RemoveAt(index);    // Remove the corresponding buffer
        Destroy(drone.gameObject);       // Destroy the drone object
    }

    public void RemoveAllDrones()
    {
        for (int i = drones.Count - 1; i >= 0; i--)
        {
            RemoveDrone(drones[i]);
        }
    }

    // Update the buffer with a new position
    private void UpdateBuffer(Queue<Vector3> buffer, Vector3 newPosition)
    {
        if (buffer.Count >= bufferLength)
        {
            buffer.Dequeue();  // Remove the oldest position
        }
        buffer.Enqueue(newPosition);  // Add the new position
    }

    public void SetAmmoType(ProjectileType type)
    {
        droneAmmoType = type;
        foreach (DroneBehavior drone in drones)
        {
            drone.SetAmmoType(type);
        }
    }

    public void SetAmmoSpeed(float speed = 1.0f)
    {
        droneAmmoSpeed = speed;
        foreach (DroneBehavior drone in drones)
        {
            drone.SetAmmoSpeed(speed);
        }
    }

    public void SetAmmoRate(float rate = 1.0f)
    {
        droneAmmoRate = rate;
        foreach (DroneBehavior drone in drones)
        {
            drone.SetAmmoRate(rate);
        }
    }

    void HandleAmmoTypeChange(ProjectileType type)
    {
        SetAmmoType(type);
    }

    void HandleAmmoSpeedChange(float speed)
    {
        SetAmmoSpeed(speed);
    }

    void HandleAmmoRateChange(float rate)
    {
        SetAmmoRate(rate);
    }
}
