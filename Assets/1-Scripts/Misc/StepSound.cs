using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StepSound : MonoBehaviour
{
    public void Step(float v)
    {
        AudioManager.managaner.Play(AudioManager.managaner.sfx_footstep, true);
    }
}
