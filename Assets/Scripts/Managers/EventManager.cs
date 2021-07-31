using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    //In Game
    //-------------------------------------------------
    public delegate void PlayerMoved();
    public static PlayerMoved playerMoved;

    public delegate void Died();
    public static Died died;

    public delegate void Respawn();
    public static Respawn respawn;

    public delegate void ReachedDestination();
    public static ReachedDestination reachedDestination;
    //---------------------------------------------------


    //Collectibles
    //------------------------------------
    public delegate void MagnetCollected();
    public static MagnetCollected magnetCollected;

    public delegate void BoostCollected();
    public static BoostCollected boostCollected;

    public delegate void CoinCollected(int Score);
    public static CoinCollected coin;

    public delegate void StarCollected(GameObject Star);
    public static StarCollected star;

    public delegate void SkinBought();
    public static SkinBought skinBought;
    //----------------------------------------------


    //Tutorial
    //------------------------
    public delegate void TutorialPop();
    public static TutorialPop pop;
    //-------------------------
}