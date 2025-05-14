using UnityEngine;
using System.Collections.Generic;

public abstract class Talent : MonoBehaviour
{
    [Header("Spell")]
    public bool isSpell = false;
    public float manaCost = 1f;
    public float cooldown = 1f;

    [Header("Talent")]
    public string myName;
    public string myClass;
    public string rarity;
    public string description;
    public int witchMind;
    public int witchBody;
    public int witchSoul;
    public int witchLuck;
    public int familiarMind;
    public int familiarBody;
    public int familiarSoul;
    public int familiarLuck;

    // Called when talent is first acquired
    public virtual void OnLearn() { }
    
    // Called when the player casts their spell
    public virtual void OnCast() { }

    // Called when gathering a star
    public virtual void OnGather(Star star, Gatherer gatherer) { }

    // Called when gathering a moon
    public virtual void OnGather(Moon moon, Gatherer gatherer) { }
    
    // Called each frame for continuous effects
    public virtual void OnFixedUpdate(Gatherer gatherer) { }

    // Constructor
    // Note: I think if you write too many of these in your life, you will go insane. Be careful!
    // Remember: Mind -> Body -> Soul -> Luck
    public Talent(string _myName, string _myClass, string _rarity, string _description, int _witchMind, int _witchBody, int _witchSoul, int _witchLuck, int _familiarMind, int _familiarBody, int _familiarSoul, int _familiarLuck)
    {
        myName = _myName;
        myClass = _myClass;
        rarity = _rarity;
        description = _description;
        witchMind = _witchMind;
        witchBody = _witchBody;
        witchSoul = _witchSoul;
        witchLuck = _witchLuck;
        familiarMind = _familiarMind;
        familiarBody = _familiarBody;
        familiarSoul = _familiarSoul;
        familiarLuck = _familiarLuck;
    }
}