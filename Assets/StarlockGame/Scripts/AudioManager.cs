using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioSource musicSource;

    [SerializeField] private AudioClip buttonClickSound;
    [SerializeField] private AudioClip buttonHoverSound;
    [SerializeField] private AudioClip panelOpenSound;
    [SerializeField] private AudioClip panelCloseSound;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        transform.SetParent(null);
        DontDestroyOnLoad(gameObject);

        if (sfxSource == null)
        {
            sfxSource = gameObject.AddComponent<AudioSource>();
            sfxSource.playOnAwake = false;
        }

        if (musicSource == null)
        {
            musicSource = gameObject.AddComponent<AudioSource>();
            musicSource.playOnAwake = false;
            musicSource.loop = true;
        }

        LoadSounds();
    }

    private void LoadSounds()
    {
        if (AddressableAssetService.Instance == null) return;

        AudioClip loaded;

        loaded = AddressableAssetService.Instance.GetAudioClip("Audio/UI/button_click");
        if (loaded != null) buttonClickSound = loaded;

        loaded = AddressableAssetService.Instance.GetAudioClip("Audio/UI/button_hover");
        if (loaded != null) buttonHoverSound = loaded;

        loaded = AddressableAssetService.Instance.GetAudioClip("Audio/UI/panel_open");
        if (loaded != null) panelOpenSound = loaded;

        loaded = AddressableAssetService.Instance.GetAudioClip("Audio/UI/panel_close");
        if (loaded != null) panelCloseSound = loaded;
    }

    public void PlayButtonClick()
    {
        PlaySFX(buttonClickSound);
    }

    public void PlayButtonHover()
    {
        PlaySFX(buttonHoverSound);
    }

    public void PlayPanelOpen()
    {
        PlaySFX(panelOpenSound);
    }

    public void PlayPanelClose()
    {
        PlaySFX(panelCloseSound);
    }

    public void PlaySFX(AudioClip clip)
    {
        if (clip == null) return;
        if (GameManager.Instance != null && !GameManager.Instance.SoundEnabled) return;

        sfxSource.PlayOneShot(clip);
    }

    public void PlayMusic(AudioClip clip)
    {
        if (clip == null) return;

        musicSource.clip = clip;
        musicSource.Play();
    }

    public void StopMusic()
    {
        musicSource.Stop();
    }

    public void SetMusicVolume(float volume)
    {
        musicSource.volume = volume;
    }

    public void SetSFXVolume(float volume)
    {
        sfxSource.volume = volume;
    }
}
