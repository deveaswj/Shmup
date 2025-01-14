using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Fleet state controller

public class EnemyCommander : MonoBehaviour
{
    [SerializeField] bool isCalm = false;
    [SerializeField] float calmTime = 5f;

    public bool IsCalm { get => isCalm; }

    public void SetCalm(bool value) => isCalm = value;

    public void Calm() => CalmForSeconds(calmTime);

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
}
