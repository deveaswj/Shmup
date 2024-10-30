using UnityEngine;

[CreateAssetMenu(fileName = "GameState", menuName = "Game/GameState")]
public class GameState : ScriptableObject
{
    int currentLevel = 1;
    int score = 0;

    public int GetScore() => score;
    public void ResetScore() => score = 0;
    public void AddScore(int amount) => score = Mathf.Clamp(score + amount, 0, int.MaxValue);
    public void SetScore(int amount) => score = Mathf.Clamp(amount, 0, int.MaxValue);

    public int GetLevel() => currentLevel;
    public void SetLevel(int amount) => currentLevel = Mathf.Clamp(amount, 1, int.MaxValue);
    public void ResetLevel() => currentLevel = 1;
    public void NextLevel() => currentLevel++;
}
