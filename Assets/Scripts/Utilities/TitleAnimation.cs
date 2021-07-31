using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleAnimation : MonoBehaviour
{
    Animation CurrentAnimation;
    Vector2 initialPosition;

    private void OnEnable()
    {
        if (CurrentAnimation == null)
        {
            CurrentAnimation = this.GetComponent<Animation>();
            initialPosition = this.GetComponent<RectTransform>().anchoredPosition;
        }

        //Place the title in its initial position
        this.GetComponent<RectTransform>().anchoredPosition = initialPosition;
        CurrentAnimation.Play("TitleFill");
        Invoke("PlayAnimation", CurrentAnimation["TitleFill"].length);
    }

    void PlayAnimation()
    {
        CurrentAnimation.Play("TitleMove");
    }
}