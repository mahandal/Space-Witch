using UnityEngine;
using UnityEngine.UI;
using TMPro;

// Handle the post game screen.
public class PostGame : MonoBehaviour
{
    [Header("Show Time!")]
    // Timer tracking sequencing.
    public float showTimer = 0f;

    // Whether we won or lost.
    public bool victorious = true;

    // Good's score.
    public int good_score = 0;
    
    // Evil's score.
    public int evil_score = 0;

    // + Neutral
    [Header("Neutral - Meta")]
    // The image in the background of the post game.
    // Also the parent object of the rest of the post game screen.
    public Image background;

    // The canvas group for the neutral meta data in the top.
    public CanvasGroup neutralMetaCanvasGroup;

    // The post game header saying "GOOD VICTORY!" or something similar.
    public TMP_Text header;

    // The text object for the time.
    public TMP_Text time;

    // The text object for the planet.
    public TMP_Text planet;

    // Button to continue onward, visible after a victory.
    public Button continueButton;

    // Button to return to the main menu, visible after defeat.
    public Button returnButton;

    [Header("MVP")]
    // The canvas group for the MVP.
    public CanvasGroup mvpCanvasGroup;

    // The canvas group for leveling up the MVP.
    public CanvasGroup mvpLevelCanvasGroup;

    // The portrait for the MVP.
    public Image mvpPortrait;

    // The MVP's name.
    public TMP_Text mvpName;

    // The MVP's accolades.
    public TMP_Text mvpAccolades;

    // The text saying 'From level 2 to level 3'
    public TMP_Text mvpLevelUpText;

    [Header("Stardust")]
    // The canvas group for stardust.
    public CanvasGroup stardustCanvasGroup;

    // The text object for yer old stardust count.
    public TMP_Text oldStardust;

    // The text object for yer new stardust count.
    public TMP_Text newStardust;


    // + GOOD

    [Header("Good - Meta")]
    // The canvas group for good's meta data.
    public CanvasGroup goodMetaCanvasGroup;

    // The text object for the good leader's name.
    public TMP_Text goodLeaderName;

    // The image object for the good leader's portrait.
    public Image goodLeaderPortrait;

    // The text object for good's remaining health.
    public TMP_Text goodRemainingHealth;

    // The text object for good's max health.
    public TMP_Text goodMaxHealth;

    // The fill for good's health bar.
    public Image goodHealthFill;


    [Header("Good - Mana")]
    // The canvas group for good's mana.
    public CanvasGroup goodManaCanvasGroup;

    // The text object for good's mana generated passively.
    public TMP_Text goodManaGenerated;

    // The text object for good's mana gathered.
    public TMP_Text goodManaGathered;

    // The text object for good's total mana.
    public TMP_Text goodManaTotal;


    [Header("Good - Cards")]
    // The canvas group for good's cards.
    public CanvasGroup goodCardCanvasGroup;

    // The text object for good's unit count.
    public TMP_Text goodUnitCount;

    // The text object for good's spell count.
    public TMP_Text goodSpellCount;

    // The text object for good's item count.
    public TMP_Text goodItemCount;

    // The text object for good's structure count.
    public TMP_Text goodStructureCount;

    // The text object for good's total number of cards played.
    public TMP_Text goodCardTotal;


    [Header("Good - Combat")]
    // The canvas group for good's combat.
    public CanvasGroup goodCombatCanvasGroup;

    // The text object for good's damage dealt.
    public TMP_Text goodDamage;

    // The text object for good's healing done.
    public TMP_Text goodHealing;

    // The text object for good's kill count.
    public TMP_Text goodKills;

    // The text object for good's loss count.
    public TMP_Text goodLosses;

    [Header("Good - Score")]
    // The canvas group for good's score.
    public CanvasGroup goodScoreCanvasGroup;

    // The text object for good's score.
    public TMP_Text goodScore;





    // + EVIL


    [Header("Evil - Meta")]
    // The canvas group for evil's meta data.
    public CanvasGroup evilMetaCanvasGroup;

    // The text object for the evil leader's name.
    public TMP_Text evilLeaderName;

    // The image object for the evil leader's portrait.
    public Image evilLeaderPortrait;

    // The text object for evil's remaining health.
    public TMP_Text evilRemainingHealth;

    // The text object for evil's max health.
    public TMP_Text evilMaxHealth;

    // The fill for evil's health bar.
    public Image evilHealthFill;


    [Header("Evil - Mana")]
    // The canvas group for evil's mana.
    public CanvasGroup evilManaCanvasGroup;

    // The text object for evil's mana generated passively.
    public TMP_Text evilManaGenerated;

    // The text object for evil's mana gathered.
    public TMP_Text evilManaGathered;

    // The text object for evil's total mana.
    public TMP_Text evilManaTotal;


    [Header("Evil - Cards")]
    // The canvas group for evil's cards.
    public CanvasGroup evilCardCanvasGroup;

    // The text object for evil's unit count.
    public TMP_Text evilUnitCount;

    // The text object for evil's spell count.
    public TMP_Text evilSpellCount;

    // The text object for evil's item count.
    public TMP_Text evilItemCount;

    // The text object for evil's structure count.
    public TMP_Text evilStructureCount;

    // The text object for evil's total number of cards played.
    public TMP_Text evilCardTotal;


    [Header("Evil - Combat")]
    // The canvas group for evil's combat.
    public CanvasGroup evilCombatCanvasGroup;

    // The text object for evil's damage dealt.
    public TMP_Text evilDamage;

    // The text object for evil's healing done.
    public TMP_Text evilHealing;

    // The text object for evil's kill count.
    public TMP_Text evilKills;

    // The text object for evil's loss count.
    public TMP_Text evilLosses;

    [Header("Evil - Score")]
    // The canvas group for evil's score.
    public CanvasGroup evilScoreCanvasGroup;

    // The text object for evil's score.
    public TMP_Text evilScore;




     // Singleton.
    public static PostGame I;

    // + Initialization

    // Awaken!
    void Awake()
    {
        // Singleton.
        if (I == null)
            I = this;
    }

    // Reset everything.
    void Reset()
    {
        // Show timer.
        showTimer = 0f;

        // Hide canvas groups.
        mvpCanvasGroup.gameObject.SetActive(false);
        mvpLevelCanvasGroup.gameObject.SetActive(false);
        stardustCanvasGroup.gameObject.SetActive(false);
        neutralMetaCanvasGroup.gameObject.SetActive(false);
        goodMetaCanvasGroup.gameObject.SetActive(false);
        evilMetaCanvasGroup.gameObject.SetActive(false);
        goodManaCanvasGroup.gameObject.SetActive(false);
        evilManaCanvasGroup.gameObject.SetActive(false);
        goodCardCanvasGroup.gameObject.SetActive(false);
        evilCardCanvasGroup.gameObject.SetActive(false);
        goodCombatCanvasGroup.gameObject.SetActive(false);
        evilCombatCanvasGroup.gameObject.SetActive(false);
        goodScoreCanvasGroup.gameObject.SetActive(false);
        evilScoreCanvasGroup.gameObject.SetActive(false);

        // Hide buttons.
        continueButton.gameObject.SetActive(false);
        returnButton.gameObject.SetActive(false);

        // Reset background color.
        background.color = new Color(0f, 0f, 0f, 1f);
    }

    // Game over!
    // Time to set up the post game screen!
    public void GameOver(bool victory)
    {
        // + Reset
        Reset();

        // Set bool.
        victorious = victory;

        // Get leaders.
        Leader g = DM.I.goodLeader;
        Leader e = DM.I.evilLeader;
        // Victory
        if (victory)
        {
            // Load random background image.
            string imageFilePath = "Victory/Win " + UnityEngine.Random.Range(1, 101).ToString();
            Utility.LoadImage(background, imageFilePath);

            // Set header.
            header.text = "GOOD VICTORY!";
            header.color = new Color(0.26f, 0.62f, 0.58f);
        }
        // Defeat
        else
        {
            // Load random background image.
            string imageFilePath = "Defeat/Loss " + UnityEngine.Random.Range(1, 101).ToString();
            Utility.LoadImage(background, imageFilePath);

            // Set header text.
            header.text = "YE LOST!";
            header.color = Color.red;
        }

        // Enable background image.
        background.gameObject.SetActive(true);


        // + Neutral
        // Time
        time.text = Utility.TimeString(DM.I.gameTimer);

        // Planet
        planet.text = StarManager.I.GetCurrentPlanet().myName;


        // +++ Good
        // + Meta
        // Good leader name.
        goodLeaderName.text = g.myName;

        // Good leader portrait.
        Utility.LoadImage(goodLeaderPortrait, "Leaders/" + g.myName);

        // + Health
        // Good remaining health.
        goodRemainingHealth.text = g.currentHealth.ToString("0");

        // Good max health.
        goodMaxHealth.text = g.maxHealth.ToString("0");

        // Good health percent.
        float goodHealthPercent = g.currentHealth / g.maxHealth;
        goodHealthFill.fillAmount = goodHealthPercent;

        // + Mana
        goodManaGenerated.text = "0";
        goodManaGathered.text = "0";
        goodManaTotal.text = "0";

        // + Cards
        goodUnitCount.text = "0";
        goodSpellCount.text = "0";
        goodItemCount.text = "0";
        goodStructureCount.text = "0";
        goodCardTotal.text = "0";


        // + Combat
        goodDamage.text = "0";
        goodHealing.text = "0";
        goodKills.text = "0";
        goodLosses.text = "0";

        // + Score
        goodScore.text = "0";
        good_score = CalculateScore(g);

        // + Stardust
        // Set text.
        oldStardust.text = MenuManager.I.saveData.stardust.ToString();
        newStardust.text = MenuManager.I.saveData.stardust.ToString();

        // Add score to stardust.
        MenuManager.I.saveData.stardust += good_score;


        // +++ Evil

        // + Meta
        // Evil leader name.
        evilLeaderName.text = e.myName;

        // Evil leader portrait.
        Utility.LoadImage(evilLeaderPortrait, "Leaders/" + e.myName);

        // + Health
        // Evil remaining health.
        evilRemainingHealth.text = e.currentHealth.ToString("0");

        // Evil max health.
        evilMaxHealth.text = e.maxHealth.ToString("0");

        // Evil health percent.
        float evilHealthPercent = e.currentHealth / e.maxHealth;
        evilHealthFill.fillAmount = evilHealthPercent;

        // + Mana
        evilManaGenerated.text = "0";
        evilManaGathered.text = "0";
        evilManaTotal.text = "0";

        // + Cards
        evilUnitCount.text = "0";
        evilSpellCount.text = "0";
        evilItemCount.text = "0";
        evilStructureCount.text = "0";
        evilCardTotal.text = "0";

        // + Combat
        evilDamage.text = "0";
        evilHealing.text = "0";
        evilKills.text = "0";
        evilLosses.text = "0";

        // + Score
        evilScore.text = "0";
        evil_score = CalculateScore(e);
    }


    // + Running the Show
    void FixedUpdate()
    {
        // Shorthand for leaders.
        Leader g = DM.I.goodLeader;
        Leader e = DM.I.evilLeader;

        // Get MVP.
        Unit mvp = g.mvpCandidate;
        if (!victorious)
            mvp = e.mvpCandidate;

        // Show timer.
        showTimer += Time.fixedDeltaTime;

        // For the first second, fade in the background.
        if (showTimer < 1f)
        {
            float a = showTimer / 2f;
            background.color = new Color (a, a, a, 1f);
        } else {
            background.color = new Color(0.5f, 0.5f, 0.5f, 1f);
        }

        // After 1 second, reveal neutral meta data.
        if (showTimer >= 1f)
        {
            // Activate game object.
            if (!neutralMetaCanvasGroup.gameObject.activeSelf)
                neutralMetaCanvasGroup.gameObject.SetActive(true);
        }

        // After 2 seconds, reveal good and evil meta data.
        if (showTimer >= 2f)
        {
            // Activate game objects.
            if (!goodMetaCanvasGroup.gameObject.activeSelf)
                goodMetaCanvasGroup.gameObject.SetActive(true);
            if (!evilMetaCanvasGroup.gameObject.activeSelf)
                evilMetaCanvasGroup.gameObject.SetActive(true);
        }

        // After 3 seconds, reveal good and evil mana.
        if (showTimer >= 3f)
        {
            // Activate game objects.
            if (!goodManaCanvasGroup.gameObject.activeSelf)
                goodManaCanvasGroup.gameObject.SetActive(true);
            if (!evilManaCanvasGroup.gameObject.activeSelf)
                evilManaCanvasGroup.gameObject.SetActive(true);

            // Good
            Utility.IncrementText(goodManaGenerated, 1, g.manaGenerated);
            Utility.IncrementText(goodManaGathered, 1, g.manaGathered);
            Utility.IncrementText(goodManaTotal, 1, g.manaGathered + g.manaGenerated); 

            // Evil
            Utility.IncrementText(evilManaGenerated, 1, e.manaGenerated);
            Utility.IncrementText(evilManaGathered, 1, e.manaGathered);
            Utility.IncrementText(evilManaTotal, 1, e.manaGathered + e.manaGenerated); 
        }

        // After 4 seconds, reveal good and evil cards.
        if (showTimer >= 4f)
        {
            // Activate game objects.
            if (!goodCardCanvasGroup.gameObject.activeSelf)
                goodCardCanvasGroup.gameObject.SetActive(true);
            if (!evilCardCanvasGroup.gameObject.activeSelf)
                evilCardCanvasGroup.gameObject.SetActive(true);

            // Good
            Utility.IncrementText(goodUnitCount, 1, g.unitsPlayed);
            Utility.IncrementText(goodSpellCount, 1, g.spellsPlayed);
            Utility.IncrementText(goodItemCount, 1, g.itemsPlayed);
            Utility.IncrementText(goodStructureCount, 1, g.structuresPlayed);
            Utility.IncrementText(goodCardTotal, 1, g.unitsPlayed + g.spellsPlayed + g.itemsPlayed + g.structuresPlayed);

            // Evil
            Utility.IncrementText(evilUnitCount, 1, e.unitsPlayed);
            Utility.IncrementText(evilSpellCount, 1, e.spellsPlayed);
            Utility.IncrementText(evilItemCount, 1, e.itemsPlayed);
            Utility.IncrementText(evilStructureCount, 1, e.structuresPlayed);
            Utility.IncrementText(evilCardTotal, 1, e.unitsPlayed + e.spellsPlayed + e.itemsPlayed + e.structuresPlayed);
        }

        // After 5 seconds, reveal good and evil combat.
        if (showTimer >= 5f)
        {
            // Activate game objects.
            if (!goodCombatCanvasGroup.gameObject.activeSelf)
                goodCombatCanvasGroup.gameObject.SetActive(true);
            if (!evilCombatCanvasGroup.gameObject.activeSelf)
                evilCombatCanvasGroup.gameObject.SetActive(true);

            // Good
            Utility.IncrementText(goodDamage, 1, Mathf.RoundToInt(g.damageDealt));
            Utility.IncrementText(goodHealing, 1, Mathf.RoundToInt(g.healingDone));
            Utility.IncrementText(goodKills, 1, g.kills);
            Utility.IncrementText(goodLosses, 1, g.losses);

            // Evil
            Utility.IncrementText(evilDamage, 1, Mathf.RoundToInt(e.damageDealt));
            Utility.IncrementText(evilHealing, 1, Mathf.RoundToInt(e.healingDone));
            Utility.IncrementText(evilKills, 1, e.kills);
            Utility.IncrementText(evilLosses, 1, e.losses);
        }

        // After 6 seconds, reveal score.
        if (showTimer >= 6f)
        {
            // Activate score game objects.
            if (!goodScoreCanvasGroup.gameObject.activeSelf)
                goodScoreCanvasGroup.gameObject.SetActive(true);
            if (!evilScoreCanvasGroup.gameObject.activeSelf)
                evilScoreCanvasGroup.gameObject.SetActive(true);

            // Activate stardust game object.
            if (!stardustCanvasGroup.gameObject.activeSelf)
                stardustCanvasGroup.gameObject.SetActive(true);

            // Good
            Utility.IncrementText(goodScore, 1, good_score);

            // Evil
            Utility.IncrementText(evilScore, 1, evil_score);

            // Stardust
            Utility.IncrementText(newStardust, 1, MenuManager.I.saveData.stardust);
        }

        // After 7 seconds, reveal mvp.
        if (showTimer >= 7f && !mvpCanvasGroup.gameObject.activeSelf)
        {
            // Load portrait.
            Utility.LoadImage(mvpPortrait, "Cards/" + mvp.GetBaseName());

            // Set name.
            mvpName.text = mvp.myName;

            // Write accolades.
            mvpAccolades.text = MVPAccolades(mvp);

            // Activate.
            mvpCanvasGroup.gameObject.SetActive(true);
        }

        // After 8 seconds, reveal mvp level up and continue and return buttons.
        if (showTimer >= 8f)
        {
            // If we won, reveal continue button.
            // If we lost, reveal return button.
            if (victorious)
                continueButton.gameObject.SetActive(true);
            else
                returnButton.gameObject.SetActive(true);

            // MVP levels up if they are good and already in your deck.
            if (mvp.good && mvp.level > 1)
            {
                // Prevent double leveling.
                if (!mvpLevelCanvasGroup.gameObject.activeSelf)
                {
                    // Set level up text.
                    int nextLevel = mvp.level + 1;
                    mvpLevelUpText.text = "From level " + mvp.level + " to level " + nextLevel;

                    // Reveal level up canvas.
                    mvpLevelCanvasGroup.gameObject.SetActive(true);

                    // Level up card in your deck.
                    MenuManager.I.saveData.LevelUp(mvp.myName);
                }
            }
        }
    }

    // Write accolades for an MVP.
    public string MVPAccolades(Unit mvp)
    {
        // Have it all?
        if (mvp.manaGathered > 0 && mvp.kills > 0)
        {
            // Multiple flowers and kills.
            if (mvp.manaGathered > 1 && mvp.kills > 1)
            {
                return "This heroic " + mvp.GetBaseName() + " survived for " + Utility.TimeString(mvp.timeAlive)
                        + ", gathered " + mvp.manaGathered + " flowers"
                        + ", dealt " + mvp.damageDealt.ToString("0")  + " damage"
                        + ", and slew " + mvp.kills + " foes!";
            }
            // Multiple flowers, single kill.
            else if (mvp.manaGathered > 1)
            {
                return "This graceful " + mvp.GetBaseName() + " survived for " + Utility.TimeString(mvp.timeAlive)
                        + ", gathered " + mvp.manaGathered + " flowers"
                        + ", dealt " + mvp.damageDealt.ToString("0")  + " damage"
                        + ", and slew a mighty foe!";
            }
            // Single flower, multiple kills.
            else if (mvp.kills > 1)
            {
                return "This daring " + mvp.GetBaseName() + " survived for " + Utility.TimeString(mvp.timeAlive)
                        + ", gathered a magical flower"
                        + ", dealt " + mvp.damageDealt.ToString("0")  + " damage"
                        + ", and slew " + mvp.kills + " foes!";
            }
            // Single flower, single kill.
            else
            {
                return "This beautiful " + mvp.GetBaseName() + " survived for " + Utility.TimeString(mvp.timeAlive)
                        + ", gathered a pretty flower"
                        + ", dealt " + mvp.damageDealt.ToString("0")  + " damage"
                        + ", and bested a foe in combat!";
            }

        // Gatherer?
        } else if (mvp.manaGathered > 0)
        {
            // Pacifist?
            if (mvp.damageDealt <= 0)
            {
                if (mvp.manaGathered > 1)
                    return "This most noble " + mvp.GetBaseName() + " gathered " + mvp.manaGathered + " flowers!";
                else
                    return "Aren't they just the sweetest thing?";
            } else {
                if (mvp.manaGathered > 1)
                    return "This noble " + mvp.GetBaseName() + " gathered " + mvp.manaGathered + " flowers!";
                else
                    return "What a cool " + mvp.GetBaseName() + "!";
            }

        // Hunter?
        } else if (mvp.kills > 0)
        {
            if (mvp.kills > 1)
            {
                return "The strongest survive, and this " + mvp.GetBaseName() + " is the strongest! "
                        + mvp.kills + " foes lay slain by their might! "
                        + mvp.damageDealt.ToString("0") + " damage did they deal!";
            } else {
                return "MVP! MVP! MVP!";
            }
        }
        // Weak set prolly?
        else
        {
            return "Even the bottom of the barrel has its best day!";
        }
        
    }

    // Calculate a leader's score.
    public int CalculateScore(Leader leader)
    {
        // Initialize score.
        int score = 0;

        // + Health.
        score += Mathf.RoundToInt(leader.currentHealth / 13f);

        // + Mana.
        score += leader.manaGenerated;
        score += leader.manaGathered;

        // + Cards.
        score += leader.unitsPlayed;
        score += leader.spellsPlayed;
        score += leader.itemsPlayed;
        score += leader.structuresPlayed;

        // + Combat
        score += Mathf.RoundToInt(leader.damageDealt / 100f);
        score += Mathf.RoundToInt(leader.healingDone / 100f);
        score += leader.kills;
        score += leader.losses;

        // Return.
        return score;
    }
}
