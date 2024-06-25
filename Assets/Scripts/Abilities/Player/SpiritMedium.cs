using System.Collections;
using System.Collections.Generic;
using UnityEditor.Playables;
using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/Player/SpiritMedium")]
public class SpiritMedium : Ability
{
    public float radius;
    public int amountToCharm;
    public Ability allyStatBuff;
    public EntityType targetType;
    public EntityType newAllegience;
    public EntityType damages;
    public override void CooldownActivation(Entity self)
    {
        List<Entity> picks = new List<Entity>();
        foreach (Entity target in GameManager.Instance.entities)
        {
            if (!target.stats.allegience.HasFlag(targetType)) continue;
            if ((target.transform.position - self.transform.position).magnitude > radius) continue;
            picks.Add(target);
        }
        if (picks.Count == 0) return;
        for(int i = 0; i < amountToCharm; i++)
        {
            Entity charm = picks[Random.Range(0, picks.Count)];
            picks.Remove(charm);
            charm.owner = self;
            charm.stats.allegience = newAllegience;
            charm.stats.overrideDamageAllegience = damages;
            charm.abilities.Add(allyStatBuff);
            allyStatBuff.Initialize(charm);
        }
        PlayAnimation(self);
    }
}