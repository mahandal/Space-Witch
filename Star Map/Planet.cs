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

    // This planet's tilemap shown for its big battle.
    public GameObject battleMap;

    // This planet's explore map.
    public GameObject exploreMap;

    // Where the player starts on this planet's explore map.
    public Transform exploreStart;
}
