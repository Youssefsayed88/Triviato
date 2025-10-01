using UnityEngine;
using System.Collections;

public static class UIFader
{
    private class FaderRunner : MonoBehaviour { } // helper for running coroutines
    private static FaderRunner runner;

    private static void InitRunner()
    {
        if (runner == null)
        {
            var go = new GameObject("UIFaderRunner");
            Object.DontDestroyOnLoad(go);
            runner = go.AddComponent<FaderRunner>();
        }
    }

    public static void FadeIn(CanvasGroup canvasGroup, float duration = 1f)
    {
        StartFade(canvasGroup, 1f, duration);
    }

    public static void FadeOut(CanvasGroup canvasGroup, float duration = 1f)
    {
        StartFade(canvasGroup, 0f, duration);
    }

    private static void StartFade(CanvasGroup canvasGroup, float targetAlpha, float duration)
    {
        InitRunner();
        runner.StartCoroutine(FadeRoutine(canvasGroup, targetAlpha, duration));
    }

    private static IEnumerator FadeRoutine(CanvasGroup canvasGroup, float targetAlpha, float duration)
    {
        float startAlpha = canvasGroup.alpha;
        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, time / duration);
            yield return null;
        }

        canvasGroup.alpha = targetAlpha;

        // Disable interaction if fully invisible
        canvasGroup.interactable = targetAlpha > 0.9f;
        canvasGroup.blocksRaycasts = targetAlpha > 0.9f;
    }
}
