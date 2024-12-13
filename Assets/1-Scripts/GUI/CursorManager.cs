using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorManager : MonoBehaviour
{
    [SerializeField] Texture2D normal;
    [SerializeField] Texture2D hover;

    public void SetNormal()
    {
        Cursor.SetCursor(normal, Vector2.zero, CursorMode.Auto);
    }
    public void SetHover()
    {
        Cursor.SetCursor(hover, Vector2.zero, CursorMode.Auto);
    }
}
