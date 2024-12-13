using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFreezer : InteractableObject
{
    [SerializeField] float freezeTime = 10f;

    public override void Active()
    {
        base.Active();

        GameEvents.CallOnFreezeEnemies(freezeTime);     
    }
}
