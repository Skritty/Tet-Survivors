using Trevor.Tools.Audio;
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
        // Particle effects
        public PooledObject VFX;
        public AudioDefinitionSO SFX;
        public PooledObject playingVFX;

        public void CollisionCheckAll(Entity origin, Vector2 forward)
        {
            foreach (Entity target in GameManager.Instance.entities.ToArray())
            {
                CollisionCheckSingle(origin, forward, target);
            }
        }

        public void CollisionCheckSingle(Entity origin, Vector2 forward, Entity target)
        {
            if (!target.isActiveAndEnabled) return;
            if(!(origin.stats.overrideDamageAllegience == EntityType.None))
            {
                if (!origin.stats.overrideDamageAllegience.HasFlag(target.stats.allegience))
                {
                    return;
                }
            }
            else if (!damage.entitiesDamaged.HasFlag(target.stats.allegience))
            {
                return;
            }


            Vector2 toTarget = target.transform.position - origin.transform.position;
            if (toTarget.magnitude > radius * origin.stats.areaScaling) return;
            if (toTarget.magnitude == 0 || angleDegrees == 360 || Mathf.Acos(Vector2.Dot(toTarget, forward) / (toTarget.magnitude * forward.magnitude)) * Mathf.Rad2Deg <= angleDegrees / 2f)
            {
                DamageInstance damageInstance = damage.CreateCopy();
                damageInstance.damageScale *= origin.stats.baseDamage * origin.stats.damageMultiplier;
                target.DamageTaken(origin, damageInstance);
            }
        }

        public void StartFX(Entity origin)
        {
            if (!playingVFX)
            {
                //SFX.PlayFollowing(origin.transform);
                playingVFX = VFX?.RequestObject().GetComponent<PooledObject>();
            }
            
            if (playingVFX)
            {
                origin.StartCoroutine(Follow());
                // Set AoE stuff here
                //playingVFX?.GetComponent<ParticleSystem>().Play();
            }
            IEnumerator Follow()
            {
                while (playingVFX)
                {
                    playingVFX.transform.rotation = Quaternion.FromToRotation(Vector3.up, origin.stats.facing);
                    playingVFX.transform.localScale = Vector3.one * radius * origin.stats.areaScaling;
                    playingVFX.transform.position = origin.transform.position;
                    yield return null;
                }
            }
        }

        public void EndFX()
        {
            if (playingVFX)
            {
                playingVFX.ReleaseObject();
                playingVFX = null;
            }
        }

        public AoEStage CreateClone()
        {
            return (AoEStage)MemberwiseClone();
        }
    }

    public List<AoEStage> stages = new List<AoEStage>();
    [HideInInspector]
    public bool playing;

    public void Trigger(Entity origin, bool keepFXAlive = false, Entity target = null)
    {
        origin.StartCoroutine(PlayStages());
        IEnumerator PlayStages()
        {
            playing = true;
            foreach (AoEStage stage in stages)
            {
                stage.StartFX(origin);
                for (int repeat = 0; repeat < stage.timesRepeated + 1; repeat++)
                {
                    origin.stats.buffs.Add(new Buff(BuffType.Slow, stage.tickDuration, stage.selfSlowMultiDuringStage, origin));
                    
                    if (stage.damage.damageScale != 0 || stage.damage.slowTickDuration > 0 || stage.damage.stunTickDuration > 0 || stage.damage.knockbackTickDuration > 0)
                    {
                        if (target == null)
                        {
                            stage.CollisionCheckAll(origin, origin.stats.facing);
                        }
                        else
                        {
                            stage.CollisionCheckSingle(origin, origin.stats.facing, target);
                        }
                    }
                    for (int i = 0; i < stage.tickDuration; i++)
                    {
                        yield return new WaitForFixedUpdate();
                    }
                }
                if(!keepFXAlive)
                    stage.EndFX();
            }
            playing = false;
        }
    }

    public void StopAllFX()
    {
        foreach (AoEStage stage in stages)
        {
            stage.playingVFX?.ReleaseObject();
            stage.playingVFX = null;
        }
    }

    public AoE CreateClone()
    {
        AoE aoe = (AoE)MemberwiseClone();
        aoe.stages = new List<AoEStage>();
        foreach(AoEStage stage in stages)
        {
            aoe.stages.Add(stage.CreateClone());
        }
        return aoe;
    }
}