using System.Collections;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/SpawnProxy")]
public class SpawnProxy : Ability
{
    public Entity proxy;
    public float slowOnSpawnMulti;
    public int slowOnSpawnTickDuration;
    public int removeAfterTicks = -1;
    public override void CooldownActivation(Entity self)
    {
        if(slowOnSpawnTickDuration > 0)
        {
            self.stats.buffs.Add(new Buff(BuffType.Slow, slowOnSpawnTickDuration, slowOnSpawnMulti, self));
        }
        Entity entity = proxy.RequestObject().GetComponent<Entity>();
        entity.owner = self;
        entity.transform.position = self.transform.position;
        PlayAnimation(self);

        if (removeAfterTicks > 0)
        {
            self.StartCoroutine(RemoveTimer());
        }
        IEnumerator RemoveTimer()
        {
            for (int i = 0; i < removeAfterTicks; i++)
            {
                yield return new WaitForFixedUpdate();
            }
            entity.ReleaseObject();
        }
    }
}