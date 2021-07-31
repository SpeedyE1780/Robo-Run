using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class UIManager : MonoBehaviour
{
    #region P U B L I C  V A R I A B L E S

    public static UIManager Instance { get { return _instance; } } //Singleton Reference

    [Header("Main Menu")]
    public GameObject MainMenuUI;
    public GameObject TitleScreenUI;
    public GameObject LevelSelectionUI;
    public GameObject SettingsUI;
    public GameObject StoreUI;

    [Header("Level Selection")]
    public GameObject LeftArrow;
    public GameObject RightArrow;
    public Scrollbar LevelSlider;

    [Header("InGame Menus")]
    public GameObject GameUI;
    public GameObject GetReadyUI;
    public RectTransform PauseUI;
    public GameObject NextLevelUI;
    public Button NextButton;
    public GameObject DiedUI;

    [Header("Store")]
    public Text CoinText;
    public Button UpgradesButton;
    public RectTransform UpgradesTab;
    public Button CosmeticsButton;
    public RectTransform CosmeticsTab;

    [Header("Upgrades")]
    public Button StoreMagnet;
    public Text MagnetRank;
    public Text MagnetPrice;
    public Button StoreBoost;
    public Text BoostRank;
    public Text BoostPrice;

    [Header("Store Pop Up")]
    public GameObject StorePopUps;
    public GameObject DismissPopUp;

    [Header("Boost Pop Up")]
    public Animation BoostPopUp;
    public Text BoostDuration;
    public Text BoostSpeed;

    [Header("Magnet Pop Up")]
    public Animation MagnetPopUp;
    public Text MagnetDuration;
    public Text MagnetRange;

    [Header("Settings")]
    public Toggle TutorialToggle;

    [Header("Game UI")]
    public TextMeshProUGUI CurrentLevel;
    public TextMeshProUGUI NextLevel;
    public Text CoinCollected;
    public List<Image> Stars;

    [Header("Power Ups")]
    public GameObject MagnetIcon;
    public Animation MagnetText;
    public GameObject BoostIcon;
    public Animation BoostText;

    [Header("Next Level UI")]
    public Animation LevelComplete;
    public RectTransform LCPopUp;
    public RectTransform LCText;
    public Text LCCoins;
    public ParticleSystem LCStarEffect;
    public List<Image> LCStars;
    public List<Transform> LCStarsPositions;

    [Header("Images")]
    public Sprite Star;
    public Sprite FadedStar;
    public RectTransform Shockwave;

    #endregion

    #region P R I V A T E  V A R I A B L E S

    int starsCollected;
    int coinsCollected;
    bool finalLevel;
    private static UIManager _instance; //Singleton

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

        //Forces the game to start on the title screen
        MainMenuUI.SetActive(true);
        TitleScreenUI.SetActive(true);
        LevelSelectionUI.SetActive(false);
        StoreUI.SetActive(false);
        SettingsUI.SetActive(false);
        GameUI.SetActive(false);
        PauseUI.gameObject.SetActive(false);
        GetReadyUI.SetActive(false);
        DiedUI.SetActive(false);
        NextLevelUI.SetActive(false);

        //Forces the store to appear with the upgrades tab first
        UpgradesButton.interactable = false;
        UpgradesTab.anchoredPosition = new Vector2(0, -75);
        CosmeticsButton.interactable = true;
        CosmeticsTab.anchoredPosition = new Vector2(2340, -75);
    }

    #endregion

    #region M A I N  M E N U

    //Menu Transitions
    //-------------------
    public void ToggleMenus()
    {
        MainMenuUI.SetActive(!MainMenuUI.activeSelf);
        TitleScreenUI.SetActive(MainMenuUI.activeSelf);
        GameUI.SetActive(!GameUI.activeSelf);
    }

    public void ToggleLevelSelection()
    {
        TitleScreenUI.SetActive(!TitleScreenUI.activeSelf);
        LevelSelectionUI.SetActive(!LevelSelectionUI.activeSelf);

        if(LevelSelectionUI.activeSelf)
        {
            LevelSlider.value = 0;
            CheckSlider();
        }
    }

    public void ToggleSettings()
    {
        TitleScreenUI.SetActive(!TitleScreenUI.activeSelf);
        SettingsUI.SetActive(!SettingsUI.activeSelf);

        if (SettingsUI.activeSelf)
        {
            if (GameManager.Instance.SaveFile.LevelProgress == 0)
            {
                TutorialToggle.interactable = false;
            }

            else
            {
                TutorialToggle.interactable = true;
            }

            TutorialToggle.isOn = GameManager.Instance.tutorialEnabled;
        }
    }

    public void ToggleStore()
    {
        TitleScreenUI.SetActive(!TitleScreenUI.activeSelf);
        StoreUI.SetActive(!StoreUI.activeSelf);
    }

    public void CheckSlider()
    {
        if(LevelSlider.value <= 0)
        {
            LeftArrow.SetActive(false);
        }

        else if(LevelSlider.value >= 1)
        {
            RightArrow.SetActive(false);
        }

        else
        {
            LeftArrow.SetActive(true);
            RightArrow.SetActive(true);
        }
    }

    //------------------------------------------------

    //STORE
    //-----------------------------------

    //Switch between upgrades and cosmetics tab
    public void SwitchTab()
    {
        CosmeticsButton.interactable = !CosmeticsButton.interactable;
        UpgradesButton.interactable = !UpgradesButton.interactable;

        CosmeticsTab.DOAnchorPos(UpgradesTab.anchoredPosition, 0.25f);
        UpgradesTab.DOAnchorPos(CosmeticsTab.anchoredPosition, 0.25f);
    }


    //Pop Up on purchase
    //--------------------------
    public void StartMagnetPop()
    {
        //Update text properties
        MagnetDuration.text = "DURATION: " + PowerUpManager.Instance.MagnetDuration + " SECONDS";
        MagnetRange.text = "RANGE: " + PowerUpManager.Instance.MagnetRange + " METERS";

        //Activate pop up gameObjects
        StorePopUps.SetActive(true);
        MagnetPopUp.gameObject.SetActive(true);

        //Initialize the animation and play it
        MagnetPopUp[MagnetPopUp.clip.name].speed = 1;
        MagnetPopUp[MagnetPopUp.clip.name].time = 0;
        MagnetPopUp.Play();

        //Enable dismissing after animation ends
        Invoke("ToggleDismissStorePopUp", MagnetPopUp.clip.length);
    }

    public void StartBoostPop()
    {
        //Update text properties
        BoostDuration.text = "DURATION: " + PowerUpManager.Instance.BoostDuration + " SECONDS";
        BoostSpeed.text = "SPEED: " + PowerUpManager.Instance.BoostSpeed + " METERS/SECONDS";

        //Activate pop up gameObjects
        StorePopUps.SetActive(true);
        BoostPopUp.gameObject.SetActive(true);

        //Initialize the animation and play it
        BoostPopUp[BoostPopUp.clip.name].speed = 1;
        BoostPopUp[BoostPopUp.clip.name].time = 0;
        BoostPopUp.Play();

        //Enable dismissing after animation ends
        Invoke("ToggleDismissStorePopUp", BoostPopUp.clip.length);
    }

    public void StartDismiss()
    {
        DismissPopUp.SetActive(false);
        StartCoroutine("Dismiss");
    }

    IEnumerator Dismiss()
    {
        if (BoostPopUp.gameObject.activeSelf)
        {
            BoostPopUp[BoostPopUp.clip.name].speed = -1;
            BoostPopUp[BoostPopUp.clip.name].time = BoostPopUp[BoostPopUp.clip.name].length;
            BoostPopUp.Play();

            yield return new WaitUntil(() => !BoostPopUp.isPlaying);
            BoostPopUp.gameObject.SetActive(false);
        }

        else
        {
            MagnetPopUp[MagnetPopUp.clip.name].speed = -1;
            MagnetPopUp[MagnetPopUp.clip.name].time = MagnetPopUp[MagnetPopUp.clip.name].length;
            MagnetPopUp.Play();

            yield return new WaitUntil(() => !MagnetPopUp.isPlaying);
            MagnetPopUp.gameObject.SetActive(false);
        }

        StorePopUps.SetActive(false);
    }

    void ToggleDismissStorePopUp()
    {
        DismissPopUp.SetActive(!DismissPopUp.activeSelf);
    }
    //----------------------------------------

    //Update Price/Rank/Coin Text
    public void UpdateStoreUI(int C, int ML, int MP, int BL, int BP)
    {
        CoinText.text = C.ToString();

        MagnetRank.text = "RANK: " + ML;

        if(MP == -1)
        {
            MagnetPrice.text = "MAX";
        }

        else
        {
            MagnetPrice.text = MP.ToString();
        }
        

        BoostRank.text = "RANK: " + BL;
        
        if(BP == -1)
        {
            BoostPrice.text = "MAX";
        }

        else
        {
            BoostPrice.text =  BP.ToString();
        }
    }

    //Enable/Disable Upgrade Button
    public void SetMagnetState(bool state)
    {
        StoreMagnet.interactable = state;
    }

    public void SetBoostState(bool state)
    {
        StoreBoost.interactable = state;
    }

    //---------------------------------------------

    //Fade Between Menues
    //--------------------
    public void StartSwitchToLevelSelection()
    {
        SoundManager.Instance.ButtonClick.Play();
        StartCoroutine("SwitchToLevelSelection");
    }

    IEnumerator SwitchToLevelSelection()
    {
        FadeScreenScript.Instance.FadeIn();

        yield return new WaitForSeconds(0.4f);

        ToggleLevelSelection();

        FadeScreenScript.Instance.FadeOut();
    }

    public void StartSwitchToSettings()
    {
        SoundManager.Instance.ButtonClick.Play();
        StartCoroutine("SwitchToSettings");
    }

    IEnumerator SwitchToSettings()
    {
        FadeScreenScript.Instance.FadeIn();

        yield return new WaitForSeconds(0.4f);

        ToggleSettings();

        FadeScreenScript.Instance.FadeOut();
    }

    public void StartSwitchToStore()
    {
        SoundManager.Instance.ButtonClick.Play();
        StartCoroutine("SwitchToStore");
    }

    IEnumerator SwitchToStore()
    {
        FadeScreenScript.Instance.FadeIn();

        yield return new WaitForSeconds(0.4f);

        ToggleStore();

        FadeScreenScript.Instance.FadeOut();
    }
    //------------------------------------------

    #endregion

    #region I N  G A M E  M E N U

    public void GotoGameMenu()
    {
        StartCoroutine("ShowGameMenu");
    }

    IEnumerator ShowGameMenu()
    {
        //Play button click sound
        SoundManager.Instance.ButtonClick.Play();

        FadeScreenScript.Instance.FadeIn();
        yield return new WaitForSeconds(0.4f);

        ToggleMenus();
        ToggleLevelSelection();
        ToggleGetReady();

        FadeScreenScript.Instance.FadeOut();
    }

    public void ToggleMagnet()
    {
        MagnetIcon.SetActive(!MagnetIcon.gameObject.activeSelf);
    }

    public void StartMagnetText()
    {
        StartCoroutine("MagnetTextPop");
    }

    IEnumerator MagnetTextPop()
    {
        MagnetText.gameObject.SetActive(true);
        MagnetText.Play();

        yield return new WaitUntil(() => !MagnetText.isPlaying);

        MagnetText.gameObject.SetActive(false);
    }

    public void StartBoostText()
    {
        StartCoroutine("BoostTextPop");
    }

    IEnumerator BoostTextPop()
    {
        BoostText.gameObject.SetActive(true);
        BoostText.Play();

        yield return new WaitUntil(() => !BoostText.isPlaying);

        BoostText.gameObject.SetActive(false);
    }

    public void ToggleBoost()
    {
        BoostIcon.SetActive(!BoostIcon.gameObject.activeSelf);
    }

    public void UpdateLevelText()
    {
        CurrentLevel.text = (GameManager.Instance.CurrentLevel + 1).ToString();

        //Last Level reached disable next level bubble next to progression
        if (GameManager.Instance.CurrentLevel + 2 <= GameManager.Instance.MaxLevel)
        {
            NextLevel.transform.parent.gameObject.SetActive(true);
            NextLevel.text = (GameManager.Instance.CurrentLevel + 2).ToString();
            finalLevel = false;
        }

        else
        {
            NextLevel.transform.parent.gameObject.SetActive(false);
            finalLevel = true;
        }
    }

    public void UpdateCoinText(int coins)
    {
        CoinCollected.text = coins.ToString();
        coinsCollected = coins;
    }

    public void TogglePauseUI()
    {
        if(!PauseUI.gameObject.activeSelf)
        {
            PauseUI.gameObject.SetActive(!PauseUI.gameObject.activeSelf);
            DOTween.defaultTimeScaleIndependent = true;
            PauseUI.DOScale(Vector3.one, 0.1f);
        }

        else
        {
            PauseUI.localScale = Vector3.zero;
            PauseUI.gameObject.SetActive(!PauseUI.gameObject.activeSelf);
        }
    }

    public void ResumePause()
    {
        PauseUI.DOScale(Vector3.zero, 0.1f).OnComplete(() =>
        {
            GameManager.Instance.TogglePause();
            PauseUI.gameObject.SetActive(false);
            DOTween.defaultTimeScaleIndependent = false;
        });
    }

    public void ToggleGetReady()
    {
        GetReadyUI.SetActive(!GetReadyUI.activeSelf);
    }

    public void ToggleNextLevel()
    {
        NextLevelUI.SetActive(!NextLevelUI.activeSelf);

        if (NextLevelUI.activeSelf)
        {
            //Reset the animation then play it
            InitializeLevelCompleteAnimation();
            StartCoroutine("LevelCompleteAnimation");
            
            //Enable to go to the next level after 2 collected stars
            if (!finalLevel)
            {
                NextButton.interactable = true;
            }

            else
            {
                NextButton.interactable = false;
            }
        }
    }

    IEnumerator LevelCompleteAnimation()
    {
        //Set the coin text before showing the pop up
        int currentCoins = GameManager.Instance.SaveFile.Coins - coinsCollected;
        LCCoins.text = currentCoins.ToString();

        //Play pop up and text animation
        LevelComplete.Play();
        yield return new WaitUntil(() => LCText.localScale == Vector3.one);


        
        //Play Shockwave animation and turn on the collected stars
        for (int i = 0; i < Stars.Count; i++)
        {
            if(Stars[i].sprite == Star)
            {
                LCStars[i].sprite = Star;
                SoundManager.Instance.Star.Play();

                LCStarEffect.transform.position = LCStarsPositions[i].position;
                LCStarEffect.Play();
                yield return new WaitUntil(() => !LCStarEffect.isPlaying);
            }
        }

        //Coin text animation
        for(int i = currentCoins; i <= GameManager.Instance.SaveFile.Coins; i++)
        {
            yield return new WaitForSeconds(0.05f);
            LCCoins.text = i.ToString();
            SoundManager.Instance.Coin.Play();
        }

        yield return null;
    }

    void InitializeLevelCompleteAnimation()
    {
        LCPopUp.localScale = Vector3.one * 2;
        LCText.localScale = Vector3.one * 3;
        LCText.gameObject.SetActive(false);
        foreach (Image star in LCStars)
        {
            star.sprite = FadedStar;
        }
    }

    public void ToggleDiedUI()
    {
        DiedUI.SetActive(!DiedUI.activeSelf);
    }

    //Set the picked up stars to a star and not picked up to faded
    public void SetStars(int stars)
    {
        for (int i = 0; i < Stars.Count; i++)
        {
            if (i < stars)
            {
                Stars[i].sprite = Star;
            }

            else
            {
                Stars[i].sprite = FadedStar;
            }
        }

        starsCollected = stars;
    }

    //Update when a star is collected and play shockwave animation
    public void UpdateStars()
    {
        //Activate the shockwave and position it over the star
        Shockwave.localScale = Vector3.zero;
        Shockwave.gameObject.SetActive(true);
        Shockwave.anchoredPosition = Stars[starsCollected].rectTransform.anchoredPosition;

        //Scale animation change the sprite star and disable shockwave
        Shockwave.DOScale(Stars[starsCollected].transform.localScale, 0.25f).OnComplete(() =>
        {
            Stars[starsCollected].sprite = Star;
            Shockwave.gameObject.SetActive(false);
            starsCollected++;
        });
    }

    //Pause Menu Functions
    //-------------------------------------------
    public void StartPauseRetry()
    {
        SoundManager.Instance.ButtonClick.Play();
        StartCoroutine("PauseRetry");
    }

    IEnumerator PauseRetry()
    {
        FadeScreenScript.Instance.FadeIn();

        yield return new WaitForSecondsRealtime(0.4f);

        TogglePauseUI();
        EventManager.respawn.Invoke();

        FadeScreenScript.Instance.FadeOut();
    }

    public void StartPauseQuit()
    {
        AnalyticsManager.Instance.PauseQuit(); //Send event that player quit after pausing
        SoundManager.Instance.ButtonClick.Play();
        StartCoroutine("PauseQuit");
    }

    IEnumerator PauseQuit()
    {
        FadeScreenScript.Instance.FadeIn();

        yield return new WaitForSecondsRealtime(0.4f);

        TogglePauseUI();
        ToggleMenus();

        GameManager.Instance.DestroyLevel();

        FadeScreenScript.Instance.FadeOut();
    }
    //------------------------------------------

    //Next Level Menu Functions
    //------------------------------------------
    public void StartGOTONextLevel()
    {
        AnalyticsManager.Instance.GOTONextLevel(); //Send event player going to next level
        SoundManager.Instance.ButtonClick.Play();
        StartCoroutine("GOTONextLevel");
    }

    IEnumerator GOTONextLevel()
    {
        FadeScreenScript.Instance.FadeIn();
        yield return new WaitForSeconds(0.4f);

        ToggleNextLevel();
        GameManager.Instance.StartGame();
        ToggleGetReady();

        FadeScreenScript.Instance.FadeOut();
    }

    public void StartWinRetry()
    {
        SoundManager.Instance.ButtonClick.Play();
        StartCoroutine("WinRetry");
    }

    IEnumerator WinRetry()
    {
        FadeScreenScript.Instance.FadeIn();

        yield return new WaitForSeconds(0.4f);

        ToggleNextLevel();
        EventManager.respawn.Invoke();

        FadeScreenScript.Instance.FadeOut();
    }

    public void StartWinQuit()
    {
        AnalyticsManager.Instance.WinQuit(); //Send event that player won and quit the game
        SoundManager.Instance.ButtonClick.Play();
        StartCoroutine("WinQuit");
    }

    IEnumerator WinQuit()
    {
        FadeScreenScript.Instance.FadeIn();

        yield return new WaitForSeconds(0.4f);

        ToggleNextLevel();
        ToggleMenus();

        GameManager.Instance.DestroyLevel();

        FadeScreenScript.Instance.FadeOut();
    }
    //------------------------------------------

    //Died Menu Functions
    //-------------------------------------------
    public void StartRespawn()
    {
        SoundManager.Instance.ButtonClick.Play();
        StartCoroutine("Respawn");
    }

    IEnumerator Respawn()
    {
        FadeScreenScript.Instance.FadeIn();

        yield return new WaitForSeconds(0.4f);

        EventManager.respawn.Invoke();
        ToggleDiedUI();

        FadeScreenScript.Instance.FadeOut();
    }

    public void StartDiedQuit()
    {
        AnalyticsManager.Instance.DiedQuit(); //Send event that player quit after dying
        SoundManager.Instance.ButtonClick.Play();
        StartCoroutine("DiedQuit");
    }

    IEnumerator DiedQuit()
    {
        FadeScreenScript.Instance.FadeIn();

        yield return new WaitForSeconds(0.4f);

        ToggleDiedUI();
        ToggleMenus();
        GameManager.Instance.DestroyLevel();

        FadeScreenScript.Instance.FadeOut();
    }
    //------------------------------------------

    #endregion
}