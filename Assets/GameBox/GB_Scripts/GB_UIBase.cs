using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public abstract class GB_UIBase : MonoBehaviour
{
    protected CanvasGroup canvasGroup;
    private bool firstShow = true;
    protected virtual void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        firstShow = true;
        canvasGroup.alpha = 0;
        canvasGroup.blocksRaycasts = false;
    }
    public virtual IEnumerator OnEnter()
    {
        if (firstShow)
        {
            firstShow = false;
            yield return null;
        }
        Transform transAll = transform.GetChild(1);
        transAll.localScale = new Vector3(0.8f, 0.8f, 1);
        canvasGroup.alpha = 0.8f;
        canvasGroup.blocksRaycasts = true;
        while (transAll.localScale.x < 1)
        {
            yield return null;
            float addValue = Time.unscaledDeltaTime * 2;
            transAll.localScale += new Vector3(addValue, addValue);
            canvasGroup.alpha += addValue;
        }
        transAll.localScale = Vector3.one;
        canvasGroup.alpha = 1;
        canvasGroup.interactable = true;
    }
    public virtual void OnPause() { }
    public virtual void OnResume() { }
    public virtual IEnumerator OnExit()
    {
        Transform transAll = transform.GetChild(1);
        canvasGroup.interactable = false;
        while (transAll.localScale.x > 0.8f)
        {
            yield return null;
            float addValue = Time.unscaledDeltaTime * 2;
            transAll.localScale -= new Vector3(addValue, addValue);
            canvasGroup.alpha -= addValue;
        }
        transAll.localScale = new Vector3(0.8f, 0.8f, 1);
        canvasGroup.alpha = 0;
        canvasGroup.blocksRaycasts = false;
    }
}
