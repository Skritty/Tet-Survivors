using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/Player/InputTrigger")]
public class InputTrigger : Ability
{
    public KeyCode key;
    public Ability toTrigger;

    public override void Initialize(Entity self)
    {
        self.stats.activationTicks.Add(this, 0);
        base.Initialize(self);
        toTrigger.Initialize(self);
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
            if(abilityName == "Firecracker")
            {
                // This is NOT the correct way to do things
                GameManager.Instance.OnFirecrackerUsed();
            }
            self.stats.activationTicks[this] = self.currentTick;
            toTrigger.CooldownActivation(self);
        }
    }
}