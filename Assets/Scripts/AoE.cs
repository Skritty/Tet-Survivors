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
        public DamageInstance damage;
        public int tickDuration;
        public int timesRepeated;
        public float radius;
        [Range(0, 360f)]
        public float angleDegrees;
        public float selfSlowMultiDuringStage = 1;
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
            if (!target.isActiveAndEnabled) return;
            if(!damage.entitiesDamaged.HasFlag(target.stats.allegience))
            {
                return;
            }

            Vector2 toTarget = target.transform.position - origin.transform.position;
            if (toTarget.magnitude > radius) return;
            if (toTarget.magnitude == 0 || angleDegrees == 360 || Mathf.Acos(Vector2.Dot(toTarget, forward) / (toTarget.magnitude * forward.magnitude)) * Mathf.Rad2Deg <= angleDegrees / 2f)
            {
                HitFX();
                DamageInstance damageInstance = damage.CreateCopy();
                damageInstance.damageScale *= origin.stats.baseDamage * origin.stats.damageMultiplier;
                target.DamageTaken(origin, damageInstance);
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
                    origin.stats.buffs.Add(new Buff(BuffType.Slow, stage.tickDuration, stage.selfSlowMultiDuringStage, origin));
                    stage.StartFX();
                    if (stage.damage.damageScale != 0 || stage.damage.slowTickDuration > 0 || stage.damage.stunTickDuration > 0 || stage.damage.knockbackTickDuration > 0)
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