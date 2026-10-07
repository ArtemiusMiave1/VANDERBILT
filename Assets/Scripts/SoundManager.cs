using System.Collections;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [Header("Audio Source")]
    [SerializeField]
    private AudioSource SFXSource;


    [Header("Request Sounds")]
    public AudioClip Request;
    public AudioClip RequestComplete;
    public AudioClip Button;
    public AudioClip Lever;


    [Header("Fax Sounds")]
    public AudioClip FaxPrint;
    public AudioClip FaxPurchase;


    [Header("World Sounds")]
    public AudioClip Thunder;
    public AudioClip Landing;
    public AudioClip DangerAmbience;
    public AudioClip Engine;
    public AudioClip Moving;


    [Header("Danger Audio")]
    [Tooltip("How long Thunder and Danger Ambience take to fade out.")]
    public float dangerFadeOutTime = 2f;


    // =========================================================
    // DANGER AUDIO SOURCES
    // =========================================================

    private AudioSource thunderSource;
    private AudioSource dangerAmbienceSource;

    private Coroutine dangerFadeCoroutine;


    // =========================================================
    // START
    // =========================================================

    private void Start()
    {
        CreateDangerAudioSources();
    }


    // =========================================================
    // CREATE DANGER AUDIO SOURCES
    // =========================================================

    private void CreateDangerAudioSources()
    {
        // -----------------------------------------------------
        // THUNDER
        // -----------------------------------------------------

        if (thunderSource == null)
        {
            GameObject thunderObject =
                new GameObject(
                    "Thunder Audio Source"
                );

            thunderObject.transform.SetParent(
                transform
            );

            thunderSource =
                thunderObject.AddComponent<AudioSource>();

            thunderSource.playOnAwake = false;
            thunderSource.loop = true;
            thunderSource.volume = 1f;
        }


        // -----------------------------------------------------
        // DANGER AMBIENCE
        // -----------------------------------------------------

        if (dangerAmbienceSource == null)
        {
            GameObject ambienceObject =
                new GameObject(
                    "Danger Ambience Audio Source"
                );

            ambienceObject.transform.SetParent(
                transform
            );

            dangerAmbienceSource =
                ambienceObject.AddComponent<AudioSource>();

            dangerAmbienceSource.playOnAwake = false;
            dangerAmbienceSource.loop = true;
            dangerAmbienceSource.volume = 1f;
        }
    }


    // =========================================================
    // NORMAL SOUND EFFECTS
    // =========================================================

    public void PlaySFX(
        AudioClip clip
    )
    {
        if (SFXSource == null)
            return;

        if (clip == null)
            return;

        SFXSource.PlayOneShot(
            clip
        );
    }


    // =========================================================
    // START DANGER AUDIO
    // =========================================================

    public void StartDangerAudio()
    {
        if (
            thunderSource == null ||
            dangerAmbienceSource == null
        )
        {
            CreateDangerAudioSources();
        }


        // -----------------------------------------------------
        // CANCEL ANY FADE
        // -----------------------------------------------------

        if (dangerFadeCoroutine != null)
        {
            StopCoroutine(
                dangerFadeCoroutine
            );

            dangerFadeCoroutine = null;
        }


        // -----------------------------------------------------
        // THUNDER
        // -----------------------------------------------------

        if (Thunder != null)
        {
            thunderSource.clip =
                Thunder;

            thunderSource.volume = 1f;


            // IMPORTANT:
            // Don't restart Thunder if it is already playing.

            if (!thunderSource.isPlaying)
            {
                thunderSource.Play();
            }
        }


        // -----------------------------------------------------
        // DANGER AMBIENCE
        // -----------------------------------------------------

        if (DangerAmbience != null)
        {
            dangerAmbienceSource.clip =
                DangerAmbience;

            dangerAmbienceSource.volume = 1f;


            // IMPORTANT:
            // Don't restart Danger Ambience if it is already playing.

            if (!dangerAmbienceSource.isPlaying)
            {
                dangerAmbienceSource.Play();
            }
        }
    }


    // =========================================================
    // STOP DANGER AUDIO
    // =========================================================

    public void StopDangerAudio()
    {
        if (
            thunderSource == null &&
            dangerAmbienceSource == null
        )
        {
            return;
        }


        // -----------------------------------------------------
        // CANCEL EXISTING FADE
        // -----------------------------------------------------

        if (dangerFadeCoroutine != null)
        {
            StopCoroutine(
                dangerFadeCoroutine
            );
        }


        // -----------------------------------------------------
        // START FADE
        // -----------------------------------------------------

        dangerFadeCoroutine =
            StartCoroutine(
                FadeOutDangerAudio()
            );
    }


    // =========================================================
    // FADE OUT BOTH DANGER SOUNDS
    // =========================================================

    private IEnumerator FadeOutDangerAudio()
    {
        float startingThunderVolume =
            thunderSource != null
                ? thunderSource.volume
                : 0f;

        float startingAmbienceVolume =
            dangerAmbienceSource != null
                ? dangerAmbienceSource.volume
                : 0f;


        float timer = 0f;


        // -----------------------------------------------------
        // FADE BOTH TOGETHER
        // -----------------------------------------------------

        while (
            timer <
            dangerFadeOutTime
        )
        {
            timer +=
                Time.deltaTime;


            float percentage =
                Mathf.Clamp01(
                    timer /
                    dangerFadeOutTime
                );


            if (thunderSource != null)
            {
                thunderSource.volume =
                    Mathf.Lerp(
                        startingThunderVolume,
                        0f,
                        percentage
                    );
            }


            if (dangerAmbienceSource != null)
            {
                dangerAmbienceSource.volume =
                    Mathf.Lerp(
                        startingAmbienceVolume,
                        0f,
                        percentage
                    );
            }


            yield return null;
        }


        // -----------------------------------------------------
        // STOP BOTH
        // -----------------------------------------------------

        if (thunderSource != null)
        {
            thunderSource.volume = 0f;
            thunderSource.Stop();
        }


        if (dangerAmbienceSource != null)
        {
            dangerAmbienceSource.volume = 0f;
            dangerAmbienceSource.Stop();
        }


        dangerFadeCoroutine = null;
    }
}
