using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A dispatcher that allows code to be invoked on the main Unity thread.
/// Useful for callbacks from background threads or non-Unity threads.
/// </summary>
public class UnityMainThreadDispatcher : MonoBehaviour
{
    private static UnityMainThreadDispatcher _instance;
    private static readonly Queue<Action> _executionQueue = new Queue<Action>();

    /// <summary>
    /// Gets the singleton instance of the dispatcher. Ensure it exists in your scene.
    /// </summary>
    public static UnityMainThreadDispatcher Instance()
    {
        if (_instance == null)
        {
            // Optionally create one if not found (not always recommended)
            var obj = new GameObject("UnityMainThreadDispatcher");
            _instance = obj.AddComponent<UnityMainThreadDispatcher>();
            DontDestroyOnLoad(obj);
        }

        return _instance;
    }

    private void Awake()
    {
        _instance = this;
    }

    /// <summary>
    /// Adds an action to be executed on the main Unity thread at the next Update().
    /// </summary>
    public void Enqueue(Action action)
    {
        if (action == null)
            throw new ArgumentNullException(nameof(action));

        lock (_executionQueue)
        {
            _executionQueue.Enqueue(action);
        }
    }

    private void Update()
    {
        // Execute all actions queued this frame
        Action[] actions = null;
        lock (_executionQueue)
        {
            if (_executionQueue.Count > 0)
            {
                actions = _executionQueue.ToArray();
                _executionQueue.Clear();
            }
        }

        if (actions != null)
        {
            foreach (var action in actions)
            {
                action.Invoke();
            }
        }
    }
}