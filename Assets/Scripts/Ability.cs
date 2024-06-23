using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Ability : ScriptableObject
{
    [Header("Tree Info")]
    public bool removeFromTreeOnUnlock = true;
    public List<Ability> unlockedOnObtain = new List<Ability>();

    [Header("Ability Info")]
    public string abilityName;
    public string abilityDescription;
    public bool enabled = true;
    public bool activateOnSpawn;
    public string animationTrigger;
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
    public virtual void CooldownActivation(Entity self) { }
    public virtual void CalculateStats(Entity self, Stats stats) { }
    public virtual void PlayAnimation(Entity self)
    {
        if (self.animator && animationTrigger != "")
        {
            self.StartCoroutine(Play());
        }
        
        IEnumerator Play()
        {
            self.animator.SetBool(animationTrigger, true);
            yield return new WaitUntil(() => self.animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1 && !self.animator.IsInTransition(0));
            self.animator.SetBool(animationTrigger, false);
        }
    }
}