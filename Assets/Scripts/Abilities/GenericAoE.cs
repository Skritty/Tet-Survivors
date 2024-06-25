using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/GenericAoE")]
public class GenericAoE : Ability
{
    public bool continuous;
    public AoE aoe;
    
    public override void Initialize(Entity self)
    {
        self.stats.aoes.Add(this, aoe.CreateClone());
        base.Initialize(self);
    }
    public override void CheckCooldownTrigger(int tick, Entity self)
    {
        if (self.IsStunned) return;
        if (continuous)
        {
            if (!self.stats.aoes[this].playing) CooldownActivation(self);
        }
        else
        {
            if (cooldownTicks == 0 || tick % (cooldownTicks / self.stats.cooldownReduction) < 1) CooldownActivation(self);
        }
    }
    public override void CooldownActivation(Entity self)
    {
        self.stats.aoes[this].Trigger(self, cooldownTicks == 0);
        PlayAnimation(self);
    }
}