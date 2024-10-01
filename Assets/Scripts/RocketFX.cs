using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.InputSystem;

// For toggling visibility of rocket fire FX objects.

public class RocketFX : MonoBehaviour
{
    [SerializeField] List<GameObject> fxObjects = new();
    // [SerializeField] float fadeTime = 0.5f;

    List<SpriteRenderer> spriteRenderers = new();
    Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        foreach (GameObject fxObject in fxObjects)
        {
            spriteRenderers.Add(fxObject.GetComponent<SpriteRenderer>());
        }
    }

    void Update()
    {
        // if the object's position has changed since the last update, show, otherwise hide
        Toggle(rb.IsAwake());
    }

    void Toggle(bool show)
    {
        foreach (SpriteRenderer sr in spriteRenderers)
        {
            sr.enabled = show;
        }
    }
}
