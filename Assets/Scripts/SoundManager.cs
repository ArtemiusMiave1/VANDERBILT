using System.Collections;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [Header("Audio Source")]
    [SerializeField] private AudioSource SFXSource;


    [Header("Request Sounds")]
    public AudioClip Request;
    public AudioClip RequestAccepted;
    public AudioClip RequestComplete;
    public AudioClip Button;
    public AudioClip Lever;
    public AudioClip RequestDeliveryHatch;

    [Header("Request Sound Volumes")]
    [Range(0f, 1f)] public float RequestVolume = 1f;
    [Range(0f, 1f)] public float RequestAcceptedVolume = 1f;
    [Range(0f, 1f)] public float RequestCompleteVolume = 1f;
    [Range(0f, 1f)] public float ButtonVolume = 1f;
    [Range(0f, 1f)] public float LeverVolume = 1f;
    [Range(0f, 1f)] public float RequestDeliveryHatchVolume = 1f;


    [Header("Fax Sounds")]
    public AudioClip FaxPrint;
    public AudioClip FaxPurchase;

    [Header("Fax Sound Volumes")]
    [Range(0f, 1f)] public float FaxPrintVolume = 1f;
    [Range(0f, 1f)] public float FaxPurchaseVolume = 1f;


    [Header("World Sounds")]
    public AudioClip Thunder;
    public AudioClip Landing;
    public AudioClip DangerAmbience;
    public AudioClip Moving;
    public AudioClip StopMoving;

    [Header("World Sound Volumes")]
    [Range(0f, 1f)] public float ThunderVolume = 1f;
    [Range(0f, 1f)] public float LandingVolume = 1f;
    [Range(0f, 1f)] public float DangerAmbienceVolume = 1f;
    [Range(0f, 1f)] public float MovingVolume = 1f;
    [Range(0f, 1f)] public float StopMovingVolume = 1f;


    [Header("Danger Audio")]
    [Tooltip("How long Thunder and Danger Ambience take to fade out.")]
    public float dangerFadeOutTime = 2f;

    [Header("Moving Audio")]
    [Tooltip("How long the Moving sound takes to fade out when the ship stops.")]
    public float movingFadeOutTime = 0.5f;


    // =========================================================
    // AUDIO SOURCES
    // =========================================================

    private AudioSource thunderSource;
    private AudioSource dangerAmbienceSource;
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
        if (thunderSource == null)
        {
            GameObject thunderObject =
                new GameObject("Thunder Audio Source");

            thunderObject.transform.SetParent(transform);

            thunderSource =
                thunderObject.AddComponent<AudioSource>();

            thunderSource.playOnAwake = false;
            thunderSource.loop = true;
            thunderSource.volume = ThunderVolume;
        }

        if (dangerAmbienceSource == null)
        {
            GameObject ambienceObject =
                new GameObject("Danger Ambience Audio Source");

            ambienceObject.transform.SetParent(transform);

            dangerAmbienceSource =
                ambienceObject.AddComponent<AudioSource>();

            dangerAmbienceSource.playOnAwake = false;
            dangerAmbienceSource.loop = true;
            dangerAmbienceSource.volume = DangerAmbienceVolume;
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
            new GameObject("Moving Audio Source");

        movingObject.transform.SetParent(transform);

        movingSource =
            movingObject.AddComponent<AudioSource>();

        movingSource.playOnAwake = false;
        movingSource.loop = true;
        movingSource.volume = MovingVolume;
    }


    // =========================================================
    // NORMAL SOUND EFFECTS
    // =========================================================

    public void PlaySFX(AudioClip clip)
    {
        if (SFXSource == null || clip == null)
            return;

        float volume = 1f;

        if (clip == Request)
            volume = RequestVolume;
        else if (clip == RequestAccepted)
            volume = RequestAcceptedVolume;
        else if (clip == RequestComplete)
            volume = RequestCompleteVolume;
        else if (clip == Button)
            volume = ButtonVolume;
        else if (clip == Lever)
            volume = LeverVolume;
        else if (clip == RequestDeliveryHatch)
            volume = RequestDeliveryHatchVolume;
        else if (clip == FaxPrint)
            volume = FaxPrintVolume;
        else if (clip == FaxPurchase)
            volume = FaxPurchaseVolume;
        else if (clip == Landing)
            volume = LandingVolume;
        else if (clip == StopMoving)
            volume = StopMovingVolume;

        SFXSource.PlayOneShot(clip, volume);
    }


    // =========================================================
    // START MOVING AUDIO
    // =========================================================

    public void StartMovingAudio()
    {
        if (movingSource == null)
            CreateMovingAudioSource();

        if (Moving == null)
            return;

        // Cancel fade if the ship starts moving again.
        if (movingFadeCoroutine != null)
        {
            StopCoroutine(movingFadeCoroutine);
            movingFadeCoroutine = null;
        }

        movingSource.clip = Moving;
        movingSource.loop = true;
        movingSource.volume = MovingVolume;

        if (!movingSource.isPlaying)
            movingSource.Play();
    }


    // =========================================================
    // STOP MOVING AUDIO
    // =========================================================

    public void StopMovingAudio()
    {
        if (movingSource == null || !movingSource.isPlaying)
            return;

        if (movingFadeCoroutine != null)
            StopCoroutine(movingFadeCoroutine);

        movingFadeCoroutine =
            StartCoroutine(FadeOutMovingAudio());
    }


    // =========================================================
    // FADE OUT MOVING AUDIO
    // =========================================================

    private IEnumerator FadeOutMovingAudio()
    {
        float startingVolume = movingSource != null
            ? movingSource.volume
            : 0f;

        float timer = 0f;

        while (timer < movingFadeOutTime)
        {
            if (movingSource == null)
            {
                movingFadeCoroutine = null;
                yield break;
            }

            timer += Time.deltaTime;

            float percentage = movingFadeOutTime <= 0f
                ? 1f
                : Mathf.Clamp01(timer / movingFadeOutTime);

            movingSource.volume = Mathf.Lerp(
                startingVolume,
                0f,
                percentage);

            yield return null;
        }

        if (movingSource != null)
        {
            movingSource.volume = 0f;
            movingSource.Stop();
            movingSource.volume = MovingVolume;
        }

        movingFadeCoroutine = null;
    }


    // =========================================================
    // START DANGER AUDIO
    // =========================================================

    public void StartDangerAudio()
    {
        if (thunderSource == null ||
            dangerAmbienceSource == null)
        {
            CreateDangerAudioSources();
        }

        // Cancel any fade.
        if (dangerFadeCoroutine != null)
        {
            StopCoroutine(dangerFadeCoroutine);
            dangerFadeCoroutine = null;
        }

        // Thunder.
        if (Thunder != null)
        {
            thunderSource.clip = Thunder;
            thunderSource.volume = ThunderVolume;

            if (!thunderSource.isPlaying)
                thunderSource.Play();
        }

        // Danger ambience.
        if (DangerAmbience != null)
        {
            dangerAmbienceSource.clip = DangerAmbience;
            dangerAmbienceSource.volume = DangerAmbienceVolume;

            if (!dangerAmbienceSource.isPlaying)
                dangerAmbienceSource.Play();
        }
    }


    // =========================================================
    // STOP DANGER AUDIO
    // =========================================================

    public void StopDangerAudio()
    {
        if (thunderSource == null &&
            dangerAmbienceSource == null)
        {
            return;
        }

        if (dangerFadeCoroutine != null)
            StopCoroutine(dangerFadeCoroutine);

        dangerFadeCoroutine =
            StartCoroutine(FadeOutDangerAudio());
    }


    // =========================================================
    // FADE OUT BOTH DANGER SOUNDS
    // =========================================================

    private IEnumerator FadeOutDangerAudio()
    {
        float startingThunderVolume = thunderSource != null
            ? thunderSource.volume
            : 0f;

        float startingAmbienceVolume = dangerAmbienceSource != null
            ? dangerAmbienceSource.volume
            : 0f;

        float timer = 0f;

        while (timer < dangerFadeOutTime)
        {
            timer += Time.deltaTime;

            float percentage = dangerFadeOutTime <= 0f
                ? 1f
                : Mathf.Clamp01(timer / dangerFadeOutTime);

            if (thunderSource != null)
            {
                thunderSource.volume = Mathf.Lerp(
                    startingThunderVolume,
                    0f,
                    percentage);
            }

            if (dangerAmbienceSource != null)
            {
                dangerAmbienceSource.volume = Mathf.Lerp(
                    startingAmbienceVolume,
                    0f,
                    percentage);
            }

            yield return null;
        }

        if (thunderSource != null)
        {
            thunderSource.volume = 0f;
            thunderSource.Stop();
            thunderSource.volume = ThunderVolume;
        }

        if (dangerAmbienceSource != null)
        {
            dangerAmbienceSource.volume = 0f;
            dangerAmbienceSource.Stop();
            dangerAmbienceSource.volume = DangerAmbienceVolume;
        }

        dangerFadeCoroutine = null;
    }


    // =========================================================
    // UPDATE VOLUMES DURING PLAY MODE
    // =========================================================

    private void Update()
    {
        // Update looping audio volumes without overriding fades.
        if (movingSource != null &&
            movingSource.isPlaying &&
            movingFadeCoroutine == null)
        {
            movingSource.volume = MovingVolume;
        }

        if (thunderSource != null &&
            thunderSource.isPlaying &&
            dangerFadeCoroutine == null)
        {
            thunderSource.volume = ThunderVolume;
        }

        if (dangerAmbienceSource != null &&
            dangerAmbienceSource.isPlaying &&
            dangerFadeCoroutine == null)
        {
            dangerAmbienceSource.volume = DangerAmbienceVolume;
        }
    }
}