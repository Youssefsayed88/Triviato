using UnityEngine;
using System.Collections;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;
    
    [Header("Audio Sources")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioSource timerSource;
    
    [Header("Sound Effects")]
    [SerializeField] private AudioClip buttonClickSound;
    [SerializeField] private AudioClip startGameSound;
    [SerializeField] private AudioClip timerTickSound;
    [SerializeField] private AudioClip winSound;
    [SerializeField] private AudioClip loseSound;
    [SerializeField] private AudioClip correctAnswerSound;
    [SerializeField] private AudioClip wrongAnswerSound;
    
    [Header("Settings")]
    [SerializeField] private float sfxVolume = 1f;
    [SerializeField] private float musicVolume = 0.5f;
    [SerializeField] private bool enableTimerSound = true;
    
    private Coroutine timerSoundCoroutine;
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeAudioSources();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    private void InitializeAudioSources()
    {
        // Create audio sources if they don't exist
        if (musicSource == null)
        {
            GameObject musicGO = new GameObject("MusicSource");
            musicGO.transform.SetParent(transform);
            musicSource = musicGO.AddComponent<AudioSource>();
            musicSource.loop = true;
            musicSource.volume = musicVolume;
        }
        
        if (sfxSource == null)
        {
            GameObject sfxGO = new GameObject("SFXSource");
            sfxGO.transform.SetParent(transform);
            sfxSource = sfxGO.AddComponent<AudioSource>();
            sfxSource.volume = sfxVolume;
        }
        
        if (timerSource == null)
        {
            GameObject timerGO = new GameObject("TimerSource");
            timerGO.transform.SetParent(transform);
            timerSource = timerGO.AddComponent<AudioSource>();
            timerSource.volume = sfxVolume * 0.5f; // Timer sound slightly quieter
        }
    }
    
    // Button click sound for any UI button
    public void PlayButtonClick()
    {
        if (buttonClickSound != null)
        {
            sfxSource.PlayOneShot(buttonClickSound);
        }
    }
    
    // Start game sound
    public void PlayStartGame()
    {
        if (startGameSound != null)
        {
            sfxSource.PlayOneShot(startGameSound);
        }
    }
    
    // Timer tick sound (plays every second)
    public void StartTimerSound()
    {
        if (!enableTimerSound || timerTickSound == null) return;
        
        if (timerSoundCoroutine != null)
        {
            StopCoroutine(timerSoundCoroutine);
        }
        
        timerSoundCoroutine = StartCoroutine(TimerSoundCoroutine());
    }
    
    public void StopTimerSound()
    {
        if (timerSoundCoroutine != null)
        {
            StopCoroutine(timerSoundCoroutine);
            timerSoundCoroutine = null;
        }
    }
    
    private IEnumerator TimerSoundCoroutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);
            timerSource.PlayOneShot(timerTickSound);
        }
    }
    
    // Win sound
    public void PlayWinSound()
    {
        if (winSound != null)
        {
            sfxSource.PlayOneShot(winSound);
        }
    }
    
    // Lose sound
    public void PlayLoseSound()
    {
        if (loseSound != null)
        {
            sfxSource.PlayOneShot(loseSound);
        }
    }
    
    // Correct answer sound
    public void PlayCorrectAnswer()
    {
        if (correctAnswerSound != null)
        {
            sfxSource.PlayOneShot(correctAnswerSound);
        }
    }
    
    // Wrong answer sound
    public void PlayWrongAnswer()
    {
        if (wrongAnswerSound != null)
        {
            sfxSource.PlayOneShot(wrongAnswerSound);
        }
    }
    
    // Volume control methods
    public void SetSFXVolume(float volume)
    {
        sfxVolume = Mathf.Clamp01(volume);
        sfxSource.volume = sfxVolume;
        timerSource.volume = sfxVolume * 0.5f;
    }
    
    public void SetMusicVolume(float volume)
    {
        musicVolume = Mathf.Clamp01(volume);
        if (musicSource != null)
        {
            musicSource.volume = musicVolume;
        }
    }
    
    public void SetTimerSoundEnabled(bool enabled)
    {
        enableTimerSound = enabled;
        if (!enabled)
        {
            StopTimerSound();
        }
    }
    
    // Getter methods for volume
    public float GetSFXVolume() => sfxVolume;
    public float GetMusicVolume() => musicVolume;
    public bool IsTimerSoundEnabled() => enableTimerSound;
}
