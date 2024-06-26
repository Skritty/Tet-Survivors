using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/Player/FruitOffering")]
public class FruitOffering : Ability
{
    public override void Initialize(Entity self)
    {
        self.WhenHit.AddListener(DestroySpirit);
    }

    public override void Cleanup(Entity self)
    {
        self.WhenHit.RemoveListener(DestroySpirit);
    }

    private void DestroySpirit(Entity self, Entity spirit)
    {
        if (!spirit.stats.allegience.HasFlag(EntityType.Spirit)) return;
        spirit.DamageTaken(self, new DamageInstance(9999));
    }
}
