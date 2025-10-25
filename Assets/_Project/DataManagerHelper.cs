using System.Collections;
using System.Collections.Generic;
using Shacleau;
using TMPro;
using UnityEngine;

public class DataManagerHelper : MonoBehaviour
{
    public DataManager dataManager;
    public TMP_InputField[] InputFields;
    
    public void OnDataManager()
    {
        for (int i = 0; i < InputFields.Length; i++)
        {
            dataManager.AddToCurrentData(InputFields[i].text);
        }
        
        dataManager.AddCurrentData();
    }
}
