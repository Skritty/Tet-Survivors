using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/Player/CollectExpOrbs")]
public class CollectExpOrbs : Ability
{
    public AnimationCurve attractionCurve = new AnimationCurve(); // 0 to 1
    public float attractionRadius;
    public float collectionRadius;
    public override void CooldownActivation(Entity self)
    {
        foreach(ExpOrb orb in ExpOrb.expOrbs.ToArray())
        {
            Vector3 dir = orb.transform.position - self.transform.position;
            if (dir.magnitude > attractionRadius * self.stats.expAttractScaling) continue;
            orb.transform.position -= dir.normalized * attractionCurve.Evaluate(dir.magnitude / (attractionRadius * self.stats.expAttractScaling)) * Time.fixedDeltaTime;
            if((orb.transform.position - self.transform.position).magnitude <= collectionRadius)
            {
                self.GainExp(orb.stats.expDropped);
                orb.Despawn();
            }
        }
    }
}