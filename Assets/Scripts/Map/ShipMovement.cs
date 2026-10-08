using System.Collections;
using UnityEngine;

public class ShipMovement : MonoBehaviour
{
    [Header("Movement")]
    public float speed = 5f;

    [Header("Speed Control")]
    [Tooltip("Physical lever that controls the ship's speed.")]
    public TestLever speedControl;

    [Header("Current Movement Stats")]
    public float currentSpeed;
    public float currentCargoWeight;

    [Range(0, 5)]
    public int currentSpeedLevel = 0;

    [Header("Locations")]
    public Location currentLocation;
    public Location targetLocation;

    [Header("Route System")]
    public ShipRouteSystem shipRouteSystem;

    [Header("Route Danger")]
    public RouteDangerSystem routeDangerSystem;

    [Header("Resource Depot UI")]
    public GameObject resourceDepotPaper;

    [Header("Fuel")]
    [Tooltip("Current fuel cost based on the throttle level.")]
    public int currentFuelCost = 0;

    [Header("Cargo Weight")]
    public float maximumCargoWeight = 750f;

    [Tooltip("Lowest speed multiplier when cargo is at maximum weight.")]
    [Range(0f, 1f)]
    public float minimumSpeedMultiplier = 0.25f;


    // =========================================================
    // ENGINE AUDIO
    // =========================================================

    [Header("Engine Audio")]
    [Tooltip("3D engine sound that plays while the ship is moving.")]
    public AudioClip Engine;

    [Tooltip("AudioSource located on the ship's engine. Set Spatial Blend to 3D.")]
    public AudioSource engineAudioSource;

    [Tooltip("How long the Engine sound takes to fade out when the ship stops.")]
    public float engineFadeOutTime = 0.5f;


    private ShipCargo shipCargo;

    private bool moving = false;

    private bool wasMoving = false;

    private SoundManager soundManager;

    private Coroutine engineFadeCoroutine;


    // =========================================================
    // START
    // =========================================================

    private void Start()
    {
        shipCargo =
            FindObjectOfType<ShipCargo>();


        if (shipRouteSystem == null)
        {
            shipRouteSystem =
                GetComponent<ShipRouteSystem>();


            if (shipRouteSystem == null)
            {
                shipRouteSystem =
                    FindObjectOfType<ShipRouteSystem>();
            }
        }


        if (routeDangerSystem == null)
        {
            routeDangerSystem =
                FindObjectOfType<RouteDangerSystem>();
        }


        currentSpeed = 0f;


        GameObject audioObject =
            GameObject.FindGameObjectWithTag("Audio");


        if (audioObject != null)
        {
            soundManager =
                audioObject.GetComponent<SoundManager>();
        }


        // -----------------------------------------------------
        // ENGINE AUDIO SETUP
        // -----------------------------------------------------

        if (engineAudioSource != null)
        {
            engineAudioSource.playOnAwake = false;
            engineAudioSource.loop = true;
            engineAudioSource.clip = Engine;
        }


        UpdateResourceDepotPaper();
    }


    // =========================================================
    // UPDATE
    // =========================================================

    private void Update()
    {
        UpdateSpeedLevel();

        HandleMovementAudio();


        if (!moving)
            return;


        if (targetLocation == null)
        {
            moving = false;

            StopDangerIfNotOnDangerRoute();

            UpdateResourceDepotPaper();

            return;
        }


        UpdateMovement();
    }


    // =========================================================
    // MOVEMENT AUDIO
    // =========================================================

    private void HandleMovementAudio()
    {
        // -----------------------------------------------------
        // SHIP JUST STARTED MOVING
        // -----------------------------------------------------

        if (moving && !wasMoving)
        {
            // Start Moving sound
            if (soundManager != null)
            {
                soundManager.StartMovingAudio();
            }


            // Start Engine sound
            StartEngineAudio();
        }


        // -----------------------------------------------------
        // SHIP JUST STOPPED
        // -----------------------------------------------------

        if (!moving && wasMoving)
        {
            // Fade Moving sound
            if (soundManager != null)
            {
                soundManager.StopMovingAudio();


                // Play StopMoving once
                soundManager.PlaySFX(
                    soundManager.StopMoving
                );
            }


            // Fade Engine sound
            StopEngineAudio();
        }


        // -----------------------------------------------------
        // STORE CURRENT MOVEMENT STATE
        // -----------------------------------------------------

        wasMoving = moving;
    }


    // =========================================================
    // START ENGINE AUDIO
    // =========================================================

    private void StartEngineAudio()
    {
        if (
            engineAudioSource == null ||
            Engine == null
        )
        {
            return;
        }


        // -----------------------------------------------------
        // CANCEL ENGINE FADE
        // -----------------------------------------------------

        if (engineFadeCoroutine != null)
        {
            StopCoroutine(
                engineFadeCoroutine
            );

            engineFadeCoroutine = null;
        }


        // -----------------------------------------------------
        // SETUP ENGINE AUDIO
        // -----------------------------------------------------

        engineAudioSource.clip =
            Engine;

        engineAudioSource.loop = true;


        // Restore full volume.

        engineAudioSource.volume = 1f;


        // -----------------------------------------------------
        // START IF NOT ALREADY PLAYING
        // -----------------------------------------------------

        if (!engineAudioSource.isPlaying)
        {
            engineAudioSource.Play();
        }
    }


    // =========================================================
    // STOP ENGINE AUDIO
    // =========================================================

    private void StopEngineAudio()
    {
        if (engineAudioSource == null)
            return;


        if (!engineAudioSource.isPlaying)
            return;


        // -----------------------------------------------------
        // CANCEL EXISTING FADE
        // -----------------------------------------------------

        if (engineFadeCoroutine != null)
        {
            StopCoroutine(
                engineFadeCoroutine
            );
        }


        // -----------------------------------------------------
        // START FADE
        // -----------------------------------------------------

        engineFadeCoroutine =
            StartCoroutine(
                FadeOutEngineAudio()
            );
    }


    // =========================================================
    // FADE OUT ENGINE AUDIO
    // =========================================================

    private IEnumerator FadeOutEngineAudio()
    {
        float startingVolume =
            engineAudioSource != null
                ? engineAudioSource.volume
                : 0f;


        float timer = 0f;


        // -----------------------------------------------------
        // FADE OUT
        // -----------------------------------------------------

        while (
            timer <
            engineFadeOutTime
        )
        {
            if (engineAudioSource == null)
                yield break;


            timer +=
                Time.deltaTime;


            float percentage;


            if (engineFadeOutTime <= 0f)
            {
                percentage = 1f;
            }
            else
            {
                percentage =
                    Mathf.Clamp01(
                        timer /
                        engineFadeOutTime
                    );
            }


            engineAudioSource.volume =
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

        if (engineAudioSource != null)
        {
            engineAudioSource.volume = 0f;
            engineAudioSource.Stop();
            engineAudioSource.volume = 1f;
        }


        engineFadeCoroutine = null;
    }


    // =========================================================
    // MOVEMENT
    // =========================================================

    private void UpdateMovement()
    {
        if (shipCargo == null)
        {
            moving = false;

            StopDangerIfNotOnDangerRoute();

            UpdateResourceDepotPaper();

            return;
        }


        // -----------------------------------------------------
        // SPEED LEVEL 0
        // -----------------------------------------------------

        if (currentSpeedLevel == 0)
        {
            currentSpeed = 0f;

            /*
             * IMPORTANT:
             *
             * We do NOT stop danger audio here.
             *
             * The ship may be stopped halfway through
             * a dangerous route.
             */

            UpdateResourceDepotPaper();

            return;
        }


        // -----------------------------------------------------
        // CHECK FUEL
        // -----------------------------------------------------

        if (
            shipCargo.GetResourceAmount("fuel") <= 0
        )
        {
            currentSpeed = 0f;
            moving = false;
            targetLocation = null;

            StopDangerIfNotOnDangerRoute();

            UpdateResourceDepotPaper();

            return;
        }


        // -----------------------------------------------------
        // CALCULATE SPEED
        // -----------------------------------------------------

        currentCargoWeight =
            shipCargo.GetTotalWeight();


        currentSpeed =
            CalculateCurrentSpeed();


        // -----------------------------------------------------
        // MOVE SHIP
        // -----------------------------------------------------

        transform.position =
            Vector3.MoveTowards(
                transform.position,
                targetLocation.transform.position,
                currentSpeed * Time.deltaTime
            );


        // -----------------------------------------------------
        // CHECK ARRIVAL
        // -----------------------------------------------------

        if (
            Vector3.Distance(
                transform.position,
                targetLocation.transform.position
            ) < 0.05f
        )
        {
            transform.position =
                targetLocation.transform.position;


            ArriveAtLocation();
        }
    }


    // =========================================================
    // ARRIVAL
    // =========================================================

    private void ArriveAtLocation()
    {
        if (targetLocation == null)
        {
            moving = false;

            StopDangerIfNotOnDangerRoute();

            UpdateResourceDepotPaper();

            return;
        }


        // -----------------------------------------------------
        // STORE ARRIVED LOCATION
        // -----------------------------------------------------

        Location arrivedLocation =
            targetLocation;


        currentLocation =
            arrivedLocation;


        // -----------------------------------------------------
        // CONSUME FUEL
        // -----------------------------------------------------

        ConsumeFuel();


        // -----------------------------------------------------
        // COMPLETE REQUESTS
        // -----------------------------------------------------

        CompleteRequestsAtLocation(
            currentLocation
        );


        // -----------------------------------------------------
        // VANDERBILT PAYMENT
        // -----------------------------------------------------

        CheckForVanderbiltPayment();


        // -----------------------------------------------------
        // GET NEXT LOCATION
        // -----------------------------------------------------

        if (shipRouteSystem != null)
        {
            shipRouteSystem.OnArrivedAtLocation(
                currentLocation
            );


            targetLocation =
                shipRouteSystem.GetNextLocation();
        }
        else
        {
            targetLocation = null;
        }


        // -----------------------------------------------------
        // PREVENT TARGET = CURRENT
        // -----------------------------------------------------

        if (
            targetLocation != null &&
            targetLocation == currentLocation
        )
        {
            targetLocation = null;
        }


        // -----------------------------------------------------
        // ROUTE DANGER
        // -----------------------------------------------------

        if (targetLocation != null)
        {
            CheckNextRouteDanger();
        }
        else
        {
            StopDangerAudio();
        }


        // -----------------------------------------------------
        // CONTINUE OR STOP
        // -----------------------------------------------------

        if (targetLocation != null)
        {
            moving = true;
        }
        else
        {
            moving = false;

            StopDangerIfNotOnDangerRoute();
        }


        // -----------------------------------------------------
        // UPDATE RESOURCE DEPOT PAPER
        // -----------------------------------------------------

        UpdateResourceDepotPaper();
    }


    // =========================================================
    // ROUTE DANGER
    // =========================================================

    private void CheckNextRouteDanger()
    {
        if (
            currentLocation == null ||
            targetLocation == null
        )
        {
            StopDangerIfNotOnDangerRoute();

            return;
        }


        LocationManager locationManager =
            LocationManager.Instance;


        if (locationManager == null)
        {
            StopDangerIfNotOnDangerRoute();

            return;
        }


        RouteConnection connection =
            locationManager.GetConnection(
                currentLocation,
                targetLocation
            );


        if (connection == null)
        {
            StopDangerIfNotOnDangerRoute();

            return;
        }


        if (routeDangerSystem != null)
        {
            routeDangerSystem.CheckRouteDanger(
                connection
            );
        }
    }


    // =========================================================
    // CHECK CURRENT DANGER ROUTE
    // =========================================================

    private void StopDangerIfNotOnDangerRoute()
    {
        if (routeDangerSystem == null)
            return;


        if (
            currentLocation == null ||
            targetLocation == null
        )
        {
            StopDangerAudio();

            return;
        }


        LocationManager locationManager =
            LocationManager.Instance;


        if (locationManager == null)
        {
            StopDangerAudio();

            return;
        }


        RouteConnection connection =
            locationManager.GetConnection(
                currentLocation,
                targetLocation
            );


        if (connection == null)
        {
            StopDangerAudio();

            return;
        }


        if (connection.dangerLevel > 0)
        {
            routeDangerSystem.CheckRouteDanger(
                connection
            );

            return;
        }


        StopDangerAudio();
    }


    // =========================================================
    // STOP DANGER AUDIO
    // =========================================================

    private void StopDangerAudio()
    {
        if (soundManager == null)
            return;


        soundManager.StopDangerAudio();
    }


    // =========================================================
    // REQUESTS
    // =========================================================

    private void CompleteRequestsAtLocation(
        Location location
    )
    {
        if (location == null)
            return;


        if (location.activeRequests == null)
            return;


        RequestPaper[] requests =
            location.activeRequests.ToArray();


        foreach (
            RequestPaper request
            in requests
        )
        {
            if (request == null)
                continue;


            request.OnShipArrived(
                location
            );
        }
    }


    // =========================================================
    // VANDERBILT PAYMENT
    // =========================================================

    private void CheckForVanderbiltPayment()
    {
        if (currentLocation == null)
            return;


        if (currentLocation.locationType == null)
            return;


        if (
            currentLocation.locationType.LocationType !=
            "Vanderbilt"
        )
        {
            return;
        }


        if (shipCargo == null)
            return;


        RequestPaper[] requests =
            FindObjectsOfType<RequestPaper>();


        int totalGoldCollected = 0;


        foreach (
            RequestPaper request
            in requests
        )
        {
            if (request == null)
                continue;


            if (!request.HasPendingGold())
                continue;


            int gold =
                request.CollectGold();


            request.DeletedSelf();


            if (gold <= 0)
                continue;


            shipCargo.AddOrRemoveResource(
                "gold",
                gold
            );


            totalGoldCollected += gold;
        }


        RequestVisualIndicator.RefreshVanderbiltPaymentLights();
    }


    // =========================================================
    // FUEL
    // =========================================================

    private void ConsumeFuel()
    {
        if (shipCargo == null)
            return;


        if (currentSpeedLevel == 0)
            return;


        currentFuelCost =
            currentSpeedLevel;


        shipCargo.AddOrRemoveResource(
            "fuel",
            -currentFuelCost
        );
    }


    // =========================================================
    // START ROUTE
    // =========================================================

    public void StartRoute()
    {
        if (shipRouteSystem == null)
            return;


        if (moving)
            return;


        // -----------------------------------------------------
        // PLAY BUTTON SOUND
        // -----------------------------------------------------

        if (soundManager != null)
        {
            soundManager.PlaySFX(
                soundManager.Button
            );
        }


        targetLocation =
            shipRouteSystem.GetNextLocation();


        if (targetLocation == null)
        {
            moving = false;

            StopDangerIfNotOnDangerRoute();

            UpdateResourceDepotPaper();

            return;
        }


        if (targetLocation == currentLocation)
        {
            targetLocation = null;
            moving = false;

            StopDangerIfNotOnDangerRoute();

            UpdateResourceDepotPaper();

            return;
        }


        // -----------------------------------------------------
        // START TRAVELLING
        // -----------------------------------------------------

        moving = true;


        UpdateResourceDepotPaper();


        CheckNextRouteDanger();
    }


    // =========================================================
    // STOP MOVEMENT
    // =========================================================

    public void StopMovement()
    {
        moving = false;

        targetLocation = null;

        currentSpeed = 0f;


        // -----------------------------------------------------
        // DO NOT STOP DANGER AUDIO HERE.
        // -----------------------------------------------------
        //
        // The player may have stopped in the middle of a
        // dangerous route.
        //
        // Thunder and DangerAmbience therefore continue
        // until the danger route has actually ended.
        // -----------------------------------------------------


        UpdateResourceDepotPaper();
    }


    // =========================================================
    // RESOURCE DEPOT PAPER
    // =========================================================

    private void UpdateResourceDepotPaper()
    {
        if (resourceDepotPaper == null)
            return;


        bool atResourceDepot =
            currentLocation != null &&
            currentLocation.locationType != null &&
            currentLocation.locationType.LocationType ==
            "ResourceDepot";


        bool shipStopped =
            !moving;


        if (
            atResourceDepot &&
            shipStopped
        )
        {
            if (!resourceDepotPaper.activeSelf)
            {
                resourceDepotPaper.SetActive(true);


                if (soundManager != null)
                {
                    soundManager.PlaySFX(
                        soundManager.FaxPrint
                    );
                }
            }


            return;
        }


        if (resourceDepotPaper.activeSelf)
        {
            resourceDepotPaper.SetActive(false);
        }
    }


    // =========================================================
    // SPEED LEVEL
    // =========================================================

    private void UpdateSpeedLevel()
    {
        if (speedControl == null)
        {
            currentSpeedLevel = 5;
            currentFuelCost = 5;

            return;
        }


        float leverSpeed =
            speedControl.GetSpeed();


        if (leverSpeed <= 0f)
        {
            currentSpeedLevel = 0;
        }
        else if (leverSpeed <= 20f)
        {
            currentSpeedLevel = 1;
        }
        else if (leverSpeed <= 40f)
        {
            currentSpeedLevel = 2;
        }
        else if (leverSpeed <= 60f)
        {
            currentSpeedLevel = 3;
        }
        else if (leverSpeed <= 80f)
        {
            currentSpeedLevel = 4;
        }
        else
        {
            currentSpeedLevel = 5;
        }


        currentFuelCost =
            currentSpeedLevel;
    }


    // =========================================================
    // SPEED CALCULATION
    // =========================================================

    private float CalculateCurrentSpeed()
    {
        if (currentSpeedLevel == 0)
            return 0f;


        float speedPercentage =
            currentSpeedLevel / 5f;


        float cargoSpeedMultiplier =
            1f;


        if (shipCargo != null)
        {
            currentCargoWeight =
                shipCargo.GetTotalWeight();


            float weightPercentage =
                currentCargoWeight /
                maximumCargoWeight;


            weightPercentage =
                Mathf.Clamp01(
                    weightPercentage
                );


            cargoSpeedMultiplier =
                Mathf.Lerp(
                    1f,
                    minimumSpeedMultiplier,
                    weightPercentage
                );
        }


        return speed *
               speedPercentage *
               cargoSpeedMultiplier;
    }


    // =========================================================
    // PUBLIC VALUES
    // =========================================================

    public bool IsMoving()
    {
        return moving;
    }


    public Location GetCurrentLocation()
    {
        return currentLocation;
    }


    public Location GetTargetLocation()
    {
        return targetLocation;
    }


    public float GetCurrentSpeed()
    {
        return currentSpeed;
    }


    public float GetCurrentCargoWeight()
    {
        return currentCargoWeight;
    }


    public float GetLeverSpeed()
    {
        if (speedControl == null)
            return 100f;


        return speedControl.GetSpeed();
    }


    public int GetSpeedLevel()
    {
        return currentSpeedLevel;
    }


    public int GetFuelCost()
    {
        return currentFuelCost;
    }
}