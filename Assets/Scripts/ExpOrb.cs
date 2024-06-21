using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExpOrb : Entity
{
    public static List<ExpOrb> expOrbs = new List<ExpOrb>();
    public int expGranted;
    public void Spawn(int expValue)
    {
        ExpOrb orb = RequestObject().GetComponent<ExpOrb>();
        orb.expGranted = expValue;
        expOrbs.Add(orb);
    }

    public void Despawn()
    {
        expOrbs.Remove(this);
        ReleaseObject();
    }
}