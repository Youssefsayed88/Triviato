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
    
    private void Awake()
    {
        if (backgroundImage == null)
            backgroundImage = GetComponent<Image>();
        
        if (questionNumberText == null)
            questionNumberText = GetComponentInChildren<TMP_Text>();
    }
    
    public void Initialize(int questionIndex)
    {
        this.questionIndex = questionIndex;
        questionNumberText.text = $"Question #{questionIndex + 1}";
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

    public int GetQuestionIndex()
    {
        return questionIndex;
    }
}
