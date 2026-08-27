using UnityEngine;

public class Gatherable : MonoBehaviour
{
    [Header("Treasure")]
    // How many credits this treasure is worth when gathered.
    public int credits = 0;

    // How likely is this gatherable to spawn?
    // Set to 1f for gatherables that are always there!
    // Set to at least a bit below 1f for most things, to provide variety!
    // TBD: Handle violet flowers specially, so there are always 13!
    public float spawnRate = 0.8f;

    // This gatherable's description, if it has one.
    [TextArea(10, 30)]
    public string description = "";

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
    }

    // Called when another collider enters this collider.
    void OnTriggerEnter2D(Collider2D col)
    {
        // Get explorer.
        Unit explorer = col.GetComponent<Unit>();

        // Ignore non-explorers.
        if (explorer == null) return;

        // Check if it was the player.
        if (explorer == GM.I.player)
        {
            // Gain credits.
            // MenuManager.I.saveData.credits += credits;
            GM.I.GainCredits(credits);

            // Pop this item.
            Destroy(gameObject);
        }
    }
}
