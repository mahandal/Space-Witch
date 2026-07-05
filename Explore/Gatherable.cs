using UnityEngine;

public class Gatherable : MonoBehaviour
{
    [Header("Treasure")]
    // How many credits this treasure is worth when gathered.
    public int credits = 0;

    // Called when another collider enters this collider.
    void OnTriggerEnter2D(Collider2D col)
    {
        // Get explorer.
        Explorer explorer = col.GetComponent<Explorer>();

        // Ignore non-explorers.
        if (explorer == null) return;

        // Check if it was the player.
        if (explorer == GM.I.player)
        {
            // Gain credits.
            MenuManager.I.saveData.credits += credits;

            // Pop this item.
            Destroy(gameObject);
        }
    }
}
