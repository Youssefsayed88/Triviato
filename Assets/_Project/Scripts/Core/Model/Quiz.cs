using System.Collections.Generic;
using System;

/// <summary>
/// Represents a collection of questions (a quiz).
/// </summary>
[Serializable]
public class Quiz
{
    public List<Question> questions;
}