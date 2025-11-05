using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using RTLTMPro;
using System;
using Random = UnityEngine.Random;
using System.Xml;
using System.Text;
using System.Reflection;

public class SimpleTriviaManager : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private CanvasGroup startPanel;
    [SerializeField] private CanvasGroup questionPanel;
    [SerializeField] private CanvasGroup winPanel;
    [SerializeField] private CanvasGroup losePanel;
    
    [Header("UI References")]
    [SerializeField] private Button startButton;
    [SerializeField] private RTLTextMeshPro questionText;
    [SerializeField] private Button[] answerButtons;
    [SerializeField] private RTLTextMeshPro[] answerTexts;
    
    [Header("Quiz Data")]
    [SerializeField] private TextAsset questionsJson;
    [SerializeField] private int numberOfQuestions = 4;
    [SerializeField] private float questionWaitTime = 1.5f;
    
    [Header("Colors")]
    [SerializeField] private Color correctColor = Color.green;
    [SerializeField] private Color wrongColor = Color.red;
    [SerializeField] private Color correctNotSelectedColor = Color.blue;
    [SerializeField] private Color normalColor = Color.white;
    
    private List<Question> questions;
    private List<Question> selectedQuestions;
    private Question currentQuestion;
    private int currentQuestionIndex;
    private int questionsAsked;
    private int correctAnswers;
    private bool isAnswered;
    
    public static event Action<bool> AnswerSelected;
    
    private void Start()
    {
        LoadQuestions();
        InitializeGame();
    }
    
    private void InitializeGame()
    {
        if (startButton != null)
        {
            startButton.onClick.RemoveAllListeners();
            startButton.onClick.AddListener(StartGame);
        }
        
        ShowPanel(startPanel);
        HidePanel(questionPanel);
        HidePanel(winPanel);
        HidePanel(losePanel);
        
        questionsAsked = 0;
        correctAnswers = 0;
    }
    
    private void LoadQuestions()
    {
        if (questionsJson != null)
        {
            Quiz quiz = JsonUtility.FromJson<Quiz>(questionsJson.text);
            questions = quiz.questions;
        }
        else
        {
            Debug.LogError("No questions JSON file assigned!");
        }
    }
    
    private void StartGame()
    {
        if (questions == null || questions.Count == 0)
        {
            Debug.LogError("No questions available!");
            return;
        }
        
        SelectRandomQuestions();
        
        questionsAsked = 0;
        correctAnswers = 0;
        currentQuestionIndex = 0;
        
        HidePanel(startPanel);
        ShowPanel(questionPanel);
        
        ShowNextQuestion();
    }
    
    private void SelectRandomQuestions()
    {
        selectedQuestions = new List<Question>();
        List<int> usedIndices = new List<int>();
        
        int questionsToSelect = Mathf.Min(numberOfQuestions, questions.Count);
        
        for (int i = 0; i < questionsToSelect; i++)
        {
            int randomIndex;
            do
            {
                randomIndex = Random.Range(0, questions.Count);
            } while (usedIndices.Contains(randomIndex));
            
            usedIndices.Add(randomIndex);
            selectedQuestions.Add(questions[randomIndex]);
        }
    }
    
    private void ShowPanel(CanvasGroup panel)
    {
        if (panel != null)
        {
            panel.gameObject.SetActive(true);
            panel.alpha = 1f;
            panel.interactable = true;
            panel.blocksRaycasts = true;
        }
    }
    
    private void HidePanel(CanvasGroup panel)
    {
        if (panel != null)
        {
            panel.alpha = 0f;
            panel.interactable = false;
            panel.blocksRaycasts = false;
            panel.gameObject.SetActive(false);
        }
    }
    
    private void ShowNextQuestion()
    {
        if (selectedQuestions == null || selectedQuestions.Count == 0)
        {
            Debug.LogError("No questions available!");
            return;
        }
        
        if (currentQuestionIndex >= selectedQuestions.Count)
        {
            CheckGameResult();
            return;
        }
        
        currentQuestion = selectedQuestions[currentQuestionIndex];
        
        questionText.text = currentQuestion.question;

        questionText.UpdateText();

        ResetButtons();
        
        List<string> randomizedAnswers = RandomizeAnswers(currentQuestion.options);
        
        for (int i = 0; i < answerButtons.Length && i < randomizedAnswers.Count; i++)
        {
            answerTexts[i].text = randomizedAnswers[i];
            answerButtons[i].onClick.RemoveAllListeners();
            
            string answer = randomizedAnswers[i];
            bool isCorrect = answer == currentQuestion.correct_answer;
            
            answerButtons[i].onClick.AddListener(() => OnAnswerSelected(answer, isCorrect));
        }
        
        isAnswered = false;
    }
    
    private List<string> RandomizeAnswers(List<string> answers)
    {
        List<string> randomized = new List<string>(answers);
        
        for (int i = randomized.Count - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);
            string temp = randomized[i];
            randomized[i] = randomized[randomIndex];
            randomized[randomIndex] = temp;
        }
        
        return randomized;
    }
    
    private void OnAnswerSelected(string selectedAnswer, bool isCorrect)
    {
        if (isAnswered) return;
        
        isAnswered = true;
        questionsAsked++;
        
        if (isCorrect)
        {
            correctAnswers++;
        }
        
        for (int i = 0; i < answerButtons.Length; i++)
        {
            var colors = answerButtons[i].colors;
            var buttonText = RTLTextMeshProComparer.GetOriginalText(answerTexts[i]);
            bool isCorrectAnswer = RTLTextMeshProComparer.AreEqual(buttonText, currentQuestion.correct_answer);
            bool isSelectedAnswer = RTLTextMeshProComparer.AreEqual(buttonText, selectedAnswer);

            if (isSelectedAnswer && isCorrectAnswer)
            {
                colors.disabledColor = correctColor;
            }
            else if (isCorrectAnswer && !isSelectedAnswer)
            {
                colors.disabledColor = correctNotSelectedColor;
            }
            else
            {
                colors.disabledColor = wrongColor;
            }

            answerButtons[i].colors = colors;
            answerButtons[i].interactable = false;
        }

        AnswerSelected?.Invoke(isCorrect);
        
        if (!isCorrect)
        {
            StartCoroutine(ProceedToLosePanel());
        }
        else
        {
            StartCoroutine(ProceedToNextQuestion());
        }
    }
    
    private IEnumerator ProceedToNextQuestion()
    {
        yield return new WaitForSeconds(questionWaitTime);
        
        currentQuestionIndex++;
        ShowNextQuestion();
    }
    
    private IEnumerator ProceedToLosePanel()
    {
        yield return new WaitForSeconds(questionWaitTime);
        
        HidePanel(questionPanel);
        ShowPanel(losePanel);
    }
    
    private void CheckGameResult()
    {
        HidePanel(questionPanel);
        
        if (correctAnswers == numberOfQuestions)
        {
            ShowPanel(winPanel);
            StartCoroutine(ResetAfterWin());
        }
        else
        {
            ShowPanel(losePanel);
        }
    }
    
    private IEnumerator ResetAfterWin()
    {
        yield return new WaitForSeconds(5f);
        ResetTrivia();
    }
    
    private void ResetButtons()
    {
        for (int i = 0; i < answerButtons.Length; i++)
        {
            answerButtons[i].image.color = normalColor;
            answerButtons[i].interactable = true;
        }
    }
    
    public void ResetTrivia()
    {
        HidePanel(questionPanel);
        HidePanel(winPanel);
        HidePanel(losePanel);
        ShowPanel(startPanel);
        
        questionsAsked = 0;
        correctAnswers = 0;
        currentQuestionIndex = 0;
        isAnswered = false;
    }
}

public static class RTLTextMeshProComparer
{
    // Optional: remove Arabic diacritics (tashkeel)
    public static bool IgnoreDiacritics = true;

    private static readonly FieldInfo originalTextField =
        typeof(RTLTextMeshPro).GetField("originalText", BindingFlags.NonPublic | BindingFlags.Instance);

    /// <summary>
    /// Compare two raw RTL strings, ignoring invisible marks, Tatweel, normalization, and optionally diacritics.
    /// </summary>
    public static bool AreEqual(string a, string b)
    {
        if (a == null || b == null)
            return a == b;

        string normA = NormalizeRTL(a);
        string normB = NormalizeRTL(b);

        return string.Equals(normA, normB, System.StringComparison.Ordinal);
    }

    /// <summary>
    /// Compare two RTLTextMeshPro components by their original (unprocessed) text.
    /// </summary>
    public static bool AreEqual(RTLTextMeshPro rtlA, RTLTextMeshPro rtlB)
    {
        if (rtlA == null || rtlB == null)
            return rtlA == rtlB;

        string textA = GetOriginalText(rtlA);
        string textB = GetOriginalText(rtlB);

        return AreEqual(textA, textB);
    }

    /// <summary>
    /// Try to retrieve the original (unprocessed) text from an RTLTextMeshPro component.
    /// </summary>
    public static string GetOriginalText(RTLTextMeshPro rtl)
    {
        if (rtl == null)
            return null;

        // Try reflection (fallback method)
        string reflected = (string)originalTextField?.GetValue(rtl);
        if (!string.IsNullOrEmpty(reflected))
            return reflected;

        // Fallback to .text if reflection fails (processed string)
        return rtl.text;
    }

    /// <summary>
    /// Normalize RTL text: unify Unicode form, remove Tatweel, invisible marks, and optionally diacritics.
    /// </summary>
    private static string NormalizeRTL(string input)
    {
        if (input == null) return null;

        string normalized = input.Normalize(NormalizationForm.FormC);

        // Remove invisible RTL formatting characters
        normalized = normalized
            .Replace("\u200F", "") // Right-to-Left Mark
            .Replace("\u202B", "") // RTL Embedding
            .Replace("\u202C", "") // Pop Directional Formatting
            .Replace("\u202A", "") // LTR Embedding
            .Replace("\u202E", "") // RTL Override
            .Replace("\u0640", ""); // Tatweel (ـ)

        // Optionally remove diacritics (tashkeel)
        if (IgnoreDiacritics)
        {
            // Arabic diacritics range: 0x0610–0x061A, 0x064B–0x065F
            StringBuilder sb = new StringBuilder();
            foreach (char c in normalized)
            {
                if ((c >= 0x0610 && c <= 0x061A) ||
                    (c >= 0x064B && c <= 0x065F))
                    continue; // skip diacritics
                sb.Append(c);
            }
            normalized = sb.ToString();
        }

        return normalized;
    }
}