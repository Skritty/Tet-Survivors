using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/Player/FortuneTeller")]
public class FortuneTeller : Ability
{
    public float expAttractMultiplier;
    public override void CalculateStats(Entity self, Stats stats)
    {
        stats.expAttractScaling *= expAttractMultiplier;
    }
}