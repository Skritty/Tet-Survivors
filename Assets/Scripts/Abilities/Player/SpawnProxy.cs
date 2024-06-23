using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/SpawnProxy")]
public class SpawnProxy : Ability
{
    public Entity proxy;
    public float slowOnSpawnMulti;
    public int slowOnSpawnTickDuration;
    public override void CooldownActivation(Entity self)
    {
        if(slowOnSpawnTickDuration > 0)
        {
            self.stats.buffs.Add(new Buff(BuffType.Slow, slowOnSpawnTickDuration, slowOnSpawnMulti, self));
        }
        Entity entity = proxy.RequestObject().GetComponent<Entity>();
        entity.owner = self;
        PlayAnimation(self);
    }
}