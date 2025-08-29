using UnityEngine;

public static class QuizLoader
{
    public static Quiz LoadQuizFromJson(TextAsset jsonFile)
    {
        return JsonUtility.FromJson<Quiz>(jsonFile.text);
    }
}