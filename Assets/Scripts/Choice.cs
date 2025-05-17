using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

// Talent choice
// TBD: Refactor to ???
public class Choice : MonoBehaviour, IPointerClickHandler
{
    [Header("Choice")]
    public int id;
    public string myName;
    private Image image;

    void Awake()
    {
        image = GetComponent<Image>();
    }

    // Load a talent into this choice by name
    public void LoadTalent(string talentName)
    {
        // Set current talent
        myName = talentName;

        // Load image
        Utility.LoadImage(image, talentName);
    }

    // Select this talent.
    public void Selected()
    {
        GM.I.player.LearnTalent(myName);

        // Check if we should level again
        if (GM.I.player.xp >= GM.I.player.level * 100)
        {
            GM.I.player.BeginLevelUp();
        } else {
            GM.I.ui.CloseLevelUpScreen();
        }
    }

    // When a choice is hovered, load its details.
    public void Hover()
    {
        // Null check
        if (!GM.I.talents.ContainsKey(myName))
        {
            Debug.Log("Error! NULL talent!");
            return;
        }

        // Get talent
        Talent talent = GM.I.talents[myName];

        // Load the details of the talent
        GM.I.ui.LoadDetails(talent);

        // Set image opacity
        image.color = new Color(1, 1, 1, 0.5f);
    }

    public void Unhover()
    {
        // Set image opacity
        image.color = new Color(1, 1, 1, 1);
    }

    // When the 'Bet Your Luck' button is hovered, ominously offer a chance to twist fate.
    public void Hover2()
    {
        // Set image opacity
        image.color = new Color(1, 1, 1, 0.5f);

        // Set details
        BetYourLuckDetails();
    }

    // Load the details to bet your luck
    public void BetYourLuckDetails()
    {
        // Hide preview stats
        GM.I.ui.HideAllPreviewStats();

        // Load stats into right side of screen
        GM.I.ui.detail_name.text = "Bet Your Luck";
        GM.I.ui.detail_class.text = "(Witch)";
        GM.I.ui.detail_class.color = new Color (0.85f, 0.62f, 0.82f, 0.6f);
        GM.I.ui.detail_description.text = "Pull a thread of fate?";
        GM.I.ui.detail_witchMind.text = "+0";
        GM.I.ui.detail_witchBody.text = "+0";
        GM.I.ui.detail_witchSoul.text = "+0";
        GM.I.ui.detail_witchLuck.text = "-?";
        GM.I.ui.detail_familiarMind.text = "+0";
        GM.I.ui.detail_familiarBody.text = "+0";
        GM.I.ui.detail_familiarSoul.text = "+0";
        GM.I.ui.detail_familiarLuck.text = "-?";

        // Load image
        string fileName = "Bet Your Luck";
        Utility.LoadImage(GM.I.ui.detail_image, fileName);
    }

    public void Unhover2()
    {
        // Set image opacity
        image.color = new Color(1, 1, 1, 0.2f);
    }

    public void BetYourLuck()
    {
        // Roll d100
        int roll = Random.Range(0, 100);

        // Check your roll?
        if (roll == 100)
        {
            // Win!
            GM.I.player.luck++;
            GM.I.familiar.luck++;

            // Play sound effect
            GM.I.dj.PlayEffect("bet_your_luck_1", GM.I.player.transform.position, 1f);
        } else if (roll < 25)
        {
            // Witch loss
            GM.I.player.luck--;

            // Play sound effect
            GM.I.dj.PlayEffect("bet_your_luck_2", GM.I.player.transform.position, 1f);
        } else if (roll < 50)
        {
            // Familiar loss
            GM.I.familiar.luck--;

            // Play sound effect
            GM.I.dj.PlayEffect("bet_your_luck_3", GM.I.player.transform.position, 1f);
        } else {
            // Draw...
            
            // Play sound effect
            GM.I.dj.PlayEffect("bet_your_luck_4", GM.I.player.transform.position, 1f);
        }

        /*
        // Primitive version that always go one way or the other.
        // Effective, but maybe too strong.
        // Maybe not though so keeping it here for now.
        if (Random.Range(0, 100) > 50)
        {
            GM.I.player.luck--;
        } else {
            GM.I.familiar.luck--;
        }
        */
        
        // Reload character to show new luck
        GM.I.ui.LoadCharacter();

        // Roll new set of talents!
        GM.I.ui.RollNewTalents();

        // Play sound effect
        //GM.I.dj.PlayEffect("bet_your_luck", GM.I.player.transform.position, 1f);
    }

    // Called when a pointer clicks on this choice
    public void OnPointerClick(PointerEventData eventData)
    {
        // Check for right clicks
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            // Get talent
            Talent talent = GM.I.talents[myName];

            // Open wheel of talents
            GM.I.ui.OpenWheelOfTalents(talent);
        }
    }
}
