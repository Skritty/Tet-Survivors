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
    public bool enabled = true;
    public int cooldownTicks;

    public virtual void Initialize(Entity self) { }
    public virtual void CheckCooldownTrigger(int tick, Entity self)
    {
        if (cooldownTicks == 0 || tick % cooldownTicks == 0) CooldownActivation(self);
    }
    public virtual void CooldownActivation(Entity self) { }
    public virtual void CalculateStats(Stats stats) { }
}
