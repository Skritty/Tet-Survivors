using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

public class Entity : PooledObject
{
    public Entity owner; // For if this is a proxy
    public StatsSO baseStats;
    //[HideInInspector]
    public Stats stats;
    //[HideInInspector]
    public int currentTick;
    public List<Ability> abilities;

    public Action OnDeath;
    public Action OnHit;
    public Action WhenHit;

    public bool IsStunned
    {
        get
        {
            foreach(Buff buff in stats.buffs)
            {
                if(buff.type == BuffType.Stun)
                {
                    return true;
                }
            }
            return false;
        }
    }

    public float SlowMulti
    {
        get
        {
            float slow = 1;
            foreach (Buff buff in stats.buffs)
            {
                if (buff.type == BuffType.Slow)
                {
                    slow *= buff.intensity;
                }
            }
            return slow;
        }
    }

    public Vector3 Knockback
    {
        get
        {
            Vector3 dir = Vector3.zero;
            foreach (Buff buff in stats.buffs)
            {
                if (buff.type == BuffType.Knockback)
                {
                    if(buff.dir == Vector3.zero)
                        dir += (transform.position - buff.source.transform.position).normalized * buff.intensity;
                    else
                        dir += buff.dir.normalized * buff.intensity;
                }
            }
            return dir;
        }
    }

    public float DoT
    {
        get
        {
            float damage = 0;
            foreach (Buff buff in stats.buffs)
            {
                if (buff.type == BuffType.DoT)
                {
                    damage += buff.intensity;
                }
            }
            return damage;
        }
    }

    protected void OnEnable()
    {
        GameManager.Instance.entities.Add(this);
        Initialize();
    }

    protected void OnDisable()
    {
        GameManager.Instance.entities.Remove(this);
    }

    public virtual void Initialize()
    {
        stats = baseStats.FetchStats();
        CalculateStats();
        stats.currentHealth = stats.maxHealth;
        // Initialization
        foreach (Ability ability in abilities)
        {
            ability.Initialize(this);
        }
    }

    public void FixedUpdate()
    {
        currentTick++;
        CalculateStats();
        AbilityTriggers();
        BuffDecrement();
    }

    protected virtual void CalculateStats()
    {
        stats = baseStats.FetchStats(stats);

        if(owner != null && owner != this)
        {
            stats.cooldownReduction = owner.stats.cooldownReduction;
            stats.damageMultiplier = owner.stats.damageMultiplier;
            stats.movementScaling = owner.stats.movementScaling;
            stats.areaScaling = owner.stats.areaScaling;
        }

        if (stats.allegience.HasFlag(EntityType.Spirit) || stats.allegience.HasFlag(EntityType.Zombie) || stats.allegience.HasFlag(EntityType.Demon))
        {
            stats.maxHealth *= GameManager.enemyHealthMulti;
        }

        foreach (Buff buff in stats.buffs)
        {
            if (buff.type == BuffType.Stat)
            {
                switch (buff.combineMethod)
                {
                    case StatCombineMethod.Additive:
                        stats.StatCombineAdditive(buff.statBuff);
                        break;
                    case StatCombineMethod.Multiplicative:
                        stats.StatCombineMultiplicative(buff.statBuff);
                        break;
                }
            }
        }

        foreach (Ability ability in abilities)
        {
            ability.CalculateStats(this, stats);
        }
    }

    protected virtual void AbilityTriggers()
    {
        foreach (Ability ability in abilities)
        {
            ability.CheckCooldownTrigger(currentTick, this);
        }
    }

    protected virtual void BuffDecrement()
    {
        // DoT apply
        stats.currentHealth -= DoT;

        foreach (Buff buff in stats.buffs.ToArray())
        {
            buff.ticksRemaining--;
            if (buff.ticksRemaining <= 0)
            {
                stats.buffs.Remove(buff);
            }
        }
    }

    public void AddAbility(Ability ability)
    {
        if (abilities.Contains(ability.replaceAbility))
        {
            abilities.Remove(ability.replaceAbility);
        }
        abilities.Add(ability);
        ability.Initialize(this);
    }

    public void DamageTaken(Entity source, DamageInstance damage)
    {
        stats.currentHealth -= damage.damageScale;
        Debug.Log($"{name} took {damage.damageScale} damage!");
        if(stats.currentHealth <= 0)
        {
            Die();
        }
        else
        {
            // Slow
            if(damage.slowTickDuration > 0)
            {
                stats.buffs.Add(new Buff(BuffType.Slow, damage.slowTickDuration, damage.slowMulti, source));
            }

            // Stun
            if (damage.stunTickDuration > 0)
            {
                stats.buffs.Add(new Buff(BuffType.Stun, damage.stunTickDuration, 1, source));
            }

            // Knockback
            if (damage.knockbackTickDuration > 0)
            {
                stats.buffs.Add(new Buff(BuffType.Knockback, damage.knockbackTickDuration, damage.knockbackPower, source));
            }
        }
    }

    public void Die()
    {
        if(baseStats.stats.expDropped != 0)
        {
            GameManager.Instance.expOrb.Spawn(baseStats.stats.expDropped, transform.position);
        }
        ReleaseObject();
    }

    public void GainExp(int amount)
    {
        Debug.Log($"Exp gained: {amount}");
        stats.currentExp += amount;
        if(stats.expLevelCurve.Evaluate(stats.currentExp) > stats.level)
        {
            stats.level++;
            // ON LEVEL UP STUFF
            GameManager.Instance.abilityTree.gameObject.SetActive(true);
        }
    }
}