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
    // Tier is an optional parameter used to limit which cards will be pulled.
    public void GeneratePack(List<string> availableCards, int tier = -1)
    {
        // Decide how many cards this pack will have.
        int cardCount = Random.Range(1, tier + 5);

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
            // Use tier to limit which card we might pull.
            // (+1 because Random.Range is max exclusive with ints)
            int endIndex = tier + 1;

            // Mercenary cards ignore tiers, always pulling from the whole list.
            if (tier < 0)
                endIndex = availableCards.Count;

            // Roll card index.
            int cardIndex = Random.Range(0, endIndex);

            // Get card name.
            string cardName = availableCards[cardIndex];

            // Get random level.
            int level = Constance.I.RandomLevel();

            // Add level to name.
            cardName = "Level " + level + " " + cardName;

            // Load minicard.
            minicards[i].LoadCard(cardName);
        }
    }
}
