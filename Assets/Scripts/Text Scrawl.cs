using UnityEngine;
using TMPro;

public class TextScrawl : MonoBehaviour
{
    public float moveSpeed = 21f;

    [Header("Black Hole Effect")]
    private bool isFading = false;
    private TMP_Text text;
    private Color originalColor;

    void Start()
    {
        text = GetComponent<TMP_Text>();
        if (text != null)
            originalColor = text.color;
    }

    void FixedUpdate()
    {
        // Check if black hole effect should start
        if (!isFading && MainMenu.I.introPlaying)
        {
            float timeRemaining = MainMenu.I.introDuration - MainMenu.I.introTimer;
            if (timeRemaining <= MainMenu.I.blackHoleStartTime) // Your black hole start time
            {
                isFading = true;
            }
        }

        
        
        if (isFading)
        {
            FadeOut();
        }

        // Move on up!
        transform.position += Vector3.up * moveSpeed * Time.deltaTime;
    }

    void FadeOut()
    {
        if (text != null && MainMenu.I != null)
        {
            float timeRemaining = MainMenu.I.introDuration - MainMenu.I.introTimer;
            float fadeProgress = 1f - (timeRemaining / 13f); // 0 to 1 over 13 seconds
            
            Color currentColor = originalColor;
            currentColor.a = originalColor.a * (1f - fadeProgress); // Fade from original alpha to 0
            text.color = currentColor;
            
            if (fadeProgress >= 1f)
                gameObject.SetActive(false);
        }
    }
}
