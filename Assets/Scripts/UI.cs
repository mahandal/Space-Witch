using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class UI : MonoBehaviour
{
    [Header("Top")]
    public GameObject top;
    public TMP_Text xText;
    public TMP_Text yText;
    public TMP_Text starCount;
    public TMP_Text songTimer;
    
    
    [Header("Middle")]
    public GameObject tooltip;
    public TMP_Text tooltipName;
    public TMP_Text tooltipDescription;
    public Image tooltipImage;
    public Image CD_Circle;
    public GameObject meditationWheel;
    public GameObject wheelOfTalents;
    //public List<SpellChoice> spells;
    public List<WheelChoice> spells;
    public List<WheelChoice> talents;

    [Header("Bottom")]
    public Slider playerHealthBar;
    public Slider playerManaBar;
    public Slider familiarHealthBar;
    public Slider familiarManaBar;

    // Our current level
    public TMP_Text levelText;

    // Text telling us to level up!
    public TMP_Text levelUpText;
    public Image xpBar;

    [Header("Left")]
    // Current values
    public TMP_Text witchHealth;
    public TMP_Text witchMana;
    public TMP_Text witchMind;
    public TMP_Text witchBody;
    public TMP_Text witchSoul;
    public TMP_Text witchLuck;
    public TMP_Text familiarHealth;
    public TMP_Text familiarMana;
    public TMP_Text familiarMind;
    public TMP_Text familiarBody;
    public TMP_Text familiarSoul;
    public TMP_Text familiarLuck;

    // New values (if hovered talent is chosen)
    public TMP_Text newWitchHealth;
    public TMP_Text newWitchMana;
    public TMP_Text newWitchMind;
    public TMP_Text newWitchBody;
    public TMP_Text newWitchSoul;
    public TMP_Text newWitchLuck;
    public TMP_Text newFamiliarHealth;
    public TMP_Text newFamiliarMana;
    public TMP_Text newFamiliarMind;
    public TMP_Text newFamiliarBody;
    public TMP_Text newFamiliarSoul;
    public TMP_Text newFamiliarLuck;

    // List of known talents
    public List<KnownTalent> knownTalents;

    [Header("Center")]
    public Choice alchemistTalent;
    public Choice enchantressTalent;
    public Choice engineerTalent;
    public Choice druidTalent;
    public Choice oracleTalent;

    [Header("Right")]
    public TMP_Text detail_name;
    public TMP_Text detail_class;
    public TMP_Text detail_description;
    public TMP_Text detail_witchMind;
    public TMP_Text detail_witchBody;
    public TMP_Text detail_witchSoul;
    public TMP_Text detail_witchLuck;
    public TMP_Text detail_familiarMind;
    public TMP_Text detail_familiarBody;
    public TMP_Text detail_familiarSoul;
    public TMP_Text detail_familiarLuck;
    public RawImage detail_image;

    [Header("Settings")]
    // Audio
    public Slider masterVolume;
    public Slider musicVolume;
    public Slider sfxVolume;
    public Slider gatherVolume;

    [Header("Full")]
    public GameObject pauseMenu;
    //public GameObject preGame;
    public GameObject scoreboard;
    public GameObject levelUpScreen;
    public GameObject mySelf;
    public GameObject growth;
    public GameObject postGame;
    public GameObject winBackground;
    public GameObject lossBackground;

    //[Header("States")]

    // Health change flash
    private bool healthChanging = false;
    private float targetHealthValue = 1f;
    private float currentDisplayHealth = 1f;
    private float healthDrainSpeed = 2.5f;
    private float healthFlashTimer = 0f;
    private float healthFlashDuration = 0.8f;

    private bool familiarHealthChanging = false;
    private float targetFamiliarHealthValue = 1f;
    private float currentDisplayFamiliarHealth = 1f;
    private float familiarHealthFlashTimer = 0f;

    // Mana change flash

    private bool manaChanging = false;
    private float targetManaValue = 1f;
    private float currentDisplayMana = 1f;
    private float manaDrainSpeed = 5f;
    private float manaFlashTimer = 0f;
    private float manaFlashDuration = 1f;

    private bool familiarManaChanging = false;
    private float targetFamiliarManaValue = 1f;
    private float currentDisplayFamiliarMana = 1f;
    private float familiarManaFlashTimer = 0f;

    // Spells
    public float lastSpellCastCooldown = 1f;

    // Cooldown flash
    private bool flashCooldown = false;
    private float cooldownFlashTimer = 0f;
    private float cooldownFlashDuration = 0.5f;

    // Mana flash
    private bool flashManaBar = false;
    //private float manaFlashTimer = 0f;
    //private float manaFlashDuration = 0.5f;

    // XP
    private bool canLevelUp = false;
    private float flashTimer = 0f;
    private float flashSpeed = 2f;
    public Color originalXPColor;

    // Other
    private Player player;
    private Familiar familiar;

    void Awake()
    {
        // Store stuff for later(?)
        originalXPColor = xpBar.color;

        // Disable stuff that don't need to be
        // (mostly unnecessary, just makes building easier)
        meditationWheel.SetActive(false);
        CloseWheelOfTalents();
    }

    void Start()
    {
        player = GM.I.player;
        familiar = GM.I.familiar;

        // Load initial spell image
        LoadSpellImage(GM.I.player.currentSpell);

        // Load settings
        LoadSettings();
    }

    public void LoadSpellImage(string spellName)
    {
        string fileName = spellName + " (Spell)";
        
        Utility.LoadImage(CD_Circle, fileName);
    }

    void FixedUpdate()
    {
        // Early return pre-game
        /* if (GM.I.gameState < 1)
            return; */

        // Hide top bar at home
        if (GM.I.nebula == "Home")
            top.SetActive(false);
        else
            top.SetActive(true);
            
        // Coordinates
        UpdateCoordinates();

        // Star count
        UpdateStarCount();

        // Song timer
        UpdateSongTimer();

        // Cooldown circle
        UpdateCDCircle();

        // Health bars
        UpdateHealthBars();

        // Mana bars
        UpdateManaBars();

        // XP
        UpdateXP();

        // Black hole
        UpdateBlackHole();
    }

    void UpdateSongTimer()
    {
        int minutes = (int)GM.I.gameTimer / 60;
        int seconds = (int)GM.I.gameTimer % 60;
        string minuteString = minutes.ToString();
        string secondString = seconds.ToString();
        if (seconds < 10)
            secondString = "0" + seconds.ToString();
        string currentTimeString = minuteString + ":" + secondString;
        songTimer.text = currentTimeString;
    }

    void UpdateStarCount()
    {
        // Set text
        starCount.text = Gatherer.starsGathered.ToString("0");
    }

    void UpdateHealthBars()
    {
        // Get actual health percentages
        float playerHealthPercent = GM.I.player.currentHealth / GM.I.player.maxHealth;
        float familiarHealthPercent = GM.I.familiar.currentHealth / GM.I.familiar.maxHealth;
        
        // PLAYER HEALTH UPDATES
        if (playerHealthPercent != currentDisplayHealth && !healthChanging)
        {
            // Start health animation effect
            healthChanging = true;
            targetHealthValue = playerHealthPercent;
            healthFlashTimer = 0f;
        }
        
        // Handle health animation effect
        if (healthChanging)
        {
            // Flash the player health bar with appropriate color
            healthFlashTimer += Time.deltaTime;
            float flashIntensity = Mathf.PingPong(healthFlashTimer * 12f, 1f);
            
            // Red for damage, green for healing
            Color flashColor = targetHealthValue < currentDisplayHealth ? 
                            Color.Lerp(Color.red, Color.white, flashIntensity) : 
                            Color.Lerp(Color.green, Color.white, flashIntensity);
                            
            playerHealthBar.fillRect.GetComponent<Image>().color = flashColor;
            
            // Adjust health bar at appropriate speed
            float animationSpeed = targetHealthValue < currentDisplayHealth ? 
                                healthDrainSpeed : healthDrainSpeed * 0.8f; // Healing slightly slower
                                
            currentDisplayHealth = Mathf.Lerp(currentDisplayHealth, targetHealthValue, Time.deltaTime * animationSpeed);
            playerHealthBar.value = currentDisplayHealth;
            
            // Check if effect should end
            if (Mathf.Abs(currentDisplayHealth - targetHealthValue) < 0.01f && healthFlashTimer >= healthFlashDuration)
            {
                // Finalize effect
                currentDisplayHealth = targetHealthValue;
                playerHealthBar.value = currentDisplayHealth;
                healthChanging = false;
                // Reset to normal color - red for consistency with existing system
                playerHealthBar.fillRect.GetComponent<Image>().color = Color.red;
            }
        }
        
        // FAMILIAR HEALTH UPDATES
        if (familiarHealthPercent != currentDisplayFamiliarHealth && !familiarHealthChanging)
        {
            // Start damage effect
            familiarHealthChanging = true;
            targetFamiliarHealthValue = familiarHealthPercent;
            familiarHealthFlashTimer = 0f;
        }
        
        // Handle familiar health animation
        if (familiarHealthChanging)
        {
            // Flash the familiar health bar with appropriate color
            familiarHealthFlashTimer += Time.deltaTime;
            float flashIntensity = Mathf.PingPong(familiarHealthFlashTimer * 12f, 1f);
            
            // Red for damage, green for healing
            Color flashColor = targetFamiliarHealthValue < currentDisplayFamiliarHealth ? 
                            Color.Lerp(Color.red, Color.white, flashIntensity) : 
                            Color.Lerp(Color.green, Color.white, flashIntensity);
                            
            familiarHealthBar.fillRect.GetComponent<Image>().color = flashColor;
            
            // Adjust health bar at appropriate speed
            float animationSpeed = targetFamiliarHealthValue < currentDisplayFamiliarHealth ? 
                                healthDrainSpeed : healthDrainSpeed * 0.8f; // Healing slightly slower
                                
            currentDisplayFamiliarHealth = Mathf.Lerp(currentDisplayFamiliarHealth, targetFamiliarHealthValue, Time.deltaTime * animationSpeed);
            familiarHealthBar.value = currentDisplayFamiliarHealth;
            
            // Check if effect should end
            if (Mathf.Abs(currentDisplayFamiliarHealth - targetFamiliarHealthValue) < 0.01f && familiarHealthFlashTimer >= healthFlashDuration)
            {
                // Finalize effect
                currentDisplayFamiliarHealth = targetFamiliarHealthValue;
                familiarHealthBar.value = currentDisplayFamiliarHealth;
                familiarHealthChanging = false;
                // Reset to normal color - red for consistency with existing system
                familiarHealthBar.fillRect.GetComponent<Image>().color = Color.red;
            }
        }
    }

    void UpdateManaBars()
    {
        // Remember what's normal
        Color normalColor = new Color(0f, 0.1f, 1f);

        // Get actual mana percentages
        float playerManaPercent = GM.I.player.currentMana / GM.I.player.maxMana;
        float familiarManaPercent = GM.I.familiar.currentMana / GM.I.familiar.maxMana;
        
        // PLAYER MANA UPDATES
        // Always update the target, not just when we detect a change
        if (playerManaPercent != targetManaValue || !manaChanging)
        {
            targetManaValue = playerManaPercent;
            
            // Only start the flash effect for significant changes
            if (Mathf.Abs(currentDisplayMana - targetManaValue) > 0.01f && !manaChanging)
            {
                manaChanging = true;
                manaFlashTimer = 0f;
            }
        }
        
        // Handle mana animation effect - always smoothly move toward target
        currentDisplayMana = Mathf.Lerp(currentDisplayMana, targetManaValue, Time.deltaTime * manaDrainSpeed);
        playerManaBar.value = currentDisplayMana;
        
        // Handle flashing only when in "changing" state
        if (manaChanging)
        {
            // Flash the player mana bar with appropriate color
            manaFlashTimer += Time.deltaTime;
            float flashIntensity = Mathf.PingPong(manaFlashTimer, 1f);
            
            // Blue for loss, brighter blue for gain
            Color flashColor = targetManaValue < currentDisplayMana ? 
                            Color.Lerp(normalColor, Color.white, flashIntensity) : 
                            Color.Lerp(normalColor, Color.blue, flashIntensity);
                            
            playerManaBar.fillRect.GetComponent<Image>().color = flashColor;
            
            // Check if effect should end
            if (Mathf.Abs(currentDisplayMana - targetManaValue) < 0.005f && manaFlashTimer >= manaFlashDuration)
            {
                manaChanging = false;
                // Reset to normal color - blue for mana
                //playerManaBar.fillRect.GetComponent<Image>().color = new Color(0, 0.1f, 1f);
                playerManaBar.fillRect.GetComponent<Image>().color = normalColor;
            }
        }
        
        // FAMILIAR MANA UPDATES - Same concept as player
        if (familiarManaPercent != targetFamiliarManaValue || !familiarManaChanging)
        {
            targetFamiliarManaValue = familiarManaPercent;
            
            if (Mathf.Abs(currentDisplayFamiliarMana - targetFamiliarManaValue) > 0.01f && !familiarManaChanging)
            {
                familiarManaChanging = true;
                familiarManaFlashTimer = 0f;
            }
        }
        
        // Always smoothly update
        currentDisplayFamiliarMana = Mathf.Lerp(currentDisplayFamiliarMana, targetFamiliarManaValue, Time.deltaTime * manaDrainSpeed);
        familiarManaBar.value = currentDisplayFamiliarMana;
        
        if (familiarManaChanging)
        {
            // Flash the familiar mana bar with appropriate color
            familiarManaFlashTimer += Time.deltaTime;
            float flashIntensity = Mathf.PingPong(familiarManaFlashTimer, 1f);
            
            Color flashColor = targetFamiliarManaValue < currentDisplayFamiliarMana ? 
                            Color.Lerp(normalColor, Color.white, flashIntensity) : 
                            Color.Lerp(normalColor, Color.blue, flashIntensity);
                            
            familiarManaBar.fillRect.GetComponent<Image>().color = flashColor;
            
            if (Mathf.Abs(currentDisplayFamiliarMana - targetFamiliarManaValue) < 0.005f && familiarManaFlashTimer >= manaFlashDuration)
            {
                familiarManaChanging = false;
                //familiarManaBar.fillRect.GetComponent<Image>().color = new Color(0, 0.1f, 1f);
                familiarManaBar.fillRect.GetComponent<Image>().color = normalColor;
            }
        }

        
        
        // Insufficient mana flash
        if (flashManaBar)
        {
            // Flash the player mana bar
            manaFlashTimer += Time.deltaTime;
            
            float flashIntensity = Mathf.PingPong(manaFlashTimer * 10f, 1f);
            playerManaBar.fillRect.GetComponent<Image>().color = Color.Lerp(normalColor, Color.white, flashIntensity);
            
            if (manaFlashTimer >= manaFlashDuration)
            {
                flashManaBar = false;
                manaFlashTimer = 0f;
                // playerManaBar.fillRect.GetComponent<Image>().color = new Color(0, 0.1f, 1f);
                playerManaBar.fillRect.GetComponent<Image>().color = normalColor;
            }
        }
    }


    public void TriggerManaFlash()
    {
        flashManaBar = true;
        manaFlashTimer = 0f;
    }

    void UpdateCDCircle()
    {
        // Set the cd circle's fill equal to our current spell cooldown divided by the cooldown of the last spell we cast, stored here in UI for specifically this purpose.
        //CD_Circle.fillAmount = GM.I.player.spellCooldown / lastSpellCastCooldown;

        // Same as above but inverse so we fill up to full as we cool down
        //CD_Circle.fillAmount = 1 - (GM.I.player.spellCooldown / lastSpellCastCooldown);

        // Get current spell's cooldown
        string currentSpell = GM.I.player.currentSpell;
        float cooldownTime = GM.I.player.spellCooldowns.ContainsKey(currentSpell) ? 
                            GM.I.player.spellCooldowns[currentSpell] : 0f;
        float maxCooldown = GM.I.talents[currentSpell].cooldown;
    
        // Fill amount shows remaining cooldown (1 = ready, 0 = just used)
        CD_Circle.fillAmount = 1 - (cooldownTime / maxCooldown);

        // Flash opacity if the player tries to cast a spell while cooling down
        if (flashCooldown)
        {
            // Flash the circle red
            cooldownFlashTimer += Time.deltaTime;
            
            // Make the color pulse between normal and red
            float flashIntensity = Mathf.PingPong(cooldownFlashTimer * 10f, 1f);
            CD_Circle.color = Color.Lerp(Color.red, Color.white, flashIntensity);
            
            // End the flash effect after duration
            if (cooldownFlashTimer >= cooldownFlashDuration)
            {
                flashCooldown = false;
                cooldownFlashTimer = 0f;
                CD_Circle.color = new Color(1, 1, 1, 0.13f); // Reset to normal color
            }
        }
    }

    // Flash the cd circle to indicate you can't cast another spell yet.
    public void TriggerCooldownFlash()
    {
        flashCooldown = true;
        cooldownFlashTimer = 0f;
    }

    void UpdateCoordinates()
    {
        xText.text = GM.I.player.transform.position.x.ToString("0");
        yText.text = GM.I.player.transform.position.y.ToString("0");
    }

    void UpdateXP()
    {
        xpBar.fillAmount = GM.I.player.xp / (GM.I.player.level * 100);
        levelText.text = GM.I.player.level.ToString("0");
        
        // Check if player can level up
        canLevelUp = GM.I.player.xp >= GM.I.player.level * 100;
        
        // Make the XP bar flash when ready to level up
        if (canLevelUp)
        {
            // Ping pong color
            flashTimer += Time.deltaTime * flashSpeed;
            Color flashColor = new Color(originalXPColor.g, originalXPColor.b, originalXPColor.r, Mathf.PingPong(flashTimer, 1f));
            xpBar.color = flashColor;
            
            // Show and animate level up text
            levelUpText.gameObject.SetActive(true);
            levelUpText.color = flashColor;
            //levelUpText.color = new Color(1f, 1f, 1f, Mathf.PingPong(flashTimer * 2f, 1f));
        }
        else
        {
            // Reset to normal color when not ready to level up
            xpBar.color = originalXPColor;

            // Hide level up text
            levelUpText.gameObject.SetActive(false);
        }
    }

    public void OpenLevelUpScreen()
    {
        // Pause universe
        GM.I.universe.gameObject.SetActive(false);

        // Note: We don't just open the level up screen like so:
        //levelUpScreen.SetActive(true);
        // because we want the character and details sections of it to be available when meditating.
        // So instead...

        // Activate each section of the level up screen
        mySelf.SetActive(true);
        growth.SetActive(true);

        // Load the left side of the level up screen
        LoadCharacter();

        // Load the middle of the level up screen
        RollNewTalents();
    }

    public void CloseLevelUpScreen()
    {
        // Reload character
        LoadCharacter();

        // Deactivate level up screen
        mySelf.SetActive(false);
        growth.SetActive(false);
        
        // Resume universe
        GM.I.universe.gameObject.SetActive(true);

        // Start the game if we just chose our first talent
        if (GM.I.player.level == 1)
            GM.I.BeginRun();

        // Close level up screen
        //levelUpScreen.SetActive(false);
    }

    // Roll a new set of talents
    public void RollNewTalents()
    {
        // Initialize
        string alchemist = "Alchemist";
        string enchantress = "Enchantress";
        string engineer = "Engineer";
        string druid = "Druid";
        string oracle = "Oracle";

        // Randomize
        // TBD: Incorporate other rarities.
        alchemist = TalentManager.GetRandomTalent("Alchemist", "Common");
        enchantress = TalentManager.GetRandomTalent("Enchantress", "Common");
        engineer = TalentManager.GetRandomTalent("Engineer", "Common");
        druid = TalentManager.GetRandomTalent("Druid", "Common");
        oracle = TalentManager.GetRandomTalent("Oracle", "Common");

        // --- Load talents

        // - Alchemist
        alchemistTalent.LoadTalent(alchemist);

        // - Enchantress
        enchantressTalent.LoadTalent(enchantress);

        // - Engineer
        engineerTalent.LoadTalent(engineer);

        // - Druid
        druidTalent.LoadTalent(druid);

        // - Oracle
        oracleTalent.LoadTalent(oracle);
    }

    // Load the details of a given talent
    public void LoadDetails(Talent talent, bool costYourSoul = false)
    {
        // Load stats into left side of screen
        PreviewStats(talent, costYourSoul);

        // Load stats into right side of screen
        detail_name.text = talent.myName;
        detail_class.text = "(" + talent.myClass + ")";

        // Set class color
        if (talent.myClass == "Alchemist")
            GM.I.ui.detail_class.color = new Color (0.78f, 0.42f, 0.69f, 0.5f);
        
        if (talent.myClass == "Enchantress")
            GM.I.ui.detail_class.color = new Color (0.69f, 0.78f, 0.42f, 0.5f);
            
        if (talent.myClass == "Engineer")
            GM.I.ui.detail_class.color = new Color (0.78f, 0.78f, 0.78f, 0.5f);
        
        if (talent.myClass == "Druid")
            GM.I.ui.detail_class.color = new Color (0.42f, 0.78f, 0.69f, 0.5f);
        
        if (talent.myClass == "Oracle")
            GM.I.ui.detail_class.color = new Color (0.42f, 0.69f, 0.78f, 0.5f);
            
        detail_description.text = talent.description;
        detail_witchMind.text = "+" + talent.witchMind.ToString();
        detail_witchBody.text = "+" + talent.witchBody.ToString();
        detail_witchSoul.text = "+" + talent.witchSoul.ToString();
        detail_witchLuck.text = "+" + talent.witchLuck.ToString();
        detail_familiarMind.text = "+" + talent.familiarMind.ToString();
        detail_familiarBody.text = "+" + talent.familiarBody.ToString();
        detail_familiarSoul.text = "+" + talent.familiarSoul.ToString();
        detail_familiarLuck.text = "+" + talent.familiarLuck.ToString();

        // Load image
        string fileName = talent.myName;
        Utility.LoadImage(detail_image, fileName);
    }

    public void PreviewStats(Talent talent, bool costYourSoul = false)
    {
        // First hide all
        HideAllPreviewStats();

        // Then go through each stat and check if it should be shown

        // Witch body & health
        if (talent.witchBody > 0)
        {
            // Body
            int newBody = player.body + talent.witchBody;
            newWitchBody.text = newBody.ToString();
            newWitchBody.gameObject.SetActive(true);

            // Health
            int newHealth = player.maxHealth + talent.witchBody * 50;
            newWitchHealth.text = newHealth.ToString();
            newWitchHealth.gameObject.SetActive(true);
        }

        // Witch mind & mana
        if (talent.witchMind > 0)
        {
            // Mind
            int newMind = player.mind + talent.witchMind;
            newWitchMind.text = newMind.ToString();
            newWitchMind.gameObject.SetActive(true);

            // Mana
            int newMana = player.maxMana + talent.witchMind * 12;
            newWitchMana.text = newMana.ToString();
            newWitchMana.gameObject.SetActive(true);
        }

        // Are you spending your soul for this?
        if (costYourSoul)
        {
            int soulGain = talent.witchSoul;
            int soulCost = GM.I.player.talentsBought + 1;
            int netSoulChange = soulGain - soulCost;
            int newSoul = player.soul + netSoulChange;
            
            newWitchSoul.text = newSoul.ToString();
            
            // If we're losing soul overall (cost > gain), show red
            if (netSoulChange <= 0)
                newWitchSoul.color = Color.red;
            else
                newWitchSoul.color = Color.green;
                
            newWitchSoul.gameObject.SetActive(true);
        }
        // Normal witch soul
        else if (talent.witchSoul > 0)
        {
            int newSoul = player.soul + talent.witchSoul;
            newWitchSoul.text = newSoul.ToString();
            newWitchSoul.color = Color.green;
            newWitchSoul.gameObject.SetActive(true);
        }

        // Witch luck
        if (talent.witchLuck > 0)
        {
            int newLuck = player.luck + talent.witchLuck;
            newWitchLuck.text = newLuck.ToString();
            newWitchLuck.gameObject.SetActive(true);
        }

        // Familiar body & health
        if (talent.familiarBody > 0)
        {
            // Body
            int newBody = familiar.body + talent.familiarBody;
            newFamiliarBody.text = newBody.ToString();
            newFamiliarBody.gameObject.SetActive(true);

            // Health
            int newHealth = familiar.maxHealth + talent.familiarBody * 50;
            newFamiliarHealth.text = newHealth.ToString();
            newFamiliarHealth.gameObject.SetActive(true);
        }

        // Familiar mind & mana
        if (talent.familiarMind > 0)
        {
            // Mind
            int newMind = familiar.mind + talent.familiarMind;
            newFamiliarMind.text = newMind.ToString();
            newFamiliarMind.gameObject.SetActive(true);

            // Mana
            int newMana = familiar.maxMana + talent.familiarMind * 12;
            newFamiliarMana.text = newMana.ToString();
            newFamiliarMana.gameObject.SetActive(true);
        }

        // Familiar soul
        if (talent.familiarSoul > 0)
        {
            int newSoul = familiar.soul + talent.familiarSoul;
            newFamiliarSoul.text = newSoul.ToString();
            newFamiliarSoul.gameObject.SetActive(true);
        }

        // Familiar luck
        if (talent.familiarLuck > 0)
        {
            int newLuck = familiar.luck + talent.familiarLuck;
            newFamiliarLuck.text = newLuck.ToString();
            newFamiliarLuck.gameObject.SetActive(true);
        }
    }

    public void HideAllPreviewStats()
    {
        newWitchHealth.gameObject.SetActive(false);
        newWitchMana.gameObject.SetActive(false);
        newWitchMind.gameObject.SetActive(false);
        newWitchBody.gameObject.SetActive(false);
        newWitchSoul.gameObject.SetActive(false);
        newWitchLuck.gameObject.SetActive(false);

        newFamiliarHealth.gameObject.SetActive(false);
        newFamiliarMana.gameObject.SetActive(false);
        newFamiliarMind.gameObject.SetActive(false);
        newFamiliarBody.gameObject.SetActive(false);
        newFamiliarSoul.gameObject.SetActive(false);
        newFamiliarLuck.gameObject.SetActive(false);
    }

    // Loads the left side of the level up screen
    public void LoadCharacter()
    {
        // Stats
        witchHealth.text = player.maxHealth.ToString();
        witchMana.text = player.maxMana.ToString();
        witchMind.text = player.mind.ToString();
        witchBody.text = player.body.ToString();
        witchSoul.text = player.soul.ToString();
        witchLuck.text = player.luck.ToString();
        familiarHealth.text = familiar.maxHealth.ToString();
        familiarMana.text = familiar.maxMana.ToString();
        familiarMind.text = familiar.mind.ToString();
        familiarBody.text = familiar.body.ToString();
        familiarSoul.text = familiar.soul.ToString();
        familiarLuck.text = familiar.luck.ToString();

        // - Talents

        // First disable all
        foreach(KnownTalent knownTalent in knownTalents)
        {
            knownTalent.gameObject.SetActive(false);
        }

        // Keep track of how many talents we have
        int numTalents = 0;

        // Go through each of the player's talents
        foreach(string talentName in player.talents.Keys)
        {
            // Get KnownTalent
            KnownTalent knownTalent = knownTalents[numTalents];
            
            // Load talent
            knownTalent.LoadTalent(talentName);
            
            // Activate
            knownTalent.gameObject.SetActive(true);

            // Increment num talents
            numTalents++;
        }
    }

    // Prepares the pre game UI
    /* public void PreGame()
    {
        // Activate pre-game object
        preGame.SetActive(true);

        // Activate scoreboard
        scoreboard.SetActive(true);
    } */

    // Closes the pre-game UI
    /* public void BeginGame()
    {
        // Deactivate pre-game object
        preGame.SetActive(false);

        // Deactivate scoreboard
        scoreboard.SetActive(false);
    } */

    // Opens the post-game UI
    public void PostGame()
    {
        // Activate post-game object
        postGame.gameObject.SetActive(true);

        // Activate scoreboard
        scoreboard.SetActive(true);
    }

    // Opens the pause menu
    public void Pause()
    {
        pauseMenu.SetActive(true);
    }

    // Closes the pause menu
    public void Unpause()
    {
        pauseMenu.SetActive(false);
    }

    public void Meditate()
    {
        // Load character if we're at home cause we haven't done that yet
        if (GM.I.nebula == "Home")
            LoadCharacter();
            
        // First forget everything
        foreach (WheelChoice spell in spells)
        {
            spell.gameObject.SetActive(false);
        }

        // Load known spells
        for (int i = 0; i < GM.I.player.knownSpells.Count; i++)
        {
            // Get name
            string spellName = GM.I.player.knownSpells[i];

            // Load spell
            spells[i].LoadSpell(spellName);

            // Check: is it our current spell?
            if (spellName == GM.I.player.currentSpell)
            {
                // If it is our current spell, highlight it!
                spells[i].Highlight();
            } else {
                // If it's not our current spell, reset it.
                spells[i].Unhighlight();
            }
                
            // Activate
            spells[i].gameObject.SetActive(true);
        }

        // Start the wheel
        meditationWheel.SetActive(true);
        mySelf.SetActive(true);

        // Highlight current spell also
        CD_Circle.color = new Color(1, 1, 1, 1);

        // Hide the universe
        GM.I.universe.gameObject.SetActive(false);
    }

    public void EndMeditation()
    {
        // Stop the wheel
        meditationWheel.SetActive(false);

        if (!growth.activeSelf)
            mySelf.SetActive(false);

        // Fade current spell
        CD_Circle.color = new Color(0.5f, 0.5f, 0.5f, 0.5f);

        // Reveal the universe
        GM.I.universe.gameObject.SetActive(true);
    }

    public void OpenWheelOfTalents(Talent relatedTalent)
    {
        // Load
        WheelChoice.LoadWheelOfTalents(relatedTalent);

        // Activate
        wheelOfTalents.SetActive(true);
    }

    public void CloseWheelOfTalents()
    {
        wheelOfTalents.SetActive(false);
    }

    // Load the settings menu
    // TBD: Move other settings here
    public void LoadSettings()
    {
        //screenShakeToggle.isOn = GM.I.screenShakeEnabled;
    }

    // Black holes
    public float blackHoleChannelTimer = 0f;
    public float blackHoleChannelTime = 3f;
    //public bool inBlackHoleTransition = false;
    private Image blackHoleVignette;

    void UpdateBlackHole()
    {
        // If we're not in a black hole, fade back out
        if (GM.I.player.currentBlackHole == null)
        {
            if (blackHoleChannelTimer > 0)
            {
                blackHoleChannelTimer -= Time.deltaTime;

                // Get regress percentage
                float regress = 1 - (blackHoleChannelTimer / blackHoleChannelTime);

                // Set camera 
                GM.I.player.mainCam.orthographicSize = Mathf.Lerp(0.1f, 5f, regress);

                // Set scale
                GM.I.player.transform.localScale = GM.I.player.originalScale * (0.5f + regress * 0.5f);

                // Un-darken screen edges
                if (blackHoleVignette != null)
                    blackHoleVignette.color = new Color(0, 0, 0, (1 - regress) * 0.8f);
            }

            return;
        }
        
        blackHoleChannelTimer += Time.deltaTime;
        float progress = blackHoleChannelTimer / blackHoleChannelTime;
        
        // Screen vignette effect
        if (blackHoleVignette == null)
        {
            blackHoleVignette = new GameObject("BlackHoleVignette").AddComponent<Image>();
            blackHoleVignette.transform.SetParent(transform, false);
            blackHoleVignette.rectTransform.anchorMin = Vector2.zero;
            blackHoleVignette.rectTransform.anchorMax = Vector2.one;
            blackHoleVignette.rectTransform.sizeDelta = Vector2.zero;
            blackHoleVignette.color = new Color(0, 0, 0, 0);
        }
        
        // Darken screen edges
        blackHoleVignette.color = new Color(0, 0, 0, progress * 0.8f);
        
        // Rotate and scale player
        GM.I.player.transform.Rotate(0, 0, progress * 360 * Time.deltaTime);
        GM.I.player.transform.localScale = GM.I.player.originalScale * (1 - progress * 0.5f);
        
        // Camera zoom effect
        if (GM.I.player.mainCam != null)
        {
            GM.I.player.mainCam.orthographicSize = Mathf.Lerp(5, 0.1f, progress);
        }
        
        // Execute transition when complete
        if (blackHoleChannelTimer >= blackHoleChannelTime)
        {
            // Clean up effects
            if (blackHoleVignette != null)
                Destroy(blackHoleVignette.gameObject);
                
            // Reset camera and player
            GM.I.player.mainCam.orthographicSize = 5;
            GM.I.player.transform.localScale = GM.I.player.originalScale;
                
            // Transition complete
            EndMeditation();
            //inBlackHoleTransition = false;
            blackHoleChannelTimer = 0f;
            
            // Begin the run
            GM.I.GoBig(GM.I.player.currentBlackHole.destination);
        }
    }

    // Load tooltip
    /* public void LoadTooltip(string myName, string description, Sprite sprite)
    {
        tooltip.SetActive(true);
        tooltipName.text = myName;
        tooltipDescription.text = description;
        tooltipImage.sprite = sprite;
    } */

    /* public void LoadTooltip(Tooltip tt)
    {
        // activate
        tooltip.SetActive(true);

        // load name
        tooltipName.text = tt.myName;

        // load description
        tooltipDescription.text = tt.description;

        // Check if we should use our special tooltip
        if (tt.specialImage != null && GM.I.player.isCalm)
            tooltipImage.sprite = tt.specialImage.sprite;
        else
            tooltipImage.sprite = tt.sprite;
    } */
}
