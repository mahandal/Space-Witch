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

            // + Roll a random card.
            // Use tier to limit which card we might pull.
            int endIndex = tier + 1;

            // Mercenary cards ignore tiers, always pulling from the whole list.
            if (tier < 0)
                endIndex = availableCards.Count;

            // Roll card index.
            int cardIndex = Random.Range(0, endIndex);

            // Get card name.
            string cardName = availableCards[cardIndex];

            // + Level
            // Default to level 2.
            int level = 2;

            // Roll for higher level.
            int d100 = Random.Range(1, 101);

            // 80% level 2
            // 15% level 3
            // 4% level 4
            // 1% level 5
            if (d100 > 80 && d100 < 95)
                level = 3;
            else if (d100 >= 95 && d100 < 100)
                level = 4;
            else if (d100 == 100)
                level = 5;

            // Add level to name.
            cardName = "Level " + level + " " + cardName;

            // Load minicard.
            minicards[i].LoadCard(cardName);
        }
    }
}
