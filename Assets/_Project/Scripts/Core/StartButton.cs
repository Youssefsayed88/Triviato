using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class StartButton : MonoBehaviour
{
    [SerializeField] private Button startButton;
    [SerializeField] private Image loadingImage;
    
    private const float LOADING_INCREMENT = 0.01f;
    private const float LOADING_DELAY = 0.01f;
    private const float MAX_LOADING_AMOUNT = 1f;
    private const float MIN_LOADING_AMOUNT = 0f;
    
    #region Unity Lifecycle
    
    private void Start()
    {
        InitializeButton();
        ResetLoadingImage();
    }
    
    private void OnEnable()
    {
        SubscribeToEvents();
    }
    
    private void OnDisable()
    {
        UnsubscribeFromEvents();
    }
    
    #endregion
    
    #region Initialization
    
    private void InitializeButton()
    {
        startButton.onClick.AddListener(OnStartButtonClicked);
    }
    
    private void ResetLoadingImage()
    {
        loadingImage.fillAmount = MIN_LOADING_AMOUNT;
    }
    
    #endregion
    
    #region Event Management
    
    private void SubscribeToEvents()
    {
        GameManager.Instance.GameRestarted += OnGameReset;
    }
    
    private void UnsubscribeFromEvents()
    {
        GameManager.Instance.GameRestarted -= OnGameReset;
    }
    
    #endregion
    
    #region Event Handlers
    
    private void OnGameReset()
    {
        ResetButtonState();
        ResetLoadingImage();
    }
    
    private void OnStartButtonClicked()
    {
        DisableButton();
        PlayStartGameSound();
        StartLoadingAnimation();
    }
    
    #endregion
    
    #region Button Management
    
    private void DisableButton()
    {
        startButton.interactable = false;
    }
    
    private void ResetButtonState()
    {
        startButton.interactable = true;
    }
    
    #endregion
    
    #region Audio Methods
    
    private void PlayStartGameSound()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayStartGame();
        }
    }
    
    #endregion
    
    #region Loading Animation
    
    private void StartLoadingAnimation()
    {
        StartCoroutine(FillLoadingImage());
    }
    
    private IEnumerator FillLoadingImage()
    {
        while (loadingImage.fillAmount < MAX_LOADING_AMOUNT)
        {
            loadingImage.fillAmount += LOADING_INCREMENT;
            yield return new WaitForSeconds(LOADING_DELAY);
        }
        
        CompleteLoading();
    }
    
    private void CompleteLoading()
    {
        loadingImage.fillAmount = MAX_LOADING_AMOUNT;
        StartGame();
    }
    
    #endregion
    
    #region Game Management
    
    private void StartGame()
    {
        GameManager.Instance.StartGame();
    }
    
    #endregion
}
