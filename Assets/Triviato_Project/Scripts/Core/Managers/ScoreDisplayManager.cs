using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Collections;

public class ScoreDisplayManager : MonoBehaviour
{
    [Header("Score Display UI")]
    [SerializeField] private GameObject scoreDisplayPanel;
    [SerializeField] private Slider scoreSlider; // The slider (1-10)
    [SerializeField] private TMP_Text scoreText;
    
    [Header("Animation Settings")]
    [SerializeField] private float sliderAnimationDuration = 2f;
    
    public Action OnScoreDisplayComplete;
    public Action OnLoseScoreDisplayComplete;
    
    private bool isAnimating = false;
    private bool isWin = true;
    
    private void Start()
    {
        SetupSlider();
    }
    
    private void SetupSlider()
    {
        if (scoreSlider != null)
        {
            scoreSlider.minValue = 1f;
            scoreSlider.maxValue = 10f;
            scoreSlider.value = 1f;
            scoreSlider.interactable = false; // Make it non-interactive
        }
    }
    
    public void ShowScoreDisplay(int correctAnswers, int totalQuestions, bool isWin = true)
    {
        Debug.Log($"ScoreDisplayManager.ShowScoreDisplay called with: {correctAnswers}/{totalQuestions}, isWin: {isWin}");
        
        if (scoreDisplayPanel != null)
        {
            scoreDisplayPanel.SetActive(true);
            Debug.Log("Score display panel activated!");
        }
        else
        {
            Debug.LogError("Score display panel is not assigned!");
        }
        
        // Calculate score position (1-10)
        float percentage = (float)correctAnswers / totalQuestions;
        int score = Mathf.RoundToInt(Mathf.Lerp(1, 10, percentage));
        score = Mathf.Clamp(score, 1, 10);
        
        // Update UI text
        if (scoreText != null)
        {
            scoreText.text = $"Score: {correctAnswers}/{totalQuestions}";
        }
        
        // Start the score animation
        StartCoroutine(AnimateSlider(score, isWin));
    }
    
    private IEnumerator AnimateSlider(int targetScore, bool isWin = true)
    {
        isAnimating = true;
        
        if (scoreSlider == null)
        {
            Debug.LogError("Score slider not assigned!");
            yield break;
        }
        
        // Store the win state for button click
        this.isWin = isWin;
        
        // Animate the slider to the target score
        float startValue = scoreSlider.value;
        float targetValue = targetScore;
        float elapsedTime = 0f;
        
        while (elapsedTime < sliderAnimationDuration)
        {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / sliderAnimationDuration;
            
            // Use smooth interpolation
            scoreSlider.value = Mathf.Lerp(startValue, targetValue, progress);
            yield return null;
        }
        
        scoreSlider.value = targetValue;
        isAnimating = false;
        
        // For win state, show giveaway panel after score animation
        if (isWin)
        {
            OnScoreDisplayComplete?.Invoke();
        }
        else
        {
            OnLoseScoreDisplayComplete?.Invoke();
        }
    }
    
    
    public void HideScoreDisplay()
    {
        if (scoreDisplayPanel != null)
        {
            scoreDisplayPanel.SetActive(false);
        }
    }
    
    public void ResetScoreDisplay()
    {
        HideScoreDisplay();
        isAnimating = false;
        
        // Reset slider to starting position
        if (scoreSlider != null)
        {
            scoreSlider.value = 1f;
        }
    }
    
    public bool IsAnimating => isAnimating;
}