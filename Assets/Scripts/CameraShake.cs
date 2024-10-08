using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{

    [SerializeField] private ShakeSettings defaultShakeSettings;

    Vector3 initialPosition;

    public ShakeSettings Settings => defaultShakeSettings;

    void Start()
    {
        initialPosition = transform.position;
    }

    public void Play()
    {
        if (defaultShakeSettings == null) { return; }
        Play(defaultShakeSettings);
    }

    public void Play(ShakeSettings shakeSettings)
    {
        if (shakeSettings == null) { return; }
        if (shakeSettings.smooth)
        {
        StartCoroutine(SmoothShake(shakeSettings));
        }
        else
        {
        StartCoroutine(RoughShake(shakeSettings));
        }
    }

    IEnumerator SmoothShake(ShakeSettings settings)
    {
        float elapsedTime = 0;
        float xOffset, yOffset;

        while (elapsedTime < settings.duration)
        {
            xOffset = Mathf.PerlinNoise(elapsedTime * settings.frequency, 0) * 2 - 1;
            yOffset = Mathf.PerlinNoise(0, elapsedTime * settings.frequency) * 2 - 1;

            transform.position = initialPosition + new Vector3(xOffset, yOffset, 0) * settings.magnitude;

            elapsedTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        // Smoothly return to initial position
        float returnDuration = 0.1f; // Adjust this for the desired smoothness
        elapsedTime = 0;
        Vector3 currentPos = transform.position;

        while (elapsedTime < returnDuration)
        {
            transform.position = Vector3.Lerp(currentPos, initialPosition, elapsedTime / returnDuration);
            elapsedTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        transform.position = initialPosition;
    }

    IEnumerator RoughShake(ShakeSettings settings)
    {
        float elapsedTime = 0;
        while (elapsedTime < settings.duration)
        {
            transform.position = initialPosition + (Vector3)Random.insideUnitCircle * settings.magnitude;
            elapsedTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        transform.position = initialPosition;
    }

}
