using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/Player/Frenzy")]
public class Frenzy : Ability
{
    public int advanceCooldownOnKillAmount;
    public override void Initialize(Entity self)
    {
        self.OnKill.AddListener(AdvanceCooldowns);
    }

    public override void Cleanup(Entity self)
    {
        self.OnKill.RemoveListener(AdvanceCooldowns);
    }

    private void AdvanceCooldowns(Entity self, Entity other)
    {
        for(int i = 0; i < advanceCooldownOnKillAmount; i++)
        {
            self.stats.advanceCooldown++;
            foreach (Ability ability in self.abilities)
            {
                if (!ability.useAdvanceCooldown) return;
                ability.CheckCooldownTrigger(self.currentTick, self);
            }
        }
    }
}
