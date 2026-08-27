using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class Star : MonoBehaviour
{
    [Header("Mana Scaling")]
    // Mana scaling.
    public float goodManaScaling = 1f;
    public float evilManaScaling = 1f;

    [Header("Star")]
    // This star's name.
    public string myName;

    // This star's description.
    [TextArea]
    public string description;

    // This star's list of cards.
    public List<string> cards;

    // This star's list of planets.
    public List<Planet> planets;

    // This star's list of stars that can be traveled to, after completing this star.
    public List<Star> nextStars;

    [Header("Evil Leader")]
    public string localEvilLeader;

    [Header("Machinery")]

    // This star's image.
    public Image image;

    // This star's highlight.
    public GameObject highlight;

    void Awake()
    {
        // Get image.
        image = GetComponent<Image>();

        // Hide highlight.
        highlight.SetActive(false);
    }

    // Recursively set the opacity for each star, to show its status:
    // - Opaque: Available to fly to.
    // - Faded: Completed/No longer available.
    // - Hidden: Locked.
    public void SetStarOpacity()
    {
        // Set to active, so we can deactivate stars that are not available.
        gameObject.SetActive(true);

        // Check if this is the current star
        if (this == StarManager.I.currentStar)
            image.color = new Color (1f, 1f, 1f, 1f);
        // Check if this star is in the list of stars we can fly to.
        else if (StarManager.I.currentStar.nextStars.Contains(this))
            image.color = new Color (1f, 1f, 1f, 0.5f);
        else
            gameObject.SetActive(false);

        // Recurse on next stars.
        foreach (Star child in nextStars)
        {
            child.SetStarOpacity();
        }
    }

    // Recursively return all planets of this star and each star after it in the sector.
    public List<Planet> GetAllPlanetsRecursively()
    {
        // Initialize a new list of planets.
        List<Planet> allPlanets = new List<Planet>();

        // Add each of this star's planets.
        foreach (Planet p in planets)
        {
            allPlanets.Add(p);
        }

        // Recurse on each of this planet's next stars.
        foreach (Star s in nextStars)
        {
            // Add each planet to allPlanets.
            foreach(Planet p in s.GetAllPlanetsRecursively())
            {
                allPlanets.Add(p);
            }
        }

        return allPlanets;
    }

    // Clicked on.
    public void B_Click()
    {
        // Select this star, unless it was already select, in which case deselect it!
        if (StarManager.I.selectedStar != this)
            Select();
        else
            Deselect();
    }

    // Select this star.
    public void Select()
    {
        // Tell our manager we're selected.
        StarManager.I.SelectStar(this);

        // Highlight visually.
        highlight.SetActive(true);
    }

    // Deselect this star.
    public void Deselect()
    {
        // Clear selection.
        StarManager.I.selectedStar = null;

        // Clear highlight.
        highlight.SetActive(false);
    }
}
