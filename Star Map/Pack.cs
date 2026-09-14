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
    // Star and tier determine available cards.
    public void GeneratePack(Star s, int tier)
    {
        // Decide how many cards this pack will have.
        int cardCount = Random.Range(1, 5);

        // Go through each minicard.
        for (int i = 0; i < minicards.Count; i++)
        {
            // Enable cards in use.
            // Disable unused minicards.
            if (i < cardCount)
                minicards[i].gameObject.SetActive(true);
            else
                minicards[i].gameObject.SetActive(false);

            // Roll a random card.
            int cardIndex = Random.Range(0, tier);

            // Get card name.
            string cardName = s.cards[cardIndex];

            // Add level.
            cardName = "Level 2 " + cardName;

            // Load minicard.
            minicards[i].LoadCard(cardName);
        }
    }
}
