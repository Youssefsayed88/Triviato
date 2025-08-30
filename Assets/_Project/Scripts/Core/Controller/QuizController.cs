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
    [SerializeField] private TMP_Text timerText;
    [SerializeField] private TMP_Text questionCounterText;
    
    [Header("Choice System")]
    [SerializeField] private GameObject choicePrefab;
    [SerializeField] private Transform choiceGridLayout;
    
    [Header("Settings")]
    [SerializeField] private float questionTimeLimit = 30f;
    [SerializeField] private float answerDelay = 2f;
    
    private Quiz quizData;
    private List<Question> randomizedQuestions = new();
    private int currentQuestionIndex = 0;
    private int correctAnswers = 0;
    private float currentTimer;
    private bool isAnswering = false;
    private Coroutine timerCoroutine;
    private List<ChoiceController> currentChoices = new List<ChoiceController>();
    
    private void Start()
    {
        LoadQuizData();
        GameManager.Instance.GameStarted += StartQuiz;
        GameManager.Instance.GameRestarted += ResetQuiz;
    }

    private void OnDisable()
    {
        if (GameManager.Instance == null) return;
        GameManager.Instance.GameStarted -= StartQuiz;
        GameManager.Instance.GameRestarted -= ResetQuiz;
    }
    
    private void LoadQuizData()
    {
        quizData = QuizLoader.LoadQuizFromJson(quizJson);
        Debug.Log($"Loaded {quizData.questions.Count} questions");
    }
    
    private void StartQuiz()
    {
        // Randomize questions
        randomizedQuestions = new List<Question>(quizData.questions);
        for (int i = randomizedQuestions.Count - 1; i > 0; i--)
        {
            var randomIndex = Random.Range(0, i + 1);
            var temp = randomizedQuestions[i];
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
        
        ClearCurrentChoices();
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
        questionCounterText.text = $"#{currentQuestionIndex + 1}";
        
        // Clear previous choices
        ClearCurrentChoices();
        
        // Randomize answer order
        List<string> randomizedOptions = new List<string>(currentQuestion.options);
        for (int i = randomizedOptions.Count - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);
            string temp = randomizedOptions[i];
            randomizedOptions[i] = randomizedOptions[randomIndex];
            randomizedOptions[randomIndex] = temp;
        }
        
        // Create choice buttons
        CreateChoiceButtons(randomizedOptions, currentQuestion.correct_answer);
        
        // Start timer
        StartTimer();
    }
    
    private void CreateChoiceButtons(List<string> options, string correctAnswer)
    {
        foreach (string option in options)
        {
            // Instantiate choice prefab under grid layout
            GameObject choiceGO = Instantiate(choicePrefab, choiceGridLayout);
            ChoiceController choiceController = choiceGO.GetComponent<ChoiceController>();
            
            if (choiceController != null)
            {
                bool isCorrect = option == correctAnswer;
                choiceController.Initialize(option, isCorrect, OnChoiceSelected);
                currentChoices.Add(choiceController);
            }
        }
    }
    
    private void ClearCurrentChoices()
    {
        foreach (ChoiceController choice in currentChoices)
        {
            if (choice != null)
            {
                Destroy(choice.gameObject);
            }
        }
        currentChoices.Clear();
    }
    
    private void OnChoiceSelected(string selectedAnswer, bool isCorrect)
    {
        if (isAnswering) return;
        
        isAnswering = true;
        
        if (isCorrect)
        {
            correctAnswers++;
        }
        
        // Show visual feedback
        ShowAnswerFeedback(selectedAnswer, isCorrect);
        
        // Move to next question after delay
        StartCoroutine(NextQuestionAfterDelay());
    }
    
    private void ShowAnswerFeedback(string selectedAnswer, bool isCorrect)
    {
        foreach (ChoiceController choice in currentChoices)
        {
            if (choice.GetChoiceText() == selectedAnswer)
            {
                if (isCorrect)
                {
                    choice.SetAsCorrect();
                }
                else
                {
                    choice.SetAsWrong();
                }
            }
            else if (choice.IsCorrectAnswer())
            {
                // Show correct answer in green
                choice.ShowCorrectAnswer();
            }
            
            choice.DisableInteraction();
        }
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
        
        // Show correct answer
        foreach (ChoiceController choice in currentChoices)
        {
            if (choice.IsCorrectAnswer())
            {
                choice.ShowCorrectAnswer();
            }
            choice.DisableInteraction();
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
        GameManager.Instance.ShowResult();
    }
    
    void Update()
    {
        // Hidden restart button (press R key)
        if (Input.GetKeyDown(KeyCode.R))
        {
            GameManager.Instance.RestartGame();
        }
    }
}
