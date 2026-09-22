using UnityEngine;
using System.Collections.Generic;

// A pack of cards.
public class Pack : MonoBehaviour
{
    [Header("Pack")]
    // The full list of this pack's minicards.
    // (Some may be inactive!)
    public List<MiniCard> minicards;

    // Randomly generate a new pack of mercenary cards.
    // Note: Very similar to normal pack generation below. Could combine maybe?
    public void GenerateMercenaryPack()
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
            int cardIndex = Random.Range(0, Constance.I.mercenaries.Count);

            // Get card name.
            string cardName = Constance.I.mercenaries[cardIndex];

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
            int cardIndex = Random.Range(0, tier + 1);

            // Get card name.
            string cardName = s.cards[cardIndex];

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
