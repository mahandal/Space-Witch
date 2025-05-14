using UnityEngine;
using TMPro;

public class HitMarker : MonoBehaviour
{
    [Header("Configuration")]
    public float value = 1f;
    public float lifetime = 1.0f;
    public float moveSpeed = 2.0f;
    public float fadeStartTime = 0.5f;
    
    [Header("Manual Machinery")]
    public TMP_Text text;
    
    private float timeAlive = 0;
    private Color originalColor;
    
    void Awake()
    {
        originalColor = text.color;
    }
    
    void Update()
    {
        // Move upward
        transform.position += Vector3.up * moveSpeed * Time.deltaTime;
        
        // Track time
        timeAlive += Time.deltaTime;
        
        // Fade out
        if (timeAlive > fadeStartTime)
        {
            float alphaPercentage = 1 - ((timeAlive - fadeStartTime) / (lifetime - fadeStartTime));
            text.color = new Color(originalColor.r, originalColor.g, originalColor.b, alphaPercentage);
        }
        
        // Destroy when lifetime is over
        if (timeAlive >= lifetime)
        {
            Destroy(gameObject);
        }
    }

    // Create a new hit marker at the given position with the given value.
    public static HitMarker CreateHitMarker(Vector3 position, float value)
    {
        // Make a new hit marker
        HitMarker newHitMarker = Object.Instantiate(GM.I.spawnManager.progenitor_HitMarker);

        // Set its position
        newHitMarker.transform.position = position;

        // Set its value
        newHitMarker.value = value;

        // Get its scaled value for later use
        float scaledValue = value;
        if (scaledValue > 5f)
            scaledValue = 5 + Mathf.Log(value - 5, 2);

        // Set its text
        newHitMarker.text.text = "+" + value.ToString("0");
        newHitMarker.text.color = new Color(0.69f, 0.17f, 0.58f);

        // Set its size
        /*
        float size = value;
        if (size > 5f)
            size = 5 + Mathf.Log(value - 5, 2);
        */
        newHitMarker.transform.localScale = new Vector3(scaledValue, scaledValue, 1);

        // Set its lifetime
        newHitMarker.lifetime = value;

        // Set its move speed
        newHitMarker.moveSpeed = scaledValue;

        // Activate
        newHitMarker.gameObject.SetActive(true);

        return newHitMarker;
    }

    // Create a new hit marker at the given position with the given text and a default value of 5.
    public static HitMarker CreateHitMarker(Vector3 position, string text, float value = 5f)
    {
        // Make a new hit marker
        HitMarker newHitMarker = Object.Instantiate(GM.I.spawnManager.progenitor_HitMarker);

        // Set its position
        newHitMarker.transform.position = position;

        // Set its value
        newHitMarker.value = value;

        // Get its scaled value for later use
        float scaledValue = value;
        if (scaledValue > 5f)
            scaledValue = 5 + Mathf.Log(value - 5, 2);

        // Set its text
        newHitMarker.text.text = text;
        newHitMarker.text.color = new Color(0.77f, 0.81f, 0.1f);

        // Set its size
        /*
        float size = value;
        if (size > 5f)
            size = 5 + Mathf.Log(value - 5, 2);
        */
        newHitMarker.transform.localScale = new Vector3(scaledValue, scaledValue, 1);

        // Set its lifetime
        newHitMarker.lifetime = value;

        // Set its move speed
        newHitMarker.moveSpeed = scaledValue;

        // Activate
        newHitMarker.gameObject.SetActive(true);

        return newHitMarker;
    }
    
    public static HitMarker CreateDamageMarker(Vector3 position, float damage)
    {
        // Make a new hit marker
        HitMarker newHitMarker = Object.Instantiate(GM.I.spawnManager.progenitor_HitMarker);

        // Set its position
        newHitMarker.transform.position = position;

        // Set its value
        newHitMarker.value = damage;

        // Set its text
        newHitMarker.text.text = "-" + damage.ToString("0");

        // Set its color to red
        newHitMarker.text.color = Color.red;

        // Set its size
        float size = Mathf.Min(damage / 10f, 3f);
        size = Mathf.Max(size, 1f); // Minimum size
        newHitMarker.transform.localScale = new Vector3(size, size, 1);

        // Set its lifetime
        newHitMarker.lifetime = 1.5f;

        // Set its move speed
        newHitMarker.moveSpeed = 2f;

        // Activate
        newHitMarker.gameObject.SetActive(true);

        return newHitMarker;
    }

    // For learning new talents
    public static HitMarker CreateLearnMarker(Vector3 position, string text)
    {
        HitMarker newHitMarker = Object.Instantiate(GM.I.spawnManager.progenitor_HitMarker);
        
        // Position and properties
        newHitMarker.transform.position = position;
        newHitMarker.value = 5f;
        
        // Set text
        newHitMarker.text.text = text;

        // Color(?)
        //newHitMarker.text.color = new Color(1f, 0.3f, 0.85f); // Vibrant purple
        Color c = new Color(GM.I.ui.originalXPColor.g, GM.I.ui.originalXPColor.b, GM.I.ui.originalXPColor.r);
        newHitMarker.text.color = c;
        
        // Slightly larger than normal
        newHitMarker.transform.localScale = new Vector3(1.2f, 1.2f, 1f);
        
        // Faster movement speed, shorter lifetime
        newHitMarker.lifetime = 3f;
        newHitMarker.moveSpeed = 3.5f;
        
        // Move at a slight angle instead of straight up
        //newHitMarker.movementAngle = 15f; // Add this field to HitMarker class
        
        // Activate
        newHitMarker.gameObject.SetActive(true);
        
        return newHitMarker;
    }

    // For unlocking new songs
    public static HitMarker CreateSongMarker(Vector3 position, string text)
    {
        HitMarker newHitMarker = Object.Instantiate(GM.I.spawnManager.progenitor_HitMarker);
        
        // Position and properties
        newHitMarker.transform.position = position;
        newHitMarker.value = 5f;
        
        // Text
        newHitMarker.text.text = text;
        newHitMarker.text.color = new Color(0.2f, 0.4f, 0.8f);
        
        // Add music icon or symbol if possible - we can modify TMPro text
        string musicSymbol = "♪: ";
        newHitMarker.text.text = musicSymbol + text;
        
        // Slightly larger than normal?
        newHitMarker.transform.localScale = new Vector3(2.1f, 2.1f, 1f);
        
        // Slower movement, longer lifetime
        newHitMarker.lifetime = 4f;
        newHitMarker.moveSpeed = 1.5f;
        
        // Move at opposite angle to other markers
        //newHitMarker.movementAngle = -10f;
        
        // Wave-like motion
        //newHitMarker.useWaveMotion = true; // Add this field to HitMarker class
        //newHitMarker.waveFrequency = 2f;   // Add this field to HitMarker class
        //newHitMarker.waveAmplitude = 0.3f; // Add this field to HitMarker class
        
        // Activate
        newHitMarker.gameObject.SetActive(true);
        
        return newHitMarker;
    }

    // For battle/combat messages (evasion, etc.)
    public static HitMarker CreateCombatMarker(Vector3 position, string text)
    {
        HitMarker newHitMarker = Object.Instantiate(GM.I.spawnManager.progenitor_HitMarker);
        
        // Position and properties
        newHitMarker.transform.position = position;
        newHitMarker.value = 4f;
        
        // Orange for combat text
        newHitMarker.text.text = text;
        newHitMarker.text.color = new Color(1f, 0.6f, 0.2f); // Orange
        
        // Slightly different size
        newHitMarker.transform.localScale = new Vector3(0.9f, 0.9f, 1f);
        
        // Fast movement, short lifetime
        newHitMarker.lifetime = 2.5f;
        newHitMarker.moveSpeed = 4f;
        
        // More dramatic fading
        newHitMarker.fadeStartTime = 0.3f;
        
        // Activate
        newHitMarker.gameObject.SetActive(true);
        
        return newHitMarker;
    }

    public static HitMarker CreateHealMarker(Vector3 position, float healAmount)
    {
        // Make a new hit marker
        HitMarker newHitMarker = Object.Instantiate(GM.I.spawnManager.progenitor_HitMarker);

        // Position and value setup
        newHitMarker.transform.position = position;
        newHitMarker.value = healAmount;
        
        // Set text
        newHitMarker.text.text = "+" + healAmount.ToString("0");
        newHitMarker.text.color = new Color(0.2f, 0.9f, 0.3f); // Bright green
        
        // Size based on heal amount
        float size = Mathf.Min(healAmount / 10f, 3f);
        size = Mathf.Max(size, 1f); // Minimum size
        newHitMarker.transform.localScale = new Vector3(size, size, 1);
        
        // Different animation properties
        newHitMarker.lifetime = 2f;
        newHitMarker.moveSpeed = 1.5f; // Slower, more floaty movement
        
        // Activate
        newHitMarker.gameObject.SetActive(true);
        
        return newHitMarker;
    }

    public static HitMarker CreateNarrativeMarker(Vector3 position, string text)
    {
        HitMarker newHitMarker = Object.Instantiate(GM.I.spawnManager.progenitor_HitMarker);
        
        // Position and properties
        newHitMarker.transform.position = position;
        newHitMarker.value = 3f;
        
        // Text setup
        newHitMarker.text.text = text;
        newHitMarker.text.color = new Color(1f, 1f, 1f); // Pure white for narrative text
        
        // Distinctive size and movement
        newHitMarker.transform.localScale = new Vector3(1.5f, 1.5f, 1f);
        
        // Slower movement, longer lifetime
        newHitMarker.lifetime = 13f;
        newHitMarker.moveSpeed = -0.7f;
        
        // Activate
        newHitMarker.gameObject.SetActive(true);
        
        return newHitMarker;
    }
}