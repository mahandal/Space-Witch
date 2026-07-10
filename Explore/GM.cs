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
    public Unit player;

    // The units that are exploring in the player's squad.
    public List<string> exploring;

    // The rest of the cards in the player's deck, resting for the next big battle.
    public List<string> resting;

    [Header("Interact")]
    // The nearest interactable object, if there is one within the player's vision range.
    public Interactable nearestInteractable;

    // The nearest other explorer, if there is one within the player's vision range.
    public Unit exploree;

    [Header("Planet")]
    // Which planet we are currently exploring.
    public Planet currentPlanet;

    [Header("UI")]
    public ExploreUI exploreUI;

    // Singleton
    public static GM I;

    // +++ Initialization

    // Initialize.
    // Called once when the game is loaded.
    public void Initialize()
    {
        // Singleton.
        if (I == null || I == this)
            I = this;
        else
            Destroy(gameObject);

        // Initialize UI.
        exploreUI.Initialize();
    }

    // Set up the given planet for the player to explore!
    public void Explore(Planet p)
    {
        // Set current planet.
        currentPlanet = p;

        // Set resting.
        resting = new List<string>(MenuManager.I.saveData.decklist);

        // Pop leader(?) into exploring.
        exploring = new List<string>();
        exploring.Add(resting[0]);
        resting.RemoveAt(0);

        // Set squad.
        player.squad = new List<Unit>();
        player.squad.Add(player);

        // Deploy.
        Deploy(player);

        // Move player into starting position.
        // player.transform.position = currentPlanet.exploreStart.position;

        // // Set player's deployment time.
        // player.deployTimer = player.deployTime;
        // player.state = 0;

        // UI.
        ExploreUI.I.Explore(p);
    }

    // Set a unit to deploying and move it to the current planet's spawn position.
    public void Deploy(Unit unit)
    {
        // Set deploying.
        unit.deployTimer = unit.deployTime;
        unit.state = 0;
        unit.animator.SetInteger("State", unit.state);

        // Show full deployment.
        unit.showFullDeployment = true;

        // Move into position.
        unit.transform.position = currentPlanet.exploreStart.position;

        // Reset rotation.
        unit.transform.eulerAngles = Vector3.zero;

        // Reset hurt timer.
        unit.hurtTimer = 0f;
    }

    // +++ Exploring!

    // Fixed update
    void FixedUpdate()
    {
        // Wait for player to finish deploying.
        if (player.deployTimer > 0f) return;

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
            Unit e = col.GetComponent<Unit>();
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

        // Add to player's squad.
        player.AddToSquad(exploree);

        // Add to exploring list.
        exploring.Add(exploree.myName);

        // Return successful.
        return true;
    }

    // + Spawning
    // Spawn a new unit.
    public Unit SpawnUnit(string unitName, Vector3 position)
    {
        // Get the progenitor for the unit.
        Unit progenitor = Progenitors.I.GetProgenitor(unitName);

        // Instantiate a new copy of the unit.
        Unit newUnit = Object.Instantiate(progenitor);

        // Move unit into position.
        newUnit.transform.position = position;

        // Show full deployment.
        newUnit.showFullDeployment = true;

        // Activate.
        newUnit.gameObject.SetActive(true);

        // Return.
        return newUnit;
    }
}