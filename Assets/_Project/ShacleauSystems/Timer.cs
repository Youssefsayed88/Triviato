using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace ShacleauSystems
{
    public class Timer : MonoBehaviour
    {
        [Tooltip("Total time in seconds for the countdown.")]
        public int GameTimer = 60;

        private int currentTimer;

        [Tooltip("Invoked with the current time string (mm:ss) every tick.")]
        public UnityEvent<string> timer;

        [Tooltip("Invoked when the timer finishes (reaches zero).")]
        public UnityEvent TimerFinished;

        private Coroutine timerCoroutine;

        private void Start()
        {
            // Load the configured timer value if available
            if (Config.Instance != null)
            {
                GameTimer = Config.Instance.settings["Timer"].GetValueOrDefault(30);
            }
        }

        /// <summary>
        /// Starts or restarts the timer.
        /// </summary>
        public void StartTimer()
        {
            // Stop any running timer coroutine before starting a new one
            if (timerCoroutine != null)
            {
                StopCoroutine(timerCoroutine);
            }
            
            // Load the configured timer value if available
            if (Config.Instance != null)
            {
                GameTimer = Config.Instance.settings["Timer"].GetValueOrDefault(30);
            }

            currentTimer = GameTimer;
            // Invoke immediately with the initial time
            timer.Invoke(GetTimeFormat());

            // Start the countdown
            timerCoroutine = StartCoroutine(TimerRoutine());
        }

        /// <summary>
        /// The routine that counts down every second.
        /// </summary>
        private IEnumerator TimerRoutine()
        {
            // Run while we still have time left
            while (currentTimer > 0)
            {
                // Wait one second
                yield return new WaitForSeconds(1f);
                // Decrement the current time
                currentTimer--;
                // Invoke updated time
                timer.Invoke(GetTimeFormat());
            }

            // Once the loop finishes, the timer reached zero
            TimerFinished.Invoke();
        }

        /// <summary>
        /// Returns the current time in "mm:ss" format.
        /// </summary>
        private string GetTimeFormat()
        {
            TimeSpan timeSpan = TimeSpan.FromSeconds(currentTimer);
            return string.Format("{0:00}:{1:00}", timeSpan.Minutes, timeSpan.Seconds);
        }
    }
}
