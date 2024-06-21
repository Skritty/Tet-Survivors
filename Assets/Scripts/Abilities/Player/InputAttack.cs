using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/Player/InputAttack")]
public class InputAttack : Ability
{
    public KeyCode key;
    private int activationTick;
    public AoE aoe;

    public override void Initialize(Entity self)
    {
        activationTick = 0;
    }

    public override void CheckCooldownTrigger(int tick, Entity self)
    {
        if (tick >= activationTick + cooldownTicks) CooldownActivation(self);
    }

    public override void CooldownActivation(Entity self)
    {
        if (Input.GetKey(key))
        {
            Debug.Log(abilityName);
            activationTick = self.currentTick;
            aoe.Trigger(self, self.stats.facing);
        }
    }
}