using UnityEngine;
using UnityEngine.Events;

namespace DefaultNamespace
{
    public class BooleanSwitch : MonoBehaviour
    {
        public UnityEvent OnTrue;
        public UnityEvent OnFalse;

        public bool SavedBoolean;
        public bool IsCalled;
        
        public void OnBooleanEvent(bool Event)
        {
            IsCalled = true;
            SavedBoolean = Event;
        }
        
        

        public void FireEvent()
        {
            if(!IsCalled)
                return;
            
            if(SavedBoolean)
                OnTrue.Invoke();
            else
            {
                OnFalse.Invoke();
            }
        }
    }
}