using UnityEngine;
using System.Collections;
using System;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public Action GameStarted;
    public Action GameEnded;
    public Action GameRestarted;
    public Action OnWinStateEntered;
    public Action OnLoseStateEntered;
    public enum GameState { Start, Quiz, Lose, Win }
    public GameState CurrentState { get; private set; }
    
    [Header("Win State Management")]
    [SerializeField] private WinStateManager winStateManager;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SetState(GameState.Start);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetState(GameState newState)
    {
        Debug.Log("Game State changed to: " + newState);

        CurrentState = newState;

        if (newState == GameState.Lose) 
        {
            OnLoseStateEntered?.Invoke();
            StartLoseSequence();
        }
        else if (newState == GameState.Win)
        {
            OnWinStateEntered?.Invoke();
            StartWinSequence();
        }
    }

    public void StartGame()
    {
        SetState(GameState.Quiz);
        GameStarted?.Invoke();
    }

    public void ShowResult(bool didWin)
    {
        if(didWin) SetState(GameState.Win);
        else SetState(GameState.Lose);
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
    
    private void StartWinSequence()
    {
        if (winStateManager != null)
        {
            winStateManager.StartWinSequence();
        }
        else
        {
            Debug.LogWarning("WinStateManager is not assigned! Win sequence will not start.");
        }
    }
    
    private void StartLoseSequence()
    {
        if (winStateManager != null)
        {
            winStateManager.StartLoseSequence();
        }
        else
        {
            Debug.LogWarning("WinStateManager is not assigned! Lose sequence will not start.");
        }
    }
}