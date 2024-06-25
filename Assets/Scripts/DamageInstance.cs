using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class DamageInstance
{
    public EntityType entitiesDamaged;
    public float damageScale;
    public float knockbackPower;
    public int knockbackTickDuration;
    public int stunTickDuration;
    public float slowMulti = 1;
    public int slowTickDuration;

    public DamageInstance() { }
    public DamageInstance(float damage)
    {
        damageScale = damage;
    }

    public DamageInstance CreateCopy()
    {
        return (DamageInstance) MemberwiseClone();
    }
}
