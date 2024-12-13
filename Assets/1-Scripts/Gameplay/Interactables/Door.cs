using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Door : MonoBehaviour, IRestartable
{
    [SerializeField] DoorTerminal terminalA;
    [SerializeField] DoorTerminal terminalB;

    [SerializeField] Transform door;
    [SerializeField] Transform openPos;
    [SerializeField] Transform closedPos;
    [SerializeField] float animTime = .2f;

    [SerializeField] GameObject obj;

    [SerializeField] bool isOpen;
    public bool IsOpen { get { return isOpen; } }

    [SerializeField] AudioClip door_clip;

    private void Awake()
    {
        //obstacle = GetComponentInChildren<NavMeshObstacle>();

        terminalA.ConnectedTerminal = terminalB;
        terminalA.ConnectedDoor = this;

        terminalB.ConnectedTerminal = terminalA;
        terminalB.ConnectedDoor = this;
    }

    public void OpenCloseDoor()
    {
        if (IsOpen) Close();
        else Open();
    }

    public void Open()
    {
        StartCoroutine(AnimateDoor());
        isOpen = true;
        //obstacle.enabled = false;
        obj.SetActive(false);
        AudioManager.managaner.Play(door_clip, true);
    }

    public void Close(bool forced = false)
    {
        StartCoroutine(AnimateDoor(forced ? closedPos : null));
        isOpen = false;
        //obstacle.enabled = true;
        obj.SetActive(true);
        if (!forced) AudioManager.managaner.Play(door_clip, true);
    }

    protected virtual IEnumerator AnimateDoor(Transform definedTarget = null)
    {
        float time = 0;
        Vector3 origin, target;
        if (!definedTarget)
        {
            origin = IsOpen ? openPos.position : closedPos.position;
            target = IsOpen ? closedPos.position : openPos.position;
        }
        else
        {
            origin = definedTarget.position;
            target = definedTarget.position;
        }


        while (time < 1)
        {
            door.position = Vector3.Slerp(origin, target, time);
            time += Time.deltaTime / animTime;
            yield return new WaitForEndOfFrame();
        }

        door.position = target;
    }

    public void Restart()
    {
        Close(true);
    }
}
