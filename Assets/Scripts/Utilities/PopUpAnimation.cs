using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopUpAnimation : MonoBehaviour
{
    Animation popUp;
    AnimationState defaultClip;
    public bool PauseOnDisable;

    private void OnEnable()
    {
        if(popUp == null)
        {
            popUp = GetComponent<Animation>();
            defaultClip = popUp[popUp.clip.name];
        }

        defaultClip.speed = 1;
        defaultClip.time = 0;
        popUp.Play();
    }


    public void StartDismiss()
    {
        //Prevent the player from calling the animation twice
        if (!popUp.isPlaying)
        {
            SoundManager.Instance.ButtonClick.Play();
            defaultClip.speed = -1;
            defaultClip.time = defaultClip.length;
            popUp.Play();
            Invoke("Dismiss", defaultClip.length);
        }
    }

    void Dismiss()
    {
        this.gameObject.SetActive(false);
    }
}