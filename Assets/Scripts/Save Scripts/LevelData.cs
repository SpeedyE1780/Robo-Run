using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LevelData
{
    public int Level;
    public List<StarData> Stars;

    /*public int Star1;
    public int Star2;
    public int Star3;*/

    public int StarCollected {
        get 
        {
            int amount = 0;

            foreach(StarData star in Stars)
            {
                amount += star.PickedUp ? 1 : 0;
            }

            return amount;
            //return Star1 + Star2 + Star3; 
        } 
    }

    public LevelData(int level)
    {
        Level = level;
        Stars = new List<StarData>();

        /*
         Star1 = 0;
         Star2 = 0;
         Star3 = 0;
        */
    }
}