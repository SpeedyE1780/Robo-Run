using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData
{
    public LevelData[] Levels; 
    public int Coins;
    public int LevelProgress; //Last unlocked level
    public int MagnetLevel;
    public int BoostLevel;
    public List<int> SkinID;
    public int CurrentSkinID;

    public GameData(int levelCount)
    {
        //Initialize levels array
        Levels = new LevelData[levelCount];
        for (int i = 0; i < levelCount; i++)
        {
            Levels[i] = new LevelData(i);
        }

        Coins = 0;
        LevelProgress = 0;

        MagnetLevel = 1;
        BoostLevel = 1;

        SkinID = new List<int>();
        SkinID.Add(0);
        CurrentSkinID = 0;
    }

    //Copy old Data and add levels
    public GameData(GameData old , int levelCount)
    {
        //Initialize levels array
        Levels = new LevelData[levelCount];

        for (int i = 0; i < levelCount; i++)
        {
            if(i < old.Levels.Length)
            {
                Levels[i] = old.Levels[i];
            }

            else
            {
                Levels[i] = new LevelData(i);
            }
        }

        Coins = old.Coins;

        //Make sure that level progress is not greater than level count
        if(old.LevelProgress > levelCount)
        {
            LevelProgress = levelCount;
        }

        else
        {
            LevelProgress = old.LevelProgress;
        }
        
        MagnetLevel = old.MagnetLevel;
        BoostLevel = old.BoostLevel;
        SkinID = old.SkinID;
        CurrentSkinID = old.CurrentSkinID;
    }
}