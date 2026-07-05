using UnityEngine;

// Terrain for Explore mode.
// See Tile.cs for Battle mode.
public class Region : MonoBehaviour
{
    [Header("Terrain")]
    // What type of region is this?
    // Types of region:
    // End = Where the big battle begins.
    // Water = Slow field
    public string myType;

    [Header("Machinery")]
    // A sprite renderer to show this region visually.
    // Automatically hidden, so players should not see!
    public SpriteRenderer spriteRenderer;

    // + Initialization
    void Awake()
    {
        // Hide sprite renderer.
        if (spriteRenderer != null)
            spriteRenderer.enabled = false;
    }

    // Called when another collider enters this collider.
    void OnTriggerEnter2D(Collider2D col)
    {
        // Get explorer.
        Explorer explorer = col.GetComponent<Explorer>();

        // Ignore non-explorers.
        if (explorer == null) return;

        // End?
        if (myType == "End" && explorer == GM.I.player)
            DM.I.BeginBattle();

        // Water?
        if (myType == "Water")
            explorer.speedModifiers["Water"] = 0.5f;
    }

    void OnTriggerExit2D(Collider2D col)
    {
        // Get explorer.
        Explorer explorer = col.GetComponent<Explorer>();

        // Ignore non-explorers.
        if (explorer == null) return;

        // Water?
        if (myType == "Water")
            explorer.speedModifiers.Remove("Water");
    }
}