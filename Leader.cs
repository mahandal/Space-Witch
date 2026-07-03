using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public partial class Leader : MonoBehaviour
{
    [Header("Meta")]
    public string myName;
    public bool autoPilot;
    public bool good = true;
    public Star homeStar;

    [Header("Health")]
    // How much health this leader has remaining.
    public float health = 100;

    // A list of vital units that share health with the leader.
    // Hard-coded to include dragon statues.
    public List<Unit> vitalUnits = new List<Unit>();

    [Header("Mana")]
    // How much mana this leader currently has.
    public int mana = 0;

    // How many seconds it takes for this leader to generate mana.
    public float secondsPerMana;

    // The timer counting down until we generate mana.
    public float manaTimer = 1f;

    [Header("Signature Cards")]
    // This leader's signature cards, which they periodically play for free automatically.
    public List<string> signatureCards = new List<string>();

    // A list of cooldowns for this leader's signature cards.
    public List<float> signatureCooldowns = new List<float>();

    // A list of timers tracking the cooldowns for this leader's signature cards.
    public List<float> signatureTimers = new List<float>();

    // The mini cards showing the leader's signature cards cooling down.
    public List<CardInHand> signatureDisplayCards = new List<CardInHand>();

    [Header("Hand")]
    public List<CardInHand> hand = new List<CardInHand>();

    [Header("Deck")]
    public List<string> deck = new List<string>();

    [Header("Deployment")]
    public int numColumnsDeployable = 4;

    [Header("Reinforcements")]
    // A flag tracking whether we have ran out of cards in our deck yet.
    public bool reservesDepleted = false;

    // How long it will take for reinforcements to arrive, when you run out of cards in your deck.
    public float timeUntilReinforcements = 30f;

    // The timer tracking how long until reinforcements arrive.
    // Begins ticking down when you run out of cards in your deck.
    public float reinforcementTimer = 30f;

    // + Initialization

    // Fill in this leader from a bio.
    public void LoadBio(LeaderBio bio)
    {
        myName = bio.myName;
        homeStar = bio.homeStar;
        signatureCards = bio.signatureCards;
        signatureCooldowns = bio.signatureCooldowns;
    }

    // Start a new battle:
    // Reset our health and mana, shuffle our deck, and draw a starting hand.
    public void NewBattle()
    {
        // Reset bools.
        reservesDepleted = false;

        // Set our starting health.
        float startingHealth = GM.I.startingHealth;
        if (myName == "Wubalin Brightforge")
            startingHealth *= 1.2f;
        SetHealth(startingHealth);

        // Set our starting mana.
        mana = 0;

        // Set our secondsPerMana.
        // Both good and evil divide the base speed of 1.5 by the current star's mana scaling.
        // Evil also reduces their seconds per mana by 10% per planet index.
        // Note: Planet counts must not reach 10! Or we'll divide by 0 and break everything.
        if (good)
            secondsPerMana = GM.I.secondsPerMana / StarManager.I.currentStar.goodManaScaling;
        else

            secondsPerMana = GM.I.secondsPerMana / StarManager.I.currentStar.evilManaScaling * (1f - 0.1f * StarManager.I.planetIndex);

        // Reset our mana timer.
        manaTimer = secondsPerMana;

        // Shuffle our deck!
        deck = Utility.Shuffle(deck);

        // Draw our starting hand.
        DrawStartingHand();

        // + Signature cards
        // Reset timers.
        signatureTimers.Clear();

        // Loop through each of our signature cards.
        for (int i = 0; i < signatureCards.Count; i++)
        {
            // Set timer.
            signatureTimers.Add(signatureCooldowns[i]);

            // Load card.
            signatureDisplayCards[i].LoadCard(signatureCards[i]);

            // Hide card to begin with.
            signatureDisplayCards[i].canvasGroup.alpha = 0f;
        }


        // + Reinforcement times:

        // For good, it means how long you wait between playing your deck and playing local reinforcements.
        // Evil has no deck so they wait at the beginning of the game, but for less time.

        // Set time until reinforcements randomly.
        if (good)
            timeUntilReinforcements = Random.Range(20, 40);
        else
            timeUntilReinforcements = Random.Range(1, 10);

        // Set reinforcement timer.
        reinforcementTimer = timeUntilReinforcements;
    }

    // Initialize our deck.
    public void InitializeRandomDeck()
    {
        // Start off with a new list.
        deck = new List<string>();

        // Randomize deck length.
        int deckLength = Random.Range(20, 100);

        // Loop through each card.
        for (int i = 0; i < deckLength; i++)
        {
            // Generate a random card name.
            string cardName = "Moose";

            int randomRoll = Random.Range(0, 7);

            if (randomRoll == 1) cardName = "Hyena";
            if (randomRoll == 2) cardName = "Polar Bear";
            if (randomRoll == 3) cardName = "Rhino";
            if (randomRoll == 4) cardName = "Lion";
            if (randomRoll == 5) cardName = "Wallflower";
            if (randomRoll == 6) cardName = "Charm";

            // Add the card name to our deck list.
            deck.Add(cardName);
        }
    }

    public void DrawStartingHand()
    {
        // Draw 5 cards.
        for (int i = 0; i < 5; i ++)
        {
            // // Draw a card.
            // DrawCard(i);

            // Hide each card for a moment, so they reveal one by one.
            // Note: Drawing replacement cards is handled in CardInHand.
            hand[i].Hide(0.2f + i * 0.2f);
        }
    }

    // Fixed update.
    void FixedUpdate()
    {
        // Wait for battle.
        if (GM.I.gameState != 1) return;
        if (GM.I.gameTimer < 1f) return;

        // + Signature Cards
        // Loop through each signature card.
        for (int i = 0; i < signatureCards.Count; i++)
        {
            // Count down timer.
            signatureTimers[i] -= Time.fixedDeltaTime;

            // Get percent cooled down.
            float percent = 1f - (signatureTimers[i] / signatureCooldowns[i]);

            // Set the opacity of the displayed signature card.
            signatureDisplayCards[i].canvasGroup.alpha = percent;

            // Time to play our signature card?
            if (signatureTimers[i] <= 0f)
            {
                // Reset timer.
                signatureTimers[i] = signatureCooldowns[i];

                // Get the card to play's name.
                string cardName = signatureCards[i];

                // Auto play the card.
                AutoPlayCard(cardName, true);
            }
        }
            

        // + Mana

        // Count down mana timer.
        manaTimer -= Time.fixedDeltaTime;

        // Mana time?
        if (manaTimer <= 0f)
        {
            // Gain mana!
            mana++;
            
            // Reset mana timer.
            manaTimer = secondsPerMana;
        }

        // + Reinforcements

        // Are we counting down our reinforcement timer?
        if (reservesDepleted && reinforcementTimer > 0f)
        {
            // Count down time until reinforcements arrive.
            reinforcementTimer -= Time.fixedDeltaTime;

            // Is it time for a second front?
            if (reinforcementTimer <= 0f)
                Reinforce();
        }

        // + Auto Pilot
        if (autoPilot)
            AutoPilot();
    }

    // + Drawing cards
    // Draw a card, into the given slot in your hand.
    public void DrawCard(int index)
    {
        // Get the cardInHand.
        CardInHand cardInHand = hand[index];

        // Card name
        string cardName = "Moose";

        // Check if you have any cards remaining in your deck.
        if (deck.Count > 0)
        {
            // Draw the top card off your deck.
            cardName = Utility.Pop(deck);

            // Get the card, using its name.
            Card card = GM.I.grimoire[cardName];

            // Load the card into your hand!
            cardInHand.LoadCard(card);
        }
        else
        {
            // No cards left in your deck.

            // Check if you have reinforcements.
            if (reinforcementTimer <= 0f)
            {
                // Locals reinforce your army!
                cardName = StarManager.I.GetRandomPlanetCard(good);

                // Get the card, using its name.
                Card card = GM.I.grimoire[cardName];

                // Load the card into your hand!
                cardInHand.LoadCard(card);
            }
            else
            {
                // Check if we just depleted our reserves.
                if (!reservesDepleted)
                {
                    // Set bool.
                    reservesDepleted = true;

                    // UI popup.
                    if (good)
                        UI.I.ReservesDepleted();
                }

                // Hide until reinforcements arrive.
                cardInHand.Hide(reinforcementTimer + index + 1); 
            }
        }
    }

    // Discard a card.
    public void Discard(int index)
    {
        // Get the card in hand.
        CardInHand cardInHand = hand[index];

        // Throw the card in the trash.
        cardInHand.Throw(UI.I.trashCan.transform.position);

        // Deselect card.
        if (cardInHand == InputManager.I.selectedCard)
            InputManager.I.selectedCard.Deselect();
    }

    // + Playing cards

    // Attempt to play a card from our hand to the given tile, using index to decide which card.
    // Returns true if the card was able to be played.
    // Returns false if the card was unable to be played. E.g:
    // - No card available.
    // - Insufficient mana.
    // - Invalid location.
    public bool AttemptPlayCard(int index, Tile tile)
    {
        // Check if the index is beyond our hand size.
        if (index >= hand.Count)
            return false;

        // Get our card in hand.
        CardInHand cardInHand = hand[index];

        // Check if we have a card available.
        if (cardInHand == null || cardInHand.hideTimer > 0f)
            return false;

        // Get the card from the grimoire.
        Card card = GM.I.grimoire[cardInHand.myName];

        // Check if we have enough mana.
        if (mana < card.manaCost)
            return false;

        // Check card type to see if the tile is valid.
        if (card.cardType == "Spell")
        {
            // Spells may be played anywhere outside of your enemy's deployment zone.
            if (IsInEnemyDeploymentZone(tile))
                return false;
        }
        else
        {
            // Units, structures, and equipment must be played in your deployment zone.
            if (!IsInDeploymentZone(tile))
                return false;
        }

        // Structures and items check if there is already a structure there.
        if (card.cardType == "Structure" || card.cardType == "Item")
        {
            if (tile.structure != null)
                return false;
        }

        // Validity checks passed!

        // Spend mana!
        mana -= card.manaCost;

        // Play the card!
        PlayCard(index, tile);

        // // Remove card from hand, drawing a new card after 1 second.
        // cardInHand.Hide();

        // Deselect card.
        if (cardInHand == InputManager.I.selectedCard)
            InputManager.I.selectedCard.Deselect();

        // Return successful.
        return true;
    }

    // Play a card from our hand to the given tile.
    // Note: Validity is checked already in AttemptPlayCard
    public void PlayCard(int index, Tile tile)
    {
        // Get the card in hand.
        CardInHand cardInHand = hand[index];

        // Spawn the unit corresponding to the selected card.
        Unit newUnit = SpawnUnit(cardInHand.myName, tile);

        // Throw card into play.
        cardInHand.Throw(newUnit);

        // Remove card from hand, drawing a new card after 1 second.
        // Unless the card is cursed, in which case it takes half the deploy time instead.
        if (newUnit.keywords.Contains("Cursed"))
            cardInHand.Hide(newUnit.deployTime / 2f);
        else
            cardInHand.Hide(1f);

        // Activate!
        newUnit.gameObject.SetActive(true);
    }

    // Play one of our signature cards.
    public void PlaySignatureCard(string cardName, Tile tile)
    {
        // Get card.
        Card card = GM.I.grimoire[cardName];

        // Spawn the unit.
        Unit newUnit = SpawnUnit(cardName, tile);

        // Return here if we failed to play a card for whatever reason.
        if (newUnit == null) return;

        // Get index
        int index = signatureCards.IndexOf(cardName);

        // Throw card into play.
        signatureDisplayCards[index].Throw(newUnit);

        // Activate!
        newUnit.gameObject.SetActive(true);
    }

    // Spawn a new Unit.
    // Note: In this case Unit refers to the class which includes all card types: Units, Structures, Items, and Spells.
    // Used when playing a card, when production buildings produce, when leader abilities play free cards, etc...
    public Unit SpawnUnit(string unitName, Tile tile)
    {
        // Null check.
        if (tile == null) return null;

        // Get the unit's card.
        Card card = GM.I.grimoire[unitName];

        // Get the progenitor for the card.
        Unit progenitor = Progenitors.I.GetProgenitor(card);

        // Instantiate a new copy of the unit, as a child of this leader.
        Unit newUnit = Object.Instantiate(progenitor, transform);

        // If card is a structure, link tile to structure.
        if (card.cardType == "Structure")
            tile.structure = newUnit;

        // If card is an item, remember it in our list of items.
        if (card.cardType == "Item")
            items.Add(newUnit);

        // Get unit's position offset (i.e. how far above the ground it stands).
        float offset = progenitor.transform.position.y % 1f;

        // Modify offset by a slight random factor, so stacked units can be seen.
        offset += Random.Range(-0.1f, 0.1f);

        // Get position.
        // (also offset horizontally by 0.5f to spawn in the center of the tile)
        Vector3 position = tile.transform.position + new Vector3 (0.5f, offset, 0);

        // Move unit into position.
        newUnit.transform.position = position;

        // Remember our lane.
        newUnit.laneIndex = tile.y;

        // Set current tile.
        newUnit.currentTile = tile;

        // Set alignment.
        newUnit.good = good;

        // Evil is backwards!
        if (good)
            newUnit.transform.eulerAngles = new Vector3(0f, 0f, 0f);
        else
            newUnit.transform.eulerAngles = new Vector3(0, 180f, 0);

        // Hide (to deploy in).
        Utility.SetOpacity(newUnit.spriteRenderer, 0f);

        // OnPlayed triggers.
        OnPlayed(newUnit);

        // Activate!
        newUnit.gameObject.SetActive(true);

        // Return.
        return newUnit;
    }

    // Called when this leader plays a card.
    public void OnPlayed(Unit cardPlayed)
    {
        // + Leader Stats
        // Avalon:
        // +20% speed
        if (homeStar.myName == "Avalon")
            cardPlayed.speed *= 1.2f;

        // Bedegraine
        // +20% health
        if (homeStar.myName == "Bedegraine")
        {
            cardPlayed.maxHealth *= 1.2f;
            cardPlayed.currentHealth *= 1.2f;
        }

        // Sarras:
        // +20% vision
        if (homeStar.myName == "Sarras")
            cardPlayed.vision *= 1.2f;

        // Orkney:
        // +20% damage
        if (homeStar.myName == "Orkney")
            cardPlayed.damage *= 1.2f;

        // Logres:
        // -20% speed
        // +20% health
        // +20% damage
        // +20% size
        if (homeStar.myName == "Logres")
        {
            cardPlayed.speed *= 0.8f;
            cardPlayed.maxHealth *= 1.2f;
            cardPlayed.currentHealth *= 1.2f;
            cardPlayed.damage *= 1.2f;
            cardPlayed.transform.localScale *= 1.2f;
        }

        // Gorr:
        // -20% health
        // +20% speed
        // +20% vision
        if (homeStar.myName == "Gorr")
        {
            cardPlayed.maxHealth *= 0.8f;
            cardPlayed.currentHealth *= 0.8f;
            cardPlayed.speed *= 1.2f;
            cardPlayed.vision *= 1.2f;
        }

        // Corbenic:
        // +2 armor
        if (homeStar.myName == "Corbenic")
            cardPlayed.armor += 2;

        // Lyonesse of the Lakes:
        // -2s deploy time in the river
        // -1s deploy time adjacent to river
        if (homeStar.myName == "Lyonesse of the Lakes")
        {
            // Check if we're in the river.
            if (cardPlayed.laneIndex == 2)
                cardPlayed.deployTime -= 2f;
            else if (cardPlayed.laneIndex == 1 || cardPlayed.laneIndex == 3)
                cardPlayed.deployTime -= 1f;

            // Minimum of 1.
            if (cardPlayed.deployTime < 1)
                cardPlayed.deployTime = 1;
        }

        // Dolorous Gard:
        // -1 mana cost
        // if (homeStar.myName == "Dolorous Gard")
        //     mana++;

        // + Keywords
        // Damned: Leader loses life equal to the card's mana cost.
        if (cardPlayed.keywords.Contains("Damned"))
            LoseHealth(cardPlayed.manaCost);
    }

    // + Deployment zones
    // Returns true if the given unit is in our deployment zone.
    public bool IsInDeploymentZone(Unit unit)
    {
        // Return false for null units.
        if (unit == null) return false;

        // Check from the left for good, from the right for evil.
        if (good)
            return unit.transform.position.x < numColumnsDeployable;
        else
            return unit.transform.position.x >= GM.I.gridWidth - numColumnsDeployable;
    }

    // Returns true if the given unit is in our enemy's deployment zone.
    public bool IsInEnemyDeploymentZone(Unit unit)
    {
        // Return false for null units.
        if (unit == null) return false;

        if (good)
            return GM.I.evilLeader.IsInDeploymentZone(unit);
        else
            return GM.I.goodLeader.IsInDeploymentZone(unit);
    }

    // Returns true if the given tile is in our deployment zone.
    public bool IsInDeploymentZone(Tile tile)
    {
        if (good)
            return tile.x < numColumnsDeployable;
        else
            return tile.x >= GM.I.gridWidth - numColumnsDeployable;
    }

    // Returns true if the given tile is in our enemy's deployment zone.
    public bool IsInEnemyDeploymentZone(Tile tile)
    {
        if (good)
            return GM.I.evilLeader.IsInDeploymentZone(tile);
        else
            return GM.I.goodLeader.IsInDeploymentZone(tile);
    }

    // Returns true if the given tile is visible by at least one of our units.
    public bool IsTileVisible(Tile tile)
    {
        // Look through each one of our units
        List<Unit> units = GetAllUnits();
        foreach (Unit unit in units)
        {
            // Get their distance.
            float distance = Vector3.Distance(unit.transform.position, tile.transform.position);

            // Compare with their vision.
            if (distance <= unit.vision)
                return true;
        }

        // No units were close enough to see this tile, return false.
        return false;
    }

    // + Units
    // Get a list of all our units.
    public List<Unit> GetAllUnits()
    {
        if (good)
            return GM.I.GetAllGoodUnits();
        else
            return GM.I.GetAllEvilUnits();
    }

    // + Health
    // Lose health.
    // Used whenever a unit deals damage to this leader.
    public void LoseHealth(float healthLost, Unit source = null)
    {
        // + Lancelot
        if (myName == "Lancelot" && source != null)
        {
            // Lose health.
            LoseHealth(source.currentHealth);

            // Charm (without healing).
            source.ChangeSides(false);
        }

        // Think faster when damaged.
        thinkTimer = 0f;

        // Lose health.
        health -= healthLost;

        // Set health for vital units.
        foreach (Unit unit in vitalUnits)
        {
            unit.currentHealth = health;
        }

        // Defeat?
        if (health <= 0f)
        {
            // When a good leader dies, that's a loss!
            // When an evil leader dies, that's a win!
            if (good)
                GM.I.GameOver(false);
            else
                GM.I.GameOver(true);
        }
    }

    // Gain health.
    public void GainHealth(float healthGained)
    {
        // Gain health.
        health += healthGained;

        // Set health for vital units.
        foreach (Unit unit in vitalUnits)
        {
            unit.currentHealth = health;
        }
    }

    // Set health.
    // Used at the beginning of the game to initialize starting health.
    public void SetHealth(float newHealth)
    {
        // Set our health.
        health = newHealth;

        // Set health for vital units.
        foreach (Unit unit in vitalUnits)
        {
            unit.maxHealth = health;
            unit.currentHealth = health;
        }
    }

    // + Reinforcements

    // Locals reinforce your army!
    // Called 30 seconds after you run out of cards in your deck.
    public void Reinforce()
    {
        // Draw 5 cards(?)
        // DrawStartingHand();

        // Visuals!
        if (good)
            UI.I.ReinforcementsArrived();
    }
}
