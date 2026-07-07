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
    // The nearest interactable object, if there is one within the player's vision range.
    public Interactable nearestInteractable;

    // The nearest other explorer, if there is one within the player's vision range.
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

    // Fixed update
    void FixedUpdate()
    {
        // Find the nearest interactable object, if there is one within the player's vision range.
        FindNearestInteractable();
    }

    // Find the nearest interactable object, and/or the nearest explorer we can talk to.
    // Note: Done together to condense calls to overlapcircle.
    public void FindNearestInteractable()
    {
        // Reset nearest explorer and nearest interactable.
        exploree = null;
        nearestInteractable = null;

        // Remember nearest distances.
        float exploreeDistance = float.MaxValue;
        float interactableDistance = float.MaxValue;

        // Use overlapcircle to find nearby colliders.
        Collider2D[] colliders = Physics2D.OverlapCircleAll(player.transform.position, player.vision);

        // Look through each collider.
        foreach (Collider2D col in colliders)
        {
            // Explorer?
            Explorer e = col.GetComponent<Explorer>();
            if (e != null)
            {
                // Exclude self.
                if (e == player) continue;

                // Exclude squad members.
                if (e.squadLeader == player) continue;

                // Get distance.
                float distance = Vector3.Distance(player.transform.position, e.transform.position);

                // Have to get close enough they can see us.
                if (distance > e.vision) continue;

                // Check if distance is closer than any other.
                if (distance < exploreeDistance)
                {
                    // Set exploree.
                    exploree = e;

                    // Remember distance.
                    exploreeDistance = distance;
                }
            }

            // Interactable
            Interactable interactable = col.GetComponent<Interactable>();
            if (interactable != null)
            {
                // Get distance.
                float distance = Vector3.Distance(player.transform.position, interactable.transform.position);
                if (distance < exploreeDistance)
                {
                    // Set nearest interactable.
                    nearestInteractable = interactable;

                    // Remember distance.
                    interactableDistance = distance;
                }
            }
        }

        // Show hint to interact?
        if (exploree != null || nearestInteractable != null)
            ExploreUI.I.HintInteract();
        else
            ExploreUI.I.HideBottomHint();
    }

    // + Interact
    // Attempt to interact with the nearest explorer or interactable object.
    public void Interact()
    {
        // Nothing to interact with.
        if (exploree == null && nearestInteractable == null)
            return;

        // + Interact.

        // Pause time.
        Time.timeScale = 0f;

        // UI.
        if (exploree != null)
            ExploreUI.I.Interact(exploree);
        else
            ExploreUI.I.Interact(nearestInteractable);
    }

    // End an interaction.
    public void EndInteract()
    {
        // Clear exploree(?)
        // exploree = null;

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
        // Destroy(exploree.gameObject);

        player.AddToSquad(exploree);

        // TBD: Allow other explorers to follow you around in a little squad.

        // Return successful.
        return true;
    }
}