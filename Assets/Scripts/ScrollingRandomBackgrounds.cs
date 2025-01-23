using UnityEngine;
using System.Collections.Generic;

public class ScrollingRandomBackgrounds : MonoBehaviour
{
    [SerializeField] Transform[] backgrounds;
    [SerializeField] Sprite[] backgroundImages; // Drag and drop your 17 images here in the Inspector
    [SerializeField] float scrollSpeed = 5f;    // Speed of the background scrolling

    private float backgroundHeight;   // Height of each background in world units

    private List<SpriteRenderer> spriteRenderers = new();

    void Start()
    {
        foreach (Transform background in backgrounds)
        {
            SpriteRenderer sr = background.GetComponent<SpriteRenderer>();
            if (sr == null)
            {
                Debug.LogError("Background " + background.name + " does not have a SpriteRenderer component.");
                continue;
            }
            spriteRenderers.Add(sr);
        }

        // Get background height (based on SpriteRenderer bounds)
        backgroundHeight = spriteRenderers[0].bounds.size.y;

        Debug.Log("Background height: " + backgroundHeight);

        // Randomly assign background images
        foreach (Transform background in backgrounds)
        {
            AssignRandomSprite(background);
        }

        // Position backgrounds properly
        // position the second one dead center, the first above it, the third below it

        backgrounds[1].position = new Vector3(0, 0, 0);
        backgrounds[0].position = new Vector3(0, backgroundHeight, 0);
        backgrounds[2].position = new Vector3(0, -backgroundHeight, 0);

        // backgrounds[1].position = new Vector3(0, backgrounds[0].position.y + backgroundHeight, 0);
        // backgrounds[2].position = new Vector3(0, backgrounds[1].position.y + backgroundHeight, 0);

        foreach (Transform background in backgrounds)
        {
            Debug.Log("START Background position: " + background.position);
        }
    }

    void Update()
    {
        // Scroll all backgrounds down
        foreach (Transform background in backgrounds)
        {
            background.Translate(scrollSpeed * Time.deltaTime * Vector3.down);
        }

        // If any background has scrolled out of view, then reset
        for (int i = 0; i < backgrounds.Length; i++)
        {
            SpriteRenderer spriteRenderer = spriteRenderers[i];
            float topEdge = backgrounds[i].position.y + spriteRenderer.bounds.extents.y; // Calculate top edge
            if (topEdge < -Camera.main.orthographicSize) // Compare with bottom of the screen
            {
                RepositionAndRandomize(i);
            }
        }
    }

    private void RepositionAndRandomize(int backgroundIndex)
    {
        Debug.Log("RepositionAndRandomize(" + backgroundIndex + ")");

        // Get the next background in the cycle
        int nextIndex = (backgroundIndex + 1 + backgrounds.Length) % backgrounds.Length;

        Debug.Log("nextIndex: " + nextIndex);

        // Reposition the background above the next one
        backgrounds[backgroundIndex].position = new Vector3(0, backgrounds[nextIndex].position.y + backgroundHeight, 0);

        Debug.Log("new position: " + backgrounds[backgroundIndex].position);

        // Assign a new random sprite, ensuring no duplicates
        AssignRandomSprite(backgrounds[backgroundIndex], backgroundIndex);
    }

    private void AssignRandomSprite(Transform background, int backgroundIndex = 0)
    {
        // Find currently used sprites
        List<Sprite> availableSprites = new List<Sprite>(backgroundImages);
        foreach (SpriteRenderer sr in spriteRenderers)
        {
            Sprite currentSprite = sr.sprite;
            if (currentSprite != null)
            {
                availableSprites.Remove(currentSprite);
            }
        }

        // Pick a random sprite from the remaining available sprites
        Sprite randomSprite = availableSprites[Random.Range(0, availableSprites.Count)];
        SpriteRenderer bgsr = spriteRenderers[backgroundIndex];
        bgsr.sprite = randomSprite;

        // randomly flip the x and/or y axis
        bgsr.flipX = Random.Range(0, 2) == 0;
        bgsr.flipY = Random.Range(0, 2) == 0;

        // Resize sprite to fit the screen width
        ResizeSpriteToScreenWidth(bgsr);
    }

    private void ResizeSpriteToScreenWidth(SpriteRenderer sr)
    {
        float screenWidth = Camera.main.orthographicSize * 2.0f * Camera.main.aspect; // Calculate screen width in world units
        float spriteWidth = sr.sprite.bounds.size.x;

        // Scale the sprite proportionally to fit the screen width
        float scale = screenWidth / spriteWidth;
        sr.transform.localScale = new Vector3(scale, scale, 1);
    }
}
