using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/Player/FruitOffering")]
public class FruitOffering : Ability
{
    public float damagePerTick;
    public int tickDuration;
    public override void Initialize(Entity self)
    {
        self.stats.activationTicks.Add(this, 0);
        self.WhenHit.AddListener(DestroySpirit);
    }

public override void Cleanup(Entity self)
    {
        self.WhenHit.RemoveListener(DestroySpirit);
    }

    private void DestroySpirit(Entity self, Entity spirit)
    {
        if ((self.currentTick + (useAdvanceCooldown ? self.stats.advanceCooldown : 0)) < self.stats.activationTicks[this] + cooldownTicks) return;
        if (!spirit.stats.allegience.HasFlag(EntityType.Spirit)) return;
        self.stats.activationTicks[this] = self.currentTick;
        spirit.stats.buffs.Add(new Buff(BuffType.DoT, tickDuration, damagePerTick, self));
    }
}
