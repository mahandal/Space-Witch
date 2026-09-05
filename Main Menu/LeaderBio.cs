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

    // The description of the leader's passive ability.
    [TextArea(5, 30)]
    public string statsDescription;

    [Header("Power")]
    // The description of the leader's active ability.
    [TextArea(5, 30)]
    public string abilityDescription;

    // The maximum number of charges this leader may store of their power at a time.
    public int maxPowerCharges = 5;

    // The number of seconds it takes to charge a use of this hero's power.
    public float powerChargeTime = 13f;

    [Header("Signature Cards")]
    // The unit this leader plays as.
    public string leaderUnit;

    // This leader's signature cards.
    public List<string> signatureCards = new List<string>();

    // A list of cooldowns for this leader's signature cards.
    public List<float> signatureCooldowns = new List<float>();

    [Header("Reinforcements")]
    // Which cards this leader has available as reinforcements.
    public List<string> reinforcements;


    // + Initialization
    // Start
    void Start()
    {
        // Connect with main menu's dictionary of leader bios.
        MainMenu.I.leaderBios[myName] = this;
    }
}
