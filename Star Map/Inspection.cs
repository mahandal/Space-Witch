using UnityEngine;
using UnityEngine.UI;
using TMPro;

// Handle card inspection.
public class Inspection : MonoBehaviour
{
	[Header("Stardust")]
	// Yer current stardust count.
	public TMP_Text stardust;

	// Cost to level up.
	public int cost;
	public TMP_Text costText;

	// Stardust gained from sending home.
	public int stardustGained;
	public TMP_Text stardustGainedText;



	[Header("Machinery")]
	// Card details
	public Image cardArt;
	public TMP_Text cardName;
	public TMP_Text description;
	public TMP_Text cardType;
	public TMP_Text role;
	public TMP_Text manaCost;
	public TMP_Text deployTime;
	public TMP_Text health;
	public TMP_Text armor;
	public TMP_Text vision;
	public TMP_Text speed;
	public TMP_Text damage;
	public TMP_Text attackTime;
	public TMP_Text range;

	// A reference to the viewing deck to go back and forth.
	public ViewDeck viewDeck;



	// + Initialization

	// + Inspection
	// Begin inspecting a card.
    public void InspectCard(string _cardName)
    {
    	// + Get level.
    	int level = 1;

    	// Split name into parts.
        string[] parts = _cardName.Split(' ', 3);

        // Check if the card name has a level.
        if (parts[0] == "Level")
            level = int.Parse(parts[1]);
    

    	// Get progenitor.
    	Unit p = Progenitors.I.GetProgenitor(_cardName);

    	// Load portrait.
        Utility.LoadImage(cardArt, "Cards/" + p.GetBaseName());

    	// Fill in card details.
    	cardName.text = _cardName;
    	description.text = p.description;
    	cardType.text = p.cardType;
    	role.text = p.role;
    	manaCost.text = p.manaCost.ToString("0");
        deployTime.text = p.deployTime.ToString("0");
        health.text = (p.maxHealth * Utility.LevelScalar(level)).ToString("0");
        armor.text = p.armor.ToString("0");
        vision.text = (p.vision  * Utility.LevelScalar(level)).ToString("0.0");
        speed.text = p.speed.ToString("0.0");
        damage.text = (p.damage * Utility.LevelScalar(level)).ToString("0.0");
        attackTime.text = "per " + p.attackTime.ToString("0.0") + " seconds";

        // Bulwarks don't gain range with levels.
        if (p.role == "Bulwark")
		    range.text = p.range.ToString("0.0");
		else
			range.text = (p.range  * Utility.LevelScalar(level)).ToString("0.0");

		// Set stardust.
		stardust.text = MenuManager.I.saveData.stardust.ToString();

		// Set cost to level.
		cost = (int)Mathf.Pow(10, level);
		costText.text = cost.ToString();

		// Set stardust gained from sending home.
		stardustGained = Mathf.RoundToInt((p.manaCost + p.deployTime) * level);
		stardustGainedText.text = stardustGained.ToString();

		// Activate.
		gameObject.SetActive(true);
    }


    // + Buttons
    // Go back to the viewing deck.
    public void B_ViewDeck()
    {
    	// Re-load viewing deck.
    	viewDeck.LoadCards();

    	// Disable inspection.
    	gameObject.SetActive(false);
    }

    // Spend stardust to level up the current card.
    public void B_LevelUp()
    {
    	// Check if ye've enough stardust.
    	if (MenuManager.I.saveData.stardust < cost)
    		return;

    	// Spend stardust.
    	MenuManager.I.saveData.stardust -= cost;

    	// Level up.
    	string leveledCard = MenuManager.I.saveData.LevelUp(cardName.text);

    	// Re-load card.
    	InspectCard(leveledCard);
    }

    // Send home the current card, giving ye stardust in exchange.
    public void B_SendHome()
    {
    	// Remove from decklist.
    	MenuManager.I.saveData.decklist.Remove(cardName.text);

    	// Gain stardust.
    	MenuManager.I.saveData.stardust += stardustGained;

    	// Return to viewing deck.
    	B_ViewDeck();
    }
}
