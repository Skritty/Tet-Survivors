using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/Player/Subjugation")]
public class Subjugation : Ability
{
    public int killsNeededToActivate;
    public Ability toActivate;
    public override void Initialize(Entity self)
    {
        self.stats.activationTicks.Add(this, 0);
        self.OnKill.AddListener(OnKill);
    }

    public override void Cleanup(Entity self)
    {
        self.OnKill.RemoveListener(OnKill);
    }

    private void OnKill(Entity self, Entity other)
    {
        self.stats.activationTicks[this]++;
        if (killsNeededToActivate >= self.stats.activationTicks[this])
        {
            toActivate.CooldownActivation(self);
        }
    }
}
