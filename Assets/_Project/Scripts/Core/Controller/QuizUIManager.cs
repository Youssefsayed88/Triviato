using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class QuizUIManager : MonoBehaviour
{
    [Header("Question UI")]
    [SerializeField] private TMP_Text questionText;
    [SerializeField] private TMP_Text questionCounterText;
    
    [Header("Choice System")]
    [SerializeField] private GameObject choicePrefab;
    [SerializeField] private Transform choiceGridLayout;
    
    [Header("Question Progress System")]
    [SerializeField] private GameObject questionProgressPrefab;
    [SerializeField] private Transform questionProgressContainer;
    
    [Header("Particle Effects")]
    [SerializeField] private ParticleSystem correctAnswerParticles;
    [SerializeField] private ParticleSystem wrongAnswerParticles;
    
    private List<ChoiceController> currentChoices = new List<ChoiceController>();
    private List<QuestionProgressController> questionProgressIndicators = new List<QuestionProgressController>();
    
    private const string QUESTION_COUNTER_FORMAT = "#{0}";
    
    public void InitializeQuestionProgress(int totalQuestions)
    {
        ClearQuestionProgressIndicators();
        
        for (int i = 0; i < totalQuestions; i++)
        {
            CreateQuestionProgressIndicator(i);
        }
    }
    
    public void DisplayQuestion(Question question, int questionIndex)
    {
        questionText.text = question.question;
        questionCounterText.text = string.Format(QUESTION_COUNTER_FORMAT, questionIndex + 1);
    }
    
    public void CreateChoiceButtons(Question question, System.Action<string, bool> onChoiceSelected)
    {
        List<string> randomizedOptions = RandomizeOptions(question.options);
        
        foreach (string option in randomizedOptions)
        {
            GameObject choiceGO = Instantiate(choicePrefab, choiceGridLayout);
            ChoiceController choiceController = choiceGO.GetComponent<ChoiceController>();
            
            if (choiceController != null)
            {
                bool isCorrect = option == question.correct_answer;
                choiceController.Initialize(option, isCorrect, onChoiceSelected);
                currentChoices.Add(choiceController);
            }
        }
    }
    
    public void ClearChoices()
    {
        foreach (ChoiceController choice in currentChoices)
        {
            if (choice != null)
            {
                Destroy(choice.gameObject);
            }
        }
        currentChoices.Clear();
    }
    
    public void UpdateQuestionProgress(int questionIndex, bool isCorrect)
    {
        if (questionIndex < questionProgressIndicators.Count)
        {
            QuestionProgressController indicator = questionProgressIndicators[questionIndex];
            if (indicator != null)
            {
                if (isCorrect)
                {
                    indicator.SetAsCorrect();
                }
                else
                {
                    indicator.SetAsWrong();
                }
            }
        }
    }
    
    public void ShowAnswerFeedback(string selectedAnswer, bool isCorrect)
    {
        foreach (var choice in currentChoices)
        {
            if (choice.GetChoiceText() == selectedAnswer)
            {
                SetChoiceVisualState(choice, isCorrect);
            }
            else if (choice.IsCorrectAnswer())
            {
                choice.ShowCorrectAnswer();
            }
            
            choice.DisableInteraction();
        }
    }
    
    public void ShowCorrectAnswerForTimeUp()
    {
        foreach (var choice in currentChoices)
        {
            if (choice.IsCorrectAnswer())
            {
                choice.ShowCorrectAnswer();
            }
            choice.DisableInteraction();
        }
    }
    
    public void PlayAnswerEffects(bool isCorrect)
    {
        if (isCorrect)
        {
            correctAnswerParticles.Play();
        }
        else
        {
            wrongAnswerParticles.Play();
        }
    }
    
    public void ClearAllUI()
    {
        ClearChoices();
        ClearQuestionProgressIndicators();
    }
    
    private List<string> RandomizeOptions(List<string> options)
    {
        List<string> randomizedOptions = new List<string>(options);
        for (int i = randomizedOptions.Count - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);
            string temp = randomizedOptions[i];
            randomizedOptions[i] = randomizedOptions[randomIndex];
            randomizedOptions[randomIndex] = temp;
        }
        return randomizedOptions;
    }
    
    private void CreateQuestionProgressIndicator(int questionIndex)
    {
        GameObject progressGO = Instantiate(questionProgressPrefab, questionProgressContainer);
        QuestionProgressController progressController = progressGO.GetComponent<QuestionProgressController>();
        
        if (progressController != null)
        {
            progressController.Initialize(questionIndex);
            questionProgressIndicators.Add(progressController);
        }
    }
    
    private void ClearQuestionProgressIndicators()
    {
        foreach (QuestionProgressController indicator in questionProgressIndicators)
        {
            if (indicator != null)
            {
                Destroy(indicator.gameObject);
            }
        }
        questionProgressIndicators.Clear();
    }
    
    private void SetChoiceVisualState(ChoiceController choice, bool isCorrect)
    {
        if (isCorrect)
        {
            choice.SetAsCorrect();
        }
        else
        {
            choice.SetAsWrong();
        }
    }
}
