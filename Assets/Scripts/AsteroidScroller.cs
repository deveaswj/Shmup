using Unity.VisualScripting;
using UnityEngine;

public class AsteroidScroller : MonoBehaviour
{
    [System.Serializable]
    public class ParallaxLayer
    {
        public GameObject[] sprites; // Array of sprites for this layer
        public float speedMultiplier = 1f; // Scrolling speed relative to base speed
    }

    [SerializeField] ParallaxLayer[] layers; // Array of parallax layers
    [SerializeField] float baseSpeed = 1f; // Base scrolling speed for all layers
    [SerializeField] Camera mainCamera;

    private Vector2 screenBounds; // Store screen bounds to wrap sprites
    Vector3 v3down = Vector3.down;

    class SpriteCache
    {
        public SpriteRenderer[] spriteRenderers;
        public Transform[] transforms;
        public int Length => spriteRenderers.Length;
        public float speedMultiplier;
    }
    SpriteCache[] layerSpriteData;

// have to have an array of arrays for layers and then sprites
//    private SpriteRenderer[] spriteRenderers;
//    private Transform[] spriteTransforms;

    void Start()
    {
        // Calculate screen bounds based on camera size
        screenBounds = new Vector2(mainCamera.orthographicSize * mainCamera.aspect, mainCamera.orthographicSize);

        // For each layer, populate its layerSpriteData
        // This caches the sprite renderers and transforms for each layer
        // so that we won't have to get them every frame
        layerSpriteData = new SpriteCache[layers.Length];
        for (int i = 0; i < layers.Length; i++)
        {
            int numSprites = layers[i].sprites.Length;
            layerSpriteData[i] = new SpriteCache
            {
                speedMultiplier = layers[i].speedMultiplier,
                spriteRenderers = new SpriteRenderer[numSprites],
                transforms = new Transform[numSprites]
            };
            for (int j = 0; j < layers[i].sprites.Length; j++)
            {
                SpriteRenderer sr = layers[i].sprites[j].GetComponent<SpriteRenderer>();
                layerSpriteData[i].spriteRenderers[j] = sr;
                Transform tr = layers[i].sprites[j].GetComponent<Transform>();
                layerSpriteData[i].transforms[j] = tr;
            }
        }
    }

    void Update()
    {
        for (int i = 0; i < layers.Length; i++)
        {
            ScrollLayer(layerSpriteData[i]);
        }
    }

    void ScrollLayer(SpriteCache spriteData)
    {
        float speed = baseSpeed * spriteData.speedMultiplier;
        Vector3 movement = speed * Time.deltaTime * v3down;

        for (int i = 0; i < spriteData.Length; i++)
        {
            SpriteRenderer sr = spriteData.spriteRenderers[i];
            Transform tr = spriteData.transforms[i];
            float sry = sr.bounds.size.y;

            // Move the sprite downward at the appropriate speed
            tr.Translate(movement, Space.World);

            // Check if the sprite has moved off the bottom of the screen
            if (tr.position.y < -screenBounds.y - sry / 2)
            {
                // Reposition the sprite to the top for a seamless loop
                Vector3 newPosition = tr.position;
                newPosition.y = screenBounds.y + sry / 2;
                tr.position = newPosition;
            }

        }
    }
}
