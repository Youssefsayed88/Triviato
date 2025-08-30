using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class QuizController : MonoBehaviour
{
    [Header("Quiz Data")]
    [SerializeField] private TextAsset quizJson;
    
    [Header("Settings")]
    [SerializeField] private float answerDelay = 2f;
    
    [Header("Managers")]
    [SerializeField] private QuizTimerManager timerManager;
    [SerializeField] private QuizUIManager uiManager;
    
    private QuizStateManager stateManager;
    private QuizResultHandler resultHandler;
    private Quiz quizData;
    
    private void Start()
    {
        InitializeManagers();
        LoadQuizData();
        SubscribeToGameEvents();
    }
    
    private void OnDisable()
    {
        UnsubscribeFromGameEvents();
    }
    
    private void Update()
    {
        HandleRestartInput();
    }
    
    private void InitializeManagers()
    {
        stateManager = new QuizStateManager();
        resultHandler = new QuizResultHandler();
        timerManager.Initialize(OnTimeUp);
    }
    
    private void LoadQuizData()
    {
        quizData = QuizLoader.LoadQuizFromJson(quizJson);
        Debug.Log($"Loaded {quizData.questions.Count} questions");
    }
    
    private void SubscribeToGameEvents()
    {
        GameManager.Instance.GameStarted += StartQuiz;
        GameManager.Instance.GameRestarted += ResetQuiz;
    }
    
    private void UnsubscribeFromGameEvents()
    {
        if (GameManager.Instance == null) return;
        GameManager.Instance.GameStarted -= StartQuiz;
        GameManager.Instance.GameRestarted -= ResetQuiz;
    }
    
    private void StartQuiz()
    {
        stateManager.Initialize(quizData);
        uiManager.InitializeQuestionProgress(stateManager.TotalQuestions);
        DisplayCurrentQuestion();
    }
    
    private void ResetQuiz()
    {
        timerManager.StopTimer();
        uiManager.ClearAllUI();
        stateManager.ResetState();
    }
    
    private void DisplayCurrentQuestion()
    {
        if (stateManager.IsQuizComplete)
        {
            EndQuiz();
            return;
        }
        
        stateManager.SetAnswering(false);
        Question currentQuestion = stateManager.GetCurrentQuestion();
        uiManager.DisplayQuestion(currentQuestion, stateManager.CurrentQuestionIndex);
        uiManager.ClearChoices();
        uiManager.CreateChoiceButtons(currentQuestion, OnChoiceSelected);
        timerManager.StartTimer();
    }
    
    private void OnChoiceSelected(string selectedAnswer, bool isCorrect)
    {
        if (stateManager.IsAnswering) return;
        
        stateManager.SetAnswering(true);
        timerManager.StopTimer();
        
        HandleAnswerResult(isCorrect);
        uiManager.UpdateQuestionProgress(stateManager.CurrentQuestionIndex, isCorrect);
        uiManager.ShowAnswerFeedback(selectedAnswer, isCorrect);
        
        StartCoroutine(NextQuestionAfterDelay());
    }
    
    private void HandleAnswerResult(bool isCorrect)
    {
        uiManager.PlayAnswerEffects(isCorrect);
        
        if (isCorrect)
        {
            stateManager.IncrementCorrectAnswers();
        }
    }
    
    private void OnTimeUp()
    {
        stateManager.SetAnswering(true);
        uiManager.UpdateQuestionProgress(stateManager.CurrentQuestionIndex, false);
        uiManager.ShowCorrectAnswerForTimeUp();
        StartCoroutine(NextQuestionAfterDelay());
    }
    
    private IEnumerator NextQuestionAfterDelay()
    {
        yield return new WaitForSeconds(answerDelay);
        stateManager.MoveToNextQuestion();
        DisplayCurrentQuestion();
    }
    
    private void EndQuiz()
    {
        timerManager.StopTimer();
        
        QuizResult result = resultHandler.CalculateResult(
            stateManager.TotalQuestions, 
            stateManager.CorrectAnswers
        );
        
        resultHandler.StoreResult(result);
        resultHandler.PlayResultSound(stateManager.TotalQuestions, stateManager.CorrectAnswers);
        
        GameManager.Instance.ShowResult();
    }
    
    private void HandleRestartInput()
    {
        if (Input.GetKeyDown(KeyCode.R) && !stateManager.IsQuizComplete)
        {
            GameManager.Instance.RestartGame();
        }
    }
}