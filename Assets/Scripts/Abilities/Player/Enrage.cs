using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/Player/Enrage")]
public class Enrage : Ability
{
    public float percentIncreasedDamagePerLifeLost;
    public override void CalculateStats(Entity self, Stats stats)
    {
        stats.damageMultiplier += percentIncreasedDamagePerLifeLost * (stats.maxHealth - stats.currentHealth);
    }
}
