using UnityEngine;
using DG.Tweening;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    [HideInInspector]
    public Vector3 offset;

    private void OnEnable()
    {
        offset = this.transform.position;
    }

    void FixedUpdate()
    {
        Vector3 desiredPosition = target.position + offset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position , desiredPosition, 0.5f);
        transform.position = smoothedPosition;
    }

    public void SetZoom(float zoomFactor , bool lerp = true)
    {
        
        if (lerp)
        {
            Camera.main.transform.DOKill(false);

            if (zoomFactor == 0)
            {
                Camera.main.transform.DOLocalMoveZ(zoomFactor, 1f).SetEase(Ease.OutSine);
            }

            else
            {
                if(-zoomFactor < Camera.main.transform.localPosition.z)
                {
                    Camera.main.transform.DOLocalMoveZ(-zoomFactor, 0.5f).SetEase(Ease.InSine);
                }
            }  
        }

        else
        {
            Camera.main.transform.localPosition = Vector3.zero;
        }
    }
}