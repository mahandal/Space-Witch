using UnityEngine;
using System.Collections.Generic;

public class Progenitors : MonoBehaviour
{
    [Header("Tiles")]
    public Tile tile;

    // + Dictionaries

    // Dictionary of all unit progenitors, with their names as keys.
    [System.NonSerialized]
    public Dictionary<string, Unit> units = new Dictionary<string, Unit>();

    // Dictionary of all structure progenitors, with their names as keys.
    [System.NonSerialized]
    public Dictionary<string, Unit> structures = new Dictionary<string, Unit>();

    // Dictionary of all spell progenitors, with their names as keys.
    [System.NonSerialized]
    public Dictionary<string, Unit> spells = new Dictionary<string, Unit>();

    // Dictionary of all item progenitors, with their names as keys.
    [System.NonSerialized]
    public Dictionary<string, Unit> items = new Dictionary<string, Unit>();

    // Singleton.
    public static Progenitors I;

    // + Initialization

    // Initialize
    public void Initialize()
    {
        // Singleton.
        if (I == null)
            I = this;
        else
            Destroy(this);

        // Initialize progenitors.
        InitializeProgenitors();
    }

    // // Awaken!
    // void Awake()
    // {
    //     // Singleton.
    //     if (I == null)
    //         I = this;
    //     else
    //         Destroy(this);

    //     // Initialize progenitors.
    //     InitializeProgenitors();
    // }

    // Look through each of this object's children and add them to their dictionary.
    public void InitializeProgenitors()
    {
        // Look through each of this object's children.
        foreach (Transform child in transform)
        {
            // Make sure progenitor is disabled.
            child.gameObject.SetActive(false);

            // Get Unit.
            // Note: 'Unit' in this case refers to class and NOT card type!
            // All card types are assumed to play a Unit object.
            // This is different from the Unit card type!
            // The word 'Unit' is overloaded!
            // Some say magic isn't real. But look at me go!
            Unit unit = child.GetComponent<Unit>();

            // Skip non-units.
            if (unit == null) continue;

            // Initialize.
            unit.Initialize();

            // Check if child is a unit.
            // Note: 'Unit' in this case refers to card type and NOT class!
            if (unit.cardType == "Unit")
            {
                // Add to unit dictionary.
                units[unit.myName] = unit;

                // Add to grimoire in card form.
                DM.I.grimoire[unit.myName] = DM.I.MakeCard(unit, "Unit");
            }
            // Check if child is a structure.
            else if (unit.cardType == "Structure")
            {
                // Add to structure dictionary.
                structures[unit.myName] = unit;

                // Add to grimoire in card form.
                DM.I.grimoire[unit.myName] = DM.I.MakeCard(unit, "Structure");
            }
            // Check if child is a spell.
            else if (unit.cardType == "Spell")
            {
                // Add to spell dictionary.
                spells[unit.myName] = unit;

                // Add to grimoire in card form.
                DM.I.grimoire[unit.myName] = DM.I.MakeCard(unit, "Spell");
            }
            else if (unit.cardType == "Item")
            {
                // Add to item dictionary.
                items[unit.myName] = unit;

                // Add to grimoire in card form.
                DM.I.grimoire[unit.myName] = DM.I.MakeCard(unit, "Item");
            }
            else
            {
                Debug.LogWarning("Failed to load card: " + unit.myName + ". Unknown card type: " + unit.cardType);
            }
        }
    }

    // Get progenitor by name.
    public Unit GetProgenitor(string cardName)
    {
        // Get card from grimoire.
        Card card = DM.I.grimoire[cardName];

        // Delegate.
        return GetProgenitor(card);
    }

    // Get the progenitor for the given card.
    // Have to check the type to find the right dictionary.
    public Unit GetProgenitor(Card card)
    {
        // Units.
        if (card.cardType == "Unit")
            return units[card.myName];
        // Structures.
        else if (card.cardType == "Structure")
            return structures[card.myName];
        // Spells.
        else if (card.cardType == "Spell")
            return spells[card.myName];
        // Items.
        else if (card.cardType == "Item")
            return items[card.myName];

        // Failed to find?
        return null;
    }
}
