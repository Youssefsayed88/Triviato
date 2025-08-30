using System.Collections.Generic;
using UnityEngine;

public class QuizStateManager
{
    private Quiz quizData;
    private List<Question> randomizedQuestions = new();
    private int currentQuestionIndex = 0;
    private int correctAnswers = 0;
    private bool isAnswering = false;
    
    public int CurrentQuestionIndex => currentQuestionIndex;
    public int CorrectAnswers => correctAnswers;
    public bool IsAnswering => isAnswering;
    public int TotalQuestions => randomizedQuestions.Count;
    public bool IsQuizComplete => currentQuestionIndex >= randomizedQuestions.Count;
    
    public void Initialize(Quiz quizData)
    {
        this.quizData = quizData;
        RandomizeQuestions();
        ResetState();
    }
    
    public void ResetState()
    {
        currentQuestionIndex = 0;
        correctAnswers = 0;
        isAnswering = false;
    }
    
    public Question GetCurrentQuestion()
    {
        return randomizedQuestions[currentQuestionIndex];
    }
    
    public void SetAnswering(bool answering)
    {
        isAnswering = answering;
    }
    
    public void IncrementCorrectAnswers()
    {
        correctAnswers++;
    }
    
    public void MoveToNextQuestion()
    {
        currentQuestionIndex++;
    }
    
    public float GetWinPercentage()
    {
        return (float)correctAnswers / randomizedQuestions.Count;
    }
    
    private void RandomizeQuestions()
    {
        randomizedQuestions = new List<Question>(quizData.questions);
        for (int i = randomizedQuestions.Count - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);
            var temp = randomizedQuestions[i];
            randomizedQuestions[i] = randomizedQuestions[randomIndex];
            randomizedQuestions[randomIndex] = temp;
        }
    }
}
