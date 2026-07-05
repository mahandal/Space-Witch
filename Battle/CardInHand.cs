using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class CardInHand : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [Header("Alignment")]
    public bool good = true;

    [Header("Meta")]
    // This card's name.
    public string myName;

    [Header("Text Fields")]
    public TMP_Text nameText;
    public TMP_Text manaText;
    public TMP_Text timeText;
    public TMP_Text roleText;

    [Header("Icons")]
    // The image for this card's role.
    public Image roleIcon;

    // The image for this card's art.
    public Image image;

    // The background image for this card (matching the main image).
    public Image backgroundImage;

    [Header("Drawing a new card")]
    public float hideTimer = 1f;

    [Header("Throwing a card")]
    // Where this card is thrown from.
    public Vector3 origin;

    // Where this tile is being thrown to.
    // (Defaults to -100 z as a way to check if it is still in hand or not.)
    // (If vectors were nullable, I would have made it null!)
    public Vector3 destination = new Vector3(-100, -100, -100);

    // The unit this card is throwing into play.
    public Unit deployingUnit;

    // How long until this card is thrown into the trash.
    public float trashTimer = 2f;

    // This card's original scale.
    public Vector3 originalScale = new Vector3(1, 1, 1);

    [Header("Machinery")]
    // The index of this card in its owners hand.
    public int index;

    // A canvas group to fade the opacity of the card.
    public CanvasGroup canvasGroup;

    // Parent of visuals to display when this card is selected.
    public GameObject highlight;


    // Awaken!
    void Awake()
    {
        // Hide highlight.
        highlight.SetActive(false);

        // Remember original scale.
        originalScale = transform.localScale;
    }

    // Fixed update.
    void FixedUpdate()
    {
        // Hiding?
        if (index >= 0 && hideTimer > 0f)
        {
            // Decrement.
            hideTimer -= Time.fixedDeltaTime;

            // Done?
            if (hideTimer <= 0f)
            {
                // Draw a new card.
                if (good)
                    DM.I.goodLeader.DrawCard(index);
                else
                    DM.I.evilLeader.DrawCard(index);

                // Set hide timer cleanly to 0?
                // hideTimer = 0f;
            }
        }

        // Initialize percent deployed.
        float percentDeployed = 0f;

        // Are we in the air?
        if (destination.z >= 0f)
        {
            // Are we going toward a tile?
            if (deployingUnit != null)
            {
                // Find percent deployed.
                percentDeployed = 1f - (deployingUnit.deployTimer / deployingUnit.deployTime);
            } else {
                // No deploying unit means we're heading toward the trash!
                trashTimer -= Time.fixedDeltaTime;

                // Hard-coded deploy time of 2 seconds.
                percentDeployed = 1f - (trashTimer / 2f);
            }

            // Pop at 50% deployed.
            if (percentDeployed >= 0.5f)
            {
                // Remove from DM's list of cards in the air.
                DM.I.cardsInTheAir.Remove(this);
                
                // Destroy game object.
                Destroy(gameObject);

                // Return.
                return;
            }

            // Get distance along journey.
            // (Note: Cards reach their tile halfway along deployment.)
            float percentJourneyTraveled = percentDeployed * 2f;

            // Get vector between origin and destination.
            Vector3 journey = destination - origin;

            // Move to position along journey.
            transform.position = origin + journey * percentJourneyTraveled;



            // + Rotate.
            float rotationAmount = 13f;
            transform.Rotate(new Vector3(0, 0, rotationAmount));


            // + Shrink.
            
            // Get new scalar.
            float scalar = 1f - percentJourneyTraveled;

            // Set scale.
            transform.localScale = scalar * originalScale;

            // + Set opacity.
            canvasGroup.alpha = 1f - percentDeployed;
        }
    }

    // Visually throw a card into play, toward a tile.
    public void Throw(Unit _deployingUnit)
    {
        // Get position of unit.
        Vector3 position = Camera.main.WorldToScreenPoint(_deployingUnit.transform.position);

        // Delegate!
        Throw(position, _deployingUnit);
    }

    // Visually throw a card into play, toward a given position.
    public void Throw(Vector3 _destination, Unit _deployingUnit = null)
    {
        // Create a new card, as a child of our UI.
        CardInHand thrownCard = Object.Instantiate(this, UI.I.transform);

        // Add to DM's list of cards in the air.
        // (so they can be cleaned up if they are still in the air when the game ends)
        DM.I.cardsInTheAir.Add(thrownCard);

        // Set thrown card's position.
        thrownCard.transform.position = transform.position;

        // Set thrown card's origin.
        thrownCard.origin = transform.position;

        // Set thrown card's destination.
        thrownCard.destination = _destination;

        // Set thrown card's deploying unit.
        thrownCard.deployingUnit = _deployingUnit;

        // Hide the original card, revealing after 2 seconds.
        Hide(2f);
    }

    // Called when the player's cursor enters this card's collider box.
    public void OnPointerEnter(PointerEventData eventData)
    {
        // Highlight your available cards.
        if (good && hideTimer <= 0f)
            canvasGroup.alpha = 0.5f;
    }

    // Called when the player's cursor exits this card's collider box.
    public void OnPointerExit(PointerEventData eventData)
    {
        // Unhighlight available cards.
        if (good && hideTimer <= 0f)
            canvasGroup.alpha = 1f;
    }

    // Called when the player clicks on this card.
    public void OnPointerClick(PointerEventData eventData)
    {
        // Click on flying cards to clear them.
        // if (targetTile != null)
        if (destination.z >= 0f)
        {
            // Reveal deploying unit (if extant).
            if (deployingUnit != null)
            {
                // Reveal unit.
                Utility.SetOpacity(deployingUnit.spriteRenderer, 0.5f);

                // + Guinevere
                if (DM.I.goodLeader.myName == "Guinevere")
                    DM.I.goodLeader.GuinevereSing(deployingUnit);
            }

            // Destroy object.
            Destroy(gameObject);

            // Return.
            return;
        }

        // Select this card, unless it was already select, in which case deselect it!
        if (InputBattle.I.selectedCard != this)
            Select();
        else
            Deselect();
    }

    // Select this card.
    public void Select()
    {
        // Deselect previous card.
        if (InputBattle.I.selectedCard != null)
            InputBattle.I.selectedCard.Deselect();

        // Set this as selected card.
        InputBattle.I.selectedCard = this;

        // Highlight visually.
        highlight.SetActive(true);
    }

    // Deselect this card.
    public void Deselect()
    {
        // Clear selection.
        InputBattle.I.selectedCard = null;

        // Clear highlight.
        highlight.SetActive(false);
    }

    // Load a card into this position (by name).
    public void LoadCard(string cardName)
    {
        // Get the card from the grimoire.
        Card card = DM.I.grimoire[cardName];

        // Delegate to below!
        LoadCard(card);
    }

    // Load a card into this position.
    public void LoadCard(Card card)
    {
        // Set name.
        myName = card.myName;
        nameText.text = myName;

        // Set mana cost.
        manaText.text = card.manaCost.ToString();

        // Set deployment time.
        timeText.text = card.deployTime.ToString();

        // Set role.
        roleText.text = card.role;

        // Load images.
        Utility.LoadImage(image, "Cards/" + card.myName);
        Utility.LoadImage(backgroundImage, "Cards/" + card.myName);
        Utility.LoadImage(roleIcon, "Roles/" + card.role);

        // Reveal the card!
        Reveal();
    }

    // Hide this card.
    // Used after playing or discarding a card, lasting until a new card is drawn.
    public void Hide(float hideTime = 1f)
    {
        // Hide.
        canvasGroup.alpha = 0f;

        // Set hide timer.
        hideTimer = hideTime;
    }

    // Reveal this card.
    // Used after drawing a new card.
    public void Reveal()
    {
        // Reveal.
        canvasGroup.alpha = 1f;

        // Set hide timer to 0.
        hideTimer = 0;
    }
}
