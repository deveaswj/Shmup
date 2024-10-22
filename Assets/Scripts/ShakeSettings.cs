using UnityEngine;

[System.Serializable]
public class ShakeSettings
{
    [Tooltip("The duration of the shake. Try: 0.25f")]
    public float duration = 0.25f;

    [Tooltip("The magnitude of the shake. Try: 0.1f")]
    public float magnitude = 0.1f;

    [Tooltip("The frequency of the shake (if smooth). Try: 1f")]
    public float frequency = 1f;

    [Tooltip("Whether the shake should be smooth or rough. Try: true")]
    public bool smooth = true;

}
