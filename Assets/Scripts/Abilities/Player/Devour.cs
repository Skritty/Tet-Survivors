using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/Player/Devour")]
public class Devour : Ability
{
    public float lifeGainOnKill;
    public override void Initialize(Entity self)
    {
        self.OnKill.AddListener(GainHealth);
    }

    public override void Cleanup(Entity self)
    {
        self.OnKill.RemoveListener(GainHealth);
    }

    private void GainHealth(Entity self, Entity other)
    {
        self.DamageTaken(self, new DamageInstance(-lifeGainOnKill));
    }
}
