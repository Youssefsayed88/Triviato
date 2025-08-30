using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ButtonAudio : MonoBehaviour
{
    private Button button;
    
    private void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(OnButtonClick);
    }
    
    private void OnButtonClick()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayButtonClick();
        }
    }
    
    private void OnDestroy()
    {
        if (button != null)
        {
            button.onClick.RemoveListener(OnButtonClick);
        }
    }
}
