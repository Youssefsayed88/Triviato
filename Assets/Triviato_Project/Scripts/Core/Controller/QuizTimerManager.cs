using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class QuizTimerManager : MonoBehaviour
{
    [SerializeField] private TMP_Text timerText;
    [SerializeField] private Image timerCircleFill;
    [SerializeField] private float questionTimeLimit = 30f;
    
    private float currentTimer;
    private Coroutine timerCoroutine;
    private System.Action onTimeUp;
    private bool isAnswering = false;
    
    private const string TIMER_TEXT_FORMAT = "{0}";
    private const string ZERO_TIME_TEXT = "0";
    
    public void Initialize(System.Action onTimeUpCallback)
    {
        onTimeUp = onTimeUpCallback;
    }
    
    public void StartTimer()
    {
        currentTimer = questionTimeLimit;
        isAnswering = false;
        StopTimerCoroutine();
        ResetTimerCircle();
        StartTimerSound();
        timerCoroutine = StartCoroutine(TimerCoroutine());
    }
    
    public void StopTimer()
    {
        isAnswering = true;
        StopTimerCoroutine();
        StopTimerSound();
    }
    
    public bool IsTimerRunning()
    {
        return timerCoroutine != null;
    }
    
    private IEnumerator TimerCoroutine()
    {
        while (currentTimer > 0 && !isAnswering)
        {
            UpdateTimerDisplay();
            currentTimer -= Time.deltaTime;
            yield return null;
        }
        
        if (currentTimer <= 0 && !isAnswering)
        {
            HandleTimeUp();
        }
    }
    
    private void UpdateTimerDisplay()
    {
        timerText.text = string.Format(TIMER_TEXT_FORMAT, Mathf.CeilToInt(currentTimer));
        
        // Update circle fill (1.0 = full, 0.0 = empty)
        if (timerCircleFill != null)
        {
            float fillAmount = currentTimer / questionTimeLimit;
            timerCircleFill.fillAmount = fillAmount;
        }
    }
    
    private void HandleTimeUp()
    {
        timerText.text = ZERO_TIME_TEXT;
        
        // Set circle to empty when time runs out
        if (timerCircleFill != null)
        {
            timerCircleFill.fillAmount = 0f;
        }
        
        onTimeUp?.Invoke();
    }
    
    private void StopTimerCoroutine()
    {
        if (timerCoroutine != null)
        {
            StopCoroutine(timerCoroutine);
            timerCoroutine = null;
        }
    }
    
    private void StartTimerSound()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.StartTimerSound();
        }
    }
    
    private void StopTimerSound()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.StopTimerSound();
        }
    }
    
    private void ResetTimerCircle()
    {
        if (timerCircleFill != null)
        {
            timerCircleFill.fillAmount = 1f; // Start with full circle
        }
    }
}
