using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasGroup))]
public class BaseWindow : MonoBehaviour
{
    CanvasGroup canvasGroup;

    protected virtual void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public virtual void Show(float time = .15f)
    {
        StartCoroutine(FadeCanvas(time, 1));
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
    }
    public virtual void Hide(float time = .15f)
    {
        StartCoroutine(FadeCanvas(time, 0));
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }

    protected virtual IEnumerator FadeCanvas(float fadeTime, float targetAlpha)
    {
        float time = 0;
        float currentAlpha = canvasGroup.alpha;

        while (time < 1)
        {
            canvasGroup.alpha = Mathf.Lerp(currentAlpha, targetAlpha, time);
            time += Time.unscaledDeltaTime / fadeTime;
            yield return new WaitForEndOfFrame();
        }
        canvasGroup.alpha = targetAlpha;
    }
}
