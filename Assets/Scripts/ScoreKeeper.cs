using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreKeeper : MonoBehaviour
{
    [SerializeField] private GameState gameState;

    public int GetScore() => gameState.score;
    public void AddScore(int amount) => gameState.AddScore(amount);
    public void ResetScore() => gameState.ResetScore();
    public void SetScore(int amount) => gameState.SetScore(amount);

    void Awake()
    {
        // Let there be only one
        ScoreKeeper[] existingInstances = FindObjectsByType<ScoreKeeper>(FindObjectsSortMode.None);
        if (existingInstances.Length > 1)
        {
            gameObject.SetActive(false);
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
    }
}
