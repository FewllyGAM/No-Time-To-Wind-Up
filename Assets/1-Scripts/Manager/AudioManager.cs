using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager managaner;

    public Vector2 pitchVariation = new Vector2(.9f, 1.1f);

    AudioSource[] sources;
    int current;

    public AudioClip sfx_footstep;

    private void Awake()
    {
        if (managaner == null) managaner = this;

        sources = GetComponentsInChildren<AudioSource>();
    }

    public void Play(AudioClip _clip, bool varyPitch)
    {
        if (!_clip) return;

        sources[current].pitch = varyPitch ? Pitch() : 1;
        sources[current].clip = _clip;
        sources[current].Play();

        current++;
        current = current % sources.Length - 1;
    }

    public float Pitch()
    {
        return Random.Range(pitchVariation.x, pitchVariation.y);
    }
}
