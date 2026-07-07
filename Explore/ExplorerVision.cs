using UnityEngine;

public class ExplorerVision : MonoBehaviour
{
    [Header("Machinery")]
    // The explorer this vision is for.
    public Explorer explorer;

    // Vision circle.
    public SpriteRenderer visionCircle;

    // Attack circle.
    public SpriteRenderer attackCircle;

    void FixedUpdate()
    {
        // Check if the player is stealthing so we should show visuals.
        if (GM.I.player.isStealthing)
        {
            // Reveal vision circle.
            visionCircle.gameObject.SetActive(true);

            // Reveal attack circle for villains.
            if (!explorer.good)
                attackCircle.gameObject.SetActive(true);
        }
        else
        {
            // Hide vision and attack circles.
            visionCircle.gameObject.SetActive(false);
            attackCircle.gameObject.SetActive(false);
        }
    }

    // Called when another collider enters this collider.
    // void OnTriggerEnter2D(Collider2D col)
    // {
    //     // Get explorer.
    //     Explorer e = col.GetComponent<Explorer>();

    //     // Ignore non-explorers.
    //     if (e == null) return;

    //     // If it's the player, hint they can interact.
    //     if (e == GM.I.player)
    //         ExploreUI.I.HintInteract();
    // }

    // void OnTriggerExit2D(Collider2D col)
    // {
    //     // Get explorer.
    //     Explorer e = col.GetComponent<Explorer>();

    //     // Ignore non-explorers.
    //     if (e == null) return;

    //     // Check if it's the player.
    //     if (e == GM.I.player)
    //     {
    //         // Check if we're the closest explorer to the player.
    //         if (explorer == GM.I.player.NearestExplorer())
    //             ExploreUI.I.HideBottomHint();
    //     }
    // }
}
