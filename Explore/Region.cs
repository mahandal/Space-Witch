using UnityEngine;

// Terrain for Explore mode.
// See Tile.cs for Battle mode.
public class Region : MonoBehaviour
{
    [Header("Terrain")]
    // What type of region is this?
    // Types of region:
    // Plains = Default.
    // Water = Slow field.
    // Flora = Slow field.
    // Battle = The path of battle.
    // Harmony = The path of harmony.
    public string myType;

    // How likely is this region to spawn?
    // Set to 1f for regions that are always there, e.g. End.
    // Set to at least a bit below 1f for most things, to provide variety!
    public float spawnRate = 0.8f;

    // This region's description, if it has one.
    [TextArea(10, 30)]
    public string description = "";

    [Header("Machinery")]
    // A sprite renderer to show this region visually.
    // Automatically hidden, so players should not see!
    public SpriteRenderer spriteRenderer;

    // A unique id.
    public int id;

    // Count up the total number of regions, so each one has a unique id.
    public static int regionCount = 0;


    // + Initialization
    void Awake()
    {
        // Spawn(?)
        float spawnRoll = Random.Range(0f, 1f);
        if (spawnRoll < spawnRate)
        {
            // Enable.
            gameObject.SetActive(true);
        } else {
            // Disable.
            gameObject.SetActive(false);
            return;
        }

        // Hide sprite renderer.
        if (spriteRenderer != null)
            spriteRenderer.enabled = false;

        // Increment region count.
        regionCount++;

        // Set id.
        id = regionCount;
    }

    // Called when another collider enters this collider.
    void OnTriggerEnter2D(Collider2D col)
    {
        // Get explorer.
        Unit explorer = col.GetComponent<Unit>();

        // Ignore non-explorers.
        if (explorer == null) return;

        // + Slow fields

        // Water?
        if (myType == "Water")
            explorer.speedModifiers[GetUID()] = 0.5f;

        // Flora?
        if (myType == "Flora")
            explorer.speedModifiers[GetUID()] = 0.7f;
        if (myType == "Thick Flora")
            explorer.speedModifiers[GetUID()] = 0.3f;

        // + Paths to Victory
        if (explorer == GM.I.player)
        {
            // Hunter
            if (myType == "Way of the Hunter")
                DM.I.BeginHunt();

            if (myType == "Way of the Gatherer")
                DM.I.BeginGathering();

            // Battle
            // if (myType == "Battle")
            //     DM.I.BeginBattle();

            // // Harmony
            // if (myType == "Harmony")
            //     HM.I.BeginHarmony();
        }
    }

    void OnTriggerExit2D(Collider2D col)
    {
        // Get explorer.
        Unit explorer = col.GetComponent<Unit>();

        // Ignore non-explorers.
        if (explorer == null) return;

        // Slow fields: Water & Flora
        if (myType == "Water" || myType == "Flora" || myType == "Thick Flora")
            explorer.speedModifiers.Remove(GetUID());
    }

    // Return a unique identifer for this region.
    public string GetUID()
    {
        return myType + id;
    }
}