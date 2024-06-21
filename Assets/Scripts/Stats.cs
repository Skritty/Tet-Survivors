using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Stats")]
public class Stats : ScriptableObject
{
    public EntityType allegience;
    public Vector2 facing;
    public int expDrop;
    public int level;
    public int currentExp;
    public AnimationCurve expLevelCurve;
    public float maxHealth;
    public float currentHealth;
    public float baseDamage;
    public float damageMultiplier;
    public float movementScaling;
    public float areaScaling;
    public float expAttractScaling;

    public Stats CreateCopy()
    {
        return (Stats)this.MemberwiseClone();
    }
}

public enum EntityType { Neutral, Player, Spirit, Zombie, Demon}