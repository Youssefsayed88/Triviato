using UnityEngine;
using System.Collections;
using System;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public Action GameStarted;
    public Action GameEnded;
    public Action GameRestarted;
    public enum GameState { Start, Quiz, Result }
    public GameState CurrentState { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            //DontDestroyOnLoad(gameObject);
            SetState(GameState.Start);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetState(GameState newState)
    {
        CurrentState = newState;

        if (newState == GameState.Result) StartCoroutine(AutoRestartAfterDelay(10f));
    }

    public void StartGame()
    {
        SetState(GameState.Quiz);
        GameStarted?.Invoke();
    }

    public void ShowResult()
    {
        SetState(GameState.Result);
        GameEnded?.Invoke();
    }

    private IEnumerator AutoRestartAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        RestartGame();
        GameRestarted?.Invoke();
    }

    public void RestartGame()
    {
        SetState(GameState.Start);
        GameRestarted?.Invoke();
    }
}