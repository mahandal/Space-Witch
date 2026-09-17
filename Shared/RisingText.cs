using UnityEngine;

public class RisingText : MonoBehaviour
{
    [Header("Rising Text")]
    // How quickly the text goes up.
    public float speed = 5f;

    // How long the text lasts.
    public float duration = 3f;

    // Timer tracking how long the text has existed.
    public float timeExtant = 0f;

    [Header("Rising Text Machinery")]
    // Sprite renderer to fade alpha.
    public SpriteRenderer spriteRenderer;

    // Singleton.
    public static RisingText I;

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

    // Create a new rising text.
    public static RisingText Create(Vector3 position, float _speed = 1f, float _duration = 3f)
    {
        // Initialize new rising text object.
        RisingText newRise;

        // Create new object as a child of our UI.
        // (Check which UI to use)
        if (DM.I.gameObject.activeSelf)
            newRise = Object.Instantiate(I, UI.I.transform);
        else if (GM.I.gameObject.activeSelf)
            newRise = Object.Instantiate(I, ExploreUI.I.transform);
        else
            return null;


        // Move into position.
        newRise.transform.position = position;

        // Set speed.
        newRise.speed = _speed;

        // Set duration.
        newRise.duration = _duration;

        // Activate!
        newRise.gameObject.SetActive(true);

        // Return.
        return newRise;
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

        // Rise.
        transform.position += Vector3.up * speed * Time.fixedDeltaTime;

        // Track time alive.
        timeExtant += Time.fixedDeltaTime;

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
