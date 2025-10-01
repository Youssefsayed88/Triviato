using UnityEngine;

/// <summary>
/// Simple manager to pass quiz results between QuizController and UIManager
/// </summary>
public static class QuizResultManager
{
    private static QuizResult currentResult;
    
    public static void SetResult(QuizResult result)
    {
        currentResult = result;
    }
    
    public static QuizResult GetResult()
    {
        return currentResult;
    }
    
    public static void ClearResult()
    {
        currentResult = null;
    }
}
