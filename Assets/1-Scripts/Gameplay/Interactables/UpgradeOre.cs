using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeOre : InteractableObject
{
    public bool statUpgrade = true;

    [SerializeField] GameObject ore;
   
    public override void Active()
    {
        base.Active();

        if (statUpgrade) GameManager.gm.ChooseStatUpgradeOptions();
        else GameManager.gm.ChooseSkillUpgradeOptions();

        ore.SetActive(false);
    }

    public override void Restart()
    {
        base.Restart();
        ore.SetActive(true);
    }
}
