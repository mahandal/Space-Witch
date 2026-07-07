using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [Header("Manual Machinery")]
    // The unit this health bar is attached to, for battle mode.
    public Unit unit;

    // The explorer this health bar is attached to, for explore mode.
    public Explorer explorer;

    // The green bar representing how much health this unit has left.
    public Image greenBar;

    [Header("Automated Machinery")]
    // A canvas group to scale opacity.
    public CanvasGroup canvasGroup;

    // Awaken!
    void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    void FixedUpdate()
    {
        if (unit != null)
            Battle();
        if (explorer != null)
            Explore();
    }

    void Battle()
    {
        // Deploying?
        if (unit.state == 0)
        {
            // Check if we are halfway deployed yet.
            float percentDeployed = 1f - (unit.deployTimer / unit.deployTime);
            if (percentDeployed < 0.5f)
            {
                // Hide.
                canvasGroup.alpha = 0f;
            } else {
                // Fade in.
                canvasGroup.alpha = percentDeployed;
            }
        }

        // Get percent health.
        float percentHealth = unit.currentHealth / unit.maxHealth;

        // Set fill.
        greenBar.fillAmount = percentHealth;

        // Death?
        if (unit.currentHealth <= 0 || unit.state == -1)
        {
            // Don't destroy dragon statue health bars.
            if (unit.myName == "Dragon Statue") return;

            // Destroy the health bar.
            Destroy(gameObject);
        }
    }

    void Explore()
    {
        // Get percent health.
        float percentHealth = explorer.currentHealth / explorer.maxHealth;

        // Set fill.
        greenBar.fillAmount = percentHealth;

        // If full health hide.
        // Reveal when below full.
        if (explorer.currentHealth < explorer.maxHealth)
            canvasGroup.alpha = 1f;
        else
            canvasGroup.alpha = 0f;
    }
}
