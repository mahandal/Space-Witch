using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class LoadScreen : MonoBehaviour
{
    [Header("Machinery")]
    // How many different backgrounds to roll between.
    public int numBackgrounds = 100;

    // The background image.
    public Image background;

    // The start button.
    public Button startButton;

    // The hint text object.
    public TMP_Text hintText;

    [Header("Timer")]
    // How long it takes to fade in the start button and fade out the background.
    public float fadeInTime = 2f;

    // Timer tracking how long we've been on this screen.
    public float timeElapsed = 0f;

    // A list of hints!
    private List<string> hints = new List<string>();

    // A list of rumors!
    private List<string> rumors = new List<string>();

    // + Initialization
    void Start()
    {
        // Roll a random background.
        int roll = Random.Range(1, numBackgrounds + 1);

        // Load background.
        Utility.LoadImage(background, "Loading/Loading " + roll);

        // Load hint or rumor.
        HintOrRumor();
    }

    void FixedUpdate()
    {
        // Count up time elapsed.
        timeElapsed += Time.fixedDeltaTime;

        // Fade in.
        if (timeElapsed < fadeInTime)
        {
            // Get percent faded.
            float percentFaded = timeElapsed / fadeInTime;

            // Set button tint.
            startButton.image.color = new Color(percentFaded, percentFaded, percentFaded, percentFaded);

            // Set text color.
            hintText.color = new Color(percentFaded, percentFaded, percentFaded, percentFaded);

            // Get background tint.
            // Start at full saturation, fade out to half over time.
            float b = 1 - (percentFaded / 2);
            background.color = new Color (b, b, b, 1f);            
        } else {
            // Set background color.
            background.color = new Color (0.5f, 0.5f, 0.5f, 1f);

            // Set button color.
            startButton.image.color = new Color(1f, 1f, 1f, 1f);
        }
    }

    // + Buttons
    public void B_Start()
    {
        // Go to the main menu!
        Utility.LoadGameScene();
    }

    public void B_Reload()
    {
        Utility.LoadLoadScreen();
    }


    // + Hints, tips, and rumors!
    private void SetUpHintsAndRumor()
    {
        // Hints
        hints.Add("Gather violet flowers to gain mana!");
        hints.Add("Ye don't need to win every lane to win a battle!");
        hints.Add("Muscle wins fights. Mana wins wars!");
        hints.Add("Tis tough to be a Space Witch. Fret not on a loss!");
        hints.Add("Ye learn more from loss than victory!");
        hints.Add("Make yer own victory!");
        hints.Add("If yer having fun, ye've truly won!");
        hints.Add("Play cheap units to gather violet flowers!");
        hints.Add("Roles are broad categories meant to help understand how to play cards.");
        hints.Add("Scouts tend to cost less mana and have high speed and vision. Use them to scout ahead!");
        hints.Add("Bulwarks tend to have high health and armor, and slow move speed. They are expected to be the bulk of most battles");
        hints.Add("Hunters tend to deal high damage at long range. Or at least longer range than a bulwark!");
        hints.Add("Production generates incredible value, over time...");
        hints.Add("Units with the same movement speed stay in formation while marching across the map.");
        hints.Add("Play hunters behind bulwarks!");
        hints.Add("The keyword 'Cleave' lets a unit cut through any number of foes what stand in its way!");
        hints.Add("The keyword 'Bash' means a unit can stun enemies with its attacks.");
        hints.Add("Stunned units stand helpless for a random duration.");
        hints.Add("Dwarves are resistant to stuns.");
        hints.Add("Orcs gain damage with each attack.");
        hints.Add("Slain skeletons detach their heads and live on again!");
        hints.Add("Ye cannot kill a lich, but ye can bury it when it rises.");
        hints.Add("Each campaign mission is hand designed with some elements consistent and some randomized.");

        // Rumors
        rumors.Add("Everyone knows Constance the innkeeper is old. But some say she may be truly ancient...");
        rumors.Add("Constance once picked up a full grown ogre and threw him out of her bar. She's one strong goblin!");
        rumors.Add("Don't start fights at the Dragon's Roost! The owner, Constance, has no tolerance for violence!");
        rumors.Add("The Big Race attracts attention from all over for its daring antics. But is it as fair as they say?");
        rumors.Add("Goblintown has a reputation for chaos and danger. But if ye know yer way around, there's no better place to party!");
        rumors.Add("Camelot is a castle from times before, now used as headquarters of the Space Witch Coven.");
        rumors.Add("Morgause has betrayed us before, but never like this...");
        rumors.Add("The Pact was to prevent times like these...");
        rumors.Add("The Pact has been broken! But who broke it first?");
        rumors.Add("Most assume the orcs of Orkney broke the Pact first. But they firmly deny it!");
        rumors.Add("Some say the ogres of Logres broke the Pact first. Their struggle with temptation is well known...");
        rumors.Add("Some say the trolls of Gorr broke the Pact first. After all, they are already the most broken!");
        rumors.Add("Could it be elves of Sarras broke the Pact first? I most certainly hope not!");
        rumors.Add("Maybe twas dwarves of Bedegraine broke the Pact first? Their Mountain King has gone mad!");
        rumors.Add("I know we humans of Avalon did not break the Pact first! Except maybe that pesky Morgause...");
        rumors.Add("Merlin learned secrets of the universe that broke his mind. Ignorance is bliss!");
        rumors.Add("The old gods are weak and scattered, fighting each other to their bitter ends.");
        rumors.Add("There are twenty six old gods, one for each letter of the alphabet. Or rather, tis the other way round...");
        rumors.Add("Migue Qigue is a new mermaid demigod, beloved for his humble generosity.");
        rumors.Add("Solara is god of sunshine, warmer of worlds and light above all. Her sign is a sun. She appears as a glowing eagle.");
        rumors.Add("Arcano is god of magic, teacher of wizards and guide to the gifted. His sign is a crossed wand and staff. He appears as a wrinkly old man with a long, long beard.");
        rumors.Add("Natura is god of forests, devoted divine of druids. Her sign is a deep rooted tree. She appears as a deer, simple and sweet.");
        rumors.Add("Honoro is god of battle, hero of heroes and champion of champions! His sign is a sword and shield. He appears as a tall man with the head of a lion.");
        rumors.Add("Warriors fight to earn a place in Honoro's Hall after life!");
        rumors.Add("Glugoni is god of feasts and festivity. His sign is a spilled goblet. He appears as a jovial walrus wearing a chef's hat and apron.");
        rumors.Add("Bouldo is god of masonry, smithing, and crafting of all kinds. His sign is a hammer and nail. He appears as a wheel or a tire.");
    }

    public void HintOrRumor()
    {
        // Generate lists of hints and rumors.
        SetUpHintsAndRumor();

        // Flip a coin!
        int coinFlip = Random.Range(0, 2);

        // Hint or rumor?
        if (coinFlip == 0)
        {
            // Roll hint index.
            int hintIndex = Random.Range(0, hints.Count);

            // Set text.
            hintText.text = "Hint: " + hints[hintIndex];
        }
        else
        {
            // Roll rumor index.
            int rumorIndex = Random.Range(0, rumors.Count);

            // Set text.
            hintText.text = rumors[rumorIndex];
        }
    }
}
