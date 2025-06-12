using UnityEngine;

public class ParallaxBackground : MonoBehaviour
{
    [Header("Background Layers")]
    public Transform[] backgroundLayers;
    public float[] parallaxEffectMultipliers;  // Different speeds for each layer

    [Header("References")]
    public Transform cameraTransform;  // Reference to your main camera

    private Vector3 lastCameraPosition;
    private Vector3 originalPosition;

    void Start()
    {
        // Get reference to camera if not set
        // if (cameraTransform == null)
        //     cameraTransform = Camera.main.transform;
            
        lastCameraPosition = cameraTransform.position;
        originalPosition = transform.position;
    }

    //void LateUpdate()
    void FixedUpdate()
    {
        // Calculate camera movement
        Vector3 cameraMovement = (cameraTransform.position - lastCameraPosition);
        //Vector3 cameraMovement = (cameraTransform.position - lastCameraPosition) * Time.fixedDeltaTime;

        // Move each layer based on parallax effect
        for (int i = 0; i < backgroundLayers.Length; i++)
        {
            // Move layer in opposite direction of camera movement
            // Multiply by parallax effect (smaller value = slower movement)
            Vector3 parallaxMovement = cameraMovement * parallaxEffectMultipliers[i];
            
            // Apply movement to the background layer
            backgroundLayers[i].position += parallaxMovement;
            
            // Optional: If you want infinite scrolling backgrounds
            // Note: Never got this to work...
            //WrapBackground(backgroundLayers[i]);
        }

        // Update last camera position
        lastCameraPosition = cameraTransform.position;
    }
    
    // Optional - for infinite scrolling backgrounds
    void WrapBackground(Transform layer)
    {
        // Get the renderer from the layer
        SpriteRenderer renderer = layer.GetComponent<SpriteRenderer>();
        if (renderer == null) return;
        
        // Calculate bounds
        float spriteWidth = renderer.bounds.size.x;
        float spriteHeight = renderer.bounds.size.y;
        
        // Get camera view boundaries
        float cameraHalfWidth = Camera.main.orthographicSize * Camera.main.aspect;
        float cameraHalfHeight = Camera.main.orthographicSize;
        
        // Calculate visible boundaries
        float visibleRight = cameraTransform.position.x + cameraHalfWidth;
        float visibleLeft = cameraTransform.position.x - cameraHalfWidth;
        float visibleTop = cameraTransform.position.y + cameraHalfHeight;
        float visibleBottom = cameraTransform.position.y - cameraHalfHeight;
        
        // Wrap horizontally if needed
        if (layer.position.x + spriteWidth < visibleLeft)
            layer.position = new Vector3(layer.position.x + spriteWidth * 2, layer.position.y, layer.position.z);
        else if (layer.position.x - spriteWidth > visibleRight)
            layer.position = new Vector3(layer.position.x - spriteWidth * 2, layer.position.y, layer.position.z);
            
        // Wrap vertically if needed
        if (layer.position.y + spriteHeight < visibleBottom)
            layer.position = new Vector3(layer.position.x, layer.position.y + spriteHeight * 2, layer.position.z);
        else if (layer.position.y - spriteHeight > visibleTop)
            layer.position = new Vector3(layer.position.x, layer.position.y - spriteHeight * 2, layer.position.z);
    }
}