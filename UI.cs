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
    // The green fill for the good leader's health bar.
    public Image goodHealth;

    // The green fill for the evil leader's health bar.
    public Image evilHealth;

    [Header("Portraits")]
    // The portrait showing who is leading the forces of good.
    public Image goodPortrait;

    // The text saying the name of the good leader.
    public TMP_Text goodName;

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

    [Header("Auto Pilot")]
    public Button autoPilotOn;
    public Button autoPilotOff;

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

        // Start auto pilot off.
        B_AutoPilotOff();

        // Make sure fog of war is on!
        fogOfWar.gameObject.SetActive(true);
    }

    // Called once at the beginning of each battle.
    // (See GM.BeginBattle)
    public void BeginBattle()
    {
        // Hide what should not be.
        victoryBackground.gameObject.SetActive(false);
        defeatBackground.gameObject.SetActive(false);
        HideTooltip();
        reservesDepleted.gameObject.SetActive(false);
        reinforcementsArrived.gameObject.SetActive(false);

        // Load the current planet's image into the background.
        Utility.LoadImage(battleBackground, "Planets/" + StarManager.I.GetCurrentPlanetName());

        // Load the good leader's name.
        goodName.text = GM.I.goodLeader.myName;

        // Load the good leader's portrait.
        Utility.LoadImage(goodPortrait, "Leaders/" + GM.I.goodLeader.myName);
    }

    // + Battle

    // Fixed update.
    void FixedUpdate()
    {
        // Update mana.
        UpdateMana();

        // Update health.
        UpdateHealth();

        // Update reinforcements.
        UpdateReinforcements();
    }

    // Update the player's mana icon to show progress toward gaining mana,
    // as well as updating the mana text when mana is gained or spent.
    void UpdateMana()
    {
        // Set text.
        manaText.text = GM.I.goodLeader.mana.ToString();

        // Get percent toward next mana.
        float percent = (GM.I.goodLeader.secondsPerMana - GM.I.goodLeader.manaTimer) / GM.I.goodLeader.secondsPerMana;

        // Set image fill.
        manaSymbol.fillAmount = percent;
    }

    // Update both good and evil leader's health bars to match their current health.
    public void UpdateHealth()
    {
        // Get percentage of good leader's health.
        float goodPercent = GM.I.goodLeader.health / GM.I.startingHealth;

        // Set fill.
        goodHealth.fillAmount = goodPercent;

        // Get percentage of evil leader's health.
        float evilPercent = GM.I.evilLeader.health / GM.I.startingHealth;

        // Set fill.
        evilHealth.fillAmount = evilPercent;
    }

    // Fade out reinforcement popups.
    public void UpdateReinforcements()
    {
        // Check if we need reinforcements.
        if (GM.I.goodLeader.deck.Count == 0)
        {
            // Reserves depleted?
            if (reservesDepleted.gameObject.activeSelf)
            {
                // Get percent toward reinforcements.
                float percent = GM.I.goodLeader.reinforcementTimer / GM.I.goodLeader.timeUntilReinforcements;

                // Fade out.
                // reservesDepleted.alpha -= 0.01f;
                reservesDepleted.alpha = percent;

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
        if (InputManager.I.selectedCard == null) return;

        // Do nothing if card is hidden.
        if (InputManager.I.selectedCard.hideTimer > 0f) return;
        
        // Discard the currently selected card for good.
        GM.I.goodLeader.Discard(InputManager.I.selectedCard.index);
    }

    // Button pressed: Auto Pilot On
    public void B_AutoPilotOn()
    {
        // Enable auto pilot.
        GM.I.goodLeader.autoPilot = true;

        // Disable auto pilot on button.
        autoPilotOn.gameObject.SetActive(false);

        // Enable auto pilot off button.
        autoPilotOff.gameObject.SetActive(true);
    }

    // Button pressed: Auto Pilot Off
    public void B_AutoPilotOff()
    {
        // Disable auto pilot.
        GM.I.goodLeader.autoPilot = false;

        // Disable auto pilot off button.
        autoPilotOff.gameObject.SetActive(false);

        // Enable auto pilot on button.
        autoPilotOn.gameObject.SetActive(true);
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
        Utility.LoadImage(tooltipImage, "Cards/" + unit.myName);

        // Load the background image.
        Utility.LoadImage(tooltipBackgroundImage, "Cards/" + unit.myName);

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
