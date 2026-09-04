using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// The dungeon master!
// Runs battles.
// See GM for running Explore mode!
public class DM : MonoBehaviour
{
    [Header("DM")]
    // The time elapsed of the current battle.
    public float gameTimer = 0f;

    // Which way is the player following?
    public string way = "Hunter";
    

    [Header("Violet Flowers")]
    public List<Unit> flowers = new List<Unit>();
    public float timePerFlower = 6f;
    public float flowerTimer = 0f;

    [Header("Leaders")]
    public Leader goodLeader;
    public Leader evilLeader;
    public int startingHealth = 250;
    public float secondsPerMana = 3f;
    
    // Units
    [System.NonSerialized]
    public List<List<Unit>> goodUnits = new List<List<Unit>>();
    [System.NonSerialized]
    public List<List<Unit>> evilUnits = new List<List<Unit>>();

    [Header("Grid")]
    [System.NonSerialized]
    public Tile [,] grid;

    // The world space canvas the tiles live on.
    public Canvas tilesCanvas;

    // How many tiles high the grid.
    public int gridHeight = 5;

    // How many tiles wide the grid is.
    public int gridWidth = 16;

    // How many world units each tile takes up.
    public int tileSize = 1;

    [Header("Machinery")]
    // The progenitor manager.
    public Progenitors progenitors;

    // A list of all cards in the air.
    public List<CardInHand> cardsInTheAir = new List<CardInHand>();


    // Merlin's grimoire!
    // Stores each card, with their name as their key.
    [System.NonSerialized]
    public Dictionary<string, Card> grimoire = new Dictionary<string, Card>();

    // Singleton.
    public static DM I;

    // + Initialization.

    // Initialize.
    public void Initialize()
    {
        // Singleton.
        if (I == null)
            I = this;
        else
            Destroy(this);

        // Initialize progenitors.
        progenitors.Initialize();

        // Initialize unit lists.
        InitUnitLists();

        // Initialize grid.
        InitializeGrid();
    }

    // Begin a battle, following the way of the hunter.
    // Called from Explore/Region.cs
    public void BeginHunt()
    {
        // Set way.
        way = "Hunter";

        // Begin battle.
        BeginBattle();
    }

    // Begin a battle, following the way of the gatherer.
    // Called from Explore/Region.cs
    public void BeginGathering()
    {
        // Set way.
        way = "Gatherer";

        // Begin battle.
        BeginBattle();
    }

    // Start a battle!
    public void BeginBattle()
    {
        // Reset game timer.
        gameTimer = 0f;

        // + Initialize each leader.

        // Load evil leader's name and portrait.
        evilLeader.LoadVillain();

        // Copy your decklist into your deck.
        goodLeader.deck = new List<string>(MenuManager.I.saveData.decklist);

        // Give evil an empty deck for now.
        evilLeader.deck = new List<string>();
        // evilLeader.InitializeRandomDeck();

        // Turn on auto pilot for evil.
        // (leave it off for good!)
        evilLeader.autoPilot = true;

        // Shared new battle initialization.
        goodLeader.NewBattle();
        evilLeader.NewBattle();

        // Go to the battle map.
        MenuManager.I.GoToBattleMap();

        // UI
        UI.I.BeginBattle();

        // Re-enable time.
        Time.timeScale = 1f;
    }

    // + Grimoire
    public Card Grimoire(string cardName)
    {
        // Split the name into parts.
        string[] parts = cardName.Split(' ', 3);

        // Check if the card name has a level.
        if (parts[0] == "Level")
        {
            // Note: Level is available here in parts[1] but we don't need it.
            return grimoire[parts[2]];
        } else {
            return grimoire[cardName];
        }
    }

    // + Running the game.
    // Fixed update!
    void FixedUpdate()
    {
        // Wait for battle.
        if (MenuManager.I.gameState != 1) return;

        // Check for victory, in gatherer mode.
        if (goodLeader.flowersGathered >= 42) GameOver(true);
        if (evilLeader.flowersGathered >= 42) GameOver(false);

        // Increment time elapsed.
        gameTimer += Time.fixedDeltaTime;
        
        // Count down our flower timer.
        flowerTimer -= Time.fixedDeltaTime;

        // Time for a flower?
        if (flowerTimer <= 0f)
        {
            // Grow a flower!
            GrowFlower();

            // Reset flower timer.
            flowerTimer = timePerFlower;
        }
    }

    // Spawn a violet flower in a random tile.
    public void GrowFlower()
    {
        // Pick a random column, excluding dragon statues.
        int column = Random.Range(1, gridWidth - 2);

        // Pick a random row.
        int row = Random.Range(0, gridHeight);

        // Get tile.
        Tile tile = grid[column, row];

        // Grow a new flower!
        Unit flower = SpawnItem("Violet Flower", tile);

        // Store in list of flowers, so we can clean up post game.
        flowers.Add(flower);
    }

    // + Tiles

    // Initialize the grid.
    void InitializeGrid()
    {
        // Create a new matrix.
        grid = new Tile[gridWidth, gridHeight];

        // Loop through the matrix.
        for (int i = 0; i < gridWidth; i++)
        {
            for (int j = 0; j < gridHeight; j++)
            {
                // Instantiate a new tile.
                grid[i, j] = CreateNewTile(i, j);
            }
        }

        // Connect dragon statues to their tiles.
        for (int i = 0; i < gridHeight; i++)
        {
            // Good
            grid[0, i].structure = goodLeader.vitalUnits[i];

            // Evil
            grid[gridWidth - 1, i].structure = evilLeader.vitalUnits[i];
        }
    }

    // Create new tile at the specified coordinates.
    Tile CreateNewTile(int x, int y)
    {
        // Clone a new tile object.
        Tile newTile = Object.Instantiate(Progenitors.I.tile, tilesCanvas.transform);

        // Move tile into position.
        newTile.transform.position = new Vector3(x, y, 0);

        // Remember tile's coordinates.
        newTile.x = x;
        newTile.y = y;

        // Activate.
        newTile.gameObject.SetActive(true);

        // Return!
        return newTile;
    }

    // + Units

    // Spawn a new item in.
    public Unit SpawnItem(string unitName, Tile tile)
    {
        // Get the unit's card.
        Card card = Grimoire(unitName);

        // Get the progenitor for the card.
        Unit progenitor = Progenitors.I.GetProgenitor(card);

        // Instantiate a new copy of the unit, as a child of DM.
        Unit newUnit = Object.Instantiate(progenitor, transform);

        // Get unit's position offset (i.e. how far above the ground it stands).
        float offset = progenitor.transform.position.y % 1f;

        // Modify offset by a random factor, so stacked units can be seen.
        offset += Random.Range(-0.2f, 0.2f);

        // Get position.
        // (also offset horizontally by 0.5f to spawn in the center of the tile)
        Vector3 position = tile.transform.position + new Vector3 (0.5f, offset, 0);

        // Move unit into position.
        newUnit.transform.position = position;

        // Remember our lane.
        newUnit.laneIndex = tile.y;

        // Hide (to deploy in).
        Utility.SetOpacity(newUnit.spriteRenderer, 0f);

        // Activate!
        newUnit.gameObject.SetActive(true);

        // Return.
        return newUnit;
    }

    // Initialize the lists for each row for each side tracking each active unit.
    void InitUnitLists()
    {
        // Loop through each row.
        for (int i = 0; i < gridHeight; i++)
        {
            // Create a list for that row for good.
            goodUnits.Add(new List<Unit>());

            // Create a list for that row for evil.
            evilUnits.Add(new List<Unit>());
        }
    }

    // Get the list of enemies for the lane of the given unit.
    public List<Unit> GetEnemiesInLane(Unit unit)
    {
        // Good sees evil.
        if (unit.good)
        {
            return evilUnits[unit.laneIndex];
        } else {
            // Evil sees good.
            return goodUnits[unit.laneIndex];
        }
    }

    // Get a list of all good units.
    public List<Unit> GetAllGoodUnits()
    {
        // Initialize a new list.
        List<Unit> allGoodUnits = new List<Unit>();

        // Iterate through each lane.
        for (int i = 0; i < gridHeight; i++)
        {
            // Iterate through each good unit.
            foreach (Unit goodUnit in goodUnits[i])
            {
                allGoodUnits.Add(goodUnit);
            }
        }

        // Return.
        return allGoodUnits;
    }

    // Get a list of all evil units.
    public List<Unit> GetAllEvilUnits()
    {
        // Initialize a new list.
        List<Unit> allEvilUnits = new List<Unit>();

        // Iterate through each lane.
        for (int i = 0; i < gridHeight; i++)
        {
            // Iterate through each evil unit.
            foreach (Unit evilUnit in evilUnits[i])
            {
                allEvilUnits.Add(evilUnit);
            }
        }

        // Return.
        return allEvilUnits;
    }

    // Get a list of all enemy units, from the perspective of the given unit.
    public List<Unit> GetAllEnemies(Unit protagonist)
    {
        if (protagonist.good)
            return GetAllEvilUnits();
        else
            return GetAllGoodUnits();
    }

    // + Cards
    // Turn a unit into a card.
    // Note: Unit is both a card type, and the meta class used for all card types.
    // If you're confused, you're gettin it!
    public Card MakeCard(Unit unit, string cardType)
    {
        // Return a new card!
        return new Card(unit.myName, cardType, unit.manaCost, unit.deployTime, unit.role);
    }

    // + Game Over
    public void GameOver(bool victory)
    {
        // Avoid repeat calls.
        if (MenuManager.I.gameState >= 21)
            return;
            
        // Set game state.
        MenuManager.I.gameState = 21;

        // // Pause time.
        // Time.timeScale = 0f;

        // Disable ai.
        evilLeader.autoPilot = false;
        // UI.I.B_AutoPilotOff();

        // Reset the battle map.
        ResetBattleMap();

        // If we won, increment our planet index so the star manager knows to move us forward.
        if (victory)
            StarManager.I.planetIndex++;
            // Utility.SaveGame();
        // If we lost, reset our progress.
        else
            Utility.ResetSave();

        // Activate UI.
        UI.I.GameOver(victory);
    }

    // Reset the battle map by clearing old units and cards in the air.
    // Called at the end of each battle.
    public void ResetBattleMap()
    {
        // Destroy all old units, except Dragon Statues which are reset to deploying.
        // Good
        foreach (Unit unit in GetAllGoodUnits())
        {
            unit.Death();
            // if (unit.myName != "Dragon Statue")
            //     unit.Death();
        }
        // Evil
        foreach (Unit unit in GetAllEvilUnits())
        {
            unit.Death();
            // if (unit.myName != "Dragon Statue")
            //     unit.Death();
        }

        // Flowers
        // (Need temp list so we don't edit the list while iterating through it.)
        List<Unit> tempFlowers = new List<Unit>(flowers);
        foreach (Unit flower in tempFlowers)
        {
            flower.Death();
        }

        // Cards in the air.
        foreach (CardInHand cardInTheAir in cardsInTheAir)
        {
            if (cardInTheAir != null)
                Destroy(cardInTheAir.gameObject);
        }
        cardsInTheAir.Clear();

        // Clear selected card.
        if (InputBattle.I != null && InputBattle.I.selectedCard != null)
            InputBattle.I.selectedCard.Deselect();


        // Disable visual battle map!
        StarManager.I.GetCurrentPlanet().battleMap.SetActive(false);
    }

    // + Buttons

    // Button press: Continue!
    // Continue your adventure, after winning a battle.
    public void B_Continue()
    {
        // Safety guard(?)
        if (MenuManager.I.gameState != 21) return;

        // Re-enable time.
        // Time.timeScale = 1f;

        // Close the victory screen.
        UI.I.victoryBackground.gameObject.SetActive(false);

        // Close the battle map.
        gameObject.SetActive(false);


        // Open the star map.
        StarManager.I.GoToStarMap();
    }

    // Button press: Return!
    // Return to the main menu, after losing a battle.
    public void B_Return()
    {
        // Safety guard(?)
        if (MenuManager.I.gameState != 21) return;
        
        // Re-enable time.
        // Time.timeScale = 1f;

        // Close the defeat screen.
        // UI.I.defeatBackground.gameObject.SetActive(false);

        // // Go to the main menu.
        // MenuManager.I.GoToMainMenu();

        // Re-load game scene.
        Utility.LoadGameScene();
    }
}