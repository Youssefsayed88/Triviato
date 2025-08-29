using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("UI Panels")]
    [SerializeField] private CanvasGroup startPanel;
    [SerializeField] private CanvasGroup quizPanel;
    [SerializeField] private CanvasGroup winResultPanel;
    [SerializeField] private CanvasGroup loseResultPanel;
    
    [Header("Result UI Elements")]
    [SerializeField] private Text winScoreText;
    [SerializeField] private Text loseScoreText;
    [SerializeField] private Text winRestartTimerText;
    [SerializeField] private Text loseRestartTimerText;
    
    [Header("Animation Settings")]
    [SerializeField] private float fadeDuration = 0.5f;
    
    private Coroutine restartTimerCoroutine;
    
    private void Start() => ShowStartPanel();
    
    private void OnEnable()
    {
        GameManager.instance.GameStarted += ShowQuizPanel;
        GameManager.instance.GameEnded += ShowResultPanel;
        GameManager.instance.GameRestarted += ShowStartPanel;
    }
    
    public void OnDisable()
    {
        if (GameManager.instance == null) return;
        GameManager.instance.GameStarted -= ShowQuizPanel;
        GameManager.instance.GameEnded -= ShowResultPanel;
        GameManager.instance.GameRestarted -= ShowStartPanel;
    }
    
    public void ShowStartPanel()
    {
        // Stop any running restart timer
        if (restartTimerCoroutine != null)
        {
            StopCoroutine(restartTimerCoroutine);
        }
        
        UIFader.FadeOut(quizPanel, fadeDuration);
        UIFader.FadeOut(winResultPanel, fadeDuration);
        UIFader.FadeOut(loseResultPanel, fadeDuration);
        UIFader.FadeIn(startPanel, fadeDuration);
    }
    
    public void ShowResultPanel()
    {
        UIFader.FadeOut(quizPanel, fadeDuration);
        
        // Get the quiz result
        QuizResult result = QuizResultManager.GetResult();
        
        if (result != null)
        {
            // Update score text
            string scoreText = $"Score: {result.correctAnswers}/{result.totalQuestions} ({result.ScorePercentage:F1}%)";
            
            if (result.Passed)
            {
                // Show win panel
                winScoreText.text = scoreText;
                UIFader.FadeIn(winResultPanel, fadeDuration);
                UIFader.FadeOut(loseResultPanel, fadeDuration);
                
                // Start restart timer
                restartTimerCoroutine = StartCoroutine(RestartTimerCoroutine(winRestartTimerText));
            }
            else
            {
                // Show lose panel
                loseScoreText.text = scoreText;
                UIFader.FadeIn(loseResultPanel, fadeDuration);
                UIFader.FadeOut(winResultPanel, fadeDuration);
                
                // Start restart timer
                restartTimerCoroutine = StartCoroutine(RestartTimerCoroutine(loseRestartTimerText));
            }
        }
        else
        {
            // Fallback - show lose panel
            UIFader.FadeIn(loseResultPanel, fadeDuration);
            UIFader.FadeOut(winResultPanel, fadeDuration);
        }
    }
    
    public void ShowQuizPanel()
    {
        UIFader.FadeOut(startPanel, fadeDuration);
        UIFader.FadeIn(quizPanel, fadeDuration);
    }
    
    private System.Collections.IEnumerator RestartTimerCoroutine(Text timerText)
    {
        float timer = 10f; // 10 second restart delay
        
        while (timer > 0)
        {
            timerText.text = $"Restarting in: {Mathf.CeilToInt(timer)}s";
            timer -= Time.deltaTime;
            yield return null;
        }
        
        timerText.text = "Restarting...";
        
        // The GameManager will handle the actual restart
        // This coroutine just updates the UI
    }
}