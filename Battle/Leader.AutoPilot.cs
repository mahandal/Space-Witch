using UnityEngine;
using System.Collections.Generic;

public partial class Leader
{
    [Header("Personality")]
    // Aggression increases how often this leader plays aggressively.
    public int aggression = 30;

    // Fear increases how often this leader plays defensively.
    public int fear = 30;

    // Greed increases how often this leader plays economically.
    public int greed = 30;

    // Chaos increases how often this leader plays randomly.
    public int chaos = 10;

    // A list of bools for each lane tracking whether we know there's lava there or not.
    public List<bool> isLavaInLane = new List<bool>();

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
    // Auto play a card.
    public void AutoPlayCard(string cardName, bool signatureCard = false)
    {
        // Pick which tile to play the card in.
        Tile tile = GetTileToPlay(cardName);

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
        } else {
            // Try to play the card.
            bool successfullyPlayedCard = AttemptPlayCard(indexOfNextCard, tile);

            // If we were able to play the card, increment the index of the next card we want to play.
            if (successfullyPlayedCard)
                IncrementIndexOfNextCard();
        }        
    }

    // Get which tile to play a card in.
    public Tile GetTileToPlay(string cardName)
    {
        // Get the card from the grimoire.
        Card card = DM.I.Grimoire(cardName);

        // Spells are special!
        if (card.cardType == "Spell")
        {
            // Find closest enemy.
            Unit e = FindClosestVisibleEnemy();

            // Return their tile!
            // Note: If no enemy is visible, spell will be played like other cards instead.
            if (e != null)
                return e.currentTile;
        }

        // Personality.
        int a = aggression;
        int f = fear;
        int g = greed;
        int c = chaos;

        // + Modify personality traits using card type and role.

        // Units
        if (card.cardType == "Unit")
        {
            // Scouts are greedy.
            if (card.role == "Scout")
                g *= 2;

            // Hunters are aggressive.
            if (card.role == "Hunter")
                a *= 2;

            // Bulwarks are defensive.
            if (card.role == "Bulwark")
                f *= 2;

            // Mages are chaotic.
            if (card.role == "Mage")
                c *= 2;
        }
        // Structures
        if (card.cardType == "Structure")
        {
            // Walls and turrets are aggressive.
            if (card.role == "Wall" || card.role == "Turret")
                a *= 2;

            // Production and farms are highly defensive.
            if (card.role == "Production" || card.role == "Farm")
                f *= 10;
        }
        // Items
        if (card.cardType == "Item")
        {
            // Equipment and food are defensive.
            if (card.role == "Equipment" || card.role == "Food")
                f *= 2;

            // Traps are aggressive.
            if (card.role == "Trap")
                a *= 2;
        }

        // Roll between aggression, fear, greed, and chaos.
        int roll = Random.Range(0, a + f + g + c);

        // Aggression
        if (roll < a)
            return PlayAggressive(card);
        // Fear
        else if (roll < a + f)
            return PlayDefensive(card);
        // Greed
        else if (roll < a + f + g)
            return PlayGreedy(card);
        // Chaos
        else if (roll < a + f + g + c)
            return PlayRandom(card);

        Debug.LogError("ERROR! FAILED TO FIND TILE TO PLAY: " + cardName);
        return null;
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
            } else if (mana < 25) {
                // Too much mana, think faster!
                thinkTimer = 1f;
            } else {
                // Ahhh!
                thinkTimer = 0.1f;
            }                
        }
    }

    // Think!
    public void Think()
    {
        // Get the card we want to play.
        CardInHand cardInHand = hand[indexOfNextCard];

        // Check if we have enough mana to play our desired card.
        if (mana < cardInHand.manaCost) return;

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

    // + Where do we play cards?

    // Aggressive
    public Tile PlayAggressive(Card c)
    {
        // Find our strongest lane.
        int row = FindStrongestLane();
        
        // Find the furthest forward tile we can play in that lane.
        bool blockedByStructures = (c.cardType == "Structure");
        return FindForwardTile(row, c, blockedByStructures);
    }

    // Defensive
    public Tile PlayDefensive(Card c)
    {
        // Find our weakest lane.
        int row = FindWeakestLane();

        // Find the furthest back tile we can play in that lane.
        bool blockedByStructures = (c.cardType == "Structure");
        return FindBackTile(row, c, blockedByStructures);
    }

    // Greedy/Economic
    public Tile PlayGreedy(Card c)
    {
        // Find the lane with the most violet flowers.
        int row = FindFlowerLane();

        // Find the furthest back tile we can play in that lane.
        bool blockedByStructures = (c.cardType == "Structure");
        return FindBackTile(row, c, blockedByStructures);
    }

    // Random
    public Tile PlayRandom(Card c)
    {
        // Roll a random lane.
        int row = Random.Range(0, DM.I.gridHeight);

        // Roll a random tile.
        bool blockedByStructures = (c.cardType == "Structure");
        return RandomAvailableTile(row, c, blockedByStructures);
    }

    // + Targeting

    // Find a random available tile in the given row.
    public Tile RandomAvailableTile(int row, Card c, bool blockedByStructures = false)
    {
        // Gather claimed tiles.
        List<Tile> availableTiles = new List<Tile>();
        for (int i = 0; i < DM.I.gridWidth; i++)
        {
            // Get tile.
            Tile tile = DM.I.grid[i, row];

            // If card is a structure, ignore tiles with structures already.
            // if (c.cardType == "Structure" && tile.structure != null) continue;
            if (blockedByStructures && tile.structure != null) continue;

            // Add claimed tiles to list.

            // Good
            if (good && tile.claim > 0)
                availableTiles.Add(tile);
            // Evil
            if (!good && tile.claim < 0)
                availableTiles.Add(tile);
        }

        // No room for a new structure, repair old structure instead.
        if (availableTiles.Count == 0)
            return FindForwardTile(row, c, false);

        // Roll random tile.
        int roll = Random.Range(0, availableTiles.Count);
        Tile returnTile = availableTiles[roll];

        // Return.
        return returnTile;
    }

    // Find the furthest forward tile we have claimed in the given row.
    public Tile FindForwardTile(int row, Card c, bool blockedByStructures = false)
    {
        // Good starts from the right side and looks left.
        if (good)
        {
            // Loop through each tile.
            for (int i = DM.I.gridWidth - 1; i >= 0; i--)
            {
                // Get tile.
                Tile tile = DM.I.grid[i, row];

                // If card is a structure, ignore tiles with structures already.
                if (blockedByStructures && tile.structure != null) continue;

                // Return if claimed for good.
                if (tile.claim > 0)
                    return tile;
            }
        }
        // Evil starts from the left side and looks right.
        if (!good)
        {
            // Loop through each tile.
            for (int i = 0; i < DM.I.gridWidth; i++)
            {
                // Get tile.
                Tile tile = DM.I.grid[i, row];

                // If card is a structure, ignore tiles with structures already.
                if (blockedByStructures && tile.structure != null) continue;

                // Return if claimed by evil.
                if (tile.claim < 0)
                    return tile;
            }
        }

        // If structures are blocking all tiles in this row, repair the most forward structure.
        if (blockedByStructures)
            return FindForwardTile(row, c, false);

        Debug.LogWarning("ERROR! Failed to find forward tile! Leader: " + myName + ". Card: " + c.myName + ". Row: " + row);
        return null;
    }

    // Find the furthest back tile we have claimed in the given row.
    public Tile FindBackTile(int row, Card c, bool blockedByStructures = false)
    {
        // Initialize tile to return.
        Tile tile = null;

        // Good
        if (good)
        {
            // Start from the left.
            for (int i = 0; i < DM.I.gridWidth; i++)
            {
                // Get tile.
                tile = DM.I.grid[i, row];

                // If card is a structure, ignore tiles with structures already.
                if (blockedByStructures && tile.structure != null) continue;

                // Return tile if valid.
                return tile; 
            }
        }
        else
        {
            // Start from the right.
            for (int i = DM.I.gridWidth - 1; i >= 0; i--)
            {
                // Get tile.
                tile = DM.I.grid[i, row];

                // If card is a structure, ignore tiles with structures already.
                if (blockedByStructures && tile.structure != null) continue;

                // Return tile if valid.
                return tile; 
            }
        }

        // Can't find a tile! Go to FindForwardTile as our last resort.
        return FindForwardTile(row, c, false);
    }

    // Find the lane with the most violet flowers.
    public int FindFlowerLane()
    {
        // Create array to count flowers.
        int[] flowerCount = new int[DM.I.gridHeight];

        // Loop through each flower.
        foreach (Unit flower in DM.I.flowers)
        {
            // Count yer flowers!
            flowerCount[flower.laneIndex]++;
        }

        // Find lane with most flowers.
        int bestLane = -1;
        int bestCount = -1;

        for (int i = 0; i < DM.I.gridHeight; i++)
        {
            if (flowerCount[i] > bestCount)
            {
                bestLane = i;
                bestCount = flowerCount[i];
            }
        }

        // Return.
        return bestLane;
    }

    // Find our strongest lane.
    public int FindStrongestLane()
    {
        // Initialize.
        int strongestLane = 0;
        int strongestPower = 0;

        // Loop through each lane.
        for (int i = 0; i < DM.I.gridHeight; i++)
        {
            // Lava.
            if (isLavaInLane[i] && homeStar.myName != "Corbenic") continue;

            // Calculate total mana cost of all units in lane.
            int totalPower = 0;
            foreach (Unit u in DM.I.GetUnitsInLane(i, good))
            {
                totalPower += u.manaCost;
            }

            // Compare to current strongest.
            if (totalPower > strongestPower)
            {
                // Remember yer best.
                strongestLane = i;
                strongestPower = totalPower;
            }
        }

        // Return.
        return strongestLane;
    }

    // Find our weakest lane.
    public int FindWeakestLane()
    {
        // Initialize.
        int weakestLane = -1;
        int weakestPower = int.MaxValue;

        // Loop through each lane.
        for (int i = 0; i < DM.I.gridHeight; i++)
        {
            // Calculate total mana cost of all units in lane.
            int totalPower = 0;
            foreach (Unit u in DM.I.GetUnitsInLane(i, good))
            {
                totalPower += u.manaCost;
            }

            // Compare to weakest.
            if (totalPower < weakestPower)
            {
                // Remember yer worst.
                weakestLane = i;
                weakestPower = totalPower;
            }
        }

        // Return.
        return weakestLane;
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
