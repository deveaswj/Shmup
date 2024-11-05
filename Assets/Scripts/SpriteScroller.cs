using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteScroller : MonoBehaviour
{
    [SerializeField] Vector2 moveSpeed;
    [SerializeField] float xVariance;
    float xTargetSpeed, xCurrentSpeed;
    float timeBetweenXShifts = 90.0f;
    float timeSinceLastShift = 0.0f;
    Vector2 offset;
    Material material;

    void Awake()
    {
        material = GetComponent<SpriteRenderer>().material;
        xTargetSpeed = Mathf.Abs(moveSpeed.x);
        xCurrentSpeed = xTargetSpeed;
    }

    void Update()
    {
        if (xVariance != 0.0f)
        {
            // for the offset speed, we want the Y to remain constant, and the X to slowly lerp toward a new value every timeBetweenXShifts
            timeSinceLastShift += Time.deltaTime;
            if (timeSinceLastShift > timeBetweenXShifts)
            {
                timeSinceLastShift = 0.0f;
                // pick a new target X speed ranging from -xVariance to xVariance
                xTargetSpeed = Random.Range(-xVariance, xVariance);
            }
            // lerp from the current X speed to the target X speed
            xCurrentSpeed = Mathf.Lerp(xCurrentSpeed, xTargetSpeed, Time.deltaTime * 10.0f);
            moveSpeed = new Vector2(xCurrentSpeed, moveSpeed.y);
        }
        offset = moveSpeed * Time.deltaTime;
        material.mainTextureOffset += offset;
    }
}
