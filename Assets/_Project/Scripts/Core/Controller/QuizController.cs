using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

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

    [Header("Question Progress System")]
    [SerializeField] private GameObject questionProgressPrefab;
    [SerializeField] private Transform questionProgressContainer;

    [Header("Settings")]
    [SerializeField] private float questionTimeLimit = 30f;
    [SerializeField] private float answerDelay = 2f;

    [Header("ParticleSystem")]
    [SerializeField] private ParticleSystem correctAnswerParticles;
    [SerializeField] private ParticleSystem wrongAnswerParticles;

    private Quiz quizData;
    private List<Question> randomizedQuestions = new();
    private int currentQuestionIndex = 0;
    private int correctAnswers = 0;
    private float currentTimer;
    private bool isAnswering = false;
    private Coroutine timerCoroutine;
    private List<ChoiceController> currentChoices = new List<ChoiceController>();
    private List<QuestionProgressController> questionProgressIndicators = new List<QuestionProgressController>();

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

        // Create question progress indicators
        CreateQuestionProgressIndicators();

        DisplayCurrentQuestion();
    }

    private void ResetQuiz()
    {
        if (timerCoroutine != null)
        {
            StopCoroutine(timerCoroutine);
        }

        // Stop timer sound
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.StopTimerSound();
        }

        ClearCurrentChoices();
        ClearQuestionProgressIndicators();
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

    private void CreateQuestionProgressIndicators()
    {
        ClearQuestionProgressIndicators();

        for (int i = 0; i < randomizedQuestions.Count; i++)
        {
            GameObject progressGO = Instantiate(questionProgressPrefab, questionProgressContainer);
            QuestionProgressController progressController = progressGO.GetComponent<QuestionProgressController>();

            if (progressController != null)
            {
                progressController.Initialize(i);
                questionProgressIndicators.Add(progressController);
            }
        }
    }

    private void ClearQuestionProgressIndicators()
    {
        foreach (QuestionProgressController indicator in questionProgressIndicators)
        {
            if (indicator != null)
            {
                Destroy(indicator.gameObject);
            }
        }
        questionProgressIndicators.Clear();
    }

    private void UpdateQuestionProgressIndicator(bool isCorrect)
    {
        if (currentQuestionIndex < questionProgressIndicators.Count)
        {
            QuestionProgressController currentIndicator = questionProgressIndicators[currentQuestionIndex];
            if (currentIndicator != null)
            {
                if (isCorrect)
                {
                    currentIndicator.SetAsCorrect();
                }
                else
                {
                    currentIndicator.SetAsWrong();
                }
            }
        }
    }

    private void OnChoiceSelected(string selectedAnswer, bool isCorrect)
    {
        if (isAnswering) return;

        isAnswering = true;

        // Stop timer sound when answering
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.StopTimerSound();
        }

        if (isCorrect)
        {
            correctAnswerParticles.Play();
            correctAnswers++;
        }
        else wrongAnswerParticles.Play();

        // Update question progress indicator
        UpdateQuestionProgressIndicator(isCorrect);

        // Show visual feedback
        ShowAnswerFeedback(selectedAnswer, isCorrect);

        // Move to next question after delay
        StartCoroutine(NextQuestionAfterDelay());
    }

    private void ShowAnswerFeedback(string selectedAnswer, bool isCorrect)
    {
        foreach (var choice in currentChoices)
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
            else if (choice.IsCorrectAnswer()) choice.ShowCorrectAnswer();

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

        // Start timer sound
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.StartTimerSound();
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
            timerText.text = $"Time: 0s";
            // Time's up - count as wrong answer
            OnTimeUp();
        }
    }

    private void OnTimeUp()
    {
        isAnswering = true;

        // Stop timer sound
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.StopTimerSound();
        }

        // Update question progress indicator (time up = wrong answer)
        UpdateQuestionProgressIndicator(false);

        // Show correct answer
        foreach (var choice in currentChoices)
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
        // Stop timer sound
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.StopTimerSound();
        }

        // Calculate result
        QuizResult result = new QuizResult
        {
            totalQuestions = randomizedQuestions.Count,
            correctAnswers = correctAnswers
        };

        // Play win or lose sound based on performance
        if (AudioManager.Instance != null)
        {
            float winPercentage = (float)correctAnswers / randomizedQuestions.Count;
            if (winPercentage >= 0.7f) // 70% or higher is considered a win
            {
                AudioManager.Instance.PlayWinSound();
            }
            else
            {
                AudioManager.Instance.PlayLoseSound();
            }
        }

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