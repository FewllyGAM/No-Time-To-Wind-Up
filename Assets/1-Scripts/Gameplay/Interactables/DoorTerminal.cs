using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorTerminal : InteractableObject
{
    [SerializeField] Door connectedDoor;
    public Door ConnectedDoor { set { connectedDoor = value; } }
    
    DoorTerminal otherTerminal;
    public DoorTerminal ConnectedTerminal { get { return otherTerminal; } set { otherTerminal = value; } }

    [SerializeField] bool main;

    public override void WindUp(float windUpValue)
    {
        base.WindUp(windUpValue);

        if (main) ConnectedTerminal.WindUp(windUpValue);
    }

    protected override void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);
        main = true;
    }
    protected override void OnTriggerExit(Collider other)
    {
        base.OnTriggerExit(other);
        main = false;
    }

    public override void Active()
    {
        if (activated) return;

        base.Active();

        if (!ConnectedTerminal.activated)
        {
            ConnectedTerminal.Active();
            connectedDoor.OpenCloseDoor();
        }
    }
}
