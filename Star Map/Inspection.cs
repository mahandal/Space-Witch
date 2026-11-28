using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

// Handle card inspection.
public class Inspection : MonoBehaviour
{
	[Header("Machinery")]
	// Dummy unit to load stats.
	public Unit inspectedUnit;

	// A reference to the viewing deck to reload when closing.
	public ViewDeck viewDeck;

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

	// Singleton
	public static Inspection I;

	// + Initialization
	public void Initialize()
	{
		// Singleton
		I = this;
	}

	// + Inspection
	// Begin inspecting a card.
    public void InspectCard(string _cardName)
    {
    	// Load dummy unit.
        inspectedUnit.LoadUnit(_cardName);

        // Delegate to below.
        InspectUnit(inspectedUnit);
    }

    // Inspect a unit.
    public void InspectUnit(Unit unit)
    {
    	// Load portrait.
        Utility.LoadImage(cardArt, "Cards/" + unit.GetBaseName());

        // Meta
        cardName.text = unit.myName;
    	description.text = unit.description;

    	// +++ Stats
    	cardType.text = unit.cardType;
    	Utility.SetRoleText(unit, role);
    	manaCost.text = unit.manaCost.ToString("0");
        deployTime.text = unit.deployTime.ToString("0");
        health.text = unit.maxHealth.ToString("0");
        armor.text = unit.armor.ToString("0");
        vision.text = unit.vision.ToString("0.0");
        speed.text = unit.speed.ToString("0.0");

        // + Attack
        // Enable attack info for units and turrets.
        attackParent.SetActive(unit.cardType == "Unit" || unit.role == "Turret");

        damage.text = unit.damage.ToString("0.0");
        attackTime.text = "per " + unit.attackTime.ToString("0.0") + " seconds";
        range.text = unit.range.ToString("0.0");

        // + Farm
        // Enable farm info
        farmParent.SetActive(unit.farmTime > 0);

        farmTime.text = unit.farmTime.ToString("0.0");

        // + Production
        productionParent.SetActive(unit.timePerSpawn > 0);

        productionTime.text = unit.timePerSpawn.ToString("0.0");
        producedUnitName.text = unit.producedUnit;
        Utility.LoadImage(producedUnitPortrait, "Cards/" + Utility.ExtractBaseName(unit.producedUnit));

        // + Keywords
        B_CloseKeyword();
        for (int i = 0; i < keywords.Count; i++)
        {
        	// Check if unit has a keyword in this slot.
        	if (i < unit.keywords.Count)
        	{
        		// Set text.
        		keywords[i].text = unit.keywords[i];

        		// Activate game object.
        		keywords[i].gameObject.SetActive(true);

        		// Chaotic?
        		if (unit.keywords[i] == "Chaotic")
        			deployTime.text += " - " + (unit.deployTime * 2).ToString("0");
        	} else {
        		// No keyword.
        		keywords[i].gameObject.SetActive(false);
        	}
        }

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

    // Close the inspection screen, going back to wherever ye were before.
    public void B_Back()
    {
    	// SFX
    	DJ.I.PlaySound("Snap");

    	// Disable inspection.
    	gameObject.SetActive(false);
    }
}
