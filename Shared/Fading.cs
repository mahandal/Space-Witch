using UnityEngine;

// Simple class to handle fading and movement for basic UI elements e.g. mana popups from gathering violet flowers.
public class Fading : MonoBehaviour
{
    [Header("Speed")]
    // How quickly to move.
    public float speed = 5f;

    [Header("Rotation")]
    // How much to rotate.
    public Vector3 rotation;

    [Header("Duration")]
    // How long to last.
    public float duration = 3f;

    [Header("Destination")]
    public Vector3 destination;

    [Header("Rising Text Machinery")]
    // Timer tracking how we've existed.
    public float timeExtant = 0f;

    // Sprite renderer to fade alpha.
    public SpriteRenderer spriteRenderer;

    // Singleton.
    public static Fading I;

    // + Birth
    // Awaken!
    void Awake()
    {
        // Singleton.
        if (I == null)
        {
            // Set singleton.
            I = this;

            // Disable.
            gameObject.SetActive(false);
        }

        // Get sprite renderer.
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Create a new one.
    // For now always creates a mana symbol. Can scale out if needed!
    public static Fading Create(Vector3 position, float _speed = 1f, float _duration = 3f)
    {
        // Initialize new fading object.
        Fading newFade;

        // Create new object as a child of either DM or GM.
        // (Check which is active to use)
        if (DM.I.gameObject.activeSelf)
            newFade = Object.Instantiate(I, DM.I.transform);
        else if (GM.I.gameObject.activeSelf)
            newFade = Object.Instantiate(I, GM.I.transform);
        else
            return null;


        // Move into position.
        newFade.transform.position = position;

        // Set speed.
        newFade.speed = _speed;

        // Set duration.
        newFade.duration = _duration;

        // Activate!
        newFade.gameObject.SetActive(true);

        // Return.
        return newFade;
    }

    // + Life
    // Fixed update!
    void FixedUpdate()
    {
        // Pop when game is over.
        if (MenuManager.I.gameState == 21)
        {
            Destroy(gameObject);
            return;
        }

        // Track time alive.
        timeExtant += Time.fixedDeltaTime;

        // Move.
        transform.position -= (transform.position - destination).normalized * speed * Time.fixedDeltaTime;

        // Rotate.
        transform.eulerAngles += rotation * Time.fixedDeltaTime;

        // Shrink.
        transform.localScale *= 0.99f;

        // Set opacity.
        float percentRemaining = 1f - (timeExtant / duration);
        spriteRenderer.color = new Color(1f, 1f, 1f, percentRemaining);

        // Duration.
        if (timeExtant >= duration)
        {
            // Pop goes the weasel!
            Destroy(gameObject);
        }
    }
}
