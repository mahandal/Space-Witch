using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class WheelChoice : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    // Spell icon
    public Image image;
    public string myName = "Jump";
    public bool isSpell = false;

    // called once initially
    void Awake()
    {
        image = GetComponent<Image>();
    }

    // called every time meditation wheel is opened
    void OnEnable()
    {
        // Reset opacity
        //Reset();
    }

    // Called when pointer enters this UI element
    public void OnPointerEnter(PointerEventData eventData)
    {
        // Spell wheel
        if (isSpell)
        {
            UnhighlightSpells();
            ChooseSpell();
            LoadDetail();
        } else {
            UnhighlightTalents();
            LoadDetail(true);
        }
        
        // Focus
        Highlight();
    }

    // Called when pointer exits this UI element
    public void OnPointerExit(PointerEventData eventData)
    {
        //Reset();
    }

    public void LoadDetail(bool costYourSoul = false)
    {
        Talent talent = GM.I.talents[myName];
        GM.I.ui.LoadDetails(talent, costYourSoul);
    }

    public void Highlight()
    {
        image.color = new Color(1, 1, 1, 1);
    }

    public void Unhighlight()
    {
        image.color = new Color(0.5f, 0.5f, 0.5f, 0.5f);
    }

    public static void UnhighlightSpells()
    {
        foreach (WheelChoice spell in GM.I.ui.spells)
        {
            spell.Unhighlight();
        }
    }

    public static void UnhighlightTalents()
    {
        foreach (WheelChoice talent in GM.I.ui.talents)
        {
            talent.Unhighlight();
        }
    }

    public static void HideTalents()
    {
        foreach (WheelChoice talent in GM.I.ui.talents)
        {
            talent.gameObject.SetActive(false);
        }
    }
    
    public static void HideTrainingTalents()
    {
        foreach (WheelChoice talent in GM.I.ui.trainingTalents)
        {
            talent.gameObject.SetActive(false);
        }
    } 

    // Chooses this spell as the player's currentSpell
    public void ChooseSpell()
    {
        // Set currentSpell
        GM.I.player.currentSpell = myName;

        // Update CD Circle image with corresponding spell image
        GM.I.ui.LoadSpellImage(myName);
    }

    // Chooses this talent as the player's new talent for this level
    public void ChooseTalent()
    {
        // Check if we're at home.
        if (GM.I.nebula.myName == "Home")
        {
            // If we already know this one, forget it!
            if (GM.I.saveData.unlockedTalents.Contains(myName))
                LockTalent();
            else
                UnlockTalent();
        }
        else
        {
            // In game, learn a talent.
            LearnTalent();
        }
    }

    // Unlock a talent.
    // (at home, costing credits)
    public void UnlockTalent()
    {
        int cost = GM.I.currentWitch.baseTalentCost;

        // Check if enough credits
        if (Gatherer.credits >= cost)
        {
            // Spend credits
            Gatherer.credits -= cost;

            // Add to unlocked talents
            GM.I.saveData.unlockedTalents.Add(myName);

            // Add to known spells if it's a spell
            Talent talent = GM.I.talents[myName];
            if (talent.isSpell && !GM.I.player.knownSpells.Contains(myName))
            {
                GM.I.player.knownSpells.Add(myName);
            }

            // Visual feedback
            //HitMarker.CreateLearnMarker(GM.I.player.transform.position, "Unlocked: " + myName);

            // Save game
            GM.I.SaveGame();

            // Reload
            GM.I.currentWitch.BeginTraining();
        }
        else
        {
            // Not enough
            //HitMarker.CreateLearnMarker(GM.I.player.transform.position, "Not enough credits!");
        }
    }

    // Lock a talent.
    public void LockTalent()
    {
        int cost = GM.I.currentWitch.baseTalentCost;

        // Check if enough credits.
        if (Gatherer.credits >= cost)
        {
            // Spend credits.
            Gatherer.credits -= cost;

            // Remove from unlocked talents.
            GM.I.saveData.unlockedTalents.Remove(myName);

            // Remove from known spells if it's a spell.
            Talent talent = GM.I.talents[myName];
            if (talent.isSpell && GM.I.player.knownSpells.Contains(myName))
            {
                GM.I.player.knownSpells.Remove(myName);
            }

            // Save game
            GM.I.SaveGame();

            // Reload
            GM.I.currentWitch.BeginTraining();
        }
    }

    // Learn a talent.
    // (in-game, bought with your soul)
    public void LearnTalent()
    {
        // Count talents bought
        GM.I.player.talentsBought++;

        // Spend your soul
        GM.I.player.soul -= GM.I.player.talentsBought;

        // Learn talent
        GM.I.player.LearnTalent(myName);

        // Close wheel of talents
        GM.I.ui.CloseWheelOfTalents();

        // Check if we should level again
        if (GM.I.player.xp >= GM.I.player.level * 100)
        {
            GM.I.player.BeginLevelUp();
        }
        else
        {
            GM.I.ui.CloseLevelUpScreen();
        }
    }

    // Load a spell into this choice by name
    public void LoadSpell(string spellName)
    {
        // Check if we're already good
        if (myName == spellName)
            return;

        // Set name
        myName = spellName;

        // Get file name
        string fileName = spellName + " (Spell)";

        // Load image
        Utility.LoadImage(image, fileName);
    }

    // Load a talent into this choice by name
    public void LoadTalent(string talentName)
    {
        // Check if we're already good
        /* if (myName == talentName)
            return; */
        
        // Set name
        myName = talentName;

        // Load image
        Utility.LoadImage(image, myName);
    }
    
    // Load the wheel of training!
    public static void LoadWheelOfTraining(Witch witch)
    {
        // First unhighlight and hide all
        HideTrainingTalents();

        // Track available slots
        int slotIndex = 0;

        // Loop through teachable talents
        foreach (string talentName in witch.teachableTalents)
        {
            // Skip if already unlocked
            // if (GM.I.saveData.unlockedTalents.Contains(talentName))
            //     continue;

            if (talentName == "Alchemist" || 
                talentName == "Enchantress" || 
                talentName == "Engineer" || 
                talentName == "Druid" || 
                talentName == "Oracle") continue;

            // Load talent into slot
            GM.I.ui.trainingTalents[slotIndex].LoadTalent(talentName);

            // Activate
            GM.I.ui.trainingTalents[slotIndex].gameObject.SetActive(true);

            // Next slot
            slotIndex++;

            // Stop if we've filled all slots
            if (slotIndex >= GM.I.ui.talents.Count)
                break;
        }
    }

    // Load in all the related talents to the wheel of talents.
    // (related talents are talents that share a class and rarity)
    public static void LoadWheelOfTalents(Talent relatedTalent)
    {
        // - Get list
        List<string> talentList = TalentManager.commonAlchemistTalents;

        // Alchemist
        if (relatedTalent.myClass == "Alchemist")
        {
            // Common
            if (relatedTalent.rarity == "Common")
            {
                talentList = TalentManager.commonAlchemistTalents;
            }

            // Uncommon
            else if (relatedTalent.rarity == "Uncommon")
            {
                talentList = TalentManager.uncommonAlchemistTalents;
            }

            // Uncommon
            else if (relatedTalent.rarity == "Rare")
            {
                talentList = TalentManager.rareAlchemistTalents;
            }
        }

        // Enchantress
        if (relatedTalent.myClass == "Enchantress")
        {
            // Common
            if (relatedTalent.rarity == "Common")
            {
                talentList = TalentManager.commonEnchantressTalents;
            }

            // Uncommon
            else if (relatedTalent.rarity == "Uncommon")
            {
                talentList = TalentManager.uncommonEnchantressTalents;
            }

            // Uncommon
            else if (relatedTalent.rarity == "Rare")
            {
                talentList = TalentManager.rareEnchantressTalents;
            }
        }

        // Engineer
        if (relatedTalent.myClass == "Engineer")
        {
            // Common
            if (relatedTalent.rarity == "Common")
            {
                talentList = TalentManager.commonEngineerTalents;
            }

            // Uncommon
            else if (relatedTalent.rarity == "Uncommon")
            {
                talentList = TalentManager.uncommonEngineerTalents;
            }

            // Uncommon
            else if (relatedTalent.rarity == "Rare")
            {
                talentList = TalentManager.rareEngineerTalents;
            }
        }

        // Druid
        if (relatedTalent.myClass == "Druid")
        {
            // Common
            if (relatedTalent.rarity == "Common")
            {
                talentList = TalentManager.commonDruidTalents;
            }

            // Uncommon
            else if (relatedTalent.rarity == "Uncommon")
            {
                talentList = TalentManager.uncommonDruidTalents;
            }

            // Uncommon
            else if (relatedTalent.rarity == "Rare")
            {
                talentList = TalentManager.rareDruidTalents;
            }
        }

        // Oracle
        if (relatedTalent.myClass == "Oracle")
        {
            // Common
            if (relatedTalent.rarity == "Common")
            {
                talentList = TalentManager.commonOracleTalents;
            }

            // Uncommon
            else if (relatedTalent.rarity == "Uncommon")
            {
                talentList = TalentManager.uncommonOracleTalents;
            }

            // Uncommon
            else if (relatedTalent.rarity == "Rare")
            {
                talentList = TalentManager.rareOracleTalents;
            }
        }

        // Curate talentList to unlocked talents
        List<string> fullTalentList = talentList;
        talentList = new List<string>();
        foreach (string talentName in fullTalentList)
        {
            if (GM.I.saveData.unlockedTalents.Contains(talentName))
            {
                talentList.Add(talentName);
            }
        }

        // Ok whew got the talentList

            // Now use it!

            // First unhighlight everything else
            HideTalents();

        // Then loop through talentList to fill the wheel of talents
        for (int i = 0; i < talentList.Count; i++)
        {
            // Get talent name
            string talentName = talentList[i];

            // Load talent
            GM.I.ui.talents[i].LoadTalent(talentName);

            // Activate!
            GM.I.ui.talents[i].gameObject.SetActive(true);
        }
    }
}