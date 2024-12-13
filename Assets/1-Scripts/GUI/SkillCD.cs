using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillCD : BaseWindow, IRestartable
{
    [SerializeField] Image skill_cd;
    [SerializeField] Image skill_icon;

    public void Restart()
    {
        StopAllCoroutines();
        skill_cd.fillAmount = 0;
        Hide();
    }

    public void SetSkill(Sprite icon)
    {
        skill_icon.sprite = icon;
        skill_cd.fillAmount = 0;
    }

    public void SkillCooldown(float cooldown)
    {
        StartCoroutine(SkillCooldownRoutine(cooldown));
    }

    IEnumerator SkillCooldownRoutine(float cooldown)
    {
        float time = 1;

        while (time > 0)
        {
            skill_cd.fillAmount = time;
            time -= Time.deltaTime / cooldown;
            yield return new WaitForEndOfFrame();
        }
        skill_cd.fillAmount = 0;
    }
}
