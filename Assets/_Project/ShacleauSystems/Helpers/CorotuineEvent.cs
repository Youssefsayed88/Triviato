using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace DefaultNamespace
{
    public class CorotuineEvent : MonoBehaviour
    {
        public float Timer;
        public UnityEvent EventOnTime;
        public bool OnlyStartOneTime = false;
        private bool started = false;
        public void StartEvent()
        {
            Debug.Log("ERREREASD???!");
            if (!OnlyStartOneTime)
            {
                StopAllCoroutines();
                StartCoroutine(PlayEventAfterTime());
            }
            else
            {
                if (!started)
                {
                    StartCoroutine(PlayEventAfterTime());
                }
            }
        }

        public IEnumerator PlayEventAfterTime()
        {
            started = true;
            yield return new WaitForSeconds(Timer);
            EventOnTime.Invoke();
            started = false;
        }
    }
}