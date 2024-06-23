using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExpOrb : Entity
{
    public static List<ExpOrb> expOrbs = new List<ExpOrb>();
    public void Spawn(int expValue, Vector3 position)
    {
        ExpOrb orb = RequestObject().GetComponent<ExpOrb>();
        orb.transform.position = position;
        orb.stats.expDropped = expValue;
        expOrbs.Add(orb);
    }

    public void Despawn()
    {
        expOrbs.Remove(this);
        ReleaseObject();
    }

    private new void Die()
    {
        expOrbs.Remove(this);
        base.Die();
    }
}