using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

// This is used to make unslowable movement
[CreateAssetMenu(menuName = "Abilities/SelfKnockback")]
public class SelfKnockback : Ability
{
    public enum TargetMode { Constant, Player, NearestEntity, Random }
    public TargetMode mode;
    public Vector3 KBconstant;
    public float KBPower;
    public int KBDuration;
    public override void CooldownActivation(Entity self)
    {
        Vector3 dir = KBconstant.normalized;

        if (mode == TargetMode.NearestEntity)
        {
            if(self.stats.overrideDamageAllegience != EntityType.None)
            {
                Entity closest = null;
                float closestDist = float.MaxValue;
                foreach (Entity t in GameManager.Instance.entities)
                {
                    if (!self.stats.overrideDamageAllegience.HasFlag(t.stats.allegience)) continue;
                    if ((t.transform.position - self.transform.position).sqrMagnitude < closestDist)
                    {
                        closest = t;
                        closestDist = (t.transform.position - self.transform.position).sqrMagnitude;
                    }
                }
                if (closest == null) closest = self;
                dir = (closest.transform.position - self.transform.position).normalized;
            }
            else
            {
                dir = (GameManager.Instance.player.transform.position - self.transform.position).normalized;
            }
        }

        if(mode == TargetMode.Player)
        {
            dir = (GameManager.Instance.player.transform.position - self.transform.position).normalized;
        }
        
        if(mode == TargetMode.Random)
        {
            dir = ((Vector2)(Random.rotationUniform * Vector3.up)).normalized;
        }

        self.stats.buffs.Add(new Buff(BuffType.Knockback, KBDuration, KBPower, self, dir.x, dir.y));

        PlayAnimation(self);
    }
}