using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ScaleAnimation : MonoBehaviour
{
    public float scaleFactor;
    Vector3 initialScale;
    Tween tween;

    private void OnEnable()
    {
        if (initialScale == Vector3.zero)
        {
            initialScale = this.GetComponent<RectTransform>().localScale;
        }

        if(tween != null)
        {
            tween.TogglePause();
        }

        else
        {
            tween = transform.DOScale(transform.localScale + (Vector3.one * scaleFactor), 0.5f).SetLoops(-1, LoopType.Yoyo);
        }
    }

    private void OnDisable()
    {
        tween.TogglePause();
        this.GetComponent<RectTransform>().localScale = initialScale;
    }
}