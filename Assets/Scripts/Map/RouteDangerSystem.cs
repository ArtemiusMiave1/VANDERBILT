using UnityEngine;
using System.Collections;

public class RouteDangerSystem : MonoBehaviour
{
    [Header("Danger Event Chances")]
    [Range(0f, 1f)]
    public float dangerLevel1Chance = 0.25f;

    [Range(0f, 1f)]
    public float dangerLevel2Chance = 0.50f;


    [Header("Fragile Cargo")]
    [Range(0f, 1f)]
    [Tooltip("Chance of damaging fragile requests on a dangerous route.")]
    public float dangerousFragileDamageChance = 0.25f;

    [Range(0f, 1f)]
    [Tooltip("Chance of damaging fragile requests on a very dangerous route.")]
    public float veryDangerousFragileDamageChance = 0.50f;


    [Header("Cargo Loss")]
    public int minimumCargoLoss = 5;
    public int maximumCargoLoss = 20;


    [Header("Fuel Loss")]
    public int minimumFuelLoss = 1;
    public int maximumFuelLoss = 5;


    [Header("Ship Incident")]
    public int shipIncidentFuelLoss = 5;


    [Header("Storm")]
    public StormTrigger stormTrigger;


    [Header("Danger Audio")]
    [Tooltip("How quickly both danger sounds fade out.")]
    public float dangerAudioFadeSpeed = 1f;

    [Range(0f, 1f)]
    public float thunderVolume = 1f;

    [Range(0f, 1f)]
    public float dangerAmbienceVolume = 0.5f;


    private ShipCargo shipCargo;

    private SoundManager soundManager;


    // =========================================================
    // DANGER AUDIO SOURCES
    // =========================================================

    private AudioSource thunderSource;

    private AudioSource dangerAmbienceSource;


    // =========================================================
    // AUDIO STATE
    // =========================================================

    private bool dangerAudioPlaying = false;

    private Coroutine dangerAudioFadeCoroutine;


    // =========================================================
    // START
    // =========================================================

    private void Start()
    {
        shipCargo =
            FindObjectOfType<ShipCargo>();


        if (stormTrigger == null)
        {
            stormTrigger =
                FindObjectOfType<StormTrigger>();
        }


        // -----------------------------------------------------
        // FIND SOUND MANAGER
        // -----------------------------------------------------

        GameObject audioObject =
            GameObject.FindGameObjectWithTag("Audio");


        if (audioObject != null)
        {
            soundManager =
                audioObject.GetComponent<SoundManager>();
        }


        // -----------------------------------------------------
        // CREATE THUNDER SOURCE
        // -----------------------------------------------------

        if (soundManager != null)
        {
            thunderSource =
                gameObject.AddComponent<AudioSource>();


            thunderSource.playOnAwake = false;

            thunderSource.loop = true;

            thunderSource.clip =
                soundManager.Thunder;

            thunderSource.volume = 0f;
        }


        // -----------------------------------------------------
        // CREATE DANGER AMBIENCE SOURCE
        // -----------------------------------------------------

        if (soundManager != null)
        {
            dangerAmbienceSource =
                gameObject.AddComponent<AudioSource>();


            dangerAmbienceSource.playOnAwake = false;

            dangerAmbienceSource.loop = true;

            dangerAmbienceSource.clip =
                soundManager.DangerAmbience;

            dangerAmbienceSource.volume = 0f;
        }
    }


    // =========================================================
    // CHECK ROUTE DANGER
    // =========================================================

    public void CheckRouteDanger(
        RouteConnection route
    )
    {
        // -----------------------------------------------------
        // NO ROUTE
        // -----------------------------------------------------

        if (route == null)
        {
            SetStorm(false);

            StopDangerAudio();

            return;
        }


        // -----------------------------------------------------
        // SAFE ROUTE
        // -----------------------------------------------------

        if (route.dangerLevel <= 0)
        {
            SetStorm(false);

            StopDangerAudio();

            return;
        }


        // -----------------------------------------------------
        // DANGEROUS ROUTE
        // -----------------------------------------------------

        SetStorm(true);


        // -----------------------------------------------------
        // START DANGER AUDIO
        // -----------------------------------------------------
        //
        // If the audio is already playing, this does nothing.
        //
        // This is what allows:
        //
        // Danger -> Danger -> Danger
        //
        // without restarting either sound.
        // -----------------------------------------------------

        StartDangerAudio();


        // =====================================================
        // DANGER EVENT
        // =====================================================

        float eventChance = 0f;


        switch (route.dangerLevel)
        {
            case 1:

                eventChance =
                    dangerLevel1Chance;

                break;


            case 2:

                eventChance =
                    dangerLevel2Chance;

                break;
        }


        float roll =
            Random.value;


        if (roll <= eventChance)
        {
            TriggerRandomDangerEvent();
        }


        // =====================================================
        // FRAGILE CARGO
        // =====================================================

        CheckFragileCargo(
            route.dangerLevel
        );
    }


    // =========================================================
    // START DANGER AUDIO
    // =========================================================

    private void StartDangerAudio()
    {
        // -----------------------------------------------------
        // ALREADY PLAYING
        // -----------------------------------------------------
        //
        // Do NOT restart either sound.
        // -----------------------------------------------------

        if (dangerAudioPlaying)
        {
            return;
        }


        // -----------------------------------------------------
        // CANCEL ANY FADE
        // -----------------------------------------------------

        if (dangerAudioFadeCoroutine != null)
        {
            StopCoroutine(
                dangerAudioFadeCoroutine
            );

            dangerAudioFadeCoroutine = null;
        }


        dangerAudioPlaying = true;


        // =====================================================
        // THUNDER
        // =====================================================

        if (
            thunderSource != null &&
            thunderSource.clip != null
        )
        {
            thunderSource.loop = true;

            thunderSource.volume =
                thunderVolume;


            if (!thunderSource.isPlaying)
            {
                thunderSource.Play();
            }
        }


        // =====================================================
        // DANGER AMBIENCE
        // =====================================================

        if (
            dangerAmbienceSource != null &&
            dangerAmbienceSource.clip != null
        )
        {
            dangerAmbienceSource.loop = true;

            dangerAmbienceSource.volume =
                dangerAmbienceVolume;


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
        if (!dangerAudioPlaying)
            return;


        if (dangerAudioFadeCoroutine != null)
        {
            StopCoroutine(
                dangerAudioFadeCoroutine
            );
        }


        dangerAudioFadeCoroutine =
            StartCoroutine(
                FadeDangerAudioOut()
            );
    }


    // =========================================================
    // FADE DANGER AUDIO
    // =========================================================

    private IEnumerator FadeDangerAudioOut()
    {
        float thunderStartVolume = 0f;

        float ambienceStartVolume = 0f;


        if (thunderSource != null)
        {
            thunderStartVolume =
                thunderSource.volume;
        }


        if (dangerAmbienceSource != null)
        {
            ambienceStartVolume =
                dangerAmbienceSource.volume;
        }


        float fadeTime = 0f;


        // -----------------------------------------------------
        // FADE BOTH TOGETHER
        // -----------------------------------------------------

        while (
            fadeTime < 1f
        )
        {
            fadeTime +=
                Time.deltaTime *
                dangerAudioFadeSpeed;


            float fadeAmount =
                Mathf.Clamp01(
                    fadeTime
                );


            // -------------------------------------------------
            // THUNDER
            // -------------------------------------------------

            if (thunderSource != null)
            {
                thunderSource.volume =
                    Mathf.Lerp(
                        thunderStartVolume,
                        0f,
                        fadeAmount
                    );
            }


            // -------------------------------------------------
            // DANGER AMBIENCE
            // -------------------------------------------------

            if (dangerAmbienceSource != null)
            {
                dangerAmbienceSource.volume =
                    Mathf.Lerp(
                        ambienceStartVolume,
                        0f,
                        fadeAmount
                    );
            }


            yield return null;
        }


        // =====================================================
        // STOP THUNDER
        // =====================================================

        if (thunderSource != null)
        {
            thunderSource.volume = 0f;

            thunderSource.Stop();
        }


        // =====================================================
        // STOP DANGER AMBIENCE
        // =====================================================

        if (dangerAmbienceSource != null)
        {
            dangerAmbienceSource.volume = 0f;

            dangerAmbienceSource.Stop();
        }


        dangerAudioPlaying = false;

        dangerAudioFadeCoroutine = null;
    }


    // =========================================================
    // FRAGILE CARGO
    // =========================================================

    private void CheckFragileCargo(
        int dangerLevel
    )
    {
        float damageChance = 0f;


        switch (dangerLevel)
        {
            case 0:

                damageChance = 0f;

                break;


            case 1:

                damageChance =
                    dangerousFragileDamageChance;

                break;


            case 2:

                damageChance =
                    veryDangerousFragileDamageChance;

                break;
        }


        if (damageChance <= 0f)
            return;


        RequestPaper[] requests =
            FindObjectsOfType<RequestPaper>();


        foreach (
            RequestPaper request
            in requests
        )
        {
            if (request == null)
                continue;


            if (!request.IsAccepted())
                continue;


            if (!request.IsFragile())
                continue;


            if (request.IsCompleted())
                continue;


            if (request.IsFragileBroken())
                continue;


            float roll =
                Random.value;


            if (roll <= damageChance)
            {
                request.DamageFragileCargo();


                Debug.Log(
                    "Route danger damaged fragile cargo: " +
                    request.GetRequestTitle()
                );
            }
        }
    }


    // =========================================================
    // RANDOM DANGER EVENT
    // =========================================================

    private void TriggerRandomDangerEvent()
    {
        int eventType =
            Random.Range(
                0,
                3
            );


        switch (eventType)
        {
            case 0:

                LoseRandomCargo();

                break;


            case 1:

                LoseFuel();

                break;


            case 2:

                TriggerShipIncident();

                break;
        }
    }


    // =========================================================
    // CARGO LOSS
    // =========================================================

    private void LoseRandomCargo()
    {
        if (shipCargo == null)
            return;


        string[] cargoTypes =
        {
            "food",
            "medicine",
            "machinery",
            "weapons"
        };


        for (
            int attempt = 0;
            attempt < cargoTypes.Length;
            attempt++
        )
        {
            string resource =
                cargoTypes[
                    Random.Range(
                        0,
                        cargoTypes.Length
                    )
                ];


            int currentAmount =
                shipCargo.GetResourceAmount(
                    resource
                );


            if (currentAmount <= 0)
                continue;


            int amountToLose =
                Random.Range(
                    minimumCargoLoss,
                    maximumCargoLoss + 1
                );


            amountToLose =
                Mathf.Min(
                    amountToLose,
                    currentAmount
                );


            shipCargo.AddOrRemoveResource(
                resource,
                -amountToLose
            );


            Debug.Log(
                "Danger event: Lost " +
                amountToLose +
                " " +
                resource +
                "."
            );


            return;
        }


        Debug.Log(
            "Danger event tried to remove cargo, " +
            "but the ship had no valid cargo."
        );
    }


    // =========================================================
    // FUEL LOSS
    // =========================================================

    private void LoseFuel()
    {
        if (shipCargo == null)
            return;


        int currentFuel =
            shipCargo.GetResourceAmount(
                "fuel"
            );


        if (currentFuel <= 0)
            return;


        int amountToLose =
            Random.Range(
                minimumFuelLoss,
                maximumFuelLoss + 1
            );


        amountToLose =
            Mathf.Min(
                amountToLose,
                currentFuel
            );


        shipCargo.AddOrRemoveResource(
            "fuel",
            -amountToLose
        );


        Debug.Log(
            "Danger event: Lost " +
            amountToLose +
            " fuel."
        );
    }


    // =========================================================
    // SHIP INCIDENT
    // =========================================================

    private void TriggerShipIncident()
    {
        if (shipCargo == null)
            return;


        int currentFuel =
            shipCargo.GetResourceAmount(
                "fuel"
            );


        if (currentFuel <= 0)
            return;


        int amountToLose =
            Mathf.Min(
                shipIncidentFuelLoss,
                currentFuel
            );


        shipCargo.AddOrRemoveResource(
            "fuel",
            -amountToLose
        );


        Debug.Log(
            "Danger event: Ship incident. " +
            "Lost " +
            amountToLose +
            " fuel."
        );
    }


    // =========================================================
    // STORM
    // =========================================================

    public void SetStorm(
        bool active
    )
    {
        if (stormTrigger == null)
            return;


        if (active)
        {
            stormTrigger.ActivateStorm();
        }
        else
        {
            stormTrigger.DeactivateStorm();
        }
    }
}