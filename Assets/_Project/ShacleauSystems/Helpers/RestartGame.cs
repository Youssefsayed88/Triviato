using UnityEngine;
using UnityEngine.SceneManagement;

namespace DefaultNamespace
{
    public class RestartGame : MonoBehaviour
    {
            private void Start()
                {
                    if (Display.displays.Length > 1)
                        Display.displays[1].Activate();
                    Screen.SetResolution(1080, 1920, true);
                }

            
        public void RestartGameV()
        {
            SceneManager.LoadScene(0);
        }
    }
}