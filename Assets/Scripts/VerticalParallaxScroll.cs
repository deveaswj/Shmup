using UnityEngine;

public class VerticalParallaxScroll : MonoBehaviour
{
    [System.Serializable]
    public class BackgroundData
    {
        public GameObject background;   // a background object in the scene
        public float speed;             // the speed at which the background moves
    }

    [SerializeField] BackgroundData[] backgroundData;

    private Camera mainCamera; // Reference to the main camera
    private GameObject[,] backgroundPairs; // Stores original and clone for each background
    private float[] backgroundHeights; // Stores the height of each background
    private float viewHeight; // Height of the camera view
    int count = 0;

    void Start()
    {
        count = backgroundData.Length;
        mainCamera = Camera.main;
        viewHeight = 2f * mainCamera.orthographicSize; // Get the camera's vertical view size

        backgroundPairs = new GameObject[count, 2];
        backgroundHeights = new float[count];

        // Clone and position backgrounds
        for (int i = 0; i < count; i++)
        {
            GameObject bg = backgroundData[i].background;

            // Store the original background from the scene
            backgroundPairs[i, 0] = bg;

            // Get the SpriteRenderer component and calculate the full height of the sprite
            SpriteRenderer spriteRenderer = backgroundPairs[i, 0].GetComponent<SpriteRenderer>();
            backgroundHeights[i] = spriteRenderer.bounds.size.y;

            // Instantiate the clone directly above the original
            backgroundPairs[i, 1] = Instantiate(bg, bg.transform.position + Vector3.up * backgroundHeights[i], Quaternion.identity, transform);
        }
    }

    void Update()
    {
        for (int i = 0; i < count; i++)
        {
            Transform originalTransform = backgroundPairs[i, 0].transform;
            Transform cloneTransform = backgroundPairs[i, 1].transform;
            float speed = backgroundData[i].speed;
            float height = backgroundHeights[i];

            // Move both original and clone downwards based on their speed
            originalTransform.Translate(speed * Time.deltaTime * Vector3.down);
            cloneTransform.Translate(speed * Time.deltaTime * Vector3.down);

            // Calculate the point at which the background should be repositioned
            float repositionThreshold = -(height / 2 + viewHeight / 2); // Adjust for the background being larger than the camera view

            // Reposition the original if it moves off-screen
            if (originalTransform.position.y < repositionThreshold)
            {
                originalTransform.position = cloneTransform.position + Vector3.up * height;
            }

            // Reposition the clone if it moves off-screen
            if (cloneTransform.position.y < repositionThreshold)
            {
                cloneTransform.position = originalTransform.position + Vector3.up * height;
            }
        }
    }
}
