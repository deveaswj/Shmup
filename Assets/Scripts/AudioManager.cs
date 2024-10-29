using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AudioClipData
{
    public AudioClip audioClip;
    [Range(0f, 1f)]
    public float volume = 1f;
	[Range(-3f, 3f)]
    public float pitch = 1f;
    public int priority = 0; // Add priority here for convenience
}

public class AudioManager : MonoBehaviour
{
    [SerializeField] int maxCount = 10;
    [SerializeField] AudioClipData projectile;
    [SerializeField] AudioClipData damage;
    [SerializeField] AudioClipData explosion;
    [SerializeField] AudioClipData powerUp;
    [SerializeField] AudioClipData shieldOn;
    [SerializeField] AudioClipData shieldOff;
    [SerializeField] AudioClipData booster;

    public static AudioManager Instance { get; private set; }

    private class AudioSourceWrapper
    {
        public AudioSource Source;
        public int Priority;
    }

    Queue<AudioSource> audioPool;
    List<AudioSourceWrapper> activeSources;

    public void PlayShootingClip()
    {
        PlaySound(projectile);
    }

    public void PlayDamageClip()
    {
        PlaySound(damage);
    }

    public void PlayExplosionClip()
    {
        PlaySound(explosion);
    }

    public void PlayPowerUpClip()
    {
        PlaySound(powerUp);
    }

    public void PlayShieldOnClip()
    {
        PlaySound(shieldOn);
    }

    public void PlayShieldOffClip()
    {
        PlaySound(shieldOff);
    }

    public void PlayBoosterClip()
    {
        PlaySound(booster);
    }

    void Awake()
    {
        ManageSingleton();

        audioPool = new Queue<AudioSource>();
        activeSources = new List<AudioSourceWrapper>();

        // Initialize the pool with maxCount AudioSources
        for (int i = 0; i < maxCount; i++)
        {
            AudioSource source = gameObject.AddComponent<AudioSource>();
            source.playOnAwake = false;
            audioPool.Enqueue(source);
        }
    }

    void ManageSingleton()
    {
        if (Instance != null)
        {
            gameObject.SetActive(false);
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    void PlaySound(AudioClipData clipData)
    {
        if (clipData == null || clipData.audioClip == null)
        {
            Debug.LogWarning("Invalid AudioClipData or missing AudioClip.");
            return;
        }

        // If there is an available AudioSource in the pool
        if (audioPool.Count > 0)
        {
            UseAudioSource(clipData);
        }
        else
        {
            // If the pool is full, find the lowest-priority active source
            var lowestPrioritySource = GetLowestPrioritySource();

            // If the new sound's priority is higher, replace the lowest priority sound
            if (lowestPrioritySource != null && lowestPrioritySource.Priority < clipData.priority)
            {
                ReplaceAudioSource(lowestPrioritySource, clipData);
            }
            else
            {
                // No available source and the new clip's priority is not high enough
                Debug.Log("Max audio source count reached, and no lower priority source found. Ignoring request.");
            }
        }
    }

    void UseAudioSource(AudioClipData clipData)
    {
        // Get an available AudioSource from the pool
        AudioSource source = audioPool.Dequeue();
        ConfigureAudioSource(source, clipData);
        source.Play();

        // Add to the active list with the associated priority
        var wrapper = new AudioSourceWrapper { Source = source, Priority = clipData.priority };
        activeSources.Add(wrapper);

        // Set a callback to release it after the clip ends
        StartCoroutine(ReturnToPoolAfterPlayback(wrapper, clipData.audioClip.length));
    }

    void ReplaceAudioSource(AudioSourceWrapper wrapper, AudioClipData clipData)
    {
        // Stop the current low-priority sound
        wrapper.Source.Stop();
        ConfigureAudioSource(wrapper.Source, clipData);
        wrapper.Priority = clipData.priority;
        wrapper.Source.Play();

        // Restart the coroutine to return it to the pool after the new clip finishes
        StopAllCoroutines(); // Stop previous coroutines for clean management
        StartCoroutine(ReturnToPoolAfterPlayback(wrapper, clipData.audioClip.length));
    }

    void ConfigureAudioSource(AudioSource source, AudioClipData clipData)
    {
        source.clip = clipData.audioClip;
        source.volume = clipData.volume;
        source.pitch = clipData.pitch;
    }

    AudioSourceWrapper GetLowestPrioritySource()
    {
        // Find the audio source with the lowest priority
        AudioSourceWrapper lowest = null;
        foreach (var wrapper in activeSources)
        {
            if (lowest == null || wrapper.Priority < lowest.Priority)
            {
                lowest = wrapper;
            }
        }
        return lowest;
    }

    IEnumerator ReturnToPoolAfterPlayback(AudioSourceWrapper wrapper, float delay)
    {
        yield return new WaitForSeconds(delay);

        // Stop the AudioSource and return it to the pool
        wrapper.Source.Stop();
        wrapper.Source.clip = null;
        activeSources.Remove(wrapper);
        audioPool.Enqueue(wrapper.Source);
    }
}
