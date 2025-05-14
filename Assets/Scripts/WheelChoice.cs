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
            GM.I.player.LevelUp();
        } else {
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