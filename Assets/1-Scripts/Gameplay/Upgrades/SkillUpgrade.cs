using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TriggerSkillOccasion { OnHit, OnWindUp}

[CreateAssetMenu(menuName = "Upgrades/Skill Upgrade", fileName = "New Upgrade")]
public class SkillUpgrade : Upgrade
{
    [Space()]

    public bool active;
    public TriggerSkillOccasion occasion;
    [HideInInspector] public bool inCooldown;

    [Space()]

    public float power;//#power
    public float range;//#range
    public float duration;//#dura
    public float cooldown;//#cd

    [Space()]

    public int windUpTimes = 10;//#winT

    [Space()]

    public bool slowEnemies;
    public bool acceleratePlayer;
    public bool grantInvencibility;

    [Space()]

    [SerializeField] SkillUpgradeLevelUp levelUp;
    public GameObject skillObject;

    [Space()]
    public AudioClip sfx;
    public GameObject vfx;

    public bool setVFXDuration;

    public override void UpdateDescription()
    {
        updatedDescription = description;

        updatedDescription = updatedDescription.Replace("#power", $"{openStyle}{power * 100}%{closeStyle}");
        updatedDescription = updatedDescription.Replace("#range", $"{openStyle}{range * 100}%{closeStyle}");
        updatedDescription = updatedDescription.Replace("#dura", $"{openStyle}{duration}{closeStyle}");
        updatedDescription = updatedDescription.Replace("#cd", $"{openStyle}{cooldown}{closeStyle}");

        updatedDescription = updatedDescription.Replace("#winT", $"{openStyle}{windUpTimes}{closeStyle}");
    }

    public override void LevelUp()
    {
        power += levelUp.lv_power;
        range += levelUp.lv_range;
        duration += levelUp.lv_duration;
        cooldown += levelUp.lv_cooldown;

        windUpTimes += levelUp.lv_windUpTimes;

        base.LevelUp();
    }

    public void SetActive(bool value)
    {
        //Debug.Log("ACTIVATING SKILL");
        switch (occasion)
        {
            case TriggerSkillOccasion.OnHit:
                if (value) GameEvents.OnHitPlayer += Trigger; else GameEvents.OnHitPlayer -= Trigger;
                break;
            case TriggerSkillOccasion.OnWindUp:
                if (value) GameEvents.OnWindUp += Trigger; else GameEvents.OnWindUp -= Trigger;
                break;
            default:
                break;
        }
    }

    public void Trigger(int count)
    {
        if (count % windUpTimes == 0)
            Trigger();
    }

    public void Trigger()
    {
        if (inCooldown) return;

        Debug.Log("TRIGGERING SKILL");

        AudioManager.managaner.Play(sfx, false);

        GameManager.gm.player.TriggerSkill();
        if (slowEnemies) GameEvents.CallOnSlowEnemies(power, duration, range);    
    }
}

[System.Serializable]
public class SkillUpgradeLevelUp
{
    public float lv_power;
    public float lv_range;
    public float lv_duration;
    public float lv_cooldown;

    [Space()]

    public int lv_windUpTimes;
}
