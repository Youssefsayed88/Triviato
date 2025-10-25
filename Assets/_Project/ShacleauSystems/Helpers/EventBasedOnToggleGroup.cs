using UnityEngine;
using UnityEngine.UI;

namespace DefaultNamespace
{
    public class EventBasedOnToggleGroup : MonoBehaviour
    {
        public ToggleGroup group;
        
        public void OnFireEvent()
        {
            group.GetFirstActiveToggle().GetComponent<CallingEvent>().Fire();
        }
    }
}