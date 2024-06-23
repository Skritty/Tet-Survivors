using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelUpScreen : MonoBehaviour
{
    public AbilityTree abilityTree;
    public TextMeshProUGUI name1, name2, name3;
    public TextMeshProUGUI desc1, desc2, desc3;
    public Button b1, b2, b3;

    private void OnEnable()
    {
        Time.timeScale = 0;
        GenerateOptions();
    }

    private void GenerateOptions()
    {
        List<Ability> abilities = abilityTree.RollForAbilities(3);

        b1.onClick.RemoveAllListeners();
        b1.onClick.AddListener(CloseUI);
        b1.onClick.AddListener(() => AddAbility(abilities[0]));
        name1.text = abilities[0].abilityName;
        desc1.text = abilities[0].abilityDescription;

        b2.onClick.RemoveAllListeners();
        b2.onClick.AddListener(CloseUI);
        b2.onClick.AddListener(() => AddAbility(abilities[1]));
        name2.text = abilities[1].abilityName;
        desc2.text = abilities[1].abilityDescription;

        b3.onClick.RemoveAllListeners();
        b3.onClick.AddListener(CloseUI);
        b3.onClick.AddListener(() => AddAbility(abilities[2]));
        name3.text = abilities[2].abilityName;
        desc3.text = abilities[2].abilityDescription;
    }

    private void CloseUI()
    {
        Time.timeScale = 1;
        gameObject.SetActive(false);
    }

    private void AddAbility(Ability ability)
    {
        GameManager.Instance.player.AddAbility(ability);
        if (ability.removeFromTreeOnUnlock)
        {
            abilityTree.upgradePool.Remove(ability);
        }
        foreach(Ability a in ability.unlockedOnObtain)
        {
            abilityTree.upgradePool.Add(a);
        }
    }
}
