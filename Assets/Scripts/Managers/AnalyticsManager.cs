using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;

public class AnalyticsManager : MonoBehaviour
{
    Dictionary<string, object> level;

    public static AnalyticsManager Instance { get { return _instance; } } //Singleton Reference
    private static AnalyticsManager _instance; //Singelton

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

        //Events Parameter
        level = new Dictionary<string, object>();
        level.Add("Level", 0);
    }

    public void LevelStarted()
    {
        level["Level"] = GameManager.Instance.CurrentLevel + 1;
        Analytics.CustomEvent("Level Started", level);
    }

    public void LevelRetry()
    {
        level["Level"] = GameManager.Instance.CurrentLevel + 1;
        Analytics.CustomEvent("Level Retry", level);
    }

    public void LevelComplete()
    {
        level["Level"] = GameManager.Instance.CurrentLevel + 1;
        Analytics.CustomEvent("Level Complete", level);
    }

    public void PlayerDied()
    {
        level["Level"] = GameManager.Instance.CurrentLevel + 1;
        Analytics.CustomEvent("Player Died", level);
    }

    public void PauseQuit()
    {
        level["Level"] = GameManager.Instance.CurrentLevel + 1;
        Analytics.CustomEvent("Pause Quit", level);
    }

    public void DiedQuit()
    {
        level["Level"] = GameManager.Instance.CurrentLevel + 1;
        Analytics.CustomEvent("Died Quit", level);
    }

    public void WinQuit()
    {
        level["Level"] = GameManager.Instance.CurrentLevel + 1;
        Analytics.CustomEvent("Win Quit", level);
    }

    public void GOTONextLevel()
    {
        Dictionary<string, object> Levels = new Dictionary<string, object>();
        Levels.Add("Previous Level", GameManager.Instance.CurrentLevel + 1);
        Levels.Add("Next Level", GameManager.Instance.CurrentLevel + 2);
        Analytics.CustomEvent("Go to Next Level", Levels);
    }
}