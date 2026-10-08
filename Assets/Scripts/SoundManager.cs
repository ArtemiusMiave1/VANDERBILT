using System.Collections;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [Header("Audio Source")]
    [SerializeField]
    private AudioSource SFXSource;


    [Header("Request Sounds")]
    public AudioClip Request;
    public AudioClip RequestAccepted;
    public AudioClip RequestComplete;
    public AudioClip Button;
    public AudioClip Lever;
    public AudioClip RequestDeliveryHatch;


    [Header("Fax Sounds")]
    public AudioClip FaxPrint;
    public AudioClip FaxPurchase;


    [Header("World Sounds")]
    public AudioClip Thunder;
    public AudioClip Landing;
    public AudioClip DangerAmbience;
    public AudioClip Moving;
    public AudioClip StopMoving;
    


    [Header("Danger Audio")]
    [Tooltip("How long Thunder and Danger Ambience take to fade out.")]
    public float dangerFadeOutTime = 2f;


    [Header("Moving Audio")]
    [Tooltip("How long the Moving sound takes to fade out when the ship stops.")]
    public float movingFadeOutTime = 0.5f;


    // =========================================================
    // DANGER AUDIO SOURCES
    // =========================================================

    private AudioSource thunderSource;
    private AudioSource dangerAmbienceSource;


    // =========================================================
    // MOVING AUDIO SOURCE
    // =========================================================

    private AudioSource movingSource;


    private Coroutine dangerFadeCoroutine;
    private Coroutine movingFadeCoroutine;


    // =========================================================
    // START
    // =========================================================

    private void Start()
    {
        CreateDangerAudioSources();
        CreateMovingAudioSource();
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
    // CREATE MOVING AUDIO SOURCE
    // =========================================================

    private void CreateMovingAudioSource()
    {
        if (movingSource != null)
            return;


        GameObject movingObject =
            new GameObject(
                "Moving Audio Source"
            );


        movingObject.transform.SetParent(
            transform
        );


        movingSource =
            movingObject.AddComponent<AudioSource>();


        movingSource.playOnAwake = false;
        movingSource.loop = true;
        movingSource.volume = 1f;
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
    // START MOVING AUDIO
    // =========================================================

    public void StartMovingAudio()
    {
        if (movingSource == null)
        {
            CreateMovingAudioSource();
        }


        if (Moving == null)
            return;


        // -----------------------------------------------------
        // CANCEL FADE IF SHIP STARTS MOVING AGAIN
        // -----------------------------------------------------

        if (movingFadeCoroutine != null)
        {
            StopCoroutine(
                movingFadeCoroutine
            );

            movingFadeCoroutine = null;
        }


        // -----------------------------------------------------
        // SETUP MOVING AUDIO
        // -----------------------------------------------------

        movingSource.clip =
            Moving;

        movingSource.loop = true;


        // Restore full volume.

        movingSource.volume = 1f;


        // -----------------------------------------------------
        // DON'T RESTART IF ALREADY PLAYING
        // -----------------------------------------------------

        if (!movingSource.isPlaying)
        {
            movingSource.Play();
        }
    }


    // =========================================================
    // STOP MOVING AUDIO
    // =========================================================

    public void StopMovingAudio()
    {
        if (movingSource == null)
            return;


        if (!movingSource.isPlaying)
            return;


        // -----------------------------------------------------
        // CANCEL EXISTING FADE
        // -----------------------------------------------------

        if (movingFadeCoroutine != null)
        {
            StopCoroutine(
                movingFadeCoroutine
            );
        }


        // -----------------------------------------------------
        // START FADE
        // -----------------------------------------------------

        movingFadeCoroutine =
            StartCoroutine(
                FadeOutMovingAudio()
            );
    }


    // =========================================================
    // FADE OUT MOVING AUDIO
    // =========================================================

    private IEnumerator FadeOutMovingAudio()
    {
        float startingVolume =
            movingSource != null
                ? movingSource.volume
                : 0f;


        float timer = 0f;


        // -----------------------------------------------------
        // FADE OUT
        // -----------------------------------------------------

        while (
            timer <
            movingFadeOutTime
        )
        {
            if (movingSource == null)
                yield break;


            timer +=
                Time.deltaTime;


            float percentage;


            if (movingFadeOutTime <= 0f)
            {
                percentage = 1f;
            }
            else
            {
                percentage =
                    Mathf.Clamp01(
                        timer /
                        movingFadeOutTime
                    );
            }


            movingSource.volume =
                Mathf.Lerp(
                    startingVolume,
                    0f,
                    percentage
                );


            yield return null;
        }


        // -----------------------------------------------------
        // STOP AFTER FADE
        // -----------------------------------------------------

        if (movingSource != null)
        {
            movingSource.volume = 0f;
            movingSource.Stop();
            movingSource.volume = 1f;
        }


        movingFadeCoroutine = null;
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