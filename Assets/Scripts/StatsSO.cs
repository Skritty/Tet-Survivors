using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
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
    public float totalDistanceTraveled;
    //[HideInInspector]
    public Vector2 facing;
    //[HideInInspector]
    public List<Buff> buffs;
    //[HideInInspector]
    public Dictionary<Ability, int> activationTicks; // For abilities that don't trigger repeatedly, and buff/debuff durations
    public Dictionary<Ability, AoE> aoes;
    
    [Header("Modifiable Stats")]
    public float maxHealth;
    public float regeneration;
    public float cooldownReduction = 1;
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
            stats.expDropped = existing.expDropped;
            stats.currentHealth = existing.currentHealth;
            stats.facing = existing.facing;
            stats.buffs = existing.buffs;
            stats.activationTicks = existing.activationTicks;
            stats.aoes = existing.aoes;
            stats.overrideDamageAllegience = existing.overrideDamageAllegience;
        }
        else
        {
            stats.currentHealth = stats.maxHealth;
            stats.buffs = new List<Buff>();
            stats.activationTicks = new Dictionary<Ability, int>();
            stats.aoes = new Dictionary<Ability, AoE>();
        }
        return stats;
    }

    public void StatCombineAdditive(Stats other)
    {
        maxHealth += other.maxHealth;
        regeneration += other.regeneration;
        cooldownReduction += other.cooldownReduction - 1;
        damageMultiplier += other.damageMultiplier - 1;
        movementScaling += other.movementScaling - 1;
        areaScaling += other.areaScaling - 1;
        expAttractScaling += other.expAttractScaling - 1;
    }

    public void StatCombineMultiplicative(Stats other)
    {
        maxHealth += other.maxHealth;
        regeneration += other.regeneration;
        cooldownReduction += other.cooldownReduction;
        damageMultiplier *= other.damageMultiplier;
        movementScaling *= other.movementScaling;
        areaScaling *= other.areaScaling;
        expAttractScaling *= other.expAttractScaling;
    }
}

public enum BuffType { Slow, Stun, Knockback, DoT, Stat }
public enum StatCombineMethod { Additive, Multiplicative }

[Serializable]
public class Buff
{
    public BuffType type;
    public Stats statBuff;
    public StatCombineMethod combineMethod;
    public int ticksRemaining;
    public float intensity;
    public Entity source;
    public Vector3 dir;

    public Buff(BuffType type, int ticksRemaining, float intensity, Entity source, float x = 0, float y = 0, Stats statBuff = null, StatCombineMethod combineMethod = StatCombineMethod.Additive)
    {
        this.type = type;
        this.ticksRemaining = ticksRemaining;
        this.intensity = intensity;
        this.source = source;
        this.dir = new Vector3(x, y, 0);
        this.statBuff = statBuff;
        this.combineMethod = combineMethod;
    }
}