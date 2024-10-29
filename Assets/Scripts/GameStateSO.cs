using UnityEngine;

[CreateAssetMenu(fileName = "GameState", menuName = "Game/GameState")]
public class GameState : ScriptableObject
{
    public int CurrentLevel = 1;
    public int score = 0;

    public void ResetScore() => score = 0;
    public void AddScore(int amount) => score = Mathf.Clamp(score + amount, 0, int.MaxValue);
    public void SetScore(int amount) => score = Mathf.Clamp(amount, 0, int.MaxValue);
    public void ResetLevel() => CurrentLevel = 1;
}
