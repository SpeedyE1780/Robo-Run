using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    public Transform PopUps;
    [HideInInspector]
    public int currentMessage;
    public PlayerController player;

    public static TutorialManager Instance { get { return _instance; } } //Singleton Reference
    private static TutorialManager _instance;

    List<GameObject> Messages;

    private void OnEnable()
    {
        PopUps.gameObject.SetActive(true);
        currentMessage = 0;
        EventManager.pop += Pop;
    }

    private void OnDisable()
    {
        StopAllCoroutines();
        PopUps.gameObject.SetActive(false);
        EventManager.pop -= Pop;
    }

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

        //Initialize pop ups list
        Messages = new List<GameObject>();
        currentMessage = 0;

        foreach (Transform message in PopUps)
        {
            Messages.Add(message.gameObject);
        }

        this.enabled = false;
    }

    IEnumerator Resume()
    {
        yield return new WaitUntil(() => Input.GetMouseButtonDown(0));

        Time.timeScale = 1;
    }

    void Pop()
    {
        StartCoroutine("PopUp");

        //Freeze Player
        player.ToggleMove();
    }

    IEnumerator PopUp()
    {
        Messages[currentMessage].SetActive(true);

        yield return new WaitUntil(() => !Messages[currentMessage].activeSelf);

        if(Messages[currentMessage].GetComponent<PopUpAnimation>().PauseOnDisable)
        {
            Time.timeScale = 0;
            StartCoroutine("Resume");
        }

        //Unfreeze player and go to next message
        currentMessage++;
        player.ToggleMove();
    }
}