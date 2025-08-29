using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuizController : MonoBehaviour
{
    [Header("Quiz Data")]
    [SerializeField] private TextAsset quizJson;
    
    [Header("Quiz UI References")]
    [SerializeField] private TMP_Text questionText;
    [SerializeField] private Button[] answerButtons;
    [SerializeField] private TMP_Text timerText;
    [SerializeField] private TMP_Text questionCounterText;
    
    [Header("Settings")]
    [SerializeField] private float questionTimeLimit = 30f;
    [SerializeField] private float answerDelay = 2f;
    
    private Quiz quizData;
    private List<Question> randomizedQuestions;
    private int currentQuestionIndex = 0;
    private int correctAnswers = 0;
    private float currentTimer;
    private bool isAnswering = false;
    private Coroutine timerCoroutine;
    
    private void Start()
    {
        LoadQuizData();
        SetupAnswerButtons();
    }
    
    private void OnEnable()
    {
        GameManager.instance.GameStarted += StartQuiz;
        GameManager.instance.GameRestarted += ResetQuiz;
    }
    
    private void OnDisable()
    {
        if (GameManager.instance == null) return;
        GameManager.instance.GameStarted -= StartQuiz;
        GameManager.instance.GameRestarted -= ResetQuiz;
    }
    
    private void LoadQuizData()
    {
        quizData = QuizLoader.LoadQuizFromJson(quizJson);
        Debug.Log($"Loaded {quizData.questions.Count} questions");
    }
    
    private void SetupAnswerButtons()
    {
        for (int i = 0; i < answerButtons.Length; i++)
        {
            int buttonIndex = i;
            answerButtons[i].onClick.AddListener(() => OnAnswerSelected(buttonIndex));
        }
    }
    
    private void StartQuiz()
    {
        // Randomize questions
        randomizedQuestions = new List<Question>(quizData.questions);
        for (int i = randomizedQuestions.Count - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);
            Question temp = randomizedQuestions[i];
            randomizedQuestions[i] = randomizedQuestions[randomIndex];
            randomizedQuestions[randomIndex] = temp;
        }
        
        currentQuestionIndex = 0;
        correctAnswers = 0;
        
        DisplayCurrentQuestion();
    }
    
    private void ResetQuiz()
    {
        if (timerCoroutine != null)
        {
            StopCoroutine(timerCoroutine);
        }
        
        currentQuestionIndex = 0;
        correctAnswers = 0;
        isAnswering = false;
    }
    
    private void DisplayCurrentQuestion()
    {
        if (currentQuestionIndex >= randomizedQuestions.Count)
        {
            EndQuiz();
            return;
        }
        
        Question currentQuestion = randomizedQuestions[currentQuestionIndex];
        
        // Display question
        questionText.text = currentQuestion.question;
        questionCounterText.text = $"Question {currentQuestionIndex + 1}/{randomizedQuestions.Count}";
        
        // Randomize answer order
        List<string> randomizedOptions = new List<string>(currentQuestion.options);
        for (int i = randomizedOptions.Count - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);
            string temp = randomizedOptions[i];
            randomizedOptions[i] = randomizedOptions[randomIndex];
            randomizedOptions[randomIndex] = temp;
        }
        
        // Setup answer buttons
        for (int i = 0; i < answerButtons.Length; i++)
        {
            if (i < randomizedOptions.Count)
            {
                answerButtons[i].gameObject.SetActive(true);
                answerButtons[i].GetComponentInChildren<Text>().text = randomizedOptions[i];
                answerButtons[i].interactable = true;
                answerButtons[i].image.color = Color.white; // Reset button color
            }
            else
            {
                answerButtons[i].gameObject.SetActive(false);
            }
        }
        
        // Start timer
        StartTimer();
    }
    
    private void StartTimer()
    {
        currentTimer = questionTimeLimit;
        isAnswering = false;
        
        if (timerCoroutine != null)
        {
            StopCoroutine(timerCoroutine);
        }
        
        timerCoroutine = StartCoroutine(TimerCoroutine());
    }
    
    private IEnumerator TimerCoroutine()
    {
        while (currentTimer > 0 && !isAnswering)
        {
            timerText.text = $"Time: {Mathf.CeilToInt(currentTimer)}s";
            currentTimer -= Time.deltaTime;
            yield return null;
        }
        
        if (currentTimer <= 0 && !isAnswering)
        {
            // Time's up - count as wrong answer
            OnTimeUp();
        }
    }
    
    private void OnTimeUp()
    {
        isAnswering = true;
        
        // Show correct answer in green
        Question currentQuestion = randomizedQuestions[currentQuestionIndex];
        for (int i = 0; i < answerButtons.Length; i++)
        {
            if (answerButtons[i].GetComponentInChildren<Text>().text == currentQuestion.correct_answer)
            {
                answerButtons[i].image.color = Color.green;
                break;
            }
        }
        
        // Disable all buttons
        foreach (Button button in answerButtons)
        {
            button.interactable = false;
        }
        
        // Move to next question after delay
        StartCoroutine(NextQuestionAfterDelay());
    }
    
    private void OnAnswerSelected(int buttonIndex)
    {
        if (isAnswering) return;
        
        isAnswering = true;
        Question currentQuestion = randomizedQuestions[currentQuestionIndex];
        
        // Get the selected answer text
        string selectedAnswer = answerButtons[buttonIndex].GetComponentInChildren<Text>().text;
        
        // Check if answer is correct
        bool isCorrect = selectedAnswer == currentQuestion.correct_answer;
        
        if (isCorrect)
        {
            correctAnswers++;
            answerButtons[buttonIndex].image.color = Color.blue;
        }
        else
        {
            answerButtons[buttonIndex].image.color = Color.red;
            
            // Show correct answer in green
            for (int i = 0; i < answerButtons.Length; i++)
            {
                if (answerButtons[i].GetComponentInChildren<Text>().text == currentQuestion.correct_answer)
                {
                    answerButtons[i].image.color = Color.green;
                    break;
                }
            }
        }
        
        // Disable all buttons
        foreach (Button button in answerButtons)
        {
            button.interactable = false;
        }
        
        // Move to next question after delay
        StartCoroutine(NextQuestionAfterDelay());
    }
    
    private IEnumerator NextQuestionAfterDelay()
    {
        yield return new WaitForSeconds(answerDelay);
        currentQuestionIndex++;
        DisplayCurrentQuestion();
    }
    
    private void EndQuiz()
    {
        // Calculate result
        QuizResult result = new QuizResult
        {
            totalQuestions = randomizedQuestions.Count,
            correctAnswers = correctAnswers
        };
        
        // Store result for UIManager to access
        QuizResultManager.SetResult(result);
        
        // End the game
        GameManager.instance.ShowResult();
    }
    
    void Update()
    {
        // Hidden restart button (press R key)
        if (Input.GetKeyDown(KeyCode.R))
        {
            GameManager.instance.RestartGame();
        }
    }
}
