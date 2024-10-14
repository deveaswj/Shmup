using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "Events/FireEventChannel")]
public class FireEventChannel : ScriptableObject
{
    public UnityAction<bool> OnFireEvent;

    public void RaiseEvent(bool isFiring)
    {
        OnFireEvent?.Invoke(isFiring);
    }
}
