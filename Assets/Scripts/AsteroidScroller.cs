using UnityEngine;

public class AsteroidScroller : MonoBehaviour
{
    [System.Serializable]
    public class ParallaxLayer
    {
        public GameObject[] sprites; // Array of sprites for this layer
        public float speedMultiplier = 1f; // Scrolling speed relative to base speed
    }

    public ParallaxLayer[] layers; // Array of parallax layers
    public float baseSpeed = 1f; // Base scrolling speed for all layers

    [SerializeField] Camera mainCamera;

    private Vector2 screenBounds; // Store screen bounds to wrap sprites

    void Start()
    {
        // Calculate screen bounds based on camera size
        screenBounds = new Vector2(mainCamera.orthographicSize * mainCamera.aspect, mainCamera.orthographicSize);
    }

    void Update()
    {
        foreach (ParallaxLayer layer in layers)
        {
            ScrollLayer(layer);
        }
    }

    void ScrollLayer(ParallaxLayer layer)
    {
        foreach (GameObject sprite in layer.sprites)
        {
            // Move the sprite downward at the appropriate speed
            sprite.transform.Translate(baseSpeed * layer.speedMultiplier * Time.deltaTime * Vector3.down, Space.World);

            // Check if the sprite has moved off the bottom of the screen
            if (sprite.transform.position.y < -screenBounds.y - sprite.GetComponent<SpriteRenderer>().bounds.size.y / 2)
            {
                // Reposition the sprite to the top for a seamless loop
                Vector3 newPosition = sprite.transform.position;
                newPosition.y = screenBounds.y + sprite.GetComponent<SpriteRenderer>().bounds.size.y / 2;
                sprite.transform.position = newPosition;
            }
        }
    }
}
