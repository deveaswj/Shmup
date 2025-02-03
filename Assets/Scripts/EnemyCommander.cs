using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Fleet state controller

public class EnemyCommander : MonoBehaviour
{
    [SerializeField] bool isCalm = false;
    [SerializeField] bool isSlow = false;
    [SerializeField] float calmTime = 15f;
    [SerializeField] float slowTime = 15f;
    [SerializeField] float speedMultiplier = 1.0f;
    [SerializeField] float defaultSpeed = 1.0f;
    [SerializeField] float slowSpeed = 0.375f;

    public bool IsCalm { get => isCalm; }
    public bool IsSlow { get => isSlow; }

    public void SetCalm(bool value) => isCalm = value;

    public void Calm() => CalmForSeconds(calmTime);
    public void Slow() => SlowForSeconds(slowTime);

    public void CalmForSeconds(float seconds)
    {
        isCalm = true;
        StartCoroutine(ResetCalm(seconds));
    }

    IEnumerator ResetCalm(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        isCalm = false;
    }

    public void SlowForSeconds(float seconds)
    {
        isSlow = true;
        StartCoroutine(ResetSlow(seconds));
    }

    IEnumerator ResetSlow(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        isSlow = false;
    }

    public float GetDefaultSpeed()
    {
        return defaultSpeed;
    }

    public float GetModifiedSpeed()
    {
        return (isSlow ? slowSpeed : defaultSpeed);
    }
}
