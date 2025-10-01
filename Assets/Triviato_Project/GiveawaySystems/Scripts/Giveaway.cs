using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "Giveaway", menuName = "GiveawaySystem/Giveaway", order = 1)]
public class Giveaway : ScriptableObject
{
    public Sprite sprite;
    public string Name;
    public int Quantity;
    public string setting;
    public int GiveawayWeight; // Higher is More common //
    private bool isPictureChagned; // Weight Free;
    private bool returnPicture; // Weight Free;
    public bool isSettings; // Weight Free;
    public bool isStringSettings; // Weight Free;
    public bool IsPictureChagned => isPictureChagned; // Weight Free;
    

    public bool disableChangeName; // Weight Free;
    public bool disableChangeQuantity; // Weight Free;
    public int ID;

    public void Load()
    {
        Quantity = PlayerPrefs.GetInt("Product-" + ID, Quantity);
        setting = PlayerPrefs.GetString("Product-Setting-" + ID, setting);
        Name = PlayerPrefs.GetString("Product-N-" + ID, Name);

        if (Name == "")
        {
            Name = this.name;
        }
        if (PlayerPrefs.GetString("Product-Image-" + ID, "") != "")
        {
            string Image = PlayerPrefs.GetString("Product-Image-" + ID);
            byte[] stringAsByte = Convert.FromBase64String(Image);
            Texture2D tex = new Texture2D(2, 2);
            tex.LoadImage(stringAsByte);
            sprite = Sprite.Create(tex, new Rect(0.0f, 0.0f, tex.width, tex.height), new Vector2(0.5f, 0.5f), 100.0f);
            this.sprite = sprite;
        }
    }

    public void Save()
    {
        PlayerPrefs.SetInt("Product-" + ID, Quantity);
        PlayerPrefs.SetString("Product-N-" + ID, Name);
        PlayerPrefs.SetString("Product-Setting-" + ID, setting);
        if(isPictureChagned){
            if (returnPicture)
            {
                PlayerPrefs.SetString("Product-Image-" + ID, "");
            } else {
                byte[] texAsByte = sprite.texture.EncodeToPNG();
                string texAsString = Convert.ToBase64String(texAsByte);
                PlayerPrefs.SetString("Product-Image-" + ID, texAsString);
          
            }
        }
        PlayerPrefs.Save();
    }
    
    public void LoadPNG(String filePath)
    {
      

    }

    public void ChangeQuantity(int quantity)
    {
        Quantity = quantity;
        Save();
    }
    
    public void ChangeString(string settings)
    {
        setting = settings;
        Save();
    }
    
    public void ChangeName(string name)
    {
        Name = name;
        Save();
    }
    
    public bool CounsmeQuantity(int quantityToCounsome, bool CheckOnly = false)
    {
        if (disableChangeQuantity)
        {
            return true;
        }
        if (Quantity < quantityToCounsome)
        {
            return false;
        }
        
        if(!CheckOnly)
            Quantity -= quantityToCounsome;
        Save();
        return true;
    }
}
