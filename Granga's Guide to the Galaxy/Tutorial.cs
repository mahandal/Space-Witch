using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class Tutorial : MonoBehaviour
{
    // Singleton
    public static Tutorial I;

    [Header("Tips")]
    // Which tip are we on?
    public int tipIndex = 0;

    // A list of tips, in the order they are revealed.
    public List<GameObject> tips;

    // + Initialization
    public void Initialize()
    {
        I = this;

        HideAllTips();
    }


    // + Tutorial
    // Begin the tutorial!
    public void Begin()
    {
        // Freeze time.
        Time.timeScale = 0f;

        // Reset tip index.
        tipIndex = 0;

        // Reveal the first tip.
        RevealTip(tipIndex);
    }

    // Finish the tutorial.
    public void End()
    {
        // Fog of war!
        UI.I.fogOfWar.gameObject.SetActive(true);

        // Unfreeze time.
        Time.timeScale = 1f;
    }

    // Go to the next tip!
    public void NextTip()
    {
        // Hide all tips.
        HideAllTips();

        // Increment tip index.
        tipIndex++;

        // Done?
        if (tipIndex >= tips.Count)
        {
            // End the tutorial.
            End();

            // Return.
            return;
        }

        // Reveal the next tip.
        RevealTip(tipIndex);
    }

    // Hide all tips.
    public void HideAllTips()
    {
        // Loop through each tip.
        for (int i = 0; i < tips.Count; i++)
        {
            // Hide.
            tips[i].gameObject.SetActive(false);
        }
    }

    // Reveal the tip at the index given.
    public void RevealTip(int index)
    {
        // Hide all tips.
        HideAllTips();

        tips[index].SetActive(true);
    }

    // + Decks
    public List<string> EasyDeck()
    {
        // Initialize new list.
        List<string> decklist = new List<string>();

        // Add 100 level 5 sword fighters.
        for (int i = 0; i < 100; i++)
        {
            decklist.Add("Level 5 Sword Fighter");
        }

        // Return.
        return decklist;
    }
}
