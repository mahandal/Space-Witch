using UnityEngine;

public class ExplorerVision : MonoBehaviour
{
    // Called when another collider enters this collider.
    void OnTriggerEnter2D(Collider2D col)
    {
        // Get explorer.
        Explorer explorer = col.GetComponent<Explorer>();

        // Ignore non-explorers.
        if (explorer == null) return;

        // If it's the player, hint they can interact.
        if (explorer == GM.I.player)
            ExploreUI.I.HintInteract();
    }

    void OnTriggerExit2D(Collider2D col)
    {
        // Get explorer.
        Explorer explorer = col.GetComponent<Explorer>();

        // Ignore non-explorers.
        if (explorer == null) return;

        // If it's the player, hint they can interact.
        if (explorer == GM.I.player)
            ExploreUI.I.HideBottomHint();
    }
}
