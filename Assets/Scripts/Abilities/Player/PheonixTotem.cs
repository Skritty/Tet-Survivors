using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/Player/RespawnTotem")]
public class PheonixTotem : Ability
{
    [Range(0, 1)]
    public float percentMaxHpRestored; 
    public override void Initialize(Entity self)
    {
        self.owner.OnDeath.AddListener(player => Respawn(self));
    }

    public override void Cleanup(Entity self)
    {
        self.owner.OnDeath.RemoveListener(player => Respawn(self));
    }

    private void Respawn(Entity self)
    {
        self.owner.transform.position = self.transform.position;
        self.owner.stats.currentHealth = self.owner.stats.maxHealth * percentMaxHpRestored;
        Cleanup(self);
        self.ReleaseObject();
    }
}