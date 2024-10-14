using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneGroupController : MonoBehaviour
{
    [SerializeField] Transform player;               // Reference to the player's ship
    [SerializeField] float movingFollowSpeed = 5f;     // Speed at which drones follow the player while moving
    [SerializeField] float idleFollowSpeed = 0.5f;     // Speed at which drones follow the player while idle
    [SerializeField] int bufferLength = 10;          // How many positions to buffer for delay
    [SerializeField] GameObject dronePrefab;         // The drone prefab to instantiate
    [SerializeField] int startingDrones = 3;         // How many drones to start with

    private List<DroneBehavior> drones = new(); // List of active drones
    private List<Queue<Vector3>> droneBuffers = new(); // Buffers for staggered movement

    Vector3 lastPlayerPosition;
    bool playerIsMoving;

    void Start()
    {
        lastPlayerPosition = player.position;

        // Add the starting number of drones to the active list
        for (int i = 0; i < startingDrones; i++)
        {
            AddDrone();
        }
    }

    void Update()
    {
        // Check if the player is moving
        playerIsMoving = player.position != lastPlayerPosition;
        lastPlayerPosition = player.position;

        float followSpeed = playerIsMoving ? movingFollowSpeed : idleFollowSpeed;

        if (playerIsMoving)
        // Update the buffer for the first drone (following the player)
        if (drones.Count > 0)
        {
            UpdateBuffer(droneBuffers[0], player.position);
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
        // Instantiate a new drone object
        GameObject droneObject = Instantiate(dronePrefab);
        droneObject.transform.position = player.position;

        // Get the DroneBehavior component and add it to the list
        DroneBehavior drone = droneObject.GetComponent<DroneBehavior>();

        drones.Add(drone);  // Add the drone to the active list
        droneBuffers.Add(new Queue<Vector3>(new Vector3[bufferLength])); // Add a new buffer for the drone
    }

    // Method to remove a drone (e.g., when destroyed)
    public void RemoveDrone(DroneBehavior drone)
    {
        int index = drones.IndexOf(drone);
        drones.RemoveAt(index);          // Remove from the list
        droneBuffers.RemoveAt(index);    // Remove the corresponding buffer
        Destroy(drone.gameObject);       // Destroy the drone object
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
}
