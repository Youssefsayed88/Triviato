using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace DefaultNamespace
{
    public class ToggleEventBool : MonoBehaviour
    {
        private Toggle Toggle;
        public UnityEvent<bool> EventBool;
        private void Awake()
        {
            Toggle = GetComponent<Toggle>();
            Toggle.onValueChanged.AddListener(Call);
        }

        private void Call(bool arg0)
        {
            EventBool.Invoke(arg0);    
        }
    }
}