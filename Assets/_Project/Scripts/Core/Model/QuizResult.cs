/// <summary>
/// Represents the result after finishing a quiz.
/// </summary>
public class QuizResult
{
    public int totalQuestions;
    public int correctAnswers;

    public bool Passed => correctAnswers > (totalQuestions / 2);

    public float ScorePercentage => totalQuestions > 0
        ? (float)correctAnswers / totalQuestions * 100f
        : 0f;
}
