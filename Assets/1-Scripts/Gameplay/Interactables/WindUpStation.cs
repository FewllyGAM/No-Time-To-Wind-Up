using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindUpStation : InteractableObject
{
    [SerializeField] float windUpPower = 50f;
    [SerializeField] float windUptime = 3;

    public override void Active()
    {
        base.Active();

        Player p = GameManager.gm.player;

        p.StartCoroutine(p.WindUp(windUpPower, windUptime));

        StartCoroutine(PlayAudioSeveral());
    }

    IEnumerator PlayAudioSeveral()
    {
        for (int i = 0; i < 4; i++)
        {
            yield return new WaitForSeconds(source.clip.length);
            source.Play();
        }
    }
}
