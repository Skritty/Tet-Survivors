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
                    dir += (transform.position - buff.source.transform.position).normalized * buff.intensity;
                }
            }
            return dir;
        }
    } 

    private void Awake()
    {
        Initialize();
    }

    public virtual void Initialize()
    {
        stats = baseStats.FetchStats();
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

        foreach (Ability ability in abilities)
        {
            ability.CalculateStats(stats);
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
        foreach (Buff buff in stats.buffs.ToArray())
        {
            buff.ticksRemaining--;
            if (buff.ticksRemaining <= 0)
            {
                stats.buffs.Remove(buff);
            }
        }
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
        if(stats.expDropped != 0)
        {
            GameManager.Instance.expOrb.Spawn(stats.expDropped, transform.position);
        }
        ReleaseObject();
    }

    public void GainExp(int amount)
    {
        stats.currentExp += amount;
        if(stats.expLevelCurve.Evaluate(stats.currentExp) > stats.level)
        {
            stats.level++;
            // ON LEVEL UP STUFF
        }
    }
}