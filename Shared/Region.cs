using UnityEngine;

// Terrain & stuff
public class Region : MonoBehaviour
{
    [Header("Terrain")]
    // What type of region is this?
    // Types of region:
    // Plains = Non.
    // Water = Slow field.
    // Flora = Slow field.
    // -EXPLORE MODE ONLY-
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

    // Detect when a unit enters this region.
    // Called when another collider enters this collider.
    void OnTriggerEnter2D(Collider2D col)
    {
        Debug.Log(name + " entered by " + col.name);

        // Get unit.
        Unit unit = col.GetComponent<Unit>();

        // Ignore non-unit.
        if (unit == null) return;

        // + Slow fields

        // Water
        if (myType == "Water")
            unit.speedModifiers[GetUID()] = 0.5f;

        // Flora
        if (myType == "Flora")
            unit.speedModifiers[GetUID()] = 0.7f;
        if (myType == "Thick Flora")
            unit.speedModifiers[GetUID()] = 0.3f;

        // Furniture
        if (myType == "Furniture")
            unit.speedModifiers[GetUID()] = 0.7f;
        if (myType == "Heavy Furniture")
            unit.speedModifiers[GetUID()] = 0.3f;

        // Lava
        if (myType == "Lava")
            unit.speedModifiers[GetUID()] = 0.3f;

        // + Paths to Victory
        if (unit == GM.I.player)
        {
            // Hunter
            if (myType == "Way of the Hunter")
                DM.I.BeginHunt();

            if (myType == "Way of the Gatherer")
                DM.I.BeginGathering();
        }
    }


    void OnTriggerStay2D(Collider2D col)
    {
        // Get unit.
        Unit unit = col.GetComponent<Unit>();

        // Ignore non-units.
        if (unit == null) return;

        // Ignore deploying and dead units.
        if (unit.state <= 0) return;

        // Fire & Lava
        if (myType == "Fire" || myType == "Lava")
            unit.LoseHealth(2f * Time.fixedDeltaTime, null, true);
    }

    void OnTriggerExit2D(Collider2D col)
    {
        // Get unit.
        Unit unit = col.GetComponent<Unit>();

        // Ignore non-units.
        if (unit == null) return;

        // Slow fields: Water & Flora
        if (myType == "Water" || myType == "Flora" || myType == "Thick Flora")
            unit.speedModifiers.Remove(GetUID());
    }

    // Return a unique identifer for this region.
    public string GetUID()
    {
        return myType + id;
    }
}