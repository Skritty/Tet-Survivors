using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/Enemy/MoveToPlayer")]
public class MoveToPlayer : Ability
{
    public float keepDistance;
    public bool alwaysFacePlayer;
    public EntityType targetTypeOverride;
    [Range(-1, 360)]
    public int followAngle;
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
        if(targetTypeOverride != EntityType.None)
        {
            Entity closest = null;
            float closestDist = float.MaxValue;
            foreach (Entity t in GameManager.Instance.entities)
            {
                if (!targetTypeOverride.HasFlag(t.stats.allegience)) continue;
                if ((t.transform.position - self.transform.position).sqrMagnitude < closestDist)
                {
                    closest = t;
                    closestDist = (t.transform.position - self.transform.position).sqrMagnitude;
                }
            }
            target = closest;
            if (target == null) target = self;
        }

        Vector3 goal = target.transform.position + (self.transform.position - target.transform.position).normalized * keepDistance;
        Vector3 dir = (goal - self.transform.position).normalized;
        if (alwaysFacePlayer && dir.sqrMagnitude != 0)
        {
            self.stats.facing = dir.normalized;
        }
        dir = dir * self.stats.baseMovementSpeed * self.stats.movementScaling * self.SlowMulti * Time.fixedDeltaTime;
        if (!alwaysFacePlayer && dir.sqrMagnitude != 0)
        {
            self.stats.facing = dir.normalized;
        }
        if ((target.transform.position - self.transform.position).magnitude <= keepDistance)
        {
            dir = Vector3.zero;
        }
        self.transform.position += dir + self.Knockback;
        /*if ((target.transform.position - self.transform.position).magnitude < keepDistance)
        {
            self.transform.position = target.transform.position + -(target.transform.position - self.transform.position).normalized * keepDistance;
        }*/
    }
}
