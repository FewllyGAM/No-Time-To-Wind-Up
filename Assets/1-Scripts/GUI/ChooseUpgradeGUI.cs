using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChooseUpgradeGUI : BaseWindow
{
    UpgradeGUI[] upgrades;

    private void OnEnable()
    {
        GameEvents.OnChooseStatUpgrade += SetUpgradeOptions;
        GameEvents.OnChooseSkillUpgrade += SetUpgradeOptions;
    }
    private void OnDisable()
    {
        GameEvents.OnChooseStatUpgrade -= SetUpgradeOptions;
        GameEvents.OnChooseSkillUpgrade -= SetUpgradeOptions;
    }

    protected override void Awake()
    {
        base.Awake();

        upgrades = GetComponentsInChildren<UpgradeGUI>();
        foreach (UpgradeGUI upgradeGUI in upgrades)
        {
            upgradeGUI.ChooseUpgradeGUI = this;
        }
    }

    public override void Show(float time = 0.15F)
    {
        base.Show(time);
        GameManager.gm.choosingUpgrade = true;
    }
    public override void Hide(float time = 0.15F)
    {
        base.Hide(time);
        GameManager.gm.choosingUpgrade = false;
    }

    public void SetUpgradeOptions(Upgrade[] options)
    {
        if (!GameManager.gm.gamePaused) GameManager.gm.PauseGame(false);
        Show();

        for (int i = 0; i < upgrades.Length; i++)
        {
            if (i < options.Length)
            {
                upgrades[i].Set(options[i]);
                upgrades[i].Show();
            }
            else upgrades[i].Hide();
        }
    }
}
