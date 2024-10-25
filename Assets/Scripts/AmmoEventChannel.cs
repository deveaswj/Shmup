using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "Events/AmmoEventChannel")]
public class AmmoEventChannel : ScriptableObject
{
    public UnityAction<ProjectileType> OnAmmoTypeChange;
    public UnityAction<float> OnAmmoSpeedChange;
    public UnityAction<float> OnAmmoRateChange;

    public void RaiseTypeEvent(ProjectileType type = ProjectileType.SingleShot)
    {
        OnAmmoTypeChange?.Invoke(type);
    }

    public void RaiseSpeedEvent(float speed = 1.0f)
    {
        OnAmmoSpeedChange?.Invoke(speed);
    }

    public void RaiseRateEvent(float rate = 1.0f)
    {
        OnAmmoRateChange?.Invoke(rate);
    }
}
