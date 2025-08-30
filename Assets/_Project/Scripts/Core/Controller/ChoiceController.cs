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
    [SerializeField] private Color correctAnswerColor = Color.green;
    
    private string choiceTextValue;
    private bool isCorrectAnswer;
    private bool isSelected;
    private System.Action<string, bool> onChoiceSelected;
    
    private void Awake()
    {
        if (choiceButton == null)
            choiceButton = GetComponent<Button>();
        
        if (choiceText == null)
            choiceText = GetComponentInChildren<TMP_Text>();
        
        if (buttonImage == null)
            buttonImage = GetComponent<Image>();
        
        choiceButton.onClick.AddListener(OnChoiceClicked);
    }
    
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
            buttonImage.color = correctAnswerColor;
        }
    }
    
    public void DisableInteraction()
    {
        choiceButton.interactable = false;
    }
    
    private void OnChoiceClicked()
    {
        if (isSelected) return;
        
        isSelected = true;
        
        // Play appropriate sound effect
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
        
        onChoiceSelected?.Invoke(choiceTextValue, isCorrectAnswer);
    }
    
    public string GetChoiceText()
    {
        return choiceTextValue;
    }
    
    public bool IsCorrectAnswer()
    {
        return isCorrectAnswer;
    }
}
