using UnityEngine;

public class MenuManager : MonoBehaviour
{
    [Header("Current Menu")]
    // Which screen we are currently on:
    // - Main Menu
    // - Star Map
    // - Explore
    // - Pre-Battle Intro
    // - Battle
    // - Post Game
    public string currentMenu = "Main Menu";


    [Header("Executives")]
    // The player's saved data.
    public SaveData saveData;

    // Yer grimoire.
    public Grimoire grimoire;

    // Constance.
    public Constance constance;

    // DJ.
    public DJ dj;

    // Settings.
    public Settings settings;

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

    // Granga's Guide to the Galaxy.
    public GGG ggg;

    // Card inspection screen.
    public Inspection inspection;

    // The tutorial.
    public Tutorial tutorial;

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

        // Initialize DJ.
        dj.Initialize();

        // Initialize settings.
        settings.Initialize();

        // Initialize main menu.
        mainMenu.Initialize();

        // Initialize star manager.
        starManager.Initialize();

        // Initialize DM.
        dm.Initialize();

        // Initialize GM.
        gm.Initialize();

        // Initialize Granga's Guide to the Galaxy.
        ggg.Initialize();

        // Initialize tutorial.
        tutorial.Initialize();

        // Load save data.
        saveData = Utility.GetSaveData();

        // Initialize grimoire.
        grimoire.Initialize();

        // Initialize inspection.
        inspection.Initialize();

        // Start on the main menu.
        GoToMainMenu();
    }


    // + Menu management
    // Disable all menus.
    public void DisableAllMenus()
    {
        // Disable settings menu.
        settings.gameObject.SetActive(false);
        
        // Disable the main menu.
        mainMenu.gameObject.SetActive(false);

        // Disable star map.
        starManager.gameObject.SetActive(false);

        // Disable explore mode.
        gm.gameObject.SetActive(false);

        // Disable battle map.
        dm.gameObject.SetActive(false);

        // Disable Granga's Guide to the Galaxy.
        ggg.gameObject.SetActive(false);

        // Disable grimoire.
        grimoire.gameObject.SetActive(false);

        // Disable inspection.
        inspection.gameObject.SetActive(false);
    }

    // Go to the main menu.
    public void GoToMainMenu()
    {
        // DJ, hit it!
        DJ.HitIt("Main Menu");

        // Disable all menus.
        DisableAllMenus();

        // Set current menu.
        currentMenu = "Main Menu";

        // Initialize the main menu.
        mainMenu.Initialize();

        // Enable main menu.
        mainMenu.gameObject.SetActive(true);
    }

    // Go to the star map.
    public void GoToStarMap()
    {
        // DJ, hit it!
        DJ.HitIt("Star Map");

        // Disable all menus.
        DisableAllMenus();

        // Set current menu.
        currentMenu = "Star Map";

        // Open star map.
        starManager.OpenStarMap();
    }

    // Go to the recruitment screen.
    public void GoToRecruitment()
    {
        // DJ, hit it!
        DJ.HitIt("Recruitment");

        // Disable main menu.
        mainMenu.gameObject.SetActive(false);

        // Enable star map.
        starManager.gameObject.SetActive(true);

        // Open recruitment screen.
        starManager.recruitment.OpenRecruitmentScreen();
    }

    // Go to the battle map.
    public void GoToBattleMap()
    {
        // Get current planet.
        Planet p = StarManager.I.GetCurrentPlanet();

        // DJ, hit it!
        DJ.HitIt(p);

        // Disable all menus.
        DisableAllMenus();

        // Also disable all battle maps.
        GM.I.DisableBattleMaps();

        // Set current menu to pre-battle intro.
        currentMenu = "Pre-Battle Intro";

        // Load the appropriate battle map.
        p.battleMap.SetActive(true);

        // Enable battle map.
        dm.gameObject.SetActive(true);
    }

    // Go to the explore map.
    public void GoToExplore()
    {
        // Disable all menus.
        DisableAllMenus();

        // Also disable all explore maps.
        GM.I.DisableExploreMaps();

        // Set current menu.
        currentMenu = "Explore";

        // Get current planet.
        Planet p = StarManager.I.GetCurrentPlanet();

        // Tell GM to set up the explore map for this planet.
        gm.Explore(p);

        // Load the appropriate battle map.
        p.exploreMap.SetActive(true);

        // Enable explore map.
        gm.gameObject.SetActive(true);
    }

    // Exit game.
    public void ExitGame()
    {
        Utility.ExitGame();
    }
}
