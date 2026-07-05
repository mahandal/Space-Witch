using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// The game master!
// Runs Explore mode.
// See DM for running battles.
public class GM : MonoBehaviour
{
    [Header("Player")]
    // The player.
    public Explorer player;

    [Header("Interact")]
    // Which explorer we are currently interacting with, if any.
    public Explorer exploree;

    [Header("Planet")]
    // Which planet we are currently exploring.
    public Planet currentPlanet;

    // Singleton
    public static GM I;

    // + Initialization
    public void Initialize()
    {
        // Singleton.
        if (I == null || I == this)
            I = this;
        else
            Destroy(gameObject);
    }

    // Set up the given planet for the player to explore!
    public void Explore(Planet p)
    {
        // Set current planet.
        currentPlanet = p;

        // Move player into starting position.
        player.transform.position = currentPlanet.exploreStart.position;
    }

    // +++ Exploring!

    // + Interact
    // Look for the closest explorer to the player to interact with.
    // If no other explorer is within vision range, do nothing.
    public void Interact()
    {
        // + Find nearest other explorer.
        // Use overlapcircle to find nearby colliders.
        Collider2D[] colliders = Physics2D.OverlapCircleAll(player.transform.position, player.vision);

        // Remember nearest explorer.
        Explorer nearestExplorer = null;
        float nearestDistance = float.MaxValue;

        // Look through each collider.
        foreach (Collider2D col in colliders)
        {
            // Check if the collider is attached to an explorer.
            Explorer e = col.GetComponent<Explorer>();

            // Ignore non-explorers.
            if (e == null) continue;

            // Ignore the player.
            if (e == player) continue;

            // Get distance.
            float distance = Vector3.Distance(e.transform.position, player.transform.position);

            // Compare distance.
            if (distance < nearestDistance)
            {
                // New nearest.
                nearestDistance = distance;
                nearestExplorer = e;
            }
        }

        // Nothing to interact with.
        if (nearestExplorer == null)
            return;

        // + Interact.

        // Pause time.
        Time.timeScale = 0f;

        // Set exploree.
        exploree = nearestExplorer;

        // UI.
        ExploreUI.I.Interact(nearestExplorer);
    }

    // End an interaction.
    public void EndInteract()
    {
        // Clear exploree(?)
        exploree = null;

        // Resume time.
        Time.timeScale = 1f;
    }

    // Recruit a new explorer, added as a card to your deck.
    // Returns false if you are unable to afford them.
    // Returns true if you have credits equal to their credit cost.
    public bool Recruit()
    {
        // Check credit cost.
        if (MenuManager.I.saveData.credits < exploree.creditCost) return false;

        // Spend credits.
        MenuManager.I.saveData.credits -= exploree.creditCost;

        // Add card to deck.
        MenuManager.I.saveData.decklist.Add(exploree.myName);

        // For now, destroy explorer.
        Destroy(exploree.gameObject);

        // TBD: Allow other explorers to follow you around in a little squad.

        // Return successful.
        return true;
    }
}