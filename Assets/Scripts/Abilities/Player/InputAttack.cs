using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/Player/InputAttack")]
public class InputAttack : Ability
{
    public KeyCode key;
    public AoE aoe;

    public override void Initialize(Entity self)
    {
        self.stats.activationTicks.Add(this, 0);
    }

    public override void CheckCooldownTrigger(int tick, Entity self)
    {
        if (self.IsStunned) return;
        if (tick >= self.stats.activationTicks[this] + cooldownTicks) CooldownActivation(self);
    }

    public override void CooldownActivation(Entity self)
    {
        if (Input.GetKey(key))
        {
            Debug.Log(abilityName);
            self.stats.activationTicks[this] = self.currentTick;
            aoe.Trigger(self, self.stats.facing);
        }
    }
}