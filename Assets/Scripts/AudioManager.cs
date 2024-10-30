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
    [Header("Pool Settings")]
    [SerializeField] int maxCount = 24;

    [Header("Sound Effects")]
    [SerializeField] AudioClipData projectile;
    [SerializeField] AudioClipData damage;
    [SerializeField] AudioClipData explosion;
    [SerializeField] AudioClipData powerUp;
    [SerializeField] AudioClipData shieldOn;
    [SerializeField] AudioClipData shieldOff;
    [SerializeField] AudioClipData booster;
    
    [Header("Source Prefab")]
    [SerializeField] GameObject audioSourcePrefab;

    static AudioManager instance;

    private class AudioSourceWrapper
    {
        public AudioSource Source;
        public int Priority;
    }

    private Queue<AudioSource> audioPool;
    private List<AudioSourceWrapper> activeSources;

    public void PlayShootingClip(float? pitchOverride = null)
    {
        PlaySound(projectile, pitch: pitchOverride);
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
        if (!CreateSingleton()) return;

        audioPool = new Queue<AudioSource>();
        activeSources = new List<AudioSourceWrapper>();
        InitializePool();
        VerifyPool();
    }

    bool CreateSingleton()
    {
        if (instance != null && instance != this)
        {
            Debug.Log("Removing duplicate AudioManager");
            gameObject.SetActive(false);
            Destroy(gameObject);
            return false;
        }
        Debug.Log("Creating AudioManager");
        instance = this;
        DontDestroyOnLoad(gameObject);
        return true;
    }

    void InitializePool()
    {
        if (audioSourcePrefab == null)
        {
            Debug.LogError("AudioManager: audioSourcePrefab is null (assign it in the inspector)");
            return;
        }

        // Initialize the pool
        for (int i = 0; i < maxCount; i++)
        {
            GameObject audioObject = Instantiate(audioSourcePrefab);
            audioObject.name = "Managed Audio (" + i + ")";
            AudioSource source = audioObject.GetComponent<AudioSource>();

            if (source == null)
            {
                Debug.LogError("AudioManager: AudioSource component is missing on " + audioObject.name);
                continue;
            }
            source.playOnAwake = false;

            Debug.Log("Created AudioSource: " + source.name + " on " + gameObject.name);

            audioObject.transform.SetParent(transform); // Make the AudioSource a child of the AudioManager
            audioPool.Enqueue(source);
        }
    }

    bool VerifyPool()
    {
        // check that each AudioSource in the pool is not null
        bool isValid = true;
        int counter = 0;
        if (audioPool == null)
        {
            Debug.LogError("AudioManager: audioPool is null");
            isValid = false;
        }
        if (isValid)
        {
            Debug.Log("Verifying " + audioPool.Count + " items in audioPool...");
            foreach (var source in audioPool)
            {
                if (source == null)
                {
                    Debug.LogError("AudioSource " + counter + " in pool is null");
                    isValid = false;
                    break;
                }
                else
                {
                    Debug.Log("AudioSource " + counter + " in pool is: " + source.name);
                }
                counter++;
            }
        }
        return isValid;
    }

    void PlaySound(AudioClipData clipData, float? pitch = null, float? volume = null)
    {
        if (clipData == null || clipData.audioClip == null)
        {
            Debug.LogWarning("PlaySound: Invalid AudioClipData or missing AudioClip.");
            return;
        }

        Debug.Log("PlaySound: Clip: " + clipData.audioClip);

        if (!VerifyPool()) return;
        Debug.Log("PlaySound: audioPool count: " + audioPool.Count);

        // If there is an available AudioSource in the pool
        if (audioPool.Count > 0)
        {
            UseAudioSource(clipData, pitchOverride: pitch, volumeOverride: volume);
        }
        else
        {
            // If the pool is full, find the lowest-priority active source
            var lowestPrioritySource = GetLowestPrioritySource();

            // If the new sound's priority is higher or equal, replace the lowest priority sound
            if (lowestPrioritySource != null && lowestPrioritySource.Priority <= clipData.priority)
            {
                ReplaceAudioSource(lowestPrioritySource, clipData, pitchOverride: pitch, volumeOverride: volume);
            }
            else
            {
                // No available source and the new clip's priority is not high enough
                Debug.LogWarning("Max audio source count (" + maxCount + ") reached, and no lower priority source found. Ignoring request.");
            }
        }
    }

    void UseAudioSource(AudioClipData clipData, float? pitchOverride, float? volumeOverride)
    {
        Debug.Log("Attempting to use AudioSource for " + clipData.audioClip);

        // Get an available AudioSource from the pool
        AudioSource source = audioPool.Dequeue();

        if (source == null)
        {
            Debug.LogError("Failed to get AudioSource from pool.");
            return;
        }

        Debug.Log("Using AudioSource: " + source.name);

        ConfigureAudioSource(source, clipData, pitchOverride, volumeOverride);
        source.Play();

        // Add to the active list with the associated priority
        var wrapper = new AudioSourceWrapper { Source = source, Priority = clipData.priority };
        activeSources.Add(wrapper);

        // Set a callback to release it after the clip ends
        StartCoroutine(ReturnToPoolAfterPlayback(wrapper, clipData.audioClip.length));
    }

    void ReplaceAudioSource(AudioSourceWrapper wrapper, AudioClipData clipData, float? pitchOverride, float? volumeOverride)
    {
        // Stop the current low-priority sound
        wrapper.Source.Stop();
        ConfigureAudioSource(wrapper.Source, clipData, pitchOverride, volumeOverride);
        wrapper.Priority = clipData.priority;
        wrapper.Source.Play();

        // Restart the coroutine to return it to the pool after the new clip finishes
        StopAllCoroutines(); // Stop previous coroutines for clean management
        StartCoroutine(ReturnToPoolAfterPlayback(wrapper, clipData.audioClip.length));
    }

    void ConfigureAudioSource(AudioSource source, AudioClipData clipData, float? pitchOverride, float? volumeOverride)
    {
        source.clip = clipData.audioClip;
        source.pitch = pitchOverride ?? clipData.pitch;
        source.volume = volumeOverride ?? clipData.volume;
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

        Debug.Log("Releasing AudioSource: " + wrapper.Source.name);

        audioPool.Enqueue(wrapper.Source);
    }
}
