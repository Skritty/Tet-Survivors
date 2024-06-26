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
    public int ticksBeforeKeyboardSelection;
    private int timer;

    private void OnEnable()
    {
        Time.timeScale = 0;
        GenerateOptions();
        timer = 0;
    }

    private void Update()
    {
        timer++;
        if (timer < ticksBeforeKeyboardSelection) return;
        if (Input.GetKeyDown(KeyCode.Z))
        {
            b1.onClick.Invoke();
        }
        if (Input.GetKeyDown(KeyCode.X))
        {
            b2.onClick.Invoke();
        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            b3.onClick.Invoke();
        }
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
        GameManager.Instance.inputBlockTick = GameManager.Instance.globalTick + 5;
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
