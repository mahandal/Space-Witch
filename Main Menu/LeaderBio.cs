using UnityEngine;
using System.Collections.Generic;

public class LeaderBio : MonoBehaviour
{
    [Header("Leader Bio")]
    // Which star this leader calls home.
    public Star homeStar;

    // The leader's name.
    public string myName;

    // The leader's description.
    [TextArea(10, 30)]
    public string description;

    // The description of the leader's ability.
    [TextArea(5, 30)]
    public string abilityDescription;

    // The description of the leader's stats.
    [TextArea(5, 30)]
    public string statsDescription;


    [Header("Signature Cards")]
    // This leader's signature cards.
    public List<string> signatureCards = new List<string>();

    // A list of cooldowns for this leader's signature cards.
    public List<float> signatureCooldowns = new List<float>();


    // + Initialization
    // Start
    void Start()
    {
        // Connect with main menu's dictionary of leader bios.
        MainMenu.I.leaderBios[myName] = this;
    }
}
