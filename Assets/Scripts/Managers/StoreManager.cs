using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoreManager : MonoBehaviour
{
    public List<int> MagnetPrice;
    public List<int> BoostPrice;
    public PlayerController Player;
    public Transform Skins;

    public static StoreManager Instance { get { return _instance; } }
    private static StoreManager _instance;

    GameData refSaveFile;

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

    public void LoadRef(ref GameData saveFile)
    {
        refSaveFile = saveFile;

        foreach(Transform skin in Skins)
        {
            SkinManager skinManager = skin.GetComponent<SkinManager>();

            if (skinManager.SkinID == refSaveFile.CurrentSkinID)
            {
                Player.ChangeSkin(skinManager.Skin);
            }
        }
    }

    public void CheckBalance()
    {
        //Check if can buy more upgrades or no
        if(refSaveFile.MagnetLevel < 5)
        {
            if (refSaveFile.Coins < MagnetPrice[refSaveFile.MagnetLevel - 1])
            {
                UIManager.Instance.SetMagnetState(false);
            }

            else
            {
                UIManager.Instance.SetMagnetState(true);
            }
        }

        else
        {
            UIManager.Instance.SetMagnetState(false);
        }
        

        if(refSaveFile.BoostLevel < 5)
        {
            if (refSaveFile.Coins < BoostPrice[refSaveFile.BoostLevel - 1])
            {
                UIManager.Instance.SetBoostState(false);
            }

            else
            {
                UIManager.Instance.SetBoostState(true);
            }
        }

        else
        {
            UIManager.Instance.SetBoostState(false);
        }
        
        //Update the store's text
        UIManager.Instance.UpdateStoreUI(refSaveFile.Coins, refSaveFile.MagnetLevel, MagnetPrice[refSaveFile.MagnetLevel - 1], refSaveFile.BoostLevel, BoostPrice[refSaveFile.BoostLevel - 1]);
    }

    public void IncrementMagnetLevel()
    {
        //Check if the player have enough coins
        if (refSaveFile.Coins >= MagnetPrice[refSaveFile.MagnetLevel - 1])
        {
            //Substract players coins and level up
            refSaveFile.Coins -= MagnetPrice[refSaveFile.MagnetLevel - 1];
            refSaveFile.MagnetLevel++;
            CheckBalance();

            GameManager.Instance.SaveData(); //Save the new data
            PowerUpManager.Instance.SetMagnet(); //Level up power
            UIManager.Instance.StartMagnetPop(); //Show Pop up
        }
    }

    public void IncrementBoostLevel()
    {
        if (refSaveFile.Coins >= BoostPrice[refSaveFile.BoostLevel - 1])
        {
            refSaveFile.Coins -= BoostPrice[refSaveFile.BoostLevel - 1];
            refSaveFile.BoostLevel++;
            CheckBalance();

            GameManager.Instance.SaveData(); //Save the new data
            PowerUpManager.Instance.SetBoost(); //Level up power
            UIManager.Instance.StartBoostPop(); //Show Pop up
        }
    }

    public void BuySkin(SkinManager currentSkin)
    {
        SoundManager.Instance.ButtonClick.Play();

        if (currentSkin.skinBought)
        {
            //Change the skin and save it as the current skin
            Player.ChangeSkin(currentSkin.Skin);
            GameManager.Instance.SaveSkin(currentSkin.SkinID, false);

            //Update the skins settings
            EventManager.skinBought.Invoke();
        }

        else
        {
            if(currentSkin.Price <= refSaveFile.Coins)
            {
                //Update coin count
                refSaveFile.Coins -= currentSkin.Price;
                UIManager.Instance.CoinText.text = refSaveFile.Coins.ToString();

                //Change player skin
                Player.ChangeSkin(currentSkin.Skin);
                currentSkin.skinBought = true;

                //Save the purchase and change the current skin in the save file
                GameManager.Instance.SaveSkin(currentSkin.SkinID, true);

                //Update the skins settings
                EventManager.skinBought.Invoke();
            }
        }
    }
}