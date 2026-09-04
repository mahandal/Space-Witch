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

    // The description of the leader (lore).
    public TMP_Text leaderDetailsDescription;

    // The leader's portrait.
    public Image leaderDetailsPortrait;

    // The description of the leader's ability.
    public TMP_Text leaderDetailsAbility;

    // The description of the leader's stats.
    public TMP_Text leaderDetailsStats;

    // The cards this leader plays with.
    public List<MiniCard> leaderDetailCards = new List<MiniCard>();

    // This leader's signature cards.
    public List<MiniCard> leaderSignatureCards = new List<MiniCard>();

    // This leader's other cards.
    public List<MiniCard> leaderOtherCards;

    [Header("Automated Machinery")]
    // Are we sliding toward the leader select screen?
    public bool slidingToLeaderSelectScreen = false;

    // The leader select screen's start position.
    public Vector3 leaderSelectStartPosition;

    // A dictionary mapping leader names to their bios.
    // Automatically populated by leader bios themselves.
    [System.NonSerialized]
    public Dictionary<string, LeaderBio> leaderBios = new Dictionary<string, LeaderBio>();

    // The player's saved data.
    // public SaveData saveData;

    // Singleton.
    public static MainMenu I;


    // + Initialization
    // Initialize the main menu.
    public void Initialize()
    {
        // Singleton.
        if (I == null || I == this)
            I = this;
        else
            Destroy(this);

        // Hide what should be hidden.
        leaderSelectScreen.SetActive(false);
        leaderPortraits.alpha = 0f;
        leaderDetailsScreen.SetActive(false);

        // Remember leader select's start position.
        leaderSelectStartPosition = leaderSelectScreen.transform.localPosition;

        // Reset leader details screen position.
        leaderSelectScreen.transform.localPosition = leaderSelectStartPosition;

        // Activate the main menu object.
        // gameObject.SetActive(true);
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

        // + Load the leader's details.
        
        // Load background image, using home star's first planet.
        Utility.LoadImage(leaderDetailsBackground, "Planets/" + bio.homeStar.planets[0].myName);

        // Load portrait.
        Utility.LoadImage(leaderDetailsPortrait, "Leaders/" + bio.myName);

        // Set name.
        leaderDetailsName.text = bio.myName;

        // Set lore description.
        leaderDetailsDescription.text = bio.description;

        // Set active ability description.
        leaderDetailsAbility.text = bio.abilityDescription;

        // Set passive ability description.
        leaderDetailsStats.text = bio.statsDescription;

        // + Load cards.

        // Signature cards.
        for(int i = 0; i < bio.signatureCards.Count; i++)
        {
            leaderSignatureCards[i].LoadCard(bio.signatureCards[i]);
        }

        // Other cards

        // Get leader's full card list.
        // List<string> leaderCardList = bio.homeStar.cards;

        // // Count the index of the current displayed mini card we are loading.
        // int otherIndex = 0;

        // // Loop through each card in the leader's card list.
        // foreach (string cardName in leaderCardList)
        // {
        //     // Ignore signature cards.
        //     if (!bio.signatureCards.Contains(cardName))
        //     {
        //         // Load the card.
        //         leaderOtherCards[otherIndex].LoadCard(cardName);

        //         // Increment our index tracking which mini card to load into.
        //         otherIndex++;

        //         // Break?
        //         if (otherIndex >= leaderOtherCards.Count) break;
        //     }
        // }

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

        // Get bio for player's leader.
        LeaderBio bio = leaderBios[leaderDetailsName.text];

        // Load the player's leader for battle mode.
        DM.I.goodLeader.LoadBio(bio);

        // Add the player's leader card to their deck.
        MenuManager.I.saveData.decklist.Add(bio.leaderUnit);

        // Go to star map!
        StarManager.I.GoToStarMap(true);
    }

    // Continue from your previous save.
    public void B_ContinueAdventure()
    {
        // Fetch save data.
        MenuManager.I.saveData = Utility.GetSaveData();

        // Get bio for player's leader.
        LeaderBio bio = leaderBios[MenuManager.I.saveData.leaderName];

        // Load the player's leader.
        DM.I.goodLeader.LoadBio(bio);
        // InitializeGoodLeader(bio);

        // Go to star map!
        StarManager.I.GoToStarMap(true);
    }
}
