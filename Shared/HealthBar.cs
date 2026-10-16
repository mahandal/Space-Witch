using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [Header("Manual Machinery")]
    // The unit this health bar is attached to.
    public Unit unit;

    // The green bar representing how much health this unit has left.
    // (red for evil)
    public Image greenBar;

    [Header("Automated Machinery")]
    // Canvas to set sorting order.
    public Canvas canvas;

    // A canvas group to scale opacity.
    public CanvasGroup canvasGroup;

    // Awaken!
    void Awake()
    {
        canvas = GetComponent<Canvas>();
        canvasGroup = GetComponent<CanvasGroup>();

        canvas.sortingOrder = 800;
    }

    void FixedUpdate()
    {
        // Get percent health.
        float percentHealth = unit.currentHealth / unit.maxHealth;

        // Set fill.
        greenBar.fillAmount = percentHealth;

        // Set color.
        if (unit.role == "Treasure")
            greenBar.color = new Color (1f, 0f, 1f, 0.5f);
        else if (unit.good)
            greenBar.color = new Color (0f, 1f, 0f, 0.5f);
        else
            greenBar.color = new Color (1f, 0f, 0f, 0.5f);

        // Battle mode
        if (DM.I.gameObject.activeSelf)
        {
            // Death?
            if (unit.currentHealth <= 0 || unit.state == -1)
            {
                // Don't destroy dragon statue health bars.
                if (unit.myName == "Dragon Statue") return;

                // Destroy the health bar.
                Destroy(gameObject);
            }
        }
            

        // Deploying?
        if (unit.state == 0 && unit.deployTimer > 0f)
        {
            // Check if we are halfway deployed yet.
            float percentDeployed = 1f - (unit.deployTimer / unit.deployTime);

            if (percentDeployed >= 0.5f)
                canvasGroup.alpha = percentDeployed;
            else
                canvasGroup.alpha = 0f;
        } else {
            // Explore
            if (GM.I.gameObject.activeSelf)
            {
                // Fade full health bars.
                if (percentHealth >= 1f)
                {
                    if (canvasGroup.alpha > 0f)
                        canvasGroup.alpha -= 0.01f;
                }
                else
                {
                    // Always show health bars for injured units.
                    canvasGroup.alpha = 1f;
                }
            }
        }
    }
}
