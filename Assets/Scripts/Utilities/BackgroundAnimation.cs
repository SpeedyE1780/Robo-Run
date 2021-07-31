using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class BackgroundAnimation : MonoBehaviour
{
    public Color Start;
    public Color End;

    Tween colorTween;
    Image BG;

    private void OnEnable()
    {
        if(colorTween == null)
        {
            BG = this.GetComponent<Image>();
            BG.color = Start;
            colorTween = BG.DOColor(End, 1).SetLoops(-1, LoopType.Yoyo);
        }

        else
        {
            colorTween.TogglePause();
        }
    }

    private void OnDisable()
    {
        colorTween.TogglePause();
        BG.color = Color.black;
    }
}