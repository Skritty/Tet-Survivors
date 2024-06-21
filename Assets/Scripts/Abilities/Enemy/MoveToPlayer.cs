using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/Enemy/MoveToPlayer")]
public class MoveToPlayer : Ability
{
    public override void CooldownActivation(Entity self)
    {
        self.transform.position += (GameManager.Instance.player.transform.position - self.transform.position).normalized * self.stats.movementScaling * Time.fixedDeltaTime;
    }
}
