using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Stats")]
public class StatsSO : ScriptableObject
{
    public Stats stats;

    public Stats FetchStats(Stats existing = null)
    {
        return stats.CreateCopy(existing);
    }
}

[Flags]
public enum EntityType 
{ 
    None = 0, 
    Neutral = 1, 
    Player = 2, 
    Spirit = 4, 
    Zombie = 8, 
    Demon = 16
}

[Serializable]
public class Stats
{
    [Header("Static Stats")]
    public EntityType allegience;
    public int expDropped;
    public AnimationCurve expLevelCurve;
    public float baseDamage;
    public float baseMovementSpeed = 1;

    // Dynamic Stats (not displayed)
    [Header("Dynamic Stats")]
    //[HideInInspector]
    public int level;
    //[HideInInspector]
    public int currentExp;
    //[HideInInspector]
    public float currentHealth;
    //[HideInInspector]
    public Vector2 facing;
    [HideInInspector]
    public List<Buff> buffs;
    //[HideInInspector]
    public Dictionary<Ability, int> activationTicks; // For abilities that don't trigger repeatedly, and buff/debuff durations
    
    [Header("Modifiable Stats")]
    public float maxHealth;
    public int cooldownReduction;
    public float damageMultiplier = 1;
    public float movementScaling = 1;
    public float areaScaling = 1;
    public float expAttractScaling = 1;
    public EntityType overrideDamageAllegience;

    public Stats CreateCopy(Stats existing)
    {
        Stats stats = (Stats) MemberwiseClone();
        if (existing != null)
        {
            stats.allegience = existing.allegience;
            stats.level = existing.level;
            stats.currentExp = existing.currentExp;
            stats.currentHealth = existing.currentHealth;
            stats.facing = existing.facing;
            stats.buffs = existing.buffs;
            stats.activationTicks = existing.activationTicks;
            stats.overrideDamageAllegience = existing.overrideDamageAllegience;
        }
        else
        {
            stats.currentHealth = stats.maxHealth;
            stats.buffs = new List<Buff>();
            stats.activationTicks = new Dictionary<Ability, int>();
        }
        return stats;
    }
}

public enum BuffType { Slow, Stun, Knockback }

public class Buff
{
    public BuffType type;
    public int ticksRemaining;
    public float intensity;
    public Entity source;

    public Buff(BuffType type, int ticksRemaining, float intensity, Entity source)
    {
        this.type = type;
        this.ticksRemaining = ticksRemaining;
        this.intensity = intensity;
        this.source = source;
    }
}