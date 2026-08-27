using UnityEngine;

public partial class Leader
{
    // + Morgan le Fey
    // Charm the target.
    // Costs mana, equal to the target's mana cost.
    public void MorganCharm(Unit target)
    {
        // Fail if not enough mana.
        if (mana < target.manaCost) return;

        // Spend mana.
        mana -= target.manaCost;

        // Charm!
        target.ChangeSides(false);
    }

    // + Wubalin Brightforge
    // Shoot the target.
    // Costs 1 mana.
    public void WubalinShoot(Unit target)
    {
        // Fail if not enough mana.
        if (mana < 1) return;

        // Spend mana.
        mana -= 1;

        // Stun.
        target.Stun();

        // Deal 10 damage.
        target.LoseHealth(10);
    }

    // + Sybil Solisi
    // Heal the target.
    // Costs 1 mana.
    public void SybilHeal(Unit target)
    {
        // Fail if not enough mana.
        if (mana < 1) return;

        // Spend mana.
        mana -= 1;

        // Unstun?
        target.Unstun();

        // Heal 10 health.
        target.GainHealth(10);
    }

    // + Markaus Allstrong
    // Deal 1 damage to the target.
    // Costs 1 health.
    public void MarkausPunch(Unit target)
    {
        // Lose health.
        LoseHealth(1);

        // Deal damage.
        target.LoseHealth(1);
    }

    // + Shruk
    // Consume an item.
    public void ShrukEat(Unit target)
    {
        // Gain mana.
        mana += target.manaCost;

        // Gain health.
        GainHealth(target.currentHealth);

        // Clean up object.
        target.BeginDying();
    }

    // + Gatama the Seer
    // Heal the target for 1 health.
    // Costs 1 health.
    public void GatamaHeal(Unit target)
    {
        // Lose health.
        LoseHealth(1);

        // Heal target.
        target.GainHealth(1);
    }

    // + Penelope
    // Eat the target.
    // Costs mana, equal to the target's mana cost times its current health percent (rounded up).
    public void PenEat(Unit target)
    {
        // Get target's health percent.
        // float healthPercent = target.currentHealth / target.maxHealth;

        // Get mana needed to eat target.
        // int manaNeeded = Mathf.CeilToInt(target.manaCost * healthPercent);

        // Fail if not enough mana.
        // TBD: Meep merp!
        if (mana < target.manaCost) return;

        // Spend mana.
        mana -= target.manaCost;

        // Gain health?
        GainHealth(target.currentHealth);

        // Eat target.
        target.Death();
    }

    // Guinevere
    // Speed or slow cards deploying.
    // Costs health, equal to the card's deploy time.
    public void GuinevereSing(Unit target)
    {
        // Spend health.
        LoseHealth(target.deployTimer);

        // Ally cards are deployed twice as fast.
        // Enemy cards are deployed half as fast.
        if (good == target.good)
            target.deployTimer /= 2;
        else
            target.deployTimer *= 2;
    }

    // Lancelot
    // Sacrifice a unit, killing it immediately to gain mana equal to its cost.
    public void Sacrifice(Unit victim)
    {
        // Can't sacrifice your base!
        if (victim.role == "Base") return;
        
        // Kill victim.
        victim.BeginDying();

        // Gain mana.
        mana += victim.manaCost;
    }
}
