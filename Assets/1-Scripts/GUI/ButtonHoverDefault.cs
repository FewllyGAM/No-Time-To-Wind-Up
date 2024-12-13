using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonHoverDefault : MonoBehaviour
{
    CursorManager cursor;
    private void Start()
    {
        cursor = GameManager.gm.cursor;
    }

    public void OnEnter()
    {
        cursor.SetHover();
    }
    public void OnExit()
    {
        cursor.SetNormal();
    }
}
