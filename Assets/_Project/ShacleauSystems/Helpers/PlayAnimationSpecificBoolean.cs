using UnityEngine;

namespace DefaultNamespace
{
    public class PlayAnimationSpecificBoolean : MonoBehaviour
    {
        public Animator Animator;
        public string valueName;
        public void PlayBooleanToInt(int value)
        {
            Animator.SetInteger(valueName, value);
        }
    }
}