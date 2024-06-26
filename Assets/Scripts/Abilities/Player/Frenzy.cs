using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/Player/Frenzy")]
public class Frenzy : Ability
{
    public int advanceCooldownOnKillAmount;
    public override void Initialize(Entity self)
    {
        self.owner.OnKill.AddListener(AdvanceCooldowns);
    }

    public override void Cleanup(Entity self)
    {
        self.owner.OnKill.RemoveListener(AdvanceCooldowns);
    }

    private void AdvanceCooldowns(Entity self, Entity other)
    {
        for(int i = 0; i < advanceCooldownOnKillAmount; i++)
        {
            self.currentTick++;
            self.AbilityTriggers();
        }
    }
}
