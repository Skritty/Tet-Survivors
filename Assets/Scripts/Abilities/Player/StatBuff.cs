using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static StatBuff;

[CreateAssetMenu(menuName = "Abilities/Player/StatBuff")]
public class StatBuff : Ability
{
    public Stats statsToBuff;
    public StatCombineMethod combineMethod;
    public int tickDuration = -1; // -1 is infinite

    public override void CooldownActivation(Entity self)
    {
        if (tickDuration > 0)
        {
            self.stats.buffs.Add(new Buff(BuffType.Stat, tickDuration, 0, self, statBuff: statsToBuff, combineMethod: combineMethod));
        }
    }

    public override void CalculateStats(Entity self, Stats stats)
    {
        if(tickDuration < 0)
        {
            switch (combineMethod)
            {
                case StatCombineMethod.Additive:
                    stats.StatCombineAdditive(statsToBuff);
                    break;
                case StatCombineMethod.Multiplicative:
                    stats.StatCombineMultiplicative(statsToBuff);
                    break;
            }
        }
    }
}
