using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/Player/UtilizeEssence")]
public class UtilizeEssence : StatBuff
{
    public Stats statBuff;
    public override void CooldownActivation(Entity self)
    {
        base.CooldownActivation(self);
        self.owner.stats.buffs.Add(new Buff(BuffType.Stat, cooldownTicks, 1, self, statBuff: statBuff));
    }
}