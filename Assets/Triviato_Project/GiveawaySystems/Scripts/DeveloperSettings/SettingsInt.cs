using System;
using UnityEngine;

namespace GiveawaySystems.Scripts.DeveloperSettings
{
    [Serializable]
    public class SettingsInt
    {
        public Giveaway Settings;
        public int Default;

        public int GET()
        {
            if (Settings == null)
            {
                return Default;
            }
            else
            {
                return Settings.Quantity;
            }
        }
    }
}