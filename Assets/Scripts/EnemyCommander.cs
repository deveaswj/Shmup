using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Fleet state controller

public class EnemyCommander : MonoBehaviour
{
    [SerializeField] bool isCalm = false;
    [SerializeField] float calmTime = 15f;
    [SerializeField] float slowTime = 15f;
    [SerializeField] float speedMultiplier = 1.0f;
    [SerializeField] float defaultSpeed = 1.0f;

    public float GetSpeedMultiplier() => speedMultiplier;

    public bool IsCalm { get => isCalm; }
    public bool IsSlow { get => speedMultiplier < defaultSpeed; }

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
        speedMultiplier = 0.25f;
        StartCoroutine(ResetSlow(seconds));
    }

    IEnumerator ResetSlow(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        speedMultiplier = defaultSpeed;
    }
}
