using UnityEngine;

public class MenuManager : MonoBehaviour
{
    [Header("Game State")]
    // Which screen we are currently on:
    // -2 = Main Menu
    // -1 = Star Map
    //  0 = Explore
    //  1 = Battle
    //  2 = Postgame
    public int gameState = -2;

    [Header("Menus")]
    // Constance.
    public Constance constance;

    // The main menu.
    public MainMenu mainMenu;

    // The star manager.
    // (Also the parent object of the star map.)
    public StarManager starManager;

    // The Game Master.
    // (Also the parent object of explore mode.)
    public GM gm;

    // The Dungeon Master.
    // (Also the parent object of battle mode.)
    public DM dm;

    // The player's saved data.
    public SaveData saveData;


    // Singleton.
    public static MenuManager I;
    

    // + Initialization.

    // Awaken!
    void Awake()
    {
        // Singleton.
        if (I == null)
            I = this;
        else
            Destroy(this);

        // Initialize Constance.
        constance.Initialize();

        // Initialize main menu.
        mainMenu.Initialize();

        // Initialize star manager.
        starManager.Initialize();

        // Initialize DM.
        dm.Initialize();

        // Initialize GM.
        gm.Initialize();

        // Start on the main menu.
        GoToMainMenu();
    }


    // + Menu management
    // Go to the main menu.
    public void GoToMainMenu()
    {
        // Set game state.
        gameState = -2;

        // Disable star map.
        starManager.gameObject.SetActive(false);

        // Disable battle map.
        dm.gameObject.SetActive(false);

        // Disable explore mode.
        gm.gameObject.SetActive(false);

        // Initialize the main menu.
        mainMenu.Initialize();

        // Enable main menu.
        mainMenu.gameObject.SetActive(true);
    }

    // Go to the battle map.
    public void GoToBattleMap()
    {
        // Set game state.
        gameState = 1;

        // Get current planet.
        Planet p = StarManager.I.GetCurrentPlanet();

        // Load the appropriate battle map.
        p.battleMap.SetActive(true);

        // Disable the explore map.
        p.exploreMap.SetActive(false);

        // Disable main menu.
        mainMenu.gameObject.SetActive(false);

        // Disable star map.
        starManager.gameObject.SetActive(false);

        // Disable explore map.
        gm.gameObject.SetActive(false);

        // Enable battle map.
        dm.gameObject.SetActive(true);
    }

    // Go to the explore map.
    public void GoToExplore()
    {
        // Set game state.
        gameState = 0;

        // Get current planet.
        Planet p = StarManager.I.GetCurrentPlanet();

        // GM handles its own stuff.
        gm.Explore(p);

        // Load the appropriate battle map.
        p.exploreMap.SetActive(true);

        // Disable main menu.
        mainMenu.gameObject.SetActive(false);

        // Disable star map.
        starManager.gameObject.SetActive(false);

        // Disable battle map.
        dm.gameObject.SetActive(false);

        // Enable explore map.
        gm.gameObject.SetActive(true);
    }

    // Exit game.
    public void ExitGame()
    {
        Utility.ExitGame();
    }
}
