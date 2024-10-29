using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioPlayer : MonoBehaviour
{
    [Header("Shooting")]
    [SerializeField] AudioClip shootingClip;
    [SerializeField] [Range(0f, 1f)] float shootingVolume = 1f;

    [Header("Damage")]
    [SerializeField] AudioClip damageClip;
    [SerializeField] [Range(0f, 1f)] float damageVolume = 1f;    

    [Header("Explosion")]
    [SerializeField] AudioClip explosionClip;
    [SerializeField] [Range(0f, 1f)] float explosionVolume = 1f;

    [Header("PowerUp")]
    [SerializeField] AudioClip powerUpClip;
    [SerializeField] [Range(0f, 1f)] float powerUpVolume = 1f;

    [Header("ShieldOn")]
    [SerializeField] AudioClip shieldOnClip;
    [SerializeField] [Range(0f, 1f)] float shieldOnVolume = 1f;

    [Header("ShieldOff")]
    [SerializeField] AudioClip shieldOffClip;
    [SerializeField] [Range(0f, 1f)] float shieldOffVolume = 1f;

    [Header("Booster")]
    [SerializeField] AudioClip boosterClip;
    [SerializeField] [Range(0f, 1f)] float boosterVolume = 1f;

    static AudioPlayer instance;

    void Awake()
    {
        // The *object* this is attached to needs to be a (pseudo-)singleton so that
        // its music (not handled by this script) doesn't get interrupted across scenes.
        ManageSingleton();
    }

    void ManageSingleton()
    {
        if (instance != null && instance != this)
        {
            gameObject.SetActive(false);
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    public void PlayShootingClip()
    {
        PlayClipAtVolume(shootingClip, shootingVolume);
    }

    public void PlayDamageClip()
    {
        PlayClipAtVolume(damageClip, damageVolume);
    }

    public void PlayExplosionClip()
    {
        PlayClipAtVolume(explosionClip, explosionVolume);
    }

    public void PlayPowerUpClip()
    {
        PlayClipAtVolume(powerUpClip, powerUpVolume);
    }

    public void PlayShieldOnClip()
    {
        PlayClipAtVolume(shieldOnClip, shieldOnVolume);
    }

    public void PlayShieldOffClip()
    {
        PlayClipAtVolume(shieldOffClip, shieldOffVolume);
    }

    public void PlayBoosterClip()
    {
        PlayClipAtVolume(boosterClip, boosterVolume);
    }

    void PlayClipAtVolume(AudioClip clip, float volume)
    {
        if (!clip) { return; }
        Vector3 cameraPosition = Camera.main.transform.position;
        AudioSource.PlayClipAtPoint(clip, cameraPosition, volume);
    }
}
