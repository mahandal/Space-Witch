using UnityEngine;
using System.Collections.Generic;

// Alchemist
public class Alchemist : Talent
{
    public Alchemist() : base("Alchemist", "Alchemist", "Common", 
                                "Alchemy is the study of change and equivalent exchange.", 
                                2, 2, 2, 2, 2, 2, 2, 2)
    {
    }
}

// Healing Potion
public class HealingPotion : Talent
{
    public HealingPotion() : base("Healing Potion", "Alchemist", "Common", 
                                "SPELL: Brew a healing potion.", 
                                1, 0, 0, 0, 0, 0, 0, 0)
    {
        // Spell stats
        isSpell = true;
        manaCost = 75f;
        cooldown = 13f;
    }

    public override void OnCast()
    {
        // Instantiate
        Potion potion = Object.Instantiate(GM.I.spawnManager.progenitor_HealingPotion, GM.I.universe);

        // Set position
        potion.transform.position = GM.I.player.transform.position;

        // Set strength
        potion.strength = 100f * GM.I.player.talents[myName];

        // Set size
        float newScale = potion.transform.localScale.x * (1 + GM.I.player.talents[myName] * 0.1f);
        potion.transform.localScale = new Vector3(newScale, newScale, newScale);

        // Activate
        potion.gameObject.SetActive(true);
    }
}

// Mana Potion
public class ManaPotion : Talent
{
    public ManaPotion() : base("Mana Potion", "Alchemist", "Common", 
                                "SPELL: Brew a mana potion.", 
                                1, 0, 0, 0, 0, 0, 0, 0)
    {
        // Spell stats
        isSpell = true;
        manaCost = 35f;
        cooldown = 13f;
    }

    public override void OnCast()
    {
        // Instantiate
        Potion potion = Object.Instantiate(GM.I.spawnManager.progenitor_ManaPotion, GM.I.universe);

        // Set position
        potion.transform.position = GM.I.player.transform.position;

        // Set strength
        potion.strength = 50f * GM.I.player.talents[myName];

        // Set size
        float newScale = potion.transform.localScale.x * (1 + GM.I.player.talents[myName] * 0.1f);
        potion.transform.localScale = new Vector3(newScale, newScale, newScale);

        // Activate
        potion.gameObject.SetActive(true);
    }
}

// Explosion Potion
public class ExplosionPotion : Talent
{
    public ExplosionPotion() : base("Explosion Potion", "Alchemist", "Common", 
                                "SPELL: Brew an explosion potion.", 
                                1, 0, 0, 0, 0, 0, 0, 0)
    {
        // Spell stats
        isSpell = true;
        manaCost = 100f;
        cooldown = 13f;
    }

    public override void OnCast()
    {
        // Instantiate
        Potion potion = Object.Instantiate(GM.I.spawnManager.progenitor_ExplosionPotion, GM.I.universe);

        // Set position
        potion.transform.position = GM.I.player.transform.position;

        // Set strength
        potion.strength = 2f * GM.I.player.talents[myName];

        // Set size
        float newScale = potion.transform.localScale.x * (1 + GM.I.player.talents[myName] * 0.1f);
        potion.transform.localScale = new Vector3(newScale, newScale, newScale);

        // Activate
        potion.gameObject.SetActive(true);
    }
}