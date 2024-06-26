using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/Player/CrabShell")]
public class CrabShell : Ability
{
    public int iframesGiven;
    public override void Initialize(Entity self)
    {
        self.stats.activationTicks.Add(this, 0);
        self.WhenHit.AddListener(GrantIFrames);
    }

    public override void Cleanup(Entity self)
    {
        self.WhenHit.RemoveListener(GrantIFrames);
    }

    private void GrantIFrames(Entity self, Entity other)
    {
        if (self.currentTick >= self.stats.activationTicks[this] + cooldownTicks)
        {
            self.stats.activationTicks[this] = self.currentTick;
            self.stats.buffs.Add(new Buff(BuffType.iFrames, iframesGiven, 1, self));
        }
    }
}
