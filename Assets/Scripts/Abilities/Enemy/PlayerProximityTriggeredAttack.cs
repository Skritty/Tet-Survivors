using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

[CreateAssetMenu(menuName = "Abilities/Enemy/PlayerProximityAttack")]
public class PlayerProximityTriggeredAttack : GenericAoE
{
    public float activationDistance;

    public override void Initialize(Entity self)
    {
        self.stats.activationTicks.Add(this, 0);
    }

    public override void CheckCooldownTrigger(int tick, Entity self)
    {
        if (self.IsStunned) return;
        if (!GameManager.Instance.player.isActiveAndEnabled) return;
        if ((GameManager.Instance.player.transform.position - self.transform.position).magnitude > activationDistance) return;
        if (tick >= self.stats.activationTicks[this] + cooldownTicks)
        {
            CooldownActivation(self); 
            self.stats.activationTicks[this] = self.currentTick;
        }
    }
}
