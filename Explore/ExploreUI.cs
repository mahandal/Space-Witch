using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ExploreUI : MonoBehaviour
{
    [Header("Credits")]
    // Text object showing the player's credits.
    public TMP_Text playerCredits;

    [Header("Hints")]
    // The hint saying "Press 'e' to interact!"
    public TMP_Text bottomHint;

    [Header("Interact: Dragon Shrine")]
    public CanvasGroup dragonShrineScreen;

    [Header("Interact: Explorer")]
    // The parent object of the interact screen.
    public CanvasGroup interactScreen;

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
    public void Awake()
    {
        // Singleton.
        if (I == null || I == this)
            I = this;
        else
            Destroy(gameObject);

        // Hide what should be hidden.
        HideBottomHint();
        interactScreen.gameObject.SetActive(false);
        dragonShrineScreen.gameObject.SetActive(false);
    }

    // + Exploring!
    void Update()
    {
        // Credits
        playerCredits.text = MenuManager.I.saveData.credits.ToString();
    }

    // + Interact

    // TBD: Move to DragonShrine.cs
    // Praying at the dragon shrine.

    // Go to the next page of cards in your deck.
    public void B_NextPage()
    {

    }

    // Go to the previous page of cards in your deck.
    public void B_PreviousPage()
    {

    }

    // Promote the currently selected unit to squad leader.
    // (AKA choose to play as that unit)
    public void B_PromoteToSquadLeader()
    {

    }

    // Add the currently selected unit to your squad, bringing them out to explore with you.
    public void B_AddToSquad()
    {

    }

    // Remove the currently selected unit from your squad, allowing them to rest until the big battle.
    public void B_ReturnToRest()
    {

    }

    // Sell the currently selected card.
    public void B_Sell()
    {
        // TBD!
    }

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
            dragonShrineScreen.gameObject.SetActive(true);
            return;
        }
        
        // TBD: Normal interact screen(?)
    }

    // Set up the interact screen for the given explorer.
    public void Interact(Explorer e)
    {
        // Load explorer's name, cost, and description.
        interactName.text = e.myName;
        interactCredits.text = e.creditCost.ToString();
        interactDescription.text = e.description;

        // Load image.
        Utility.LoadImage(interactPortrait, "Cards/" + e.myName);

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
        dragonShrineScreen.gameObject.SetActive(false);
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

}
