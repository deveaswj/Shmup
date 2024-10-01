using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{

    [SerializeField] float moveSpeed = 5f;
    Vector2 rawInput;

    [SerializeField] Vector2 padding0 = new(0.5f, 2f);  // left, bottom
    [SerializeField] Vector2 padding1 = new(0.5f, 5f);  // right, top

    Vector2 minBounds;
    Vector2 maxBounds;

    Shooter shooter;

    void Awake()
    {
        shooter = GetComponent<Shooter>();
    }

    void Start()
    {
        InitializeBounds();
    }

    void InitializeBounds()
    {
        Camera mainCamera = Camera.main;
        minBounds = mainCamera.ViewportToWorldPoint(new Vector2(0, 0));
        maxBounds = mainCamera.ViewportToWorldPoint(new Vector2(1, 1));
    }

    void Update()
    {
        Move();
    }

    void Move()
    {
        Vector2 delta = moveSpeed * Time.deltaTime * rawInput;
        Vector2 newPos = new(transform.position.x + delta.x, transform.position.y + delta.y);
        newPos.x = Mathf.Clamp(newPos.x, minBounds.x + padding0.x, maxBounds.x - padding1.x);
        newPos.y = Mathf.Clamp(newPos.y, minBounds.y + padding0.y, maxBounds.y - padding1.y);
        transform.position = newPos;
    }

    void OnMove(InputValue value)
    {
        rawInput = value.Get<Vector2>();
    }

    void OnFire(InputValue value)
    {
        if (shooter != null)
        {
            shooter.SetFiring(value.isPressed);
        }
    }
    
    public void PowerUp(PowerUpType powerUpType)
    {
        switch (powerUpType)
        {
            case PowerUpType.Health:
                Debug.Log("Power up: Health");
                break;
            case PowerUpType.Shield:
                Debug.Log("Power up: Shield");
                break;
            case PowerUpType.Speed:
                Debug.Log("Power up: Speed");
                break;
            case PowerUpType.Weapon1:
                Debug.Log("Power up: Weapon 1");
                break;
            case PowerUpType.Weapon2:
                Debug.Log("Power up: Weapon 2");
                break;
            case PowerUpType.Weapon3:
                Debug.Log("Power up: Weapon 3");
                break;
            default:
                break;
        }
    }

}