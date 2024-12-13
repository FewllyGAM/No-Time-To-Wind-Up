using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrightLightEffect : MonoBehaviour
{
    SkillUpgrade skill;
    Player player;
    Light helmLight;
    SphereCollider rangeCollider;

    float baseIntensity;
    float baseRadius;

    bool activated;

    public void Initialize(Player player, SkillUpgrade skill, Light helmLight)
    {
        this.skill = skill;
        this.player = player;
        this.helmLight = helmLight;

        rangeCollider = GetComponent<SphereCollider>();

        baseRadius = rangeCollider.radius;
        baseIntensity = player.startLightIntensity;
    }

    public void UpdateLight(float currentWind)
    {
        if (currentWind >= skill.windUpTimes && !activated)
        {
            rangeCollider.radius += baseRadius * skill.range;
            helmLight.intensity += baseIntensity * skill.range;

            activated = true;
        }
        else if (currentWind < skill.windUpTimes && activated)
        {
            rangeCollider.radius -= baseRadius * skill.range;
            helmLight.intensity -= baseIntensity * skill.range;

            activated = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Enemy e = other.GetComponent<Enemy>();

        e.ApplySlow(skill.power);
    }
    private void OnTriggerExit(Collider other)
    {
        Enemy e = other.GetComponent<Enemy>();

        e.ClearSlow();
    }
}
