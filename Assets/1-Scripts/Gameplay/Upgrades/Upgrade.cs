using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum UpgrableStat { Speed, WindUpPower, WindUpCooldown, WindUpSpeed, TimeToStartWindUp, InteractionSpeed, WalkEfficiency, InteractionEfficiency }

public class Upgrade : ScriptableObject
{
    [SerializeField] string upgradeName;
    public string Name { get { return upgradeName; } }

    [TextArea] public string description;

    protected string updatedDescription;
    public string UpdatedDescription { get { return updatedDescription; } }

    public Sprite sprite;

    public bool doLevelUp;
    public int maxLevel = 10;

    [SerializeField] protected int level = 1;
    public int Level { get { return level; } }

    public string openStyle = "<style=\"SpecialValue\">";
    public string closeStyle = "</style>";

    public virtual void LevelUp()
    {
        level++;

        UpdateDescription();
    }

    public virtual void UpdateDescription()
    {
        
    }
}
