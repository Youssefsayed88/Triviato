using UnityEngine;

public class QuizResultHandler
{
    private const float WIN_PERCENTAGE_THRESHOLD = 0.7f;
    
    public QuizResult CalculateResult(int totalQuestions, int correctAnswers)
    {
        return new QuizResult
        {
            totalQuestions = totalQuestions,
            correctAnswers = correctAnswers
        };
    }
    
    public void StoreResult(QuizResult result)
    {
        QuizResultManager.SetResult(result);
    }
    
    public void PlayResultSound(int totalQuestions, int correctAnswers)
    {
        if (AudioManager.Instance == null) return;
        
        float winPercentage = (float)correctAnswers / totalQuestions;
        if (winPercentage >= WIN_PERCENTAGE_THRESHOLD)
        {
            AudioManager.Instance.PlayWinSound();
        }
        else
        {
            AudioManager.Instance.PlayLoseSound();
        }
    }
    
    public bool IsWinResult(int totalQuestions, int correctAnswers)
    {
        float winPercentage = (float)correctAnswers / totalQuestions;
        return winPercentage >= WIN_PERCENTAGE_THRESHOLD;
    }
}
