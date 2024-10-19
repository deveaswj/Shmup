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

    void PlayClipAtVolume(AudioClip clip, float volume)
    {
        if (!clip) { return; }
        Vector3 cameraPosition = Camera.main.transform.position;
        AudioSource.PlayClipAtPoint(clip, cameraPosition, volume);
    }
}
