using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI : MonoBehaviour
{
    [Header("Battle Background")]
    public SpriteRenderer battleBackground;

    [Header("Mana")]
    // The text object displaying how much mana the player currently has.
    public TMP_Text manaText;

    // The mana symbol showing the player's progress toward gaining mana.
    public Image manaSymbol;

    [Header("Health")]
    // The fill for the good leader's health bar.
    public Image goodHealth;

    // The text for the good leader's current health.
    public TMP_Text goodCurrentHealth;

    // The text for the good leader's max health.
    public TMP_Text goodMaxHealth;

    // The fill for the evil leader's health bar.
    public Image evilHealth;

    // The text for the evil leader's current health.
    public TMP_Text evilCurrentHealth;

    // The text for the evil leader's max health.
    public TMP_Text evilMaxHealth;

    [Header("Portraits")]
    // The portrait showing who is leading the forces of good.
    public Image goodPortrait;

    // The text saying the name of the good leader.
    public TMP_Text goodName;

    // The image saying VERSUS at the start of the game.
    public CanvasGroup versus;

    // The portrait showing who is leading the forces of evil.
    public Image evilPortrait;

    // The text saying the name of the evil leader.
    public TMP_Text evilName;

    [Header("Trash Can")]
    public GameObject trashCan;

    [Header("Popups for Reinforcements")]
    public CanvasGroup reservesDepleted;
    public CanvasGroup reinforcementsArrived;

    [Header("Tooltip")]
    // The parent object of the tooltip.
    public CanvasGroup tooltip;

    // The name in the tooltip.
    public TMP_Text tooltipName;

    // The mana cost in the tooltip.
    public TMP_Text tooltipMana;

    // The deployment time in the tooltip.
    public TMP_Text tooltipTime;

    // The text for the role in the tooltip.
    public TMP_Text tooltipRole;

    // The image for the role in the tooltip.
    public Image tooltipRoleImage;

    // The main image for the tooltip.
    public Image tooltipImage;

    // The background image for the tooltip.
    public Image tooltipBackgroundImage;

    // The current health text in the tooltip.
    public TMP_Text tooltipCurrentHealth;

    // The max health text in the tooltip.
    public TMP_Text tooltipMaxHealth;

    // The health fill for the tooltip.
    public Image tooltipHealthFill;

    [Header("Post game")]
    // The parent object for the victory post game screen.
    public Image victoryBackground;

    // The parent object for the defeat post game screen.
    public Image defeatBackground;

    [Header("Fog of War")]
    public SpriteRenderer fogOfWar;

    // Singleton.
    public static UI I;

    // + Initialization

    // Awaken!
    void Awake()
    {
        // Singleton.
        if (I == null)
            I = this;
        else
            Destroy(this);

        // Make sure fog of war is on!
        fogOfWar.gameObject.SetActive(true);
    }

    // Called once at the beginning of each battle.
    // (See DM.BeginBattle)
    public void BeginBattle()
    {
        // Hide what should not be.
        victoryBackground.gameObject.SetActive(false);
        defeatBackground.gameObject.SetActive(false);
        HideTooltip();
        reservesDepleted.gameObject.SetActive(false);
        reinforcementsArrived.gameObject.SetActive(false);
        versus.gameObject.SetActive(false);

        // Load the current planet's image into the background.
        Utility.LoadImage(battleBackground, "Planets/" + StarManager.I.GetCurrentPlanetName());

        // Hide leader names and portraits to begin with.
        goodName.gameObject.SetActive(false);
        evilName.gameObject.SetActive(false);
        goodPortrait.gameObject.SetActive(false);
        evilPortrait.gameObject.SetActive(false);

        // Load leader names.
        goodName.text = DM.I.goodLeader.myName;
        evilName.text = DM.I.evilLeader.myName;

        // Load leader portraits.
        Utility.LoadImage(goodPortrait, "Leaders/" + DM.I.goodLeader.myName);
        Utility.LoadImage(evilPortrait, "Leaders/" + DM.I.evilLeader.myName);

        // + Ways
        // Hunter
        if (DM.I.way == "Hunter")
        {
            // Set health bar colors.
            goodHealth.color = Color.green;
            evilHealth.color = Color.red;

            // Set max health text.
            goodMaxHealth.text = DM.I.goodLeader.health.ToString("0");
            evilMaxHealth.text = DM.I.evilLeader.health.ToString("0");
        }

        // Gatherer
        if (DM.I.way == "Gatherer")
        {
            // Set health bar colors.
            goodHealth.color = Color.violet;
            evilHealth.color = Color.violet;

            // Set goal text.
            goodMaxHealth.text = "42";
            evilMaxHealth.text = "42";
        }
    }

    // + Battle

    // Fixed update.
    void FixedUpdate()
    {
        // Versus sequence
        Versus();

        // Update reinforcements.
        UpdateReinforcements();

        // Update mana.
        UpdateMana();

        // + Top bar
        // Hunter - Update health.
        if (DM.I.way == "Hunter")
            UpdateHealth();

        // Gatherer - Count flowers.
        if (DM.I.way == "Gatherer")
            CountFlowers();
    }

    // Dramatic intro saying leader names VERSUS each other.
    // 
    void Versus()
    {
        // Time to reveal the good leader?
        if (DM.I.gameTimer >= 1f && !goodPortrait.gameObject.activeSelf)
        {
            // Reveal the good leader.
            goodPortrait.gameObject.SetActive(true);
            goodName.gameObject.SetActive(true);
        }

        // Time to reveal VERSUS?
        if (DM.I.gameTimer >= 2f && DM.I.gameTimer < 3f && !versus.gameObject.activeSelf)
        {
            versus.gameObject.SetActive(true);
            versus.alpha = 1f;
        }

        // Time to reveal the evil leader?
        if (DM.I.gameTimer >= 3f && !evilPortrait.gameObject.activeSelf)
        {
            // Reveal the evil leader.
            evilPortrait.gameObject.SetActive(true);
            evilName.gameObject.SetActive(true);
        }

        // Time to hide VERSUS?
        if (DM.I.gameTimer >= 4f && versus.gameObject.activeSelf)
        {
            // Fade versus.
            versus.alpha -= 0.01f;

            // Faded?
            if (versus.alpha <= 0f)
                versus.gameObject.SetActive(false);
        }
    }

    // Update the player's mana icon to show progress toward gaining mana,
    // as well as updating the mana text when mana is gained or spent.
    void UpdateMana()
    {
        // Set text.
        manaText.text = DM.I.goodLeader.mana.ToString();

        // Get percent toward next mana.
        float percent = (DM.I.goodLeader.secondsPerMana - DM.I.goodLeader.manaTimer) / DM.I.goodLeader.secondsPerMana;

        // Set image fill.
        manaSymbol.fillAmount = percent;
    }

    // Update both good and evil leader's health bars to match their current health.
    public void UpdateHealth()
    {
        // + Good
        // Set good current health.
        goodCurrentHealth.text = DM.I.goodLeader.health.ToString("0");

        // Get good max health.
        float goodMax = float.Parse(goodMaxHealth.text);

        // Get percentage of good leader's health.
        float goodPercent = DM.I.goodLeader.health / goodMax;

        // Set fill.
        goodHealth.fillAmount = goodPercent;

        // + Evil
        // Set evil current health.
        evilCurrentHealth.text = DM.I.evilLeader.health.ToString("0");

        // Get evil max health.
        float evilMax = float.Parse(evilMaxHealth.text);

        // Get percentage of evil leader's health.
        float evilPercent = DM.I.evilLeader.health / evilMax;

        // Set fill.
        evilHealth.fillAmount = evilPercent;
    }

    // Update both good and evil leader's progress bars to match their current flower count.
    public void CountFlowers()
    {
        // + Good
        // Set good current flower count.
        goodCurrentHealth.text = DM.I.goodLeader.flowersGathered.ToString("0");

        // Get percentage of good leader's progress toward victory.
        float goodPercent = DM.I.goodLeader.flowersGathered / 42f;

        // Set fill.
        goodHealth.fillAmount = goodPercent;

        // + Evil
        // Set evil current flower count.
        evilCurrentHealth.text = DM.I.evilLeader.flowersGathered.ToString("0");

        // Get percentage of evil leader's progress toward victory.
        float evilPercent = DM.I.evilLeader.flowersGathered / 42f;

        // Set fill.
        evilHealth.fillAmount = evilPercent;
    }

    // Fade out reinforcement popups.
    public void UpdateReinforcements()
    {
        // Check if we need reinforcements.
        if (DM.I.goodLeader.deck.Count == 0)
        {
            // Reserves depleted?
            if (reservesDepleted.gameObject.activeSelf)
            {
                // Fade out.
                reservesDepleted.alpha -= 0.01f;

                // Done?
                if (reservesDepleted.alpha <= 0f)
                    reservesDepleted.gameObject.SetActive(false);
            }

            // Reinforcements arrived?
            if (reinforcementsArrived.gameObject.activeSelf)
            {
                // Fade out.
                reinforcementsArrived.alpha -= 0.01f;

                // Done?
                if (reinforcementsArrived.alpha <= 0f)
                    reinforcementsArrived.gameObject.SetActive(false);
            }
        }

    }

    // Reveal the 'Reserves Depleted' popup.
    public void ReservesDepleted()
    {
        // Set alpha to 1.
        reservesDepleted.alpha = 1f;

        // Activate game object.
        reservesDepleted.gameObject.SetActive(true);
    }

    // Reveal the 'Reinforcements Arrived' popup.
    public void ReinforcementsArrived()
    {
        // Set alpha to 1.
        reinforcementsArrived.alpha = 1f;

        // Activate game object.
        reinforcementsArrived.gameObject.SetActive(true);
    }


    // + Post game

    // Activate the post game overlay.
    public void GameOver(bool victory)
    {
        // Enable appropriate background image.
        if (victory)
            victoryBackground.gameObject.SetActive(true);
        else
            defeatBackground.gameObject.SetActive(true);
    }


    // + Buttons

    // Button pressed: Trash Can
    public void B_TrashCan()
    {
        // Do nothing if we have no card selected.
        if (InputBattle.I.selectedCard == null) return;

        // Do nothing if card is hidden.
        if (InputBattle.I.selectedCard.hideTimer > 0f) return;
        
        // Discard the currently selected card for good.
        DM.I.goodLeader.Discard(InputBattle.I.selectedCard.index);
    }

    // + Tooltip

    // Load a unit into the tooltip.
    public void ShowTooltip(Unit unit)
    {
        // Set the name.
        tooltipName.text = unit.myName;

        // Set the mana cost.
        tooltipMana.text = unit.manaCost.ToString();

        // Set the deployment time.
        tooltipTime.text = unit.deployTime.ToString();

        // Set the role.
        tooltipRole.text = unit.role;

        // Load the image for the role.
        Utility.LoadImage(tooltipRoleImage, "Roles/" + unit.role);

        // Load the main art image.
        Utility.LoadImage(tooltipImage, "Cards/" + unit.GetBaseName());

        // Load the background image.
        Utility.LoadImage(tooltipBackgroundImage, "Cards/" + unit.GetBaseName());

        // Set the current health.
        tooltipCurrentHealth.text = unit.currentHealth.ToString("0");

        // Set the max health.
        tooltipMaxHealth.text = unit.maxHealth.ToString("0");

        // Get their percent health.
        float healthPercent = unit.currentHealth / unit.maxHealth;

        // Set health fill.
        tooltipHealthFill.fillAmount = healthPercent;

        // Reveal the tooltip.
        tooltip.alpha = 1f;
    }

    // Hide the tooltip.
    public void HideTooltip()
    {
        // Hide tooltip.
        tooltip.alpha = 0f;
    }
}
