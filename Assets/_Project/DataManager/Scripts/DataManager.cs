using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using AdvancedMonoBehaviour.Scripts.Interfaces;
using AdvancedMonoBehaviour.Scripts.Patterns;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Shacleau
{

    public class DataManager : AdvancedSingletonPresent<DataManager>
    {
        public string FileName;
        public DataLine DataLine;
        private int _currentID = 0;
        public int Columns;

        public List<DataLine> Objects = new List<DataLine>();
        
        // Add Getting from web logic?

        
        public void AddToCurrentData(string data)
        {
            if (_currentID >= Columns)
                throw new OverflowException();
            
            DataLine.SetData(_currentID, data);
            _currentID++;
        }

        public void AddCurrentData()
        {
            AddToCurrentData(DateTime.Now.ToString(CultureInfo.CurrentCulture));
            Objects.Add(DataLine);
            Sinbad.CsvUtil.SaveObjects(Objects, Application.persistentDataPath + "/" + FileName);
            DataLine = new DataLine();
            _currentID = 0;
        }
        
        

        public void Start()
        {
            DataLine = new DataLine();
            if (File.Exists(Application.persistentDataPath + "/" + FileName))
            { 
                Objects = Sinbad.CsvUtil.LoadObjects<DataLine>(Application.persistentDataPath + "/" + FileName);
            }
            else
            {
                Objects = new List<DataLine>();
            }
        }
    }
}