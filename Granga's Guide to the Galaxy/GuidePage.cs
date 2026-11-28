using UnityEngine;

public class GuidePage : MonoBehaviour
{
    [Header("Machinery")]
    // Canvas group for this page.
    public CanvasGroup canvasGroup;

    // Is this page fading in?
    public bool fadingIn = false;

    // Is this page fading out?
    public bool fadingOut = true;

    void FixedUpdate()
    {
        // Fading in?
        if (fadingIn)
        {
            // Fade in.
            canvasGroup.alpha += Time.fixedDeltaTime;

            // Faded in?
            if (canvasGroup.alpha >= 1f)
                fadingIn = false;
        }

        // Fading out?
        if (fadingOut)
        {
            // Fade out.
            canvasGroup.alpha -= Time.fixedDeltaTime;

            // Faded out?
            if (canvasGroup.alpha <= 0f)
                fadingOut = false;
        }
    }

    // Fade in.
    public void FadeIn()
    {
        // Set bools.
        fadingIn = true;
        fadingOut = false;
    }

    // Fade out.
    public void FadeOut()
    {
        // Set bools.
        fadingIn = false;
        fadingOut = true;
    }
}
