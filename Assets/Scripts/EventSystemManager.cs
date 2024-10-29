using UnityEngine;
using UnityEngine.EventSystems;

public class EventSystemManager : MonoBehaviour
{
    private void Awake()
    {
        var existingEventSystems = FindObjectsOfType<EventSystem>();
        Debug.Log("ESM: Found " + existingEventSystems.Length + " EventSystems");

        // Destroy this EventSystem if another exists
        if (existingEventSystems.Length > 1)
        {
            Debug.Log("ESM: Destroying EventSystem");
            Destroy(gameObject);
        }
        else
        {
            Debug.Log("ESM: Using this as the EventSystem");
            DontDestroyOnLoad(gameObject); // Keep this one persistent if it's the only one
        }
    }
}
