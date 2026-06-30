using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Collections;

public class MainMenu : MonoBehaviour
{
    [Header("Design")]
    public float slideSpeed = 10f;

    [Header("Manual Machinery")]
    // Parent object of the leader selection screen.
    public GameObject leaderSelectScreen;

    // A canvas group for all portraits (including names) in the leader selection screen.
    public CanvasGroup leaderPortraits;

    // Parent object of the leader details screen.
    public GameObject leaderDetailsScreen;

    [Header("MM: Leader Details")]
    // + Manual machinery for the leader details screen.

    // The background image.
    public Image leaderDetailsBackground;

    // The leader's name.
    public TMP_Text leaderDetailsName;

    // The leader's portrait.
    public Image leaderDetailsPortrait;

    // The description of the leader's ability.
    public TMP_Text leaderDetailsAbility;

    // The description of the leader (lore).
    public TMP_Text leaderDetailsDescription;

    // The cards this leader plays with.
    public List<MiniCard> leaderDetailCards = new List<MiniCard>();

    [Header("Automated Machinery")]
    // Are we sliding toward the leader select screen?
    public bool slidingToLeaderSelectScreen = false;

    // The leader select screen's start position.
    public Vector3 leaderSelectStartPosition;

    // A dictionary mapping leader names to their bios.
    // Automatically populated by leader bios themselves.
    [System.NonSerialized]
    public Dictionary<string, LeaderBio> leaderBios = new Dictionary<string, LeaderBio>();

    // Singleton.
    public static MainMenu I;


    // + Initialization
    // Awaken!
    void Awake()
    {
        // Singleton.
        if (I == null)
            I = this;
        else
            Destroy(this);

        // Remember leader select's start position.
        leaderSelectStartPosition = leaderSelectScreen.transform.localPosition;

        // Initialize.
        Initialize();
    }

    // Initialize the main menu.
    public void Initialize()
    {
        // Hide what should be hidden.
        leaderSelectScreen.SetActive(false);
        leaderPortraits.alpha = 0f;
        leaderDetailsScreen.SetActive(false);

        // Reset leader details screen position.
        leaderSelectScreen.transform.localPosition = leaderSelectStartPosition;

        // Activate the main menu object.
        gameObject.SetActive(true);
    }

    // + Transitions
    void FixedUpdate()
    {
        // Slide to the leader select screen.
        if (slidingToLeaderSelectScreen)
        {
            // Slide into place.
            if (leaderSelectScreen.transform.localPosition.y > -150)
            {
                leaderSelectScreen.transform.localPosition -= new Vector3(0f, slideSpeed * Time.fixedDeltaTime, 0f);
            } else {
                // Done?
                slidingToLeaderSelectScreen = false;
                leaderSelectScreen.transform.localPosition = new Vector3(0f, -150f, 0f);
            }

            // Fade in leader portraits.
            leaderPortraits.alpha += 0.001f;
        }
    }

    // + Buttons

    // Go to the leader details screen, from the leader selection screen.
    // Called when you click on a leader's portrait.
    public void B_LeaderPortrait(string leaderName)
    {
        // Get the bio for this leader.
        LeaderBio bio = leaderBios[leaderName];

        // Load the leader's details.
        Utility.LoadImage(leaderDetailsBackground, "Planets/" + bio.homeStar.planets[0].myName);
        Utility.LoadImage(leaderDetailsPortrait, "Leaders/" + bio.myName);
        leaderDetailsName.text = bio.myName;
        leaderDetailsAbility.text = bio.abilityDescription;
        leaderDetailsDescription.text = bio.description;
        for(int i = 0; i < leaderDetailCards.Count; i++)
        {
            leaderDetailCards[i].LoadCard(bio.homeStar.cards[i]);
        }

        // Enable the leader details screen.
        leaderDetailsScreen.SetActive(true);

        // Hide the leader selection screen.
        leaderSelectScreen.SetActive(false);
    }
    
    // Go back from examining a leader's details to the leader selection screen.
    public void B_BackFromDetails()
    {
        // Enable the leader select screen.
        leaderSelectScreen.SetActive(true);

        // Hide the leader details screen.
        leaderDetailsScreen.SetActive(false);
    }

    // Button pressed to start a new game!
    // Go to the leader selection screen.
    public void B_NewGame()
    {
        // Go to leader select screen.
        leaderSelectScreen.SetActive(true);
        slidingToLeaderSelectScreen = true;
    }

    // Button pressed to play with the leader you have selected.
    public void B_Play()
    {
        // Reset save.
        Utility.ResetSave();

        // Set good leader's name.
        GM.I.goodLeader.myName = leaderDetailsName.text;

        // Set good leader's home star.
        GM.I.goodLeader.homeStar = leaderBios[leaderDetailsName.text].homeStar;

        // Go to star map!
        StarManager.I.GoToStarMap();
    }

    // Continue from your previous save.
    public void B_ContinueAdventure()
    {
        // Go to star map!
        StarManager.I.GoToStarMap();
    }
}
