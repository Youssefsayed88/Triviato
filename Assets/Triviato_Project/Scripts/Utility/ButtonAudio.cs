using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ButtonAudio : MonoBehaviour
{
    private Button button;
    
    #region Unity Lifecycle
    
    private void Awake()
    {
        InitializeButton();
    }
    
    private void OnDestroy()
    {
        CleanupButtonListener();
    }
    
    #endregion
    
    #region Initialization
    
    private void InitializeButton()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(OnButtonClick);
    }
    
    #endregion
    
    #region Event Handlers
    
    private void OnButtonClick()
    {
        PlayButtonClickSound();
    }
    
    #endregion
    
    #region Audio Methods
    
    private void PlayButtonClickSound()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayButtonClick();
        }
    }
    
    #endregion
    
    #region Cleanup
    
    private void CleanupButtonListener()
    {
        if (button != null)
        {
            button.onClick.RemoveListener(OnButtonClick);
        }
    }
    
    #endregion
}
