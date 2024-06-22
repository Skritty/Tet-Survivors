using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/Enemy/MoveToPlayer")]
public class MoveToPlayer : Ability
{
    public float keepDistance;
    public override void CooldownActivation(Entity self)
    {
        Entity target = GameManager.Instance.player;

        if (self.stats.overrideDamageAllegience != EntityType.None)
        {
            Entity closest = null;
            float closestDist = float.MaxValue;
            foreach (Entity t in GameManager.Instance.entities)
            {
                if (!self.stats.overrideDamageAllegience.HasFlag(t.stats.allegience)) continue;
                if((t.transform.position - self.transform.position).sqrMagnitude < closestDist)
                {
                    closest = t;
                    closestDist = (t.transform.position - self.transform.position).sqrMagnitude;
                }
            }
            target = closest;
            if (target == null) target = self;
        }

        Vector3 dir = (target.transform.position - self.transform.position).normalized;
        dir = dir * self.stats.baseMovementSpeed * self.stats.movementScaling * self.SlowMulti * Time.fixedDeltaTime + self.Knockback;
        if (dir.sqrMagnitude != 0)
        {
            self.stats.facing = dir.normalized;
        }
        self.transform.position += dir;
        if ((target.transform.position - self.transform.position).magnitude < keepDistance)
        {
            self.transform.position = target.transform.position + -(target.transform.position - self.transform.position).normalized * keepDistance;
        }
    }
}
