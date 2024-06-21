using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

[Serializable]
public class AoE
{
    [Serializable]
    public class AoEStage
    {
        public float damageScale;
        public int tickDuration;
        public int timesRepeated;
        public float radius;
        [Range(0, 360f)]
        public float angleDegrees;
        // Particle effect

        public void CollisionCheckAll(Entity origin, Vector2 forward)
        {
            foreach (Entity target in GameManager.Instance.entities)
            {
                CollisionCheckSingle(origin, forward, target);
            }
        }

        public void CollisionCheckSingle(Entity origin, Vector2 forward, Entity target)
        {
            if(origin.stats.allegience == EntityType.Neutral 
                || (origin.stats.allegience == EntityType.Player && target.stats.allegience == EntityType.Player)
                || (origin.stats.allegience != EntityType.Player && target.stats.allegience != EntityType.Player))
            {
                return;
            }

            Vector2 toTarget = target.transform.position - origin.transform.position;
            if (toTarget.magnitude > radius) return;
            Debug.Log(Mathf.Acos(Vector2.Dot(toTarget, forward) / (toTarget.magnitude * forward.magnitude)) * Mathf.Rad2Deg);
            if (toTarget.magnitude == 0 || angleDegrees == 360 || Mathf.Acos(Vector2.Dot(toTarget, forward) / (toTarget.magnitude * forward.magnitude)) * Mathf.Rad2Deg <= angleDegrees / 2f)
            {
                HitFX();
                target.DamageTaken(damageScale * origin.stats.baseDamage * origin.stats.damageMultiplier);
            }
        }

        public void StartFX()
        {

        }

        public void EndFX()
        {

        }

        public void HitFX()
        {

        }
    }

    public List<AoEStage> stages = new List<AoEStage>();
    [HideInInspector]
    public bool playing;

    public void Trigger(Entity origin, Vector2 forward, Entity target = null)
    {
        origin.StartCoroutine(PlayStages());
        IEnumerator PlayStages()
        {
            playing = true;
            foreach (AoEStage stage in stages)
            {
                for (int repeat = 0; repeat < stage.timesRepeated + 1; repeat++)
                {
                    stage.StartFX();
                    if (stage.damageScale != 0)
                    {
                        if (target == null)
                        {
                            stage.CollisionCheckAll(origin, forward);
                        }
                        else
                        {
                            stage.CollisionCheckSingle(origin, forward, target);
                        }
                    }
                    for (int i = 0; i < stage.tickDuration; i++)
                    {
                        yield return new WaitForFixedUpdate();
                    }
                    stage.EndFX();
                }
            }
            yield return null;
            playing = false;
        }
    }
}