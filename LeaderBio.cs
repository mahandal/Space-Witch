using UnityEngine;

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
    [TextArea(10, 30)]
    public string abilityDescription;

    // + Initialization
    // Start
    void Start()
    {
        // Connect with main menu's dictionary of leader bios.
        MainMenu.I.leaderBios[myName] = this;
    }
}
