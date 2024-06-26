using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/Player/FutureSight")]
public class FutureSight : Ability
{
    public float damageMultiplierPerLevel;
    public float expGainMultiplier;
    public override void CalculateStats(Entity self, Stats stats)
    {
        stats.expGainScaling *= expGainMultiplier;
        stats.damageMultiplier *= 1 + damageMultiplierPerLevel * stats.level;
    }
}
