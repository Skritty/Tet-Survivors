using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/GenericAoE")]
public class GenericAoE : Ability
{
    public AoE aoe;
    public override void CooldownActivation(Entity self)
    {
        aoe.Trigger(self, self.stats.facing);
    }
}