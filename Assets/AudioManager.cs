using System.Collections;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    #region Static Instance
    private static AudioManager instance;
    public static AudioManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<AudioManager>();

                if (instance == null)
                {
                    instance = new GameObject("Spawned AudioManager", typeof(AudioManager)).GetComponent<AudioManager>();
                }
            }

            return instance;
        }

        private set
        {
            instance = value;
        }
    }
    #endregion

    #region Fields
    private AudioSource musicSource1;
    private AudioSource musicSource2;
    private AudioSource sfxSource;
    #endregion

    private void Awake()
    {
        // make sure we don't destroy this instance
        DontDestroyOnLoad(gameObject);

        // create and save audio sources
        musicSource1 = gameObject.AddComponent<AudioSource>();
        musicSource2 = gameObject.AddComponent<AudioSource>();
        sfxSource = gameObject.AddComponent<AudioSource>();

        // loop the music
        musicSource1.loop = true;
        musicSource2.loop = true;
    }

    public void PlayMusic(AudioClip musicClip)
    {
        AudioSource activeSource = musicSource1.isPlaying ? musicSource1 : musicSource2;

        activeSource.clip = musicClip;
        activeSource.volume = 1;
        activeSource.Play();
    }

    public void PlayMusicWithFade(AudioClip newClip, float transitionTime = 1.0f)
    {
        AudioSource activeSource = musicSource1.isPlaying ? musicSource1 : musicSource2;
        StartCoroutine(UpdateMusicWithFade(activeSource, newClip, transitionTime));
    }

    public void PlayMusicWithCrossFade(AudioClip musicClip, float transitionTime = 1.0f)
    {
        AudioSource activeSource = musicSource1.isPlaying ? musicSource1 : musicSource2;
        AudioSource newSource = musicSource1.isPlaying ? musicSource2 : musicSource1;

        activeSource.clip = musicClip;
        activeSource.Play();

        StartCoroutine(UpdateMusicWithCrossFade(activeSource, newSource, transitionTime));
    }

    private IEnumerator UpdateMusicWithFade(AudioSource activeSource, AudioClip newClip, float transitionTime)
    {
        if (!activeSource.isPlaying)
            activeSource.Play();

        float t;

        // fade out
        for (t = 0; t < transitionTime; t += Time.deltaTime)
        {
            activeSource.volume = (1 - (t / transitionTime));
            yield return null;
        }

        activeSource.Stop();
        activeSource.clip = newClip;
        activeSource.Play();

        // fade in
        for (t = 0; t < transitionTime; t += Time.deltaTime)
        {
            activeSource.volume = (t / transitionTime);
            yield return null;
        }
    }

    private IEnumerator UpdateMusicWithCrossFade(AudioSource activeSource, AudioSource newSource, float transitionTime)
    {
        float t;

        for (t = 0; t < transitionTime; t += Time.deltaTime)
        {
            activeSource.volume = (1 - (t / transitionTime));
            newSource.volume = (t / transitionTime);
            yield return null;
        }

        activeSource.Stop();
    }

    public void PlaySfx(AudioClip audioClip)
    {
        sfxSource.PlayOneShot(audioClip);
    }

    public void PlaySfx(AudioClip audioClip, float volume)
    {
        sfxSource.PlayOneShot(audioClip, volume);
    }

    public void SetMusicVolume(float volume)
    {
        musicSource1.volume = volume;
        musicSource2.volume = volume;
    }

    public void SetSfxVolume(float volume)
    {
        sfxSource.volume = volume;
    }
}
