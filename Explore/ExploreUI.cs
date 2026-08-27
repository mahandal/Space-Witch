using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ExploreUI : MonoBehaviour
{
    [Header("Credits")]
    // Text object showing the player's credits.
    public TMP_Text playerCredits;

    [Header("Fade in")]
    // The black overlay that fades out so you fade into the explore screen.
    public CanvasGroup fadeIn;

    [Header("Hint")]
    // The hint saying "Press 'e' to interact!"
    public TMP_Text bottomHint;

    [Header("Credits")]
    // The credits symbol that flashes whenever you gain credits.
    public CanvasGroup creditsSymbol;

    [Header("Tooltip")]
    // The parent object of the tooltip.
    public CanvasGroup tooltip;

    // The parent object of the left side of the tooltip, containing the image and cost.
    public GameObject tooltipLeftParent;

    // The parent object of the right side of the tooltip, containing the description.
    public GameObject tooltipRightParent;

    // The name in the tooltip.
    public TMP_Text tooltipName;

    // The credit cost in the tooltip.
    public TMP_Text tooltipCredits;

    // The main image in the tooltip.
    public Image tooltipImage;

    // The background image in the tooltip.
    public Image tooltipBackground;

    // The description in the tooltip.
    public TMP_Text tooltipDescription;

    [Header("Interact: Dragon Shrine")]
    public DragonShrine dragonShrine;

    [Header("Interact: Explorer")]
    // The parent object of the interact screen.
    public CanvasGroup interactScreen;

    // The background image for the interact screen.
    public Image interactBackground;

    // The name of the exploree.
    public TMP_Text interactName;

    // The credit cost of the exploree.
    public TMP_Text interactCredits;

    // The description of the exploree.
    public TMP_Text interactDescription;

    // The big portrait in the middle of the exploree.
    public Image interactPortrait;

    // The exploree's card type.
    public TMP_Text interactCardType;

    // The exploree's role.
    public TMP_Text interactRole;

    // The exploree's mana cost.
    public TMP_Text interactManaCost;

    // The exploree's deploy time.
    public TMP_Text interactDeployTime;

    // The exploree's health.
    public TMP_Text interactHealth;

    // The exploree's armor.
    public TMP_Text interactArmor;

    // The exploree's vision.
    public TMP_Text interactVision;

    // The exploree's movement speed.
    public TMP_Text interactSpeed;

    // The exploree's attack damage.
    public TMP_Text interactDamage;

    // The exploree's attack time.
    public TMP_Text interactAttackTime;

    // The exploree's attack range.
    public TMP_Text interactRange;

    // Singleton
    public static ExploreUI I;

    // + Initialization
    public void Initialize()
    {
        // Singleton.
        if (I == null || I == this)
            I = this;
        else
            Destroy(gameObject);

        // Initialize the dragon shrine.
        dragonShrine.Initialize();

        // Hide what should be hidden.
        HideBottomHint();
        HideTooltip();
        interactScreen.gameObject.SetActive(false);
        dragonShrine.gameObject.SetActive(false);
    }

    // Set up the UI for a new planet.
    public void Explore(Planet p)
    {
        // Fade in.
        fadeIn.alpha = 1f;
        fadeIn.gameObject.SetActive(true);

        // Load planet image(?)
        Image img = fadeIn.GetComponent<Image>();
        Utility.LoadImage(img, "Planets/" + p.myName);
    }

    // + Exploring!
    void Update()
    {
        // Credits
        playerCredits.text = MenuManager.I.saveData.credits.ToString();
    }

    void FixedUpdate()
    {
        // Fade in.
        if (GM.I.player.deployTimer > 0f && GM.I.player.deployTime > 0f)
        {
            // Get percent deployed.
            float percent = GM.I.player.deployTimer / GM.I.player.deployTime;

            // Set alpha.
            fadeIn.alpha = percent;
        } else if (fadeIn.gameObject.activeSelf)
        {
            // Done?
            fadeIn.gameObject.SetActive(false);
        }

        // Credits
        if (creditsSymbol.alpha > 0f)
        {
            creditsSymbol.alpha -= 0.01f;
        }
    }

    // + Credits
    // Flash the credits symbol at the top of the screen, to show you have just gained credits.
    public void FlashCredits()
    {
        creditsSymbol.alpha = 1f;
    }

    // + Interact

    // Talking to explorers.

    // Button: Walk Away
    public void B_WalkAway()
    {
        // Close the interact screen.
        EndInteract();
    }

    // Button: Recruit
    public void B_Recruit()
    {
        // Let GM handle recruitment.
        // Just close the interact screen, if recruitment is successful.
        if (GM.I.Recruit())
            EndInteract();
    }

    // Set up the interact screen for the given interactable object.
    // TBD: Other interactables...
    public void Interact(Interactable interactable)
    {
        // Dragon Shrine.
        if (interactable.myType == "Dragon Shrine")
        {
            dragonShrine.Pray();
            return;
        }

        // TBD: Big Battle(?)
        
        // TBD: Normal interact screen(?)
    }

    // Set up the interact screen for the given explorer.
    public void Interact(Unit e)
    {
        // Load explorer's name, cost, and description.
        interactName.text = e.myName;
        interactCredits.text = e.creditCost.ToString();
        interactDescription.text = e.description;

        // Load image.
        Utility.LoadImage(interactPortrait, "Cards/" + e.myName);

        // Load background, using current planet.
        Utility.LoadImage(interactBackground, "Planets/" + StarManager.I.GetCurrentPlanetName());

        // Get progenitor.
        Unit p = Progenitors.I.GetProgenitor(e.myName);

        // Load progenitor's details.
        interactCardType.text = p.cardType;
        interactRole.text = p.role;
        interactManaCost.text = p.manaCost.ToString();
        interactDeployTime.text = p.deployTime.ToString();
        interactHealth.text = p.maxHealth.ToString();
        interactArmor.text = p.armor.ToString();
        interactVision.text = p.vision.ToString();
        interactSpeed.text = p.speed.ToString();
        interactDamage.text = p.damage.ToString();
        interactAttackTime.text = "per " + p.attackTime.ToString() + " seconds";
        interactRange.text = p.range.ToString();

        // TBD: Keywords

        // Activate.
        interactScreen.gameObject.SetActive(true);
    }

    // Close the interact screen.
    public void EndInteract()
    {
        // Let GM resume time.
        GM.I.EndInteract();

        // Deactivate screens.
        interactScreen.gameObject.SetActive(false);
        dragonShrine.gameObject.SetActive(false);
    }

    // Pop up the hint saying "Press 'e' to interact!"
    public void HintInteract()
    {
        // Set text.
        bottomHint.text = "Press 'e' to interact!";

        // Reveal.
        bottomHint.gameObject.SetActive(true);
    }

    // Hide the bottom hint.
    public void HideBottomHint()
    {
        // Hide.
        bottomHint.gameObject.SetActive(false);
    }

    // + Tooltip

    // Load a unit into the tooltip.
    public void ShowTooltip(Unit unit)
    {
        // Set description.
        tooltipDescription.text = unit.description;

        // Set name.
        tooltipName.text = unit.myName;

        // Set credit cost.
        // Player
        if (unit == GM.I.player)
            tooltipCredits.text = "∞";
        // Errbody else
        else
            tooltipCredits.text = unit.creditCost.ToString();

        // Load main art image.
        Utility.LoadImage(tooltipImage, "Cards/" + unit.myName);

        // Load background image.
        Utility.LoadImage(tooltipBackground, "Cards/" + unit.myName);

        // Reveal both sides.
        tooltipLeftParent.SetActive(true);
        tooltipRightParent.SetActive(true);

        // Reveal tooltip.
        // tooltip.alpha = 1f;
    }

    // Load a region's description into the tooltip.
    public void ShowTooltip(Region region)
    {
        // Need a description.
        if (region.description == "")
        {
            HideTooltip();
            return;
        }

        // Set description.
        tooltipDescription.text = region.description;

        // Reveal right side of the tooltip.
        tooltipRightParent.SetActive(true);
    }

    // Load a gatherable item's description into the tooltip.
    public void ShowTooltip(Gatherable gatherable)
    {
        // Need a description.
        if (gatherable.description == "")
        {
            HideTooltip();
            return;
        }

        // Set description.
        tooltipDescription.text = gatherable.description;

        // Reveal right side of the tooltip.
        tooltipRightParent.SetActive(true);
    }

    // Hide the tooltip.
    public void HideTooltip()
    {
        // Hide both sides.
        tooltipLeftParent.SetActive(false);
        tooltipRightParent.SetActive(false);
        // Hide tooltip.
        // tooltip.alpha = 0f;
    }
}
