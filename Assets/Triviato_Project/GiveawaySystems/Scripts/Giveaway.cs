using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using GiveawaySystems.Scripts;

[CreateAssetMenu(fileName = "Giveaway", menuName = "GiveawaySystem/Giveaway", order = 1)]
public class Giveaway : ScriptableObject
{
    [Header("Basic Information")]
    public string Name;
    public int ID;
    public string description;
    public bool isActive = true;

    [Header("Image Settings")]
    public Sprite sprite;
    private bool isPictureChagned;
    private bool returnPicture;
    public bool IsPictureChagned => isPictureChagned;

    [Header("Quantity Settings")]
    public int Quantity;
    public int GiveawayWeight; // Higher is More common
    public bool disableChangeQuantity;

    [Header("Category Settings")]
    public GiveawayCategory category;
    public int priority; // For sorting within category

    [Header("Additional Settings")]
    public string setting;
    public bool isSettings;
    public bool isStringSettings;
    public bool disableChangeName;

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
    
    public bool LoadPNG(string filePath, ImageConstraintManager constraintManager = null)
    {
        if (!File.Exists(filePath))
        {
            Debug.LogError($"Image file not found: {filePath}");
            return false;
        }

        try
        {
            byte[] fileData = File.ReadAllBytes(filePath);
            Texture2D tex = new Texture2D(2, 2);
            tex.LoadImage(fileData);

            // If we have a constraint manager, validate and resize the image
            if (constraintManager != null)
            {
                if (!constraintManager.ValidateImage(tex))
                {
                    // Try to resize the image to meet constraints
                    tex = constraintManager.ResizeImage(tex);
                    if (!constraintManager.ValidateImage(tex))
                    {
                        Debug.LogError($"Image does not meet size constraints even after resizing: {filePath}");
                        return false;
                    }
                }
            }

            // Create sprite from the validated/resized texture
            sprite = Sprite.Create(tex, new Rect(0.0f, 0.0f, tex.width, tex.height), new Vector2(0.5f, 0.5f), 100.0f);
            isPictureChagned = true;
            returnPicture = false;
            Save();
            return true;
        }
        catch (Exception e)
        {
            Debug.LogError($"Error loading image: {e.Message}");
            return false;
        }
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
