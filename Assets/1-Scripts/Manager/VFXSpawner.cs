using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VFXSpawner : MonoBehaviour
{
    [SerializeField] float timeToDestroy;

    public void SpawnVFX(GameObject vfx, Transform parent, float duration = 0)
    {
        if (!vfx) return;

        ParticleSystem effect = Instantiate(vfx, parent).GetComponent<ParticleSystem>();
        var main = effect.main;
        if (duration > 0)
        {
            main.duration = duration;
            effect.Play();
        }

        Destroy(effect.gameObject, main.duration + .5f);
    }
}
