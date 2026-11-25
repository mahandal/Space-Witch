using UnityEngine;
using System.Collections.Generic;

// A pack of cards.
public class Pack : MonoBehaviour
{
    [Header("Pack")]
    // The full list of this pack's minicards.
    // (Some may be inactive!)
    public List<MiniCard> minicards;

    // Randomly generate a new pack of cards.
    public void GeneratePack(List<string> availableCards)
    {
        // Decide how many cards this pack will have.
        int cardCount = Random.Range(1, StarManager.I.planetIndex + 5);

        // Go through each minicard.
        for (int i = 0; i < minicards.Count; i++)
        {
            // Enable cards in use.
            // Disable unused minicards.
            if (i < cardCount)
                minicards[i].gameObject.SetActive(true);
            else
                minicards[i].gameObject.SetActive(false);

            // + Roll a random card.

            // Roll card index.
            int cardIndex = Random.Range(0, availableCards.Count);

            // Get card name.
            string cardName = availableCards[cardIndex];

            // Get power level.
            int powerLevel = 0;

            if (MainMenu.I.difficulty < 0)
                powerLevel = -MainMenu.I.difficulty;

            // Get random level.
            int level = Constance.I.RandomLevel(powerLevel);

            // Add level to name.
            cardName = "Level " + level + " " + cardName;

            // Load minicard.
            minicards[i].LoadCard(cardName);
        }
    }
}
