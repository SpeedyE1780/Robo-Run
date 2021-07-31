using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PickUpController : MonoBehaviour
{
    Vector3 initialPosition;
    Transform player;
    bool seeking;
    float speed;

    //Get the initial position
    private void OnEnable()
    {
        initialPosition = this.transform.position;
        speed = 0.75f;
    }

    // Update is called once per frame
    void Update()
    {
        this.transform.Rotate(0, 1, 0);
    }

    //Initialize the pick up and turn it on
    public void Activate()
    {
        ResetPickUp();
        this.gameObject.SetActive(true);
    }

    //Initialize the pick up and turn it off
    public void Deactivate()
    {
        ResetPickUp();
        this.gameObject.SetActive(false);
    }

    //Reset position and rotation
    public void ResetPickUp()
    {
        StopAllCoroutines();
        this.transform.rotation = Quaternion.identity;
        this.transform.position = initialPosition;
        seeking = false;
        speed = 0.75f;
    }

    public void StartSeek(Transform playerRef)
    {
        if(!seeking)
        {
            seeking = true;
            player = playerRef;
            StartCoroutine("SeekPlayer");
        }
    }

    IEnumerator SeekPlayer()
    {
        while(true)
        {
            //set the speed to the distance to make sure coin doesn't go further away than the player
            if (speed > Vector3.Distance(this.transform.position, player.transform.position))
            {
                speed = Vector3.Distance(this.transform.position, player.transform.position);
            }

            this.transform.position += (player.transform.position - this.transform.position).normalized * speed;

            //Check that the game isn't paused
            yield return new WaitUntil(() => Time.timeScale == 1);
        }
    }
}