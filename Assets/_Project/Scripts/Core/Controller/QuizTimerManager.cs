using System.Collections;
using UnityEngine;
using TMPro;

public class QuizTimerManager : MonoBehaviour
{
    [SerializeField] private TMP_Text timerText;
    [SerializeField] private float questionTimeLimit = 30f;
    
    private float currentTimer;
    private Coroutine timerCoroutine;
    private System.Action onTimeUp;
    private bool isAnswering = false;
    
    private const string TIMER_TEXT_FORMAT = "Time: {0}s";
    private const string ZERO_TIME_TEXT = "Time: 0s";
    
    public void Initialize(System.Action onTimeUpCallback)
    {
        onTimeUp = onTimeUpCallback;
    }
    
    public void StartTimer()
    {
        currentTimer = questionTimeLimit;
        isAnswering = false;
        StopTimerCoroutine();
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
    }
    
    private void HandleTimeUp()
    {
        timerText.text = ZERO_TIME_TEXT;
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
}
