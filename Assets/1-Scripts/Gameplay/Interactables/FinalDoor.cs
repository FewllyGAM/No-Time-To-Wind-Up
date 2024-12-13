using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinalDoor : InteractableObject
{
    [SerializeField] Collider doorCollider;
    [SerializeField] Transform door;
    Vector3 originPos;

    [SerializeField] Transform openedDoorTarget;

    [SerializeField] AudioSource doorSliding;
    bool sliding;

    protected override void Awake()
    {
        base.Awake();
        originPos = door.position;
    }

    public override void WindUp(float windUpValue)
    {
        base.WindUp(windUpValue);

        door.transform.position = Vector3.Lerp(originPos, openedDoorTarget.position, currentWind / windCost);
    }

    public override void Active()
    {
        currentWind = windCost;
        ObjectAnimation(1);

        activated = true;

        doorCollider.enabled = false;
    }

    public override void Restart()
    {
        base.Restart();

        door.transform.position = originPos;
    }

    protected override void OnTriggerStay(Collider other)
    {
        base.OnTriggerStay(other);

        if (!sliding && gotWind > 0 && pressing)
        {
            PlaySliding();
        }
        else if (sliding && (gotWind == 0 || !pressing))
        {
            StopSliding();
        }
    }

    protected override void OnTriggerExit(Collider other)
    {
        base.OnTriggerExit(other);
        StopSliding();
    }

    void PlaySliding()
    {
        sliding = true;
        doorSliding.enabled = true;
        doorSliding.Play();
    }
    void StopSliding()
    {
        sliding = false;
        doorSliding.enabled = false;
    }
}
