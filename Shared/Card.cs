using UnityEngine;

// Cards are simple!
public class Card
{
    // - Meta
    // Card name.
    public string myName;

    // Card type.
    // Types:
    // - Unit
    // - Structure
    // - Spell
    // - Item
    public string cardType;

    // This card's mana cost.
    public int manaCost;

    // This card's deploy time.
    public float deployTime;

    // This card's role.
    public string role;

    // Constructor.
    public Card(string _myName, string _cardType, int _manaCost, float _deployTime, string _role)
    {
        myName = _myName;
        cardType = _cardType;
        manaCost = _manaCost;
        deployTime = _deployTime;
        role = _role;
    }
}
