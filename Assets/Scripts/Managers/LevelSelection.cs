using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelSelection : MonoBehaviour
{
    public TextMeshProUGUI LevelText;
    public Button PlayButton;
    public Image ButtonImage;
    public Sprite Unlocked;
    public Sprite Locked;
    public List<Image> StarsImages;
    public Sprite FadedStar;
    public Sprite Star;
    public int currentLevel;

    private void OnEnable()
    {
        if(GameManager.Instance == null)
        {
            return;
        }

        if(currentLevel <= GameManager.Instance.SaveFile.LevelProgress)
        {
            PlayButton.interactable = true;
            ButtonImage.sprite = Unlocked;

            LevelText.text = (currentLevel + 1).ToString();

            for (int i = 0; i < GameManager.Instance.SaveFile.Levels[currentLevel].StarCollected; i++)
            {
                StarsImages[i].sprite = Star;
            }
        }

        else
        {
            ButtonImage.sprite = Locked;
        }
    }

    public void StartLevel()
    {
        GameManager.Instance.StartGame(currentLevel);
        UIManager.Instance.GotoGameMenu();
    }
}