using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GiveawayDashboardEntity : MonoBehaviour
{

    
    public Giveaway myGiveaway;
    public Image image;
    public TMP_InputField quantityField;
    public TMP_InputField nameField;
    public TMP_InputField imageField;
    
    public void Load(Giveaway _giveaway)
    {
        myGiveaway = _giveaway;
        image.sprite = myGiveaway.sprite;
        if (myGiveaway.isSettings)
        {
            if (myGiveaway.isStringSettings)
            {
                quantityField.contentType = TMP_InputField.ContentType.Standard;
                quantityField.placeholder.gameObject.GetComponent<TMP_Text>().text = "Enter Settings";
                quantityField.text = myGiveaway.setting;
            }
        }
        
        if(quantityField && !myGiveaway.isStringSettings){
            quantityField.interactable = !myGiveaway.disableChangeQuantity;
            quantityField.text = myGiveaway.Quantity.ToString();
        }
        
        if(nameField){
            nameField.interactable = !myGiveaway.disableChangeName;
            nameField.text = myGiveaway.Name;
        }

        if (myGiveaway.IsPictureChagned)
        {
            imageField.gameObject.SetActive(true);
        }
    }
    
    public void Save()
    {
        if (imageField.text != "")
        {
                myGiveaway.LoadPNG(imageField.text);
        }
   
        if (myGiveaway.isStringSettings)
        {
            myGiveaway.ChangeString(quantityField.text);

        }
        else
        {
            myGiveaway.ChangeQuantity(Int32.Parse(quantityField.text));

        }
        
        
        myGiveaway.ChangeName(nameField.text);
    }
}
