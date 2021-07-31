using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [Header("Level Data")]
    public int level;
    public Transform Stars;
    /*
    public GameObject Star1;
    public GameObject Star2;
    public GameObject Star3;
    */

    [Header("Reset Objects")]
    public Transform Platforms;
    public Transform BackgroundObjects;
    public Transform Coins;
    public Transform Obstacles;
    public Transform MovingObstacles;
    public Transform PowerUps;

    private LevelData levelData;

    private void OnEnable()
    {
        EventManager.star += UpdateStars;
    }

    private void OnDisable()
    {
        EventManager.star -= UpdateStars;
    }

    public void LoadData()
    {
        levelData = GameManager.Instance.SaveFile.Levels[level];

        if(levelData.Stars.Count == 0)
        {
            foreach(Transform star in Stars)
            {
                levelData.Stars.Add(new StarData(star.name, false));
            }

            GameManager.Instance.SaveFile.Levels[level] = levelData; // Update the level data in the save file
            GameManager.Instance.SaveData(); // Save the changes made
        }

        else
        {
            foreach(StarData star in levelData.Stars)
            {
                Stars.Find(star.Name).gameObject.SetActive(!star.PickedUp);
            }
        }

        /*if(levelData.Star1 == 1)
        {
            Star1.gameObject.SetActive(false);
        }

        if (levelData.Star2 == 1)
        {
            Star2.gameObject.SetActive(false);
        }

        if (levelData.Star3 == 1)
        {
            Star3.gameObject.SetActive(false);
        }*/
    }

    void UpdateStars(GameObject star)
    {
        star.GetComponent<PickUpController>().Deactivate();

        int index = -1;

        foreach(StarData temp in levelData.Stars)
        {
            if(temp.Name == star.name)
            {
                index = levelData.Stars.IndexOf(temp);
            }
        }

        /*
        if (star == Star1)
        {
            GameManager.Instance.UpdateStarData(0); // Star 1 is picked up
            levelData.Star1 = 1;
        }
        else if (star == Star2)
        {
            GameManager.Instance.UpdateStarData(1); // Star 2 is picked up
            levelData.Star2 = 1;
        }
        else if (star == Star3)
        {
            GameManager.Instance.UpdateStarData(2); // Star 3 is picked up
            levelData.Star3 = 1;
        }*/

        GameManager.Instance.UpdateStarData(index);
        UIManager.Instance.UpdateStars();
    }

    public void ResetObjects()
    {
        foreach (Transform child in Coins)
        {
            if (child.GetComponent<PickUpController>() != null)
            {
                child.GetComponent<PickUpController>().Activate();
            }
        }

        foreach(Transform child in PowerUps)
        {
            if(child.GetComponent<PickUpController>() != null)
            {
                child.GetComponent<PickUpController>().Activate();
            }
        }

        foreach (Transform child in Obstacles)
        {
            if (!child.gameObject.activeInHierarchy)
            {
                child.gameObject.SetActive(true);
            }
        }

        foreach (Transform child in MovingObstacles)
        {
            if (!child.gameObject.activeInHierarchy)
            {
                child.gameObject.SetActive(true);
            }

            Animation childAnimation = child.GetComponent<Animation>();
            if (childAnimation != null)
            {
                childAnimation["Pendulum"].time = Random.Range(0, childAnimation["Pendulum"].length);
                childAnimation.Play();
            }
        }
    }
}