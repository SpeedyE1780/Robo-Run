using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkinManager : MonoBehaviour
{
    public int SkinID;
    public Texture Skin;
    public Button BuyButton;
    public Text ButtonText;
    public Text Coins;
    public Text Name;
    public int Price;
    [HideInInspector]
    public bool skinBought = false;

    public void OnEnable()
    {
        EventManager.skinBought += CheckSkin;
        Name.text = Skin.name;
        CheckSkin();
    }

    public void OnDisable()
    {
        EventManager.skinBought -= CheckSkin;
    }

    void CheckSkin()
    {
        if (GameManager.Instance.SaveFile.SkinID.Contains(SkinID))
        {
            skinBought = true;
            Coins.text = "Bought";

            //The skin is currently selected
            if (GameManager.Instance.SaveFile.CurrentSkinID == SkinID)
            {
                ButtonText.text = "Selected";
                BuyButton.interactable = false;
            }

            //The skin is available but not selected
            else
            {
                ButtonText.text = "Select";
                BuyButton.interactable = true;
            }
        }

        //The skin is not bought
        else
        {
            //Set the coin text
            Coins.text = Price.ToString();

            //Check if the skin can be bought or no
            if (GameManager.Instance.SaveFile.Coins < Price)
            {
                BuyButton.interactable = false;
            }

            else
            {
                BuyButton.interactable = true;
            }
        }
    }
}