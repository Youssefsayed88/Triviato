using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace DefaultNamespace
{
    public class CorotuineOnEnable : MonoBehaviour
    {
        public float Timer;
        public UnityEvent TimeEvent;
        private void OnEnable()
        {
            StartCoroutine(Corotuine());
        }

        public IEnumerator Corotuine()
        {
            yield return new WaitForSeconds(Timer);
            TimeEvent.Invoke();
        }
    }
}