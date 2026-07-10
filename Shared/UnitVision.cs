using UnityEngine;

public class UnitVision : MonoBehaviour
{
    [Header("Machinery")]
    // The unit this vision is for.
    public Unit unit;

    // Vision circle.
    public SpriteRenderer visionCircle;

    // Attack circle.
    public SpriteRenderer attackCircle;

    void Awake()
    {
        // Start hidden(?)
        visionCircle.gameObject.SetActive(false);
        attackCircle.gameObject.SetActive(false);
    }

    void FixedUpdate()
    {
        // Check if the player is stealthing so we should show visuals.
        if (GM.I.player.isStealthing)
        {
            // Reveal vision circle.
            visionCircle.gameObject.SetActive(true);

            // Reveal attack circle for villains.
            if (!unit.good)
                attackCircle.gameObject.SetActive(true);
        }
        else
        {
            // Hide vision and attack circles.
            visionCircle.gameObject.SetActive(false);
            attackCircle.gameObject.SetActive(false);
        }
    }
}
