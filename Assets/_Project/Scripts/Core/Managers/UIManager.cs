using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("UI Panels")]
    [SerializeField] private CanvasGroup startPanel;
    [SerializeField] private CanvasGroup quizPanel;
    [SerializeField] private CanvasGroup winResultPanel;
    [SerializeField] private CanvasGroup loseResultPanel;
    
    [Header("Animation Settings")]
    [SerializeField] private float fadeDuration = 0.5f;
    
    private Coroutine restartTimerCoroutine;
    
    private void Start() => ShowStartPanel();
    
    private void OnEnable()
    {
        GameManager.Instance.GameStarted += ShowQuizPanel;
        GameManager.Instance.GameEnded += ShowResultPanel;
        GameManager.Instance.GameRestarted += ShowStartPanel;
    }
    
    public void OnDisable()
    {
        if (GameManager.Instance == null) return;
        GameManager.Instance.GameStarted -= ShowQuizPanel;
        GameManager.Instance.GameEnded -= ShowResultPanel;
        GameManager.Instance.GameRestarted -= ShowStartPanel;
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
        
        var result = QuizResultManager.GetResult();
        
        if (result != null)
        {
            if (result.Passed) UIFader.FadeIn(winResultPanel, fadeDuration);
            else UIFader.FadeIn(loseResultPanel, fadeDuration);
        }
        else
        {
            UIFader.FadeIn(loseResultPanel, fadeDuration);
            UIFader.FadeOut(winResultPanel, fadeDuration);
        }
    }
    
    public void ShowQuizPanel()
    {
        UIFader.FadeOut(startPanel, fadeDuration);
        UIFader.FadeIn(quizPanel, fadeDuration);
    }
}