using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ChoiceController : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Button choiceButton;
    [SerializeField] private TMP_Text choiceText;
    [SerializeField] private Image buttonImage;
    
    [Header("Color Settings")]
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color correctColor = Color.blue;
    [SerializeField] private Color wrongColor = Color.red;
    
    private string choiceTextValue;
    private bool isCorrectAnswer;
    private bool isSelected;
    private System.Action<string, bool> onChoiceSelected;
    
    #region Unity Lifecycle
    
    private void Awake()
    {
        InitializeComponents();
    }
    
    #endregion
    
    #region Initialization
    
    private void InitializeComponents()
    {
        InitializeButton();
        InitializeText();
        InitializeImage();
        SetupButtonListener();
    }
    
    private void InitializeButton()
    {
        if (choiceButton == null)
        {
            choiceButton = GetComponent<Button>();
        }
    }
    
    private void InitializeText()
    {
        if (choiceText == null)
        {
            choiceText = GetComponentInChildren<TMP_Text>();
        }
    }
    
    private void InitializeImage()
    {
        if (buttonImage == null)
        {
            buttonImage = GetComponent<Image>();
        }
    }
    
    private void SetupButtonListener()
    {
        choiceButton.onClick.AddListener(OnChoiceClicked);
    }
    
    #endregion
    
    #region Public Methods
    
    public void Initialize(string text, bool isCorrect, System.Action<string, bool> callback)
    {
        choiceTextValue = text;
        isCorrectAnswer = isCorrect;
        onChoiceSelected = callback;
        
        choiceText.text = text;
        ResetChoice();
    }
    
    public void ResetChoice()
    {
        isSelected = false;
        buttonImage.color = normalColor;
        choiceButton.interactable = true;
    }
    
    public void SetAsCorrect()
    {
        buttonImage.color = correctColor;
        choiceButton.interactable = false;
    }
    
    public void SetAsWrong()
    {
        buttonImage.color = wrongColor;
        choiceButton.interactable = false;
    }
    
    public void ShowCorrectAnswer()
    {
        if (isCorrectAnswer)
        {
            buttonImage.color = correctColor;
        }
    }
    
    public void DisableInteraction()
    {
        choiceButton.interactable = false;
    }
    
    #endregion
    
    #region Event Handlers
    
    private void OnChoiceClicked()
    {
        if (isSelected) return;
        
        isSelected = true;
        PlayAnswerSound();
        onChoiceSelected?.Invoke(choiceTextValue, isCorrectAnswer);
    }
    
    #endregion
    
    #region Audio Methods
    
    private void PlayAnswerSound()
    {
        if (AudioManager.Instance != null)
        {
            if (isCorrectAnswer)
            {
                AudioManager.Instance.PlayCorrectAnswer();
            }
            else
            {
                AudioManager.Instance.PlayWrongAnswer();
            }
        }
    }
    
    #endregion
    
    #region Getters
    
    public string GetChoiceText()
    {
        return choiceTextValue;
    }
    
    public bool IsCorrectAnswer()
    {
        return isCorrectAnswer;
    }
    
    #endregion
}
