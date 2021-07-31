using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoundManager : MonoBehaviour
{
    #region P U B L I C  V A R I A B L E S

    public static SoundManager Instance { get { return _instance; } } //Singleton reference

    [Header("Music")]
    public AudioSource BackgroundMusic;
    
    [Header("SFX")]
    public AudioSource ButtonClick;
    public AudioSource Jump;
    public AudioSource DoubleJump;
    public AudioSource Glide;
    public AudioSource Propeller;
    public AudioSource Star;
    public AudioSource Coin;
    public AudioSource PowerUp;
    public AudioSource Win;
    public AudioSource Lost;

    [Header("Sound Control")]
    public Slider MusicVolume;
    public Toggle MuteMusic;
    public Slider SFXVolume;
    public Toggle MuteSFX;
    public Transform SFXParent;

    #endregion

    #region P R I V A T E  V A R I A B L E S

    //Player Prefs Keys
    private string MusicMutedKey = "MusicMuted";
    private string MusicVolumeKey = "MusicVolume";
    private string SFXMutedKey = "SFXMuted";
    private string SFXVolumeKey = "SFXVolume";
    private List<AudioSource> SFXList;
    private static SoundManager _instance; //Singleton

    #endregion

    #region  A W A K E

    private void Awake()
    {
        //Initialize singleton
        if(_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }

        //Initialize SFX List
        SFXList = new List<AudioSource>();

        foreach(Transform sfx in SFXParent)
        {
            SFXList.Add(sfx.GetComponent<AudioSource>());
        }

        //Get/Set Music Muted key value
        if (PlayerPrefs.HasKey(MusicMutedKey))
        {
            MuteMusic.isOn = PlayerPrefs.GetInt(MusicMutedKey) == 1 ? true : false;

            ToggleMusic();
        }

        else
        {
            //Unmute the music
            MuteMusic.isOn = false;
            PlayerPrefs.SetInt(MusicMutedKey, 0);
            ToggleMusic();
        }

        //Get/Set Music Volume Key
        if (PlayerPrefs.HasKey(MusicVolumeKey))
        {
            MusicVolume.value = PlayerPrefs.GetFloat(MusicVolumeKey);
            ControlMusic();
        }

        else
        {
            MusicVolume.value = BackgroundMusic.volume;
            ControlMusic();
            PlayerPrefs.SetFloat(MusicVolumeKey, MusicVolume.value);
        }

        //Get/Set SFX Mute Key
        if (PlayerPrefs.HasKey(SFXMutedKey))
        {
            MuteSFX.isOn = PlayerPrefs.GetInt(SFXMutedKey) == 1 ? true : false;
            ToggleSFX();
        }

        else
        {
            //Unmute the sfx
            MuteSFX.isOn = false;
            PlayerPrefs.SetInt(SFXMutedKey, 0);
            ToggleSFX(); //Make sure all sfx are muted/unmuted
        }

        //Get/Set SFX Volume Key
        if(PlayerPrefs.HasKey(SFXVolumeKey))
        {
            SFXVolume.value = PlayerPrefs.GetFloat(SFXVolumeKey);
            ControlSFX();
        }

        else
        {
            SFXVolume.value = SFXList[0].volume;
            PlayerPrefs.SetFloat(SFXVolumeKey, SFXVolume.value);
            ControlSFX(); // Make sure all sfx have the same volume
        }

        PlayerPrefs.Save();
    }

    #endregion

    #region F U N C T I O N S

    public void ControlMusic()
    {
        BackgroundMusic.volume = MusicVolume.value;
        PlayerPrefs.SetFloat(MusicVolumeKey, MusicVolume.value);
        PlayerPrefs.Save();
    }

    public void ControlSFX()
    {
        foreach(AudioSource sfx in SFXList)
        {
            sfx.volume = SFXVolume.value;
        }

        PlayerPrefs.SetFloat(SFXVolumeKey, SFXVolume.value);
        PlayerPrefs.Save();
    }

    public void ToggleMusic()
    {
        BackgroundMusic.mute = MuteMusic.isOn;
        int value = MuteMusic.isOn ? 1 : 0;
        PlayerPrefs.SetInt(MusicMutedKey, value);
        PlayerPrefs.Save();
    }

    public void ToggleSFX()
    {
        foreach (AudioSource sfx in SFXList)
        {
            sfx.mute = MuteSFX.isOn;
        }

        int value = MuteSFX.isOn ? 1 : 0;
        PlayerPrefs.SetInt(SFXMutedKey, value);
        PlayerPrefs.Save();
    }

    #endregion
}