using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityTree : MonoBehaviour
{
    public List<Ability> upgradePool = new List<Ability>();

    public List<Ability> RollForAbilities(int count)
    {
        List<Ability> upgrades = new List<Ability>();
        if(count > upgradePool.Count)
        {
            Debug.LogError("Not Enough Upgrades in Pool!");
            count = upgradePool.Count;
        }
        for(int i = 0; i < count; i++)
        {
            Ability selection = upgradePool[Random.Range(0, upgradePool.Count-1)];
            if (upgrades.Contains(selection))
            {
                i--;
                continue;
            }
            upgrades.Add(selection);
        }
        return upgrades;
    }

    public void UnlockAbility(Ability ability)
    {
        // TODO: Add to player abilities
        if (ability.removeFromTreeOnUnlock)
        {
            upgradePool.Remove(ability);
        }
        foreach(Ability a in ability.unlockedOnObtain)
        {
            upgradePool.Add(a);
        }
    }
}
