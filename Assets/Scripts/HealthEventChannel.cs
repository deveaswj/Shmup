using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "Events/HealthEventChannel")]
public class HealthEventChannel : ScriptableObject
{
    // Something that broadcasts to this channel raised the event
    // This is the generic definition -- make an asset for Player and one for Enemies as need be
    // e.g. PLayerHealthEventChannel and EnemyHealthEventChannel
    // (and keep all event names generic, not specific to Player or Enemy)
    // But note that when all enemies broadcast to their channel, we can't know which one did it
    // If we want to know which one did it, uses actions in Health.cs instead.

    public UnityAction<int> OnHealthChange;
    public UnityAction OnDefeat;

    public void RaiseHealthChange(int amount) => OnHealthChange?.Invoke(amount);
    public void RaiseDefeat() => OnDefeat?.Invoke();
}
