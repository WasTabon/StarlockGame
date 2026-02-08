using UnityEngine;

public class GameAudio : MonoBehaviour
{
    public static GameAudio Instance { get; private set; }

    [Header("Audio Sources")]
    [SerializeField] private AudioSource sfxSource;

    [Header("Sound Clips")]
    [SerializeField] private AudioClip tapSound;
    [SerializeField] private AudioClip matchSound;
    [SerializeField] private AudioClip victorySound;
    [SerializeField] private AudioClip gameOverSound;
    [SerializeField] private AudioClip shapeEnterSound;

    [Header("Settings")]
    [SerializeField] [Range(0f, 1f)] private float sfxVolume = 1f;
    [SerializeField] private bool generateSounds = true;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        EnsureAudioSource();

        if (generateSounds)
        {
            GenerateSounds();
        }
    }

    private void EnsureAudioSource()
    {
        if (sfxSource == null)
        {
            sfxSource = gameObject.AddComponent<AudioSource>();
            sfxSource.playOnAwake = false;
        }
    }

    private void GenerateSounds()
    {
        if (tapSound == null)
            tapSound = GenerateBeep(440f, 0.05f, 0.5f);
        
        if (matchSound == null)
            matchSound = GenerateChime(660f, 0.2f, 0.8f);
        
        if (victorySound == null)
            victorySound = GenerateFanfare();
        
        if (gameOverSound == null)
            gameOverSound = GenerateSadSound();
        
        if (shapeEnterSound == null)
            shapeEnterSound = GenerateWhoosh();
    }

    public void PlayTap()
    {
        PlaySound(tapSound);
    }

    public void PlayMatch()
    {
        PlaySound(matchSound);
    }

    public void PlayVictory()
    {
        PlaySound(victorySound);
    }

    public void PlayGameOver()
    {
        PlaySound(gameOverSound);
    }

    public void PlayShapeEnter()
    {
        PlaySound(shapeEnterSound);
    }

    private void PlaySound(AudioClip clip)
    {
        if (clip == null || sfxSource == null) return;
        sfxSource.PlayOneShot(clip, sfxVolume);
    }

    public void SetSfxVolume(float volume)
    {
        sfxVolume = Mathf.Clamp01(volume);
    }

    private AudioClip GenerateBeep(float frequency, float duration, float volume)
    {
        int sampleRate = 44100;
        int sampleCount = (int)(sampleRate * duration);
        float[] samples = new float[sampleCount];

        for (int i = 0; i < sampleCount; i++)
        {
            float t = i / (float)sampleRate;
            float envelope = 1f - (t / duration);
            samples[i] = Mathf.Sin(2f * Mathf.PI * frequency * t) * envelope * volume;
        }

        AudioClip clip = AudioClip.Create("Beep", sampleCount, 1, sampleRate, false);
        clip.SetData(samples, 0);
        return clip;
    }

    private AudioClip GenerateChime(float baseFreq, float duration, float volume)
    {
        int sampleRate = 44100;
        int sampleCount = (int)(sampleRate * duration);
        float[] samples = new float[sampleCount];

        float[] freqs = { baseFreq, baseFreq * 1.5f, baseFreq * 2f };

        for (int i = 0; i < sampleCount; i++)
        {
            float t = i / (float)sampleRate;
            float envelope = Mathf.Pow(1f - (t / duration), 2f);

            float sample = 0f;
            foreach (float f in freqs)
            {
                sample += Mathf.Sin(2f * Mathf.PI * f * t) / freqs.Length;
            }

            samples[i] = sample * envelope * volume;
        }

        AudioClip clip = AudioClip.Create("Chime", sampleCount, 1, sampleRate, false);
        clip.SetData(samples, 0);
        return clip;
    }

    private AudioClip GenerateFanfare()
    {
        int sampleRate = 44100;
        float duration = 0.6f;
        int sampleCount = (int)(sampleRate * duration);
        float[] samples = new float[sampleCount];

        float[] notes = { 523f, 659f, 784f, 1047f };
        float noteDuration = duration / notes.Length;

        for (int i = 0; i < sampleCount; i++)
        {
            float t = i / (float)sampleRate;
            int noteIndex = Mathf.Min((int)(t / noteDuration), notes.Length - 1);
            float noteT = (t % noteDuration) / noteDuration;
            float envelope = 1f - noteT;

            samples[i] = Mathf.Sin(2f * Mathf.PI * notes[noteIndex] * t) * envelope * 0.6f;
        }

        AudioClip clip = AudioClip.Create("Fanfare", sampleCount, 1, sampleRate, false);
        clip.SetData(samples, 0);
        return clip;
    }

    private AudioClip GenerateSadSound()
    {
        int sampleRate = 44100;
        float duration = 0.4f;
        int sampleCount = (int)(sampleRate * duration);
        float[] samples = new float[sampleCount];

        for (int i = 0; i < sampleCount; i++)
        {
            float t = i / (float)sampleRate;
            float freq = 400f - (t / duration) * 200f;
            float envelope = 1f - (t / duration);

            samples[i] = Mathf.Sin(2f * Mathf.PI * freq * t) * envelope * 0.5f;
        }

        AudioClip clip = AudioClip.Create("Sad", sampleCount, 1, sampleRate, false);
        clip.SetData(samples, 0);
        return clip;
    }

    private AudioClip GenerateWhoosh()
    {
        int sampleRate = 44100;
        float duration = 0.15f;
        int sampleCount = (int)(sampleRate * duration);
        float[] samples = new float[sampleCount];

        for (int i = 0; i < sampleCount; i++)
        {
            float t = i / (float)sampleRate;
            float envelope = Mathf.Sin(t / duration * Mathf.PI);
            float noise = Random.Range(-1f, 1f);

            samples[i] = noise * envelope * 0.3f;
        }

        AudioClip clip = AudioClip.Create("Whoosh", sampleCount, 1, sampleRate, false);
        clip.SetData(samples, 0);
        return clip;
    }
}
