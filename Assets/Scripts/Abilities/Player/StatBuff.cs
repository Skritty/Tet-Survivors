using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/Player/StatBuff")]
public class StatBuff : Ability
{
    public Stats statsToBuff;
    public override void CalculateStats(Stats stats)
    {
        stats.maxHealth += statsToBuff.maxHealth;
        stats.damageMultiplier += statsToBuff.damageMultiplier - 1;
        stats.movementScaling += statsToBuff.movementScaling - 1; ;
        stats.areaScaling += statsToBuff.areaScaling - 1; ;
        stats.expAttractScaling += statsToBuff.expAttractScaling - 1; ;
    }
}
