using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This is used to make unslowable movement
[CreateAssetMenu(menuName = "Abilities/SelfKnockback")]
public class SelfKnockback : Ability
{
    public enum TargetMode { Constant, Player, NearestEntity, Random }
    public TargetMode mode;
    public Vector3 KBconstant;
    public float KBPower;
    public int KBDuration;
    public EntityType typeFilter;
    public override void CooldownActivation(Entity self)
    {
        Vector3 dir = KBconstant.normalized;

        if (mode == TargetMode.NearestEntity)
        {

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
    }
}