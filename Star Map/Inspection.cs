using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

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
	// A reference to the viewing deck to go back and forth.
	public ViewDeck viewDeck;

	// Dummy unit.
	public Unit inspectedUnit;

	// Main image in the middle.
	public Image cardArt;

	// Card's name at the top.
	public TMP_Text cardName;

	// Card's description on the left.
	public TMP_Text description;

	// +++ Stats
	public TMP_Text cardType;
	public TMP_Text role;
	public TMP_Text manaCost;
	public TMP_Text deployTime;
	public TMP_Text health;
	public TMP_Text armor;
	public TMP_Text vision;
	public TMP_Text speed;

	// + Attack
	public GameObject attackParent;
	public TMP_Text damage;
	public TMP_Text attackTime;
	public TMP_Text range;

	// + Farm
	public GameObject farmParent;
	public TMP_Text farmTime;

	// + Production
	public GameObject productionParent;
	public TMP_Text productionTime;
	public TMP_Text producedUnitName;
	public Image producedUnitPortrait;

	// + Keywords
	public List<TMP_Text> keywords;
	public GameObject keywordDescriptionBox;
	public TMP_Text keywordDescription;
	public TMP_Text keywordHeader;


	// + Initialization

	// + Inspection
	// Begin inspecting a card.
    public void InspectCard(string _cardName)
    {
    	// Load dummy unit.
        inspectedUnit.LoadUnit(_cardName);

        // Load portrait.
        Utility.LoadImage(cardArt, "Cards/" + inspectedUnit.GetBaseName());

        // Meta
        cardName.text = _cardName;
    	description.text = inspectedUnit.description;

    	// +++ Stats
    	cardType.text = inspectedUnit.cardType;
    	Utility.SetRoleText(inspectedUnit, role);
    	manaCost.text = inspectedUnit.manaCost.ToString("0");
        deployTime.text = inspectedUnit.deployTime.ToString("0");
        health.text = inspectedUnit.maxHealth.ToString("0");
        armor.text = inspectedUnit.armor.ToString("0");
        vision.text = inspectedUnit.vision.ToString("0.0");
        speed.text = inspectedUnit.speed.ToString("0.0");

        // + Attack
        // Enable attack info for units and turrets.
        attackParent.SetActive(inspectedUnit.cardType == "Unit" || inspectedUnit.role == "Turret");

        damage.text = inspectedUnit.damage.ToString("0.0");
        attackTime.text = "per " + inspectedUnit.attackTime.ToString("0.0") + " seconds";
        range.text = inspectedUnit.range.ToString("0.0");

        // + Farm
        // Enable farm info
        farmParent.SetActive(inspectedUnit.farmTime > 0);

        farmTime.text = inspectedUnit.farmTime.ToString("0.0");

        // + Production
        productionParent.SetActive(inspectedUnit.timePerSpawn > 0);

        productionTime.text = inspectedUnit.timePerSpawn.ToString("0.0");
        producedUnitName.text = inspectedUnit.producedUnit;
        Utility.LoadImage(producedUnitPortrait, "Cards/" + Utility.ExtractBaseName(inspectedUnit.producedUnit));

        // + Keywords
        B_CloseKeyword();
        for (int i = 0; i < keywords.Count; i++)
        {
        	// Check if unit has a keyword in this slot.
        	if (i < inspectedUnit.keywords.Count)
        	{
        		// Set text.
        		keywords[i].text = inspectedUnit.keywords[i];

        		// Activate game object.
        		keywords[i].gameObject.SetActive(true);

        		// Chaotic?
        		if (inspectedUnit.keywords[i] == "Chaotic")
        			deployTime.text += " - " + (inspectedUnit.deployTime * 2).ToString("0");
        	} else {
        		// No keyword.
        		keywords[i].gameObject.SetActive(false);
        	}
        }

        // + Stardust

		// Set stardust.
		stardust.text = MenuManager.I.saveData.stardust.ToString();

		// Set cost to level.
		cost = (int)Mathf.Pow(10, inspectedUnit.level);
		costText.text = cost.ToString();

		// Set stardust gained from sending home.
		stardustGained = Mathf.RoundToInt((inspectedUnit.manaCost + inspectedUnit.deployTime) * inspectedUnit.level);
		stardustGainedText.text = stardustGained.ToString();

		// Activate.
		gameObject.SetActive(true);
    }


    // + Buttons

    // Select a keyword.
    public void B_Keyword(int index)
    {
    	// SFX
    	DJ.I.PlaySound("Snap");

    	// Get keyword.
    	string keyword = keywords[index].text;

    	// Set header.
    	keywordHeader.text = keyword;

    	// Get the keyword's description.
    	string description = GGG.I.GetKeywordDescription(keyword);

    	// Set keyword's description.
    	keywordDescription.text = description;

    	// Open the keyword description box.
    	keywordDescriptionBox.SetActive(true);
    }

    // Close the keyword description box.
    public void B_CloseKeyword()
    {
    	// SFX
    	DJ.I.PlaySound("Snap");

    	// Close the keyword description box.
    	keywordDescriptionBox.SetActive(false);
    }

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
    	viewDeck.deck.Remove(cardName.text);
    	if (DM.I.gameMode == "Campaign")
    	{
			MenuManager.I.saveData.decklist.Remove(cardName.text);
    	} else {
    		DM.I.goodLeader.deck.Remove(cardName.text);
    	}

    	// Gain stardust.
    	MenuManager.I.saveData.stardust += stardustGained;

    	// Return to viewing deck.
    	B_ViewDeck();
    }
}
