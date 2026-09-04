using UnityEngine;
using System.Collections.Generic;

public partial class Leader
{
    [Header("Auto Pilot")]
    // The index of the next card the auto pilot wants to play.
    // Starts at 0, increments up each time the auto pilot plays a card.
    // Loops back to 0 upon exceeding hand size.
    public int indexOfNextCard = 0;

    // How long the auto pilot takes between thinking.
    public float timeToThink = 1f;

    // How much variance the auto pilot has in time between thinking.
    public float thinkTimeVariance = 1f;

    // The timer counting down until the auto pilot thinks again.
    public float thinkTimer = 1f;

    // A list of each item we have active, so we remember to grab them!
    public List<Unit> items = new List<Unit>();

    // + Auto Play
    // Auto play a specific card.
    // Used by leader abilities.
    public void AutoPlayCard(string cardName, bool signatureCard = false)
    {
        // Pick which tile to play the card in.
        Tile tile = GetTileToPlay(cardName, signatureCard);

        // If we can't find a tile, skip playing this card.
        if (tile == null)
        {
            if (!signatureCard)
                IncrementIndexOfNextCard();
            return;
        }

        if (signatureCard)
        {
            // Play a signature card!
            PlaySignatureCard(cardName, tile);

            // Put the unit directly into play!
            // SpawnUnit(cardName, tile);
        } else {
            // Try to play the card.
            bool successfullyPlayedCard = AttemptPlayCard(indexOfNextCard, tile);

            // Check if we were able to play the card successfully.
            if (successfullyPlayedCard)
            {
                // Increment the index of the next card we want to play.
                IncrementIndexOfNextCard();
            }    
        }        
    }

    // Get which tile to play a card in.
    // Note: The boolean parameter 'signature' is passed in as well,
    // so we know whether or not to discard unplayable cards.
    // (Leader abilities also call this, but for free so they don't affect your hand.)
    public Tile GetTileToPlay(string cardName, bool signature = false)
    {
        // Get the card from the grimoire.
        Card card = DM.I.Grimoire(cardName);

        // Randomize row to begin with.
        int row = Random.Range(0, DM.I.gridHeight);

        // Find the closest visible enemy unit.
        Unit closestEnemy = FindClosestVisibleEnemy();

        // Check if they are within out deployment zone.
        bool enemyNear = IsInDeploymentZone(closestEnemy);

        // Do we have an item to protect?
        if (items.Count > 0)
        {
            // Choose a random item, if we have multiple.
            int itemIndex = Random.Range(0, items.Count);

            // Set our row to the row of the chosen item.
            row = items[itemIndex].laneIndex;
        }
        // Is there an enemy nearby?
        else if (enemyNear)
        {
            // Set row to enemy's.
            row = closestEnemy.laneIndex;
        }

        // Column.
        // Start in the back.
        int column = 0;

        if (!good)
            column = DM.I.gridWidth - 1;

        // Structures.
        if (card.cardType == "Structure")
        {
            // For now, all structures are played as walls:
            // Look at the furthest out tile.
            if (good)
                column = numColumnsDeployable - 1;
            else
                column = DM.I.gridWidth - numColumnsDeployable;

            // If the tile has a structure, look one column in.
            while (DM.I.grid[column, row].structure != null)
            {
                if (good)
                    column--;
                else
                    column++;

                // If we can't find a spot, skip this card.
                if (column < 0 || column >= DM.I.gridWidth)
                {
                    // Move on to the next card.
                    IncrementIndexOfNextCard();

                    // Return.
                    return null;
                }
            }
        }

        // Items.
        if (card.cardType == "Item")
        {
            // For now, all items are played as equipment:
            // Play in a close tile.

            // Skip if enemy is in our deployment zone.
            if (enemyNear)
            {
                // Discard card.
                if (!signature)
                    Discard(indexOfNextCard);

                // Move on to the next card index.
                IncrementIndexOfNextCard();

                // Return.
                return null;
            }

            // Look at the column closest to us, right outside our dragon statues.
            if (good)
                column = 1;
            else
                column = DM.I.gridWidth - 2;

            // If the tile has a structure, look one column in.
            // TBD: Look around at different rows instead of dumbly sticking with one.
            while (DM.I.grid[column, row].structure != null)
            {
                if (good)
                    column--;
                else
                    column++;

                // If we can't find a spot, skip this card.
                if (column < 0 || column >= DM.I.gridWidth)
                {
                    // Move on to the next card.
                    IncrementIndexOfNextCard();

                    // Return.
                    return null;
                }
            }
        }

        // Spells
        if (card.cardType == "Spell")
        {
            // For now, all spells are played as hexes:
            // Find the closest enemy unit.

            // If no enemy is active, move to the next card in our hand.
            if (closestEnemy == null)
            {
                IncrementIndexOfNextCard();
                return null;
            } else {
                // Check if the closest visible enemy is too far away to target, even with a hex.
                if (IsInEnemyDeploymentZone(closestEnemy))
                    return null;

                // We have a target!
                // Set row.
                row = closestEnemy.laneIndex;

                // Get column.
                column = Mathf.FloorToInt(closestEnemy.transform.position.x);
            }
        }

        // Choose tile using column and row.
        return DM.I.grid[column, row];
    }

    // + Auto pilot
    // Let the auto pilot handle decision making, for this tick.
    public void AutoPilot()
    {
        // Decrement think timer.
        thinkTimer -= Time.fixedDeltaTime;

        // Time to think?
        if (thinkTimer <= 0f)
        {
            // Think!
            Think();

            // + Reset think timer.

            // Check mana.
            if (mana < 10)
            {
                // Roll random think time.
                float varianceRoll = Random.Range(-thinkTimeVariance, thinkTimeVariance);

                // Reset think timer.
                thinkTimer = timeToThink + varianceRoll;
            } else {
                // Too much mana, think faster!
                thinkTimer = 1f;
            }                
        }
    }

    // Think!
    public void Think()
    {
        // Get the card we want to play.
        CardInHand cardInHand = hand[indexOfNextCard];

        // Try to play it using our auto pilot.
        AutoPlayCard(cardInHand.myName);
    }

    // Increment the index of the next card the auto pilot will play.
    public void IncrementIndexOfNextCard()
    {
        // Increment.
        indexOfNextCard++;

        // Overflow?
        if (indexOfNextCard >= hand.Count)
            indexOfNextCard = 0;
    }

    // Find the closest visible enemy unit and return it.
    // Iterate through every enemy unit, remembering the closest, ignoring ones we can't see.
    public Unit FindClosestVisibleEnemy()
    {
        // Remember the closest unit.
        Unit closestUnit = null;

        // Good.
        if (good)
        {
            // Iterate through each evil unit.
            foreach (Unit unit in DM.I.GetAllEvilUnits())
            {
                // Ignore units we can't see.
                if (!unit.IsVisible())
                    continue;

                // Set first unit manually so we don't have to worry about closestUnit being null.
                if (closestUnit == null)
                    closestUnit = unit;

                // Compare distance.
                // (Good is looking for furthest left unit.)
                if (unit.transform.position.x < closestUnit.transform.position.x)
                    closestUnit = unit;
            }
        } else {
        // Evil.

            // Iterate through each good unit.
            foreach (Unit unit in DM.I.GetAllGoodUnits())
            {
                // Ignore units we can't see.
                if (!unit.IsVisible())
                    continue;

                // Set first unit manually so we don't have to worry about closestUnit being null.
                if (closestUnit == null)
                    closestUnit = unit;

                // Compare distance.
                // (Evil is looking for furthest right unit.)
                if (unit.transform.position.x > closestUnit.transform.position.x)
                    closestUnit = unit;
            }
        }

        // Return.
        return closestUnit;
    }
}
