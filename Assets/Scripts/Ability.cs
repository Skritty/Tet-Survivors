using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Ability : ScriptableObject
{
    [Header("Tree Info")]
    public bool removeFromTreeOnUnlock = true;
    public List<Ability> unlockedOnObtain = new List<Ability>();

    [Header("Effects")]
    public string animationTrigger;
    public PooledObject VFX;

    [Header("Ability Info")]
    public string abilityName;
    public string abilityDescription;
    public bool enabled = true;
    public bool activateOnSpawn;
    public int cooldownTicks;
    public Ability replaceAbility;

    public virtual void Initialize(Entity self) 
    { 
        if(activateOnSpawn) CooldownActivation(self);
    }
    public virtual void CheckCooldownTrigger(int tick, Entity self)
    {
        if (self.IsStunned) return;
        if (cooldownTicks == 0 || tick % cooldownTicks == 0) CooldownActivation(self);
    }
    public virtual void CooldownActivation(Entity self) 
    { 
        PlayEffects(self); 
    }
    public virtual void CalculateStats(Entity self, Stats stats) { }

    public void PlayEffects(Entity self)
    {
        self.GetComponent<Animator>().SetTrigger(animationTrigger);
        ParticleSystem particles = VFX.RequestObject().GetComponent<ParticleSystem>();
        self.StartCoroutine(Play());
        IEnumerator Play()
        {
            particles.Play();
            yield return null;
            /*for (int i = 0; i < )
                particles.Stop();*/
        }
    }
}