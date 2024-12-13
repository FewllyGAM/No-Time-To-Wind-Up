using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "Upgrades/Stat Upgrade", fileName = "New Upgrade")]
public class StatUpgrade : Upgrade
{
    public bool doChangeStat;
    public UpgrableStat stat;
    public float valueChange;//#value

    public bool percentageValue;

    public override void UpdateDescription()
    {
        updatedDescription = description;

        float value = valueChange * level;
 
        updatedDescription = updatedDescription.Replace("#value", $"{openStyle}{Mathf.Abs(percentageValue ? value * 100 : value)}{closeStyle}");
    }

    public void ChangeValue(ref Dictionary<UpgrableStat, float> stats)
    {
        stats[stat] += valueChange;
    }
}
