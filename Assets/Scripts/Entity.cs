using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.Events;
using System.Linq;
using UnityEditor.Experimental.GraphView;

public class Entity : PooledObject
{
    public Animator animator;
    public Entity owner; // For if this is a proxy
    public StatsSO baseStats;
    //[HideInInspector]
    public Stats stats;
    //[HideInInspector]
    public int currentTick;
    public List<Ability> abilities;

    public UnityEvent<Entity> OnDeath;
    public UnityEvent<Entity, Entity> OnHit;
    public UnityEvent<Entity, Entity> OnKill;
    public UnityEvent<Entity, Entity> WhenHit;
    private bool hitTriggeredThisTick;
    public UnityEvent<Entity> OnLevel;

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

    public bool HasIFrames
    {
        get
        {
            foreach (Buff buff in stats.buffs)
            {
                if (buff.type == BuffType.iFrames)
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
            float damage = -stats.regeneration;
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
        Initialize();
        GameManager.Instance.entities.Add(this);
    }

    protected void OnDisable()
    {
        GameManager.Instance.entities.Remove(this);
        foreach (Ability a in abilities.ToArray())
        {
            a.Cleanup(this);
        }
        owner = this;
    }

    public virtual void Initialize()
    {
        foreach(Ability a in abilities.ToArray())
        {
            if (a.temporary)
            {
                abilities.Remove(a);
            }
            
        }
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
        hitTriggeredThisTick = false;
        FixResolveOrder();
        CalculateStats();
        AbilityTriggers();
        BuffDecrement();
    }

    private void FixResolveOrder()
    {
        abilities.Sort(delegate (Ability x, Ability y)
        {
            if (x.resolveOrderModifier == y.resolveOrderModifier) return 0;
            else if (x.resolveOrderModifier > y.resolveOrderModifier) return -1;
            else return 1;
        });

        stats.buffs.Sort(delegate (Buff x, Buff y)
        {
            if (x.combineMethod == y.combineMethod) return 0;
            else if (x.combineMethod == StatCombineMethod.Multiplicative) return 1;
            else return -1;
        });
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

    public virtual void AbilityTriggers()
    {
        foreach (Ability ability in abilities)
        {
            ability.CheckCooldownTrigger(currentTick, this);
        }
    }

    protected virtual void BuffDecrement()
    {
        // DoT apply
        if(DoT != 0)
        {
            DamageTaken(this, new DamageInstance(DoT));
        }

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
        if (source == null) source = this;
        if (owner == null) owner = this;
        if (source.owner == null) source.owner = source;
        if (HasIFrames)
        {
            damage.damageScale = 0;
        }
        stats.currentHealth = Mathf.Clamp(stats.currentHealth - damage.damageScale, 0, stats.maxHealth);
        if(damage.damageScale > 0 && !hitTriggeredThisTick)
        {
            source.OnHit.Invoke(source, this); // owner does not inherit hits
            WhenHit.Invoke(this, source.owner);
            hitTriggeredThisTick = true;
        }
        //Debug.Log($"{name} took {damage.damageScale} damage!");
        if(stats.currentHealth <= 0)
        {
            source.owner.OnKill.Invoke(source.owner, this); // owner inherits kills
            OnDeath.Invoke(this);
            if (stats.currentHealth <= 0) // Check if any abilities save you
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
        if(this == GameManager.Instance.player)
        {
            GameManager.Instance.GameOver();
        }
        owner = this;
        ReleaseObject();
    }

    public void GainExp(int amount)
    {
        stats.currentExp += amount;
        Debug.Log($"Exp gained: {amount} | level = {stats.expLevelCurve.Evaluate(stats.currentExp)}");
        if ((int)stats.expLevelCurve.Evaluate(stats.currentExp) > stats.level)
        {
            stats.level++;
            OnLevel.Invoke(this);
            // ON LEVEL UP STUFF
            GameManager.Instance.CalculateNextExpRequirement();
            GameManager.Instance.abilityTree.gameObject.SetActive(true);
        }
    }
}