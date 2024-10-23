using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreKeeper : MonoBehaviour
{
    int score = 0;

    public int GetScore() => score;
    public void AddScore(int amount) => score = Mathf.Clamp(score + amount, 0, int.MaxValue);
    public void ResetScore() => score = 0;
    public void SetScore(int amount) => score = Mathf.Clamp(amount, 0, int.MaxValue);
}
