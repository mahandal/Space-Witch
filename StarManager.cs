using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class StarManager : MonoBehaviour
{
    [Header("Star Map")]
    // The parent object of the star map.
    public GameObject starMap;

    [Header("Stars")]
    // The parent object of all stars.
    public Transform starParent;

    // Which star you have selected.
    public Star selectedStar;

    // Which star you are currently on.
    public Star currentStar;

    // Which star you start on.
    public Star startingStar;

    // The final star you must beat to win!
    public Star finalStar;

    // The name of the star you are currently on.
    public string currentStarName;

    [Header("Fly Button")]
    public GameObject flyButton;

    [Header("Tooltip")]
    // The parent of the tooltip.
    public CanvasGroup tooltip;

    // The name of the star system you have selected.
    public TMP_Text tooltipName;

    // The description of the star system you have selected.
    public TMP_Text tooltipDescription;

    // The image for the star system you have selected.
    public Image tooltipImage;

    // A list of images previewing which cards will be available in the star you have selected.
    public List<Image> cardHints;

    [Header("Planet")]
    // The index of the planet we're currently on.
    public int planetIndex = 0;

    // The parent object for the planet screen.
    public GameObject planetScreen;

    // The text object for the planet's name.
    public TMP_Text planetName;

    // The text object for the planet's description.
    public TMP_Text planetDescription;

    // The background image for the planet.
    public Image planetBackground;

    // The cards available to add to your deck on this planet.
    public List<CardOnPlanet> cardsOnPlanet;

    [Header("Victory")]
    public CanvasGroup victory;

    [Header("Save Data")]
    public SaveData saveData;

    // Singleton.
    public static StarManager I;

    // + Initialization

    // Initialize the star manager.
    // Note: Ends by disabling itself, so we start on the main menu.
    public void Initialize()
    {
        // Singleton.
        if (I == null)
            I = this;
        else
            Destroy(this);

        // Fetch save data.
        saveData = Utility.GetSaveData();

        // Hide what should not be.
        planetScreen.SetActive(false);
        victory.gameObject.SetActive(false);

        // Set our current planet index to past the current star's planet count,
        // so we go to the star map instead of the planet screen.
        planetIndex = FindCurrentStar().planets.Count;

        // Disable self.
        gameObject.SetActive(false);
    }

    // Open the star map screen.
    public void GoToStarMap()
    {
        // Find which star we are on.
        currentStar = FindCurrentStar();

        // Check if we just beat the final planet on this star and should return to the star map.

        // Check if we have more planets at this star.
        if (planetIndex < currentStar.planets.Count)
        {
            // Get the next planet.
            Planet p = currentStar.planets[planetIndex];

            // Load the next planet.
            LoadPlanet(p);
        } else {
            // We completed the current star!
            // Let's save our progress.
            // Note: This is expected to trigger the first time you go to a star map also.
            // A bit redundant but nbd. Only noting it in case something weird happens cause of it!
            Utility.SaveGame();

            // Hide the planet screen.
            planetScreen.SetActive(false);
        }

        // Final victory?
        if (currentStar == finalStar)
            Victory();
        else
            victory.gameObject.SetActive(false);

        // Set star opacity.
        startingStar.SetStarOpacity();

        // Clear star selection.
        Deselect();

        // Enable star map.
        gameObject.SetActive(true);

        // Disable main menu and battle map.
        MainMenu.I.gameObject.SetActive(false);
        GM.I.battleMap.SetActive(false);

        // Set game state.
        GM.I.gameState = 0;
    }

    // + Stars

    // Find which star we are on.
    // In order:
    // 1 - Keeps the currentStarName if it is already set.
    // 2 - Looks in our save file for a star name.
    // 3 - Defaults to startingStar if no save is found.
    public Star FindCurrentStar()
    {
        // Check if we already have a star name set (in Fly).
        if (currentStarName == "")
        {
            // Set name from save data, if it exists.
            if (saveData != null && saveData.currentStarName != "")
                currentStarName = saveData.currentStarName;
            else
                // Default to starting star.
                currentStarName = startingStar.myName;
        }

        // Look through each star.
        foreach (Transform t in starParent)
        {
            // Get star.
            Star star = t.GetComponent<Star>();

            // Ignore non-stars.
            if (star == null) continue;

            // Compare star name.
            if (star.myName == currentStarName)
                return star;
        }

        // No star found, return null.
        return null;
    }

    // Select a star and load into the tooltip.
    // Note: Stars handle their own selection.
    public void SelectStar(Star star)
    {
        // Deselect previous star.
        if (selectedStar != null)
            selectedStar.Deselect();

        // Set as selected star.
        selectedStar = star;

        // ++ Load tooltip.

        // Get planet prime.
        // (Last planet in list.)
        Planet prime = star.planets[star.planets.Count - 1];

        // Load name
        tooltipName.text = star.myName;

        // Load description.
        tooltipDescription.text = star.description;

        // Load image.
        Utility.LoadImage(tooltipImage, "Planets/" + prime.myName);

        // + Card hints
        // Loop through each card hint.
        for (int i = 0; i < cardHints.Count; i++)
        {
            // Check if there is a card available for this slot.
            if (i < prime.availableCards.Count)
            {
                // Get the card name.
                string cardName = prime.availableCards[i];

                // Load the image.
                Utility.LoadImage(cardHints[i], "Cards/" + cardName);

                // Enable the game object.
                cardHints[i].gameObject.SetActive(true);
            } else {
                // Disable the game object.
                cardHints[i].gameObject.SetActive(false);
            }
        }

        // Set the tooltip's alpha to 1 so it is visible.
        tooltip.alpha = 1f;

        // If selecting the current star, hide fly button. Otherwise, reveal it.
        if (star == currentStar)
            flyButton.SetActive(false);
        else
            flyButton.SetActive(true);
    }

    // Deselect any star.
    public void Deselect()
    {
        // Deselect previous star.
        if (selectedStar != null)
            selectedStar.Deselect();

        // Hide tooltip.
        tooltip.alpha = 0f;

        // Hide fly button.
        flyButton.SetActive(false);
    }

    // Fly to your selected star!
    public void Fly()
    {
        // Set the current star to your selected star.
        currentStar = selectedStar;
        currentStarName = selectedStar.myName;

        // Set your current planet index.
        planetIndex = 0;

        // Get the star's first planet.
        Planet firstPlanet = currentStar.planets[planetIndex];

        // Load planet.
        LoadPlanet(firstPlanet);
    }

    // + Planets

    // Get the name of the current planet.
    public string GetCurrentPlanetName()
    {
        // Null check?
        if (currentStar == null) return "";

        // Get current planet.
        Planet p = currentStar.planets[planetIndex];

        // Return name.
        return p.myName;
    }

    // Load the given planet into the planet screen.
    public void LoadPlanet(Planet p)
    {
        // Set name.
        planetName.text = p.myName;

        // Set description.
        planetDescription.text = p.description;

        // Load background image.
        Utility.LoadImage(planetBackground, "Planets/" + p.myName);

        // Load the planet's cards.
        List<string> planetCards = GetPlanetCards();
        for (int i = 0; i < cardsOnPlanet.Count; i++)
        {
            // Check if the planet has a card in this slot.
            if (i < p.availableCards.Count)
            {
                // Load card.
                cardsOnPlanet[i].LoadCard(planetCards[i]);
                
                // Make sure game object is active.
                cardsOnPlanet[i].gameObject.SetActive(true);
            } else {
                // No card for this slot, hide this card.
                cardsOnPlanet[i].gameObject.SetActive(false);
            }
        }

        // Go to the planet screen.
        planetScreen.SetActive(true);
    }

    // Get a list of names of the cards available on the current planet.
    public List<string> GetPlanetCards()
    {
        // Initialize a new list of card names.
        List<string> cardNames = new List<string>();

        // Get current planet.
        Planet p = currentStar.planets[planetIndex];

        // Iterate once per card on planet.
        for (int i = 0; i < p.availableCards.Count; i++)
        {
            // Roll whether to use the local card or pull from your home star.
            // Note: Local card actually pulls from current star, not planet.
            int coinFlip = Random.Range(0, 2);
            if (coinFlip == 0)
                cardNames.Add(currentStar.cards[i]);
            else if (coinFlip == 1)
                cardNames.Add(GM.I.goodLeader.homeStar.cards[i]);
            else
                Debug.LogError("Do I not know how unity's random range works?");
        }

        // Return.
        return cardNames;

        // Return available cards.
        // return p.availableCards;
    }

    // Get a random card from the current planet, weighted toward lower mana cost cards.
    public string GetRandomPlanetCard()
    {
        // Get a list of names of the available cards for the current planet.
        List<string> availableCards = GetPlanetCards();

        // Build a weighted list of cards, using 1/manaCost as the weight.
        // This makes lower mana cost cards proportionally more likely to be drawn.
        // e.g. a 2 mana card is 5x as likely as a 10 mana card.
        List<Card> weightedPool = new List<Card>();
        float totalWeight = 0f;

        List<(Card, float)> cardWeights = new List<(Card, float)>();
        foreach (string name in availableCards)
        {
            Card c = GM.I.grimoire[name];
            float weight = 1f / c.manaCost;
            cardWeights.Add((c, weight));
            totalWeight += weight;
        }

        // Roll randomly along the total weight.
        float roll = Random.Range(0f, totalWeight);
        float cumulative = 0f;
        foreach (var (c, weight) in cardWeights)
        {
            cumulative += weight;
            if (roll < cumulative)
            {
                return c.myName;
            }
        }

        Debug.LogWarning("Failed to select a random planet card!");
        return "Hyena";
    }

    // + End game

    // Final Victory!
    // Open the victory screen.
    public void Victory()
    {
        // Activate our victory game object.
        victory.gameObject.SetActive(true);
    }

    // Restart the campaign from the beginning.
    public void RestartCampaign()
    {
        // Reset save data.
        Utility.ResetSave();

        // Re-load game.
        Utility.LoadGameScene();
    }

    // + Buttons

    // Exit the game.
    public void B_Exit()
    {
        Utility.ExitGame();
    }
}
