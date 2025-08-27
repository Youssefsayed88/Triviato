using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuizController : MonoBehaviour
{
    public TextAsset quizJson; // assign in inspector
    private Quiz quizData;

    private void Start()
    {
        quizData = QuizLoader.LoadQuizFromJson(quizJson);

        Debug.Log($"Loaded {quizData.questions.Count} questions");
        Debug.Log($"First question: {quizData.questions[0].question}");
    }
}
