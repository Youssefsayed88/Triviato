using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class QuestionProgressController : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Image backgroundImage;
    [SerializeField] private TMP_Text questionNumberText;
    
    [Header("Color Settings")]
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color correctColor = Color.blue;
    [SerializeField] private Color wrongColor = Color.red;
    
    private int questionIndex;
    
    #region Unity Lifecycle
    
    private void Awake()
    {
        InitializeComponents();
    }
    
    #endregion
    
    #region Initialization
    
    private void InitializeComponents()
    {
        InitializeBackgroundImage();
        InitializeQuestionNumberText();
    }
    
    private void InitializeBackgroundImage()
    {
        if (backgroundImage == null)
        {
            backgroundImage = GetComponent<Image>();
        }
    }
    
    private void InitializeQuestionNumberText()
    {
        if (questionNumberText == null)
        {
            questionNumberText = GetComponentInChildren<TMP_Text>();
        }
    }
    
    #endregion
    
    #region Public Methods
    
    public void Initialize(int questionIndex)
    {
        this.questionIndex = questionIndex;
        UpdateQuestionNumberText();
        ResetToNormal();
    }
    
    public void ResetToNormal()
    {
        backgroundImage.color = normalColor;
    }
    
    public void SetAsCorrect()
    {
        backgroundImage.color = correctColor;
        questionNumberText.color = normalColor;
    }
    
    public void SetAsWrong()
    {
        backgroundImage.color = wrongColor;
        questionNumberText.color = normalColor;
    }
    
    #endregion
    
    #region Private Methods
    
    private void UpdateQuestionNumberText()
    {
        questionNumberText.text = $"Question #{questionIndex + 1}";
    }
    
    #endregion
    
    #region Getters
    
    public int GetQuestionIndex()
    {
        return questionIndex;
    }
    
    #endregion
}
