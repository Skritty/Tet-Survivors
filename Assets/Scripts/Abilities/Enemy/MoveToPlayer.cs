using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/Enemy/MoveToPlayer")]
public class MoveToPlayer : Ability
{
    public override void CooldownActivation(Entity self)
    {
        Vector3 dir = (GameManager.Instance.player.transform.position - self.transform.position).normalized;
        if (dir.sqrMagnitude != 0)
        {
            self.stats.facing = dir;
        }
        self.transform.position += dir * self.stats.baseMovementSpeed * self.stats.movementScaling * self.SlowMulti * Time.fixedDeltaTime + self.Knockback;
    }
}
