using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/Player/Prayer")]
public class Prayer : Ability
{
    public float regenerationPerTick;
    public override void Initialize(Entity self)
    {
        self.stats.activationTicks.Add(this, 0);
        base.Initialize(self);
    }

    public override void CheckCooldownTrigger(int tick, Entity self)
    {
        if (self.stats.distanceMovedWithoutStopping > 0)
        {
            self.stats.activationTicks[this] = tick;
        }
        else if (tick >= self.stats.activationTicks[this] + cooldownTicks) CooldownActivation(self);
    }

    public override void CooldownActivation(Entity self)
    {
        self.stats.buffs.Add(new Buff(BuffType.DoT, 1, -regenerationPerTick, self));
    }
}