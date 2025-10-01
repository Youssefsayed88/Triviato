using System.Collections.Generic;
using System;

/// <summary>
/// Represents a single trivia question.
/// </summary>
[Serializable]
public class Question
{
    public string question;
    public List<string> options;
    public string correct_answer;
}