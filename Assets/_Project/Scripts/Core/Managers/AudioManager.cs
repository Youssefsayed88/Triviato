using UnityEngine;
using System.Collections;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }
    
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
    
    private const float TIMER_VOLUME_MULTIPLIER = 0.5f;
    private Coroutine timerSoundCoroutine;
    
    #region Unity Lifecycle
    
    private void Awake()
    {
        InitializeSingleton();
    }
    
    #endregion
    
    #region Initialization
    
    private void InitializeSingleton()
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
        InitializeMusicSource();
        InitializeSFXSource();
        InitializeTimerSource();
    }
    
    private void InitializeMusicSource()
    {
        if (musicSource == null)
        {
            GameObject musicGO = CreateAudioSourceGameObject("MusicSource");
            musicSource = musicGO.GetComponent<AudioSource>();
            musicSource.loop = true;
            musicSource.volume = musicVolume;
        }
    }
    
    private void InitializeSFXSource()
    {
        if (sfxSource == null)
        {
            GameObject sfxGO = CreateAudioSourceGameObject("SFXSource");
            sfxSource = sfxGO.GetComponent<AudioSource>();
            sfxSource.volume = sfxVolume;
        }
    }
    
    private void InitializeTimerSource()
    {
        if (timerSource == null)
        {
            GameObject timerGO = CreateAudioSourceGameObject("TimerSource");
            timerSource = timerGO.GetComponent<AudioSource>();
            timerSource.volume = sfxVolume * TIMER_VOLUME_MULTIPLIER;
        }
    }
    
    private GameObject CreateAudioSourceGameObject(string name)
    {
        GameObject audioGO = new GameObject(name);
        audioGO.transform.SetParent(transform);
        audioGO.AddComponent<AudioSource>();
        return audioGO;
    }
    
    #endregion
    
    #region Public Audio Methods
    
    public void PlayButtonClick()
    {
        PlaySoundEffect(buttonClickSound, sfxSource);
    }
    
    public void PlayStartGame()
    {
        PlaySoundEffect(startGameSound, sfxSource);
    }
    
    public void PlayWinSound()
    {
        PlaySoundEffect(winSound, sfxSource);
    }
    
    public void PlayLoseSound()
    {
        PlaySoundEffect(loseSound, sfxSource);
    }
    
    public void PlayCorrectAnswer()
    {
        PlaySoundEffect(correctAnswerSound, sfxSource);
    }
    
    public void PlayWrongAnswer()
    {
        PlaySoundEffect(wrongAnswerSound, sfxSource);
    }
    
    #endregion
    
    #region Timer Sound Management
    
    public void StartTimerSound()
    {
        if (!CanPlayTimerSound()) return;
        
        StopTimerSoundCoroutine();
        timerSoundCoroutine = StartCoroutine(TimerSoundCoroutine());
    }
    
    public void StopTimerSound()
    {
        StopTimerSoundCoroutine();
    }
    
    private bool CanPlayTimerSound()
    {
        return enableTimerSound && timerTickSound != null;
    }
    
    private void StopTimerSoundCoroutine()
    {
        if (timerSoundCoroutine != null)
        {
            StopCoroutine(timerSoundCoroutine);
            timerSoundCoroutine = null;
        }
    }
    
    private IEnumerator TimerSoundCoroutine()
    {
        const float TICK_INTERVAL = 1f;
        
        while (true)
        {
            yield return new WaitForSeconds(TICK_INTERVAL);
            PlaySoundEffect(timerTickSound, timerSource);
        }
    }
    
    #endregion
    
    #region Volume Control
    
    public void SetSFXVolume(float volume)
    {
        sfxVolume = Mathf.Clamp01(volume);
        UpdateSFXVolume();
    }
    
    public void SetMusicVolume(float volume)
    {
        musicVolume = Mathf.Clamp01(volume);
        UpdateMusicVolume();
    }
    
    public void SetTimerSoundEnabled(bool enabled)
    {
        enableTimerSound = enabled;
        
        if (!enabled)
        {
            StopTimerSound();
        }
    }
    
    private void UpdateSFXVolume()
    {
        if (sfxSource != null)
        {
            sfxSource.volume = sfxVolume;
        }
        
        if (timerSource != null)
        {
            timerSource.volume = sfxVolume * TIMER_VOLUME_MULTIPLIER;
        }
    }
    
    private void UpdateMusicVolume()
    {
        if (musicSource != null)
        {
            musicSource.volume = musicVolume;
        }
    }
    
    #endregion
    
    #region Getters
    
    public float GetSFXVolume() => sfxVolume;
    public float GetMusicVolume() => musicVolume;
    public bool IsTimerSoundEnabled() => enableTimerSound;
    
    #endregion
    
    #region Private Helper Methods
    
    private void PlaySoundEffect(AudioClip clip, AudioSource source)
    {
        if (clip != null && source != null)
        {
            source.PlayOneShot(clip);
        }
    }
    
    #endregion
}
