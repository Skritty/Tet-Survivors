using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

[CreateAssetMenu(menuName = "Abilities/Enemy/PlayerProximityAttack")]
public class PlayerProximityTriggeredAttack : GenericAoE
{
    public float activationDistance;
    public EntityType targetType;

    public override void Initialize(Entity self)
    {
        self.stats.activationTicks.Add(this, -cooldownTicks);
        base.Initialize(self);
    }

    public override void CheckCooldownTrigger(int tick, Entity self)
    {
        Entity closest = null;
        float closestDist = float.MaxValue;
        foreach (Entity t in GameManager.Instance.entities)
        {
            if (!targetType.HasFlag(t.stats.allegience)) continue;
            if ((t.transform.position - self.transform.position).sqrMagnitude < closestDist)
            {
                closest = t;
                closestDist = (t.transform.position - self.transform.position).sqrMagnitude;
            }
        }
        if (closest == null)
        {
            aoe.StopAllFX();
            return;
        }
        if ((closest.transform.position - self.transform.position).magnitude > activationDistance)
        {
            aoe.StopAllFX();
            return;
        }

        if (continuous)
        {
            if (!self.stats.aoes[this].playing) CooldownActivation(self);
        } 
        else 
        {
            if (tick >= self.stats.activationTicks[this] + cooldownTicks)
            {
                CooldownActivation(self);
                self.stats.activationTicks[this] = self.currentTick;
            }
        }
        
    }
}