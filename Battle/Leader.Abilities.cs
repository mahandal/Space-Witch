using UnityEngine;

public partial class Leader
{
    // + Morgan le Fey
    // Charm the target.
    public void MorganCharm(Unit target)
    {
        // Check charges.
        if (powerCharges <= 0) return;

        // Spend charge.
        powerCharges--;

        // Charm!
        target.ChangeSides(false);
    }

    // + Wubalin Brightforge
    // Shoot the target.
    public void WubalinShoot(Unit target)
    {
        // Check charges.
        if (powerCharges <= 0) return;

        // Spend charge.
        powerCharges--;

        // Stun.
        target.Stun();

        // Deal 10 damage.
        target.LoseHealth(20);
    }

    // + Sybil Solisi
    // Heal the target.
    public void SybilHeal(Unit target)
    {
        // Avoid wasting charges on full health targets.
        if (target.currentHealth >= target.maxHealth) return;

        // Check charges.
        if (powerCharges <= 0) return;

        // Spend charge.
        powerCharges--;

        // Unstun?
        target.Unstun();

        // Heal 10 health.
        target.GainHealth(20);
    }

    // + Markaus Allstrong
    // Punch the target.
    public void MarkausPunch(Unit target)
    {
        // Check charges.
        if (powerCharges <= 0) return;

        // Spend charge.
        powerCharges--;

        // Deal damage.
        target.LoseHealth(2);
    }

    // + Shruk
    // Consume an item.
    public void ShrukEat(Unit target)
    {
        // Check charges.
        if (powerCharges <= 0) return;

        // Spend charge.
        powerCharges--;

        // Gain mana.
        mana += target.manaCost;

        // Gain health.
        GainHealth(target.currentHealth);

        // Clean up object.
        target.BeginDying();
    }

    // + Gatama the Seer
    // Heal the target for 1 health.
    public void GatamaHeal(Unit target)
    {
        // Avoid wasting charges on full health targets.
        if (target.currentHealth >= target.maxHealth) return;
        
        // Check charges.
        if (powerCharges <= 0) return;

        // Spend charge.
        powerCharges--;

        // Heal target.
        target.GainHealth(2);
    }

    // + Penelope
    // Eat the target.
    public void PenEat(Unit target)
    {
        // Check charges.
        if (powerCharges <= 0) return;

        // Spend charge.
        powerCharges--;

        // Gain health?
        GainHealth(target.currentHealth);

        // Eat target.
        target.Death();
    }

    // Guinevere
    // Speed or slow cards deploying.
    public void GuinevereSing(Unit target)
    {
        // Check charges.
        if (powerCharges <= 0) return;

        // Spend charge.
        powerCharges--;

        // Ally cards are deployed twice as fast.
        // Enemy cards are deployed half as fast.
        if (good == target.good)
            target.deployTimer /= 2;
        else
            target.deployTimer *= 2;
    }

    // Lancelot
    // Sacrifice a unit to gain mana and health.
    public void Sacrifice(Unit victim)
    {
        // Can't sacrifice your base!
        if (victim.role == "Base") return;

        // Check charges.
        if (powerCharges <= 0) return;

        // Spend charge.
        powerCharges--;
        
        // Kill victim.
        victim.BeginDying();

        // Gain mana.
        mana += victim.manaCost;

        // Gain health.
        GainHealth(victim.currentHealth);
    }
}
