using UnityEngine;
using System.Collections;
using TMPro;

public class RadioController : MonoBehaviour
{
    [Header("List of Tracks")]
    [SerializeField] private Track[] audioTracks;
    private AudioSource radioAudioSource;
    private int trackIndex;

    [Header("Radio Options")]
    [SerializeField] private bool fadeOutOnSkip = true;
    [SerializeField] private bool fadeInOnPlay = true;
    [SerializeField] private float fadeDuration = 0.5f;
    [SerializeField] private bool loopSameTrack = false;
    [SerializeField] private bool isManuallyStopped = false;

    private Animator radioAnimation;

    private void Start()
    {
        radioAudioSource = GetComponent<AudioSource>();
        radioAnimation = GetComponent<Animator>();

        trackIndex = 0;
        radioAudioSource.clip = audioTracks[trackIndex].trackAudioClip;

        PlayAudio();
    }

    private void Update()
    {
        if(!radioAudioSource.isPlaying && !isManuallyStopped)
        {
            if (loopSameTrack)
            {
                PlayAudio();
            }
            else
            {
                SkipForwardAudio();
            }
        }
    }

    public void PlayAudio()
    {
        if (radioAudioSource != null && !radioAudioSource.isPlaying)
        {
            if (fadeInOnPlay)
            {
                StartCoroutine(FadeIn(radioAudioSource, fadeDuration));
            }
            else
            {
                radioAudioSource.Play();
            }
            PlayButtonAnimation();
            isManuallyStopped = false;
        }
    }

    public void PauseAudio()
    {
        if (radioAudioSource != null && radioAudioSource.isPlaying)
        {
            radioAudioSource.Pause();
            isManuallyStopped = true;
        }
    }

    public void StopAudio()
    {
        if (radioAudioSource != null && radioAudioSource.isPlaying)
        {
            radioAudioSource.Stop();
            isManuallyStopped = true;
        }
    }

    public void SkipForwardAudio()
    {
        if (radioAudioSource != null)
        {
            if (trackIndex < audioTracks.Length - 1) trackIndex++;
            else trackIndex = 0;
            
            if (fadeOutOnSkip)
            {
                StartCoroutine(FadeOut(radioAudioSource, fadeDuration));
            }
            else
            {
                UpdateTrack(trackIndex);
            }
        }
    }

    public void SkipBackwardAudio()
    {

        if (radioAudioSource != null)
        {
            if (trackIndex >= 1) trackIndex--;
            else trackIndex = audioTracks.Length - 1;
           
            if (fadeOutOnSkip)
            {
                StartCoroutine(FadeOut(radioAudioSource, fadeDuration));
            }
            else
            {
                UpdateTrack(trackIndex);
            }
        }
    }

    void UpdateTrack(int index)
    {
        if (trackIndex >= audioTracks.Length)
        {
            trackIndex = 0;
        }
        radioAudioSource.clip = audioTracks[trackIndex].trackAudioClip;

        PlayAudio();
    }

    void PlayButtonAnimation()
    {
        if (radioAnimation != null)
        {
            radioAnimation.SetTrigger("OnButtonPress");
        }
    }

    public IEnumerator FadeOut(AudioSource audioSource, float fadeDuration)
    {
        float startVolume = audioSource.volume;
        while (audioSource.volume > 0)
        {
            audioSource.volume -= startVolume * Time.deltaTime / fadeDuration;
            yield return null;
        }
        audioSource.Stop();
        audioSource.volume = startVolume;
        UpdateTrack(trackIndex);
    }

    public IEnumerator FadeIn(AudioSource audioSource, float fadeInDuration)
    {
        float targetVolume = audioSource.volume;
        audioSource.volume = 0;
        audioSource.Play();

        while (audioSource.volume < targetVolume)
        {
            audioSource.volume += Time.deltaTime / fadeInDuration;
            yield return null;
        }
        audioSource.volume = targetVolume;
    }
}
