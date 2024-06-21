using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

public class Entity : PooledObject
{
    public Stats baseStats;
    [HideInInspector]
    public Stats stats;
    [HideInInspector]
    public int currentTick;
    public List<Ability> abilities;
    private void Start()
    {
        stats = baseStats.CreateCopy();
        // Initialization
        foreach (Ability ability in abilities)
        {
            ability.Initialize(this);
        }
    }

    public void FixedUpdate()
    {
        currentTick++;
        foreach (Ability ability in abilities) 
        {
            ability.CheckCooldownTrigger(currentTick, this);
        }
    }

    public void DamageTaken(float amount)
    {
        stats.currentHealth -= amount;
        Debug.Log($"{name} took {amount} damage!");
        if(stats.currentHealth <= 0)
        {
            Die();
        }
    }
    public void Die()
    {
        if(stats.expDrop == 0)
        {

        }
    }

    public void GainExp(int amount)
    {
        baseStats.currentExp += amount;
        if(baseStats.expLevelCurve.Evaluate(baseStats.currentExp))
    }
}