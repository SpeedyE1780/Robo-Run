using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpManager : MonoBehaviour
{
    #region P U B L I C  V A R I A B L E S

    public PlayerController Player;
    [HideInInspector]
    public float MagnetDuration;
    [HideInInspector]
    public float MagnetRange;
    [HideInInspector]
    public float BoostDuration;
    [HideInInspector]
    public float BoostSpeed;
    public static PowerUpManager Instance { get { return _instance; } }

    #endregion

    #region P R I V A T E  V A R I A B L E S

    GameData refSaveFile;
    bool MagnetActive;
    bool BoostActive;
    private static PowerUpManager _instance;

    #endregion

    #region A W A K E || O N  E N A B L E || O N  D I S A B L E

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
    }

    private void OnEnable()
    {
        EventManager.magnetCollected += ActivateMagnet;
        EventManager.boostCollected += ActivateBoost;
    }

    private void OnDisable()
    {
        EventManager.magnetCollected -= ActivateMagnet;
        EventManager.boostCollected -= ActivateBoost;
    }

    #endregion

    #region F U N C T I O N S || C O R O U T I N E S

    public void LoadData(ref GameData refFile)
    {
        refSaveFile = refFile;

        SetMagnet();
        SetBoost();       
    }

    public void SetMagnet()
    {
        //Get the Magnet Level and set the Magnet's properties
        switch (refSaveFile.MagnetLevel)
        {
            case 1:

                MagnetDuration = 2;
                MagnetRange = 2.5f;
                break;

            case 2:

                MagnetDuration = 4;
                MagnetRange = 5;
                break;

            case 3:

                MagnetDuration = 6;
                MagnetRange = 7.5f;
                break;

            case 4:

                MagnetDuration = 8;
                MagnetRange = 9;
                break;

            case 5:

                MagnetDuration = 10;
                MagnetRange = 10;
                break;
        } 
    }

    public void SetBoost()
    {
        //Get the Boost Level and set the Boost's properties
        switch (refSaveFile.BoostLevel)
        {
            case 1:

                BoostDuration = 0.7f;
                BoostSpeed = 17.5f;
                break;

            case 2:

                BoostDuration = 1.4f;
                BoostSpeed = 20;
                break;

            case 3:

                BoostDuration = 2.1f;
                BoostSpeed = 22.5f;
                break;

            case 4:

                BoostDuration = 2.8f;
                BoostSpeed = 23.75f;
                break;

            case 5:

                BoostDuration = 3.5f;
                BoostSpeed = 25;
                break;
        }
    }

    void ActivateMagnet()
    {
        if(!MagnetActive)
        {
            Player.InitializeMagnet(MagnetRange);
            MagnetActive = true;
            UIManager.Instance.ToggleMagnet();
            StartCoroutine("MagnetPowerUp");
        }

        else
        {
            StopCoroutine("MagnetPowerUp");
            StartCoroutine("MagnetPowerUp");
        }

        UIManager.Instance.StartMagnetText();
    }

    void ActivateBoost()
    {
        if (!BoostActive)
        {
            BoostActive = true;
            UIManager.Instance.ToggleBoost();
            Player.InitializeBoostSpeed(BoostSpeed);
            Player.ToggleBoost();
            StartCoroutine("BoostPowerUp");
        }

        else
        {
            StopCoroutine("BoostPowerUp");
            StartCoroutine("BoostPowerUp");
        }

        UIManager.Instance.StartBoostText();
    }

    //Enable/Disable power up after duration done
    IEnumerator MagnetPowerUp()
    {
        yield return new WaitForSeconds(MagnetDuration);

        MagnetActive = false;
        Player.StopAllCoroutines();
        UIManager.Instance.ToggleMagnet();
    }

    IEnumerator BoostPowerUp()
    {
        yield return new WaitForSeconds(BoostDuration);

        BoostActive = false;
        UIManager.Instance.ToggleBoost();
        Player.ToggleBoost();
    }

    //Deactivate all powers
    public void DeactivatePowerUp()
    {
        StopAllCoroutines();

        if (MagnetActive)
        {
            MagnetActive = false;
            Player.StopAllCoroutines();
            UIManager.Instance.ToggleMagnet();
        }

        if(BoostActive)
        {
            BoostActive = false;
            UIManager.Instance.ToggleBoost();
            Player.ToggleBoost();
        }
    }

    #endregion
}