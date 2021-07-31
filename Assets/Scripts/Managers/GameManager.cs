using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class GameManager : MonoBehaviour
{
    #region P U B L I C  V A R I A B L E S

    public static GameManager Instance { get { return _instance; } } // Singleton Reference
    public GameObject Player;
    public Transform LevelParent;
    public Slider Slider;
    [HideInInspector]
    public GameData SaveFile;
    [HideInInspector]
    public int MaxLevel;
    [HideInInspector]
    public int CurrentLevel;
    [HideInInspector]
    public bool tutorialEnabled;

    #endregion

    #region P R I V A T E  V A R I A B L E S

    string savePath;
    float progression;
    float startDistance;
    GameObject Destination;
    LevelManager currentLevelManager;
    int CoinsCollected;
    List<GameObject> Levels;
    string tutorialKey;
    private static GameManager _instance; //Singleton

    #endregion

    #region A W A K E || S T A R T || U P D A T E

    private void OnEnable()
    {
        EventManager.died += Died;
        EventManager.respawn += ResetLevel;
        EventManager.coin += IncrementCoin;
        EventManager.reachedDestination += LevelWon;
        EventManager.playerMoved += StartProgress;
    }

    private void OnDisable()
    {
        EventManager.died -= Died;
        EventManager.respawn -= ResetLevel;
        EventManager.coin -= IncrementCoin;
        EventManager.reachedDestination -= LevelWon;
        EventManager.playerMoved -= StartProgress;
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

        Application.targetFrameRate = 60;

        //Initialize the level list
        Levels = new List<GameObject>();
        foreach(Transform level in LevelParent)
        {
            level.gameObject.SetActive(false);
            Levels.Add(level.gameObject);
        }

        MaxLevel = Levels.Count;

        Player.SetActive(false);

        tutorialKey = "Tutorial";

        if(PlayerPrefs.HasKey(tutorialKey))
        {
            tutorialEnabled = PlayerPrefs.GetInt(tutorialKey) == 1 ? true : false;
        }

        else
        {
            tutorialEnabled = true;
            PlayerPrefs.SetInt(tutorialKey, 1);
        } 
    }

    private void Start()
    {
        savePath = Application.persistentDataPath + "/GameData.json";
        LoadData();
    }

    #endregion

    #region F U N C T I O N S || C O R O U T I N E S

    public void StartGame(int Level = -1)
    {
        //Initialize The player
        Player.SetActive(true);
        Player.GetComponent<PlayerController>().InitializePlayer();

        //Place the camera in the beginning
        Camera.main.transform.parent.position = Camera.main.GetComponentInParent<CameraFollow>().offset;
        Camera.main.GetComponentInParent<CameraFollow>().SetZoom(0 , false);

        //Build the level called from the main menu
        if (Level != -1)
        {
            //Start the chosen level and set it to active
            CurrentLevel = Level;
        }

        //Build the next level
        else
        {
            //Disable the previous level
            Levels[CurrentLevel].SetActive(false);

            //Increment the current level and set it to active
            CurrentLevel++;
        }

        //Enable/Disable Tutorial Manager
        if (CurrentLevel == 0 && tutorialEnabled)
        {
            TutorialManager.Instance.enabled = true;
        }

        else
        {
            TutorialManager.Instance.enabled = false;
        }

        //Set the current level to active
        Levels[CurrentLevel].SetActive(true);

        //Set the currentLevelManager and reset it
        currentLevelManager = Levels[CurrentLevel].GetComponent<LevelManager>();
        currentLevelManager.LoadData();
        currentLevelManager.ResetObjects();

        //Set the coins collected to 0 and set the text
        CoinsCollected = 0;
        UIManager.Instance.UpdateCoinText(CoinsCollected);

        //Reset the UI stars and update the current level text
        UIManager.Instance.SetStars(SaveFile.Levels[CurrentLevel].StarCollected);
        UIManager.Instance.UpdateLevelText();

        //Reset Time
        Time.timeScale = 1;

        //Reset the progression bar
        Destination = GameObject.FindGameObjectWithTag("Destination");
        progression = 0;
        Slider.value = progression;
        startDistance = Destination.transform.position.x - Player.transform.position.x;

        //Send event that level started
        AnalyticsManager.Instance.LevelStarted();
    }

    public void ToggleTutorial(bool toggle)
    {
        tutorialEnabled = toggle;

        if (tutorialEnabled)
        {
            PlayerPrefs.SetInt(tutorialKey, 1);
        }

        else
        {
            PlayerPrefs.SetInt(tutorialKey, 0);
        }

        PlayerPrefs.Save();
    }

    public void StartLevel()
    {
        Player.GetComponent<PlayerController>().StartRoll(); //Enable player movement
    }

    public void LevelWon()
    {
        StopAllCoroutines();
        SaveData(true); //Save data
        UIManager.Instance.ToggleNextLevel(); //Show next level UI
        PowerUpManager.Instance.DeactivatePowerUp(); //Deactivate all power ups
        CoinsCollected = 0; //Reset the coins collected to prevent from adding them again when destroying the level

        if (CurrentLevel == 0)
        {
            tutorialEnabled = false;
            PlayerPrefs.SetInt(tutorialKey, 0);
            PlayerPrefs.Save();
        }

        //Send event that the level was completed
        AnalyticsManager.Instance.LevelComplete();
    }

    void Died()
    {
        StopAllCoroutines();
        UIManager.Instance.ToggleDiedUI(); //Show died UI
        PowerUpManager.Instance.DeactivatePowerUp(); //Deactivate power up
        AnalyticsManager.Instance.PlayerDied(); //Send Analytics Event that the player died during the current level
    }

    public void DestroyLevel()
    {
        Levels[CurrentLevel].SetActive(false);
        Player.gameObject.SetActive(false);
        Player.GetComponent<PlayerController>().StopMoving();
        StopAllCoroutines();

        //Reset timescale
        Time.timeScale = 1;

        //Deactivate power ups
        PowerUpManager.Instance.DeactivatePowerUp();

        //Save the coins collected during the level
        SaveData();

        //Disable the tutorial
        if(CurrentLevel == 0)
        {
            TutorialManager.Instance.enabled = false;
        }
    }

    void ResetLevel()
    {
        //Reset progression bar
        progression = 0;
        Slider.value = progression;

        //Save coins then reset them and the time scale
        SaveData();
        CoinsCollected = 0;
        UIManager.Instance.UpdateCoinText(CoinsCollected);
        Time.timeScale = 1;

        //Reset Level
        currentLevelManager.ResetObjects();

        PowerUpManager.Instance.DeactivatePowerUp();

        //Reset the tutorial pop ups
        if(CurrentLevel == 0)
        {
            TutorialManager.Instance.currentMessage = 0;
        }

        //Place the camera in the beginning
        Camera.main.transform.parent.position = Camera.main.GetComponentInParent<CameraFollow>().offset;
        Camera.main.GetComponentInParent<CameraFollow>().SetZoom(0 , false);

        //Send event that the level was retried
        AnalyticsManager.Instance.LevelRetry();
    }

    //Start Updating Progress Bar
    void StartProgress()
    {
        StartCoroutine("UpdateProgress");
    }

    //Update progress bar
    IEnumerator UpdateProgress()
    {
        while(true)
        {
            //Get the travelled distance divides it by the starting distance to know the progression%
            progression = (startDistance - (Destination.transform.position.x - Player.transform.position.x)) / startDistance;
            //Increase the slider
            Slider.value = progression;
            yield return null;
        }
    }

    void IncrementCoin(int s)
    {
        CoinsCollected += s;
        UIManager.Instance.UpdateCoinText(CoinsCollected);
    }

    public void TogglePause()
    {
        if(Time.timeScale == 0)
        {
            Time.timeScale = 1;
        }

        else
        {
            Time.timeScale = 0;
        }
    }

    //Save the new picked up star
    public void UpdateStarData(int StarIndex)
    {
        SaveFile.Levels[CurrentLevel].Stars[StarIndex].PickedUp = true;
        /*if (StarIndex == 0)
        {
            SaveFile.Levels[CurrentLevel].Star1 = 1;
        }

        else if (StarIndex == 1)
        {
            SaveFile.Levels[CurrentLevel].Star2 = 1;
        }

        else
        {
            SaveFile.Levels[CurrentLevel].Star3 = 1;
        }*/
    }

    public void SaveSkin(int ID , bool newSkin)
    {
        //Add new skin to save file
        if(newSkin)
        {
            SaveFile.SkinID.Add(ID);
        }

        //Change the current skin in the save file
        SaveFile.CurrentSkinID = ID;
        SaveData();
    }

    public void SaveData(bool IncrementLevel = false)
    {
        if(IncrementLevel)
        {
            //Increment level if 2 stars have been unlocked
            if (SaveFile.LevelProgress < CurrentLevel + 1)
            {
                SaveFile.LevelProgress = CurrentLevel + 1;
            }    
        }

        SaveFile.Coins += CoinsCollected;
        File.WriteAllText(savePath, JsonUtility.ToJson(SaveFile));
    }

    void LoadData()
    {
        if (File.Exists(savePath))
        {
            SaveFile = JsonUtility.FromJson<GameData>(File.ReadAllText(savePath));

            if (SaveFile.Levels.Length != Levels.Count)
            {
                SaveFile = new GameData(SaveFile, Levels.Count);
                SaveData();
            }
        }

        else
        {
            SaveFile = new GameData(Levels.Count);
            SaveData();
        }

        //Send a reference of the save file
        StoreManager.Instance.LoadRef(ref SaveFile);
        PowerUpManager.Instance.LoadData(ref SaveFile);
    }

    #endregion
}