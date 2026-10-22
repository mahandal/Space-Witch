using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Collections;

public class MainMenu : MonoBehaviour
{
    [Header("Design")]
    // How fast we slide up to see the heroes for campaign mode.
    public float slideSpeed = 10f;

    [Header("Skirmish")]
    // Which hero ye use for skirmishes.
    public string skirmishHero = "Morgan le Fey";

    // Which difficulty yer skirmishes are set to.
    // 0 = Easy
    // 1 = Medium
    // 2 = Hard
    public int skirmishDifficulty = 0;

    // The portrait of yer hero for skirmishes.
    public Image skirmishPortrait;

    // Parent object of hero wheel used to select skirmish hero.
    public GameObject heroWheel;

    // Whether the wheel of heroes is spinning or nay.
    public bool wheelSpinning = false;

    // How much longer the wheel will spin for.
    public float wheelSpinTime = 6f;

    // How fast the wheel of heroes spins.
    public float wheelSpinSpeed = 1f;

    // The fastest speed the wheel has spun at.
    public float peakSpinSpeed = 1f;

    // The buttons to choose yer difficulty for skirmishes.
    public List<CanvasGroup> skirmishDifficulties;

    [Header("Campaign Leader Selection")]
    // Parent object of the leader selection screen.
    public GameObject leaderSelectScreen;

    // A canvas group for all portraits (including names) in the leader selection screen.
    public CanvasGroup leaderPortraits;

    

    // + Buttons
    public Button continueButton;

    [Header("Campaign Leader Details")]
    // + Manual machinery for the leader details screen.

    // Parent object of the leader details screen.
    public GameObject leaderDetailsScreen;

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
        heroWheel.SetActive(false);

        // Remember leader select's start position.
        leaderSelectStartPosition = leaderSelectScreen.transform.localPosition;

        // Reset leader details screen position.
        leaderSelectScreen.transform.localPosition = leaderSelectStartPosition;

        // Check if we have a save to continue from.
        if (MenuManager.I.saveData.leaderName == "Dreaming Soul")
            continueButton.gameObject.SetActive(false);
        else
            continueButton.gameObject.SetActive(true);

        // Load player prefs.
        B_SkirmishHeroChoose(PlayerPrefs.GetString("skirmishHero", "Morgan le Fey"));
        B_SkirmishDifficulty(PlayerPrefs.GetInt("skirmishDifficulty", 0));

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

        // Wheel spinning?
        if (wheelSpinTime > 0f)
        {
            // Decrement wheel spin time.
            wheelSpinTime -= Time.fixedDeltaTime;

            // Increase wheel spin speed if timer is over 6 seconds.
            if (wheelSpinTime > 6f)
            {
                wheelSpinSpeed += Time.fixedDeltaTime;
                peakSpinSpeed = wheelSpinSpeed;
            } else {
                // Slow to a halt.
                float t = Mathf.Clamp01(wheelSpinTime / 6f);
                wheelSpinSpeed = peakSpinSpeed * t * t;   
            }

            // Rotate wheel.
            heroWheel.transform.eulerAngles -= new Vector3 (0, 0, wheelSpinSpeed);
        }
    }

    // +++ Buttons

    // + Campaign

    // Go to the leader details screen, from the leader selection screen.
    // Called when you click on a leader's portrait.
    public void B_LeaderPortrait(string leaderName)
    {
        // SFX
        DJ.I.PlaySound("Snap");

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
        // SFX
        DJ.I.PlaySound("Snap");

        // Enable the leader select screen.
        leaderSelectScreen.SetActive(true);

        // Hide the leader details screen.
        leaderDetailsScreen.SetActive(false);
    }

    // Button pressed to start a new game!
    // Go to the leader selection screen.
    public void B_NewGame()
    {
        // SFX
        DJ.I.PlaySound("Snap 2");

        // Go to leader select screen.
        leaderSelectScreen.SetActive(true);
        slidingToLeaderSelectScreen = true;
    }

    // Button pressed to play with the leader you have selected.
    public void B_Play()
    {
        // SFX
        DJ.I.PlaySound("Snap 2");

        // Set game mode to campaign.
        DM.I.gameMode = "Campaign";

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
        // SFX
        DJ.I.PlaySound("Snap");

        // // Fetch save data.
        // MenuManager.I.saveData = Utility.GetSaveData();

        // Set planet index.
        StarManager.I.planetIndex = MenuManager.I.saveData.currentPlanetIndex;

        // Get bio for player's leader.
        LeaderBio bio = leaderBios[MenuManager.I.saveData.leaderName];

        // Load the player's leader.
        DM.I.goodLeader.LoadBio(bio);

        // Go to star map!
        StarManager.I.GoToStarMap(true);
    }


    // + Skirmish

    // Choose yer hero!
    public void B_SkirmishHeroOpen()
    {
        // SFX
        DJ.I.PlaySound("Snap");

        // Close if open.
        if (heroWheel.activeSelf)
        {
            heroWheel.SetActive(false);
            return;
        }

        // Open yer wheel of heroes!
        heroWheel.SetActive(true);
    }

    // Spin yer wheel of heroes!
    public void B_Spin()
    {
        // SFX
        DJ.I.PlaySound("Snap");

        // Roll length of spin.
        wheelSpinTime = Random.Range(6f, 30f);
    }

    // Stop spinning yer wheel of heroes!
    public void B_Stop()
    {
        // SFX
        DJ.I.PlaySound("Snap");

        // End spin time.
        wheelSpinTime = 0f;
    }

    // Called by clicking a portrait on yer hero wheel.
    public void B_SkirmishHeroChoose(string heroName)
    {
        // SFX
        DJ.I.PlaySound("Snap");

        // Set skirmish hero.
        skirmishHero = heroName;

        // Save to player prefs.
        PlayerPrefs.SetString("skirmishHero", heroName);

        // Load portrait.
        Utility.LoadImage(skirmishPortrait, "Leaders/" + skirmishHero);

        // Close hero wheel.
        heroWheel.SetActive(false);
    }

    // Begin a skirmish!
    public void B_QuickPlay()
    {
        // SFX
        DJ.I.PlaySound("Snap 2");

        // Set game mode.
        DM.I.gameMode = "Skirmish";

        // Go to random planet.
        StarManager.I.GoToRandomPlanet(skirmishDifficulty);

        // Get random evil leader.
        DM.I.evilLeader.LoadRandomVillain(skirmishDifficulty);

        // Get yer hero's biography.
        LeaderBio bio = leaderBios[skirmishHero];

        // Load yer hero for battle mode.
        DM.I.goodLeader.LoadBio(bio);

        // Randomize decks.
        DM.I.goodLeader.RandomizeDeck(2 - skirmishDifficulty);
        DM.I.evilLeader.RandomizeDeck(skirmishDifficulty * skirmishDifficulty);

        // Begin battle!
        DM.I.BeginBattle();
    }



    // Select difficulty for skirmish.
    public void B_SkirmishDifficulty(int difficulty)
    {
        // SFX
        DJ.I.PlaySound("Snap");

        // Set difficulty.
        skirmishDifficulty = difficulty;

        // Save to player prefs.
        PlayerPrefs.SetInt("skirmishDifficulty", skirmishDifficulty);

        // Highlight selected difficulty.
        for (int i = 0; i < skirmishDifficulties.Count; i++)
        {
            // Highlight chosen.
            if (i == difficulty)
                skirmishDifficulties[i].alpha = 1f;
            // Fade the rest.
            else
                skirmishDifficulties[i].alpha = 0.1f;
        }
    }


    // + Learn

    // Begin goldfishing!
    public void B_Goldfish()
    {
        // SFX
        DJ.I.PlaySound("Snap 2");

        // Set game mode.
        DM.I.gameMode = "Goldfish";
        
        // Get goldfish bio.
        LeaderBio bio = leaderBios["Goldfish"];

        // Go to random planet.
        StarManager.I.GoToRandomPlanet(skirmishDifficulty);

        // Load leaders.
        DM.I.goodLeader.LoadBio(bio);
        DM.I.evilLeader.LoadBio(bio);

        // Randomize yer deck.
        DM.I.goodLeader.RandomizeDeck(0);

        // Begin battle!
        DM.I.BeginBattle();
    }
}
