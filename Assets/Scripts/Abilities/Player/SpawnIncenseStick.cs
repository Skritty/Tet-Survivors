using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/Player/SpawnProxyOnKill")]
public class SpawnIncenseStick : Ability
{
    public PooledObject proxy;
    public float procChance;
    public int removeAfterTicks;
    public override void Initialize(Entity self)
    {
        self.OnKill.AddListener(SpawnStick);
    }

    public override void Cleanup(Entity self)
    {
        self.OnKill.RemoveListener(SpawnStick);
    }

    private void SpawnStick(Entity self, Entity other)
    {
        if (Random.Range(0f, 1f) > procChance) return;
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
