using UnityEngine;
using System.Collections.Generic;

public class Planet : MonoBehaviour
{
    [Header("Planet")]
    // This planet's name.
    public string myName;

    // This planet's description.
    [TextArea]
    public string description;

    // This planet's list of cards you may choose from to add to your deck.
    public List<string> availableCards;
}
