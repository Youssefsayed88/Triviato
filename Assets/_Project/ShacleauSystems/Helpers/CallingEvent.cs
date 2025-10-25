using UnityEngine;
using UnityEngine.Events;

namespace DefaultNamespace
{
    public class CallingEvent : MonoBehaviour
    {
        public UnityEvent UnitycallEvent;

        public void Fire()
        {
            UnitycallEvent.Invoke();
        }
    }
}