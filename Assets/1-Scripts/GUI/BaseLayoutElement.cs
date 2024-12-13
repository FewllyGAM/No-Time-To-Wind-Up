using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(LayoutElement))]
public class BaseLayoutElement : BaseWindow
{
    LayoutElement layout;

    protected override void Awake()
    {
        base.Awake();
        layout = GetComponent<LayoutElement>();
    }

    public override void Show(float time = 0.05F)
    {
        base.Show(time);
        layout.ignoreLayout = false;
    }
    public override void Hide(float time = 0.05F)
    {
        base.Hide(time);
        layout.ignoreLayout = true;
    }
}
