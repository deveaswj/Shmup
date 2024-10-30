using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreKeeper : MonoBehaviour
{
    [SerializeField] private GameState gameState;

    public int GetScore() => gameState.GetScore();
    public void AddScore(int amount) => gameState.AddScore(amount);
    public void ResetScore() => gameState.ResetScore();
    public void SetScore(int amount) => gameState.SetScore(amount);

    static ScoreKeeper instance;

    void Awake()
    {
        CreateSingleton();
    }

    bool CreateSingleton()
    {
        if (instance != null && instance != this)
        {
            Debug.Log("Removing duplicate ScoreKeeper");
            gameObject.SetActive(false);
            Destroy(gameObject);
            return false;
        }
        Debug.Log("Creating ScoreKeeper");
        instance = this;
        DontDestroyOnLoad(gameObject);
        return true;
    }



}
