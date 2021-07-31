using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class FadeScreenScript : MonoBehaviour
{
    Image fadeImage;

    // SINGLETON
    private static FadeScreenScript _instance;
    public static FadeScreenScript Instance { get { return _instance; } }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }

        fadeImage = GetComponent<Image>();
        this.gameObject.SetActive(false);
    }

    public void FadeIn()
    {
        this.gameObject.SetActive(true);
        DOTween.defaultTimeScaleIndependent = true;
        fadeImage.DOFade(1, 0.4f);
    }

    public void FadeOut()
    {
        fadeImage.DOFade(0, 0.4f).OnComplete(()=>
        {
            this.gameObject.SetActive(false);
            DOTween.defaultTimeScaleIndependent = false;
        });
    }
}