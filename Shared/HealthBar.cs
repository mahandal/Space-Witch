using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [Header("Machinery")]
    // The unit this health bar is attached to.
    public Unit unit;

    // The green bar representing how much health this unit has left.
    public Image greenBar;

    // A canvas group to scale opacity.
    public CanvasGroup canvasGroup;

    // Awaken!
    void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    void FixedUpdate()
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
}
