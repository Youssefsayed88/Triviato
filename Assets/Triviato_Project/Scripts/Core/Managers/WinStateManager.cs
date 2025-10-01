using System.Collections;
using UnityEngine;
using System;

public class WinStateManager : MonoBehaviour
{
    [Header("Managers")]
    [SerializeField] private SpinWheelManager spinWheelManager;
    [SerializeField] private GiveawayManager giveawayManager;
    [SerializeField] private ScoreDisplayManager scoreDisplayManager;
    
    [Header("Settings")]
    [SerializeField] private float delayBetweenSpinAndGiveaway = 1f;
    
    public Action<Giveaway> OnGiveawaySelected;
    public Action OnWinSequenceComplete;
    public Action OnLoseSequenceComplete;
    public Action<int> OnWheelStopped; // New event when wheel stops
    
    private bool isWinSequenceActive = false;
    private int selectedSegment = -1;
    
    public bool IsWinSequenceActive => isWinSequenceActive;
    
    private void Start()
    {
        SubscribeToEvents();
    }
    
    private void OnDestroy()
    {
        UnsubscribeFromEvents();
    }
    
    private void SubscribeToEvents()
    {
        if (spinWheelManager != null)
        {
            spinWheelManager.OnSpinComplete += OnSpinComplete;
        }
        
        if (scoreDisplayManager != null)
        {
            scoreDisplayManager.OnScoreDisplayComplete += OnScoreDisplayComplete;
            scoreDisplayManager.OnLoseScoreDisplayComplete += OnLoseScoreDisplayComplete;
        }
    }
    
    private void UnsubscribeFromEvents()
    {
        if (spinWheelManager != null)
        {
            spinWheelManager.OnSpinComplete -= OnSpinComplete;
        }
        
        if (scoreDisplayManager != null)
        {
            scoreDisplayManager.OnScoreDisplayComplete -= OnScoreDisplayComplete;
            scoreDisplayManager.OnLoseScoreDisplayComplete -= OnLoseScoreDisplayComplete;
        }
    }
    
    public void StartWinSequence()
    {
        if (isWinSequenceActive) return;
        
        isWinSequenceActive = true;
        Debug.Log("Starting win sequence: Spin wheel first, then giveaway");
        
        // Start with spinning the wheel
        if (spinWheelManager != null)
        {
            spinWheelManager.StartSpin();
        }
        else
        {
            Debug.Log("SpinWheelManager is not assigned! Showing score display and then proceeding to giveaway.");
            // Fallback: show score display first, then proceed to giveaway
            ShowWinScoreDisplay();
        }
    }
    
    private void OnSpinComplete(int selectedSegment)
    {
        Debug.Log($"Spin complete! Selected segment: {selectedSegment}");
        this.selectedSegment = selectedSegment;
        
        // Notify that the wheel has stopped - UI will show the giveaway button
        OnWheelStopped?.Invoke(selectedSegment);
    }
    
    private IEnumerator ProceedToGiveaway()
    {
        yield return new WaitForSeconds(delayBetweenSpinAndGiveaway);
        
        // Now proceed to giveaway system
        ProcessGiveaway();
    }
    
    private IEnumerator ProceedToGiveawayImmediately()
    {
        // No delay when there's no spin wheel
        yield return new WaitForSeconds(0.1f); // Small delay to ensure UI is ready
        
        // Now proceed to giveaway system
        ProcessGiveaway();
    }
    
    private void ProcessGiveaway()
    {
        if (giveawayManager == null)
        {
            Debug.LogError("GiveawayManager is not assigned!");
            CompleteWinSequence();
            return;
        }
        
        // Check if there are available giveaways
        if (!giveawayManager.HaveQuantity())
        {
            Debug.LogWarning("No giveaways available!");
            CompleteWinSequence();
            return;
        }
        
        Giveaway selectedGiveaway = null;
        
        // If SpinWheelManager is not null, use the winning segment's giveaway
        if (spinWheelManager != null && selectedSegment >= 0)
        {
            Debug.Log($"Using winning segment {selectedSegment} giveaway");
            selectedGiveaway = giveawayManager.GetGiveawayForSegment(selectedSegment);
        }
        else
        {
            // If SpinWheelManager is null or no segment selected, get a random giveaway
            Debug.Log("SpinWheelManager is null or no segment selected, using random giveaway");
            selectedGiveaway = giveawayManager.GetRandomGiveaway();
        }
        
        if (selectedGiveaway != null)
        {
            Debug.Log($"Selected giveaway: {selectedGiveaway.Name}");
            OnGiveawaySelected?.Invoke(selectedGiveaway);
        }
        else
        {
            Debug.LogWarning("Failed to get a giveaway");
        }
        
        CompleteWinSequence();
    }
    
    private void CompleteWinSequence()
    {
        isWinSequenceActive = false;
        OnWinSequenceComplete?.Invoke();
        Debug.Log("Win sequence completed!");
    }
    
    public void TriggerGiveaway()
    {
        Debug.Log("Manually triggering giveaway...");
        ProcessGiveaway();
    }
    
    public void ResetWinSequence()
    {
        isWinSequenceActive = false;
        selectedSegment = -1;
        
        if (spinWheelManager != null)
        {
            spinWheelManager.ResetWheel();
        }
        
        if (scoreDisplayManager != null)
        {
            scoreDisplayManager.ResetScoreDisplay();
        }
    }
    
    private void ShowWinScoreDisplay()
    {
        if (scoreDisplayManager == null)
        {
            Debug.LogError("ScoreDisplayManager is not assigned! Cannot show score display.");
            StartCoroutine(ProceedToGiveaway());
            return;
        }
        
        // Get the quiz result to display the score
        var quizResult = QuizResultManager.GetResult();
        if (quizResult != null)
        {
            scoreDisplayManager.ShowScoreDisplay(quizResult.correctAnswers, quizResult.totalQuestions, true);
        }
        else
        {
            Debug.LogWarning("No quiz result found! Using default score display.");
            scoreDisplayManager.ShowScoreDisplay(5, 10, true); // Default fallback
        }
    }
    
    
    public void StartLoseSequence()
    {
        if (isWinSequenceActive) return;
        
        isWinSequenceActive = true;
        Debug.Log("Starting lose sequence: Show score display if score exists");
        
        // Check if there's a quiz result to show
        var quizResult = QuizResultManager.GetResult();
        if (quizResult != null)
        {
            // Show score display for lose state
            ShowLoseScoreDisplay();
        }
        else
        {
            Debug.Log("No quiz result found for lose state. Completing lose sequence.");
            CompleteLoseSequence();
        }
    }
    
    private void ShowLoseScoreDisplay()
    {
        if (scoreDisplayManager == null)
        {
            Debug.LogError("ScoreDisplayManager is not assigned! Cannot show score display.");
            CompleteLoseSequence();
            return;
        }
        
        // Get the quiz result to display the score
        var quizResult = QuizResultManager.GetResult();
        if (quizResult != null)
        {
            scoreDisplayManager.ShowScoreDisplay(quizResult.correctAnswers, quizResult.totalQuestions, false);
        }
        else
        {
            Debug.LogWarning("No quiz result found! Using default score display.");
            scoreDisplayManager.ShowScoreDisplay(3, 10, false); // Default fallback for lose
        }
    }
    
    private void OnScoreDisplayComplete()
    {
        Debug.Log("Score display completed! Showing giveaway panel.");
        ShowGiveawayPanel();
    }
    
    private void OnLoseScoreDisplayComplete()
    {
        Debug.Log("Lose score display completed!");
        CompleteLoseSequence();
    }
    
    private void ShowGiveawayPanel()
    {
        // Notify UI to show giveaway panel
        OnWheelStopped?.Invoke(-1); // Use -1 to indicate no wheel was spun
    }
    
    private void CompleteLoseSequence()
    {
        isWinSequenceActive = false;
        OnLoseSequenceComplete?.Invoke();
        Debug.Log("Lose sequence completed!");
    }
    
}
