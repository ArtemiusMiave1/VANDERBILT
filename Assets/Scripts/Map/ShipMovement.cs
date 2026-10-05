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

    private ShipCargo shipCargo;

    private bool moving = false;

    private SoundManager soundManager;


    // =========================================================
    // START
    // =========================================================

    private void Start()
    {
        shipCargo = FindObjectOfType<ShipCargo>();

        if (shipRouteSystem == null)
        {
            shipRouteSystem = GetComponent<ShipRouteSystem>();

            if (shipRouteSystem == null)
            {
                shipRouteSystem = FindObjectOfType<ShipRouteSystem>();
            }
        }

        if (routeDangerSystem == null)
        {
            routeDangerSystem = FindObjectOfType<RouteDangerSystem>();
        }

        currentSpeed = 0f;

        GameObject audioObject =
            GameObject.FindGameObjectWithTag("Audio");

        if (audioObject != null)
        {
            soundManager =
                audioObject.GetComponent<SoundManager>();
        }

        UpdateResourceDepotPaper();
    }


    // =========================================================
    // UPDATE
    // =========================================================

    private void Update()
    {
        // Always update throttle level,
        // even when the ship isn't travelling.
        UpdateSpeedLevel();

        if (!moving)
            return;

        if (targetLocation == null)
        {
            moving = false;

            if (routeDangerSystem != null)
            {
                routeDangerSystem.SetStorm(false);
            }

            UpdateResourceDepotPaper();

            return;
        }

        UpdateMovement();
    }


    // =========================================================
    // MOVEMENT
    // =========================================================

    private void UpdateMovement()
    {
        if (shipCargo == null)
        {
            moving = false;

            if (routeDangerSystem != null)
            {
                routeDangerSystem.SetStorm(false);
            }

            UpdateResourceDepotPaper();

            return;
        }


        // -----------------------------------------------------
        // SPEED LEVEL 0 = STOPPED
        // -----------------------------------------------------

        if (currentSpeedLevel == 0)
        {
            currentSpeed = 0f;

            UpdateResourceDepotPaper();

            return;
        }


        // -----------------------------------------------------
        // CHECK FUEL
        // -----------------------------------------------------

        if (shipCargo.GetResourceAmount("fuel") <= 0)
        {
            currentSpeed = 0f;
            moving = false;
            targetLocation = null;

            if (routeDangerSystem != null)
            {
                routeDangerSystem.SetStorm(false);
            }

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

            if (routeDangerSystem != null)
            {
                routeDangerSystem.SetStorm(false);
            }

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
        // PREVENT TARGET = CURRENT LOCATION
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
            if (routeDangerSystem != null)
            {
                routeDangerSystem.SetStorm(false);
            }
        }


        // -----------------------------------------------------
        // CONTINUE OR STOP
        // -----------------------------------------------------

        if (targetLocation != null)
        {
            // There is another location in the route.
            //
            // The ship continues travelling.
            //
            // Therefore, if this was a Resource Depot
            // that the ship is only travelling through,
            // the Fax will NOT appear.

            moving = true;
        }
        else
        {
            // There are no more locations.
            //
            // The ship has actually stopped here.

            moving = false;

            if (routeDangerSystem != null)
            {
                routeDangerSystem.SetStorm(false);
            }
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
            if (routeDangerSystem != null)
            {
                routeDangerSystem.SetStorm(false);
            }

            return;
        }


        LocationManager locationManager =
            LocationManager.Instance;

        if (locationManager == null)
        {
            if (routeDangerSystem != null)
            {
                routeDangerSystem.SetStorm(false);
            }

            return;
        }


        RouteConnection connection =
            locationManager.GetConnection(
                currentLocation,
                targetLocation
            );


        if (connection == null)
        {
            if (routeDangerSystem != null)
            {
                routeDangerSystem.SetStorm(false);
            }

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


        // Level 0 consumes nothing.
        if (currentSpeedLevel == 0)
            return;


        // Fuel cost matches speed level.
        //
        // Level 1 = 1 fuel
        // Level 2 = 2 fuel
        // Level 3 = 3 fuel
        // Level 4 = 4 fuel
        // Level 5 = 5 fuel

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
        {
            return;
        }


        if (moving)
        {
            return;
        }


        targetLocation =
            shipRouteSystem.GetNextLocation();


        if (targetLocation == null)
        {
            moving = false;

            if (routeDangerSystem != null)
            {
                routeDangerSystem.SetStorm(false);
            }

            UpdateResourceDepotPaper();

            return;
        }


        if (targetLocation == currentLocation)
        {
            targetLocation = null;
            moving = false;

            if (routeDangerSystem != null)
            {
                routeDangerSystem.SetStorm(false);
            }

            UpdateResourceDepotPaper();

            return;
        }


        // -----------------------------------------------------
        // PLAYER HAS STARTED TRAVELLING
        // -----------------------------------------------------

        moving = true;

        // Immediately hide the Fax when leaving
        // the Resource Depot.
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


        if (routeDangerSystem != null)
        {
            routeDangerSystem.SetStorm(false);
        }


        // If the ship is stopped at a Resource Depot,
        // the Fax can appear.
        UpdateResourceDepotPaper();
    }


    // =========================================================
    // RESOURCE DEPOT PAPER
    // =========================================================

    private void UpdateResourceDepotPaper()
    {
        if (resourceDepotPaper == null)
            return;


        // -----------------------------------------------------
        // CHECK CURRENT LOCATION
        // -----------------------------------------------------

        bool atResourceDepot =
            currentLocation != null &&
            currentLocation.locationType != null &&
            currentLocation.locationType.LocationType ==
            "ResourceDepot";


        // -----------------------------------------------------
        // CHECK WHETHER SHIP IS STOPPED
        // -----------------------------------------------------

        bool shipStopped =
            !moving;


        // -----------------------------------------------------
        // SHOW FAX
        // -----------------------------------------------------
        //
        // The Fax ONLY appears when:
        //
        // 1. The ship's CURRENT location is a Resource Depot.
        //
        // 2. The ship is NOT moving.
        //
        // Therefore:
        //
        // Travelling through Resource Depot = NO Fax.
        //
        // Stopped at Resource Depot = Fax.
        // -----------------------------------------------------

        if (
            atResourceDepot &&
            shipStopped
        )
        {
            if (!resourceDepotPaper.activeSelf)
            {
                resourceDepotPaper.SetActive(true);


                // Play Fax print sound only when
                // the Fax actually appears.

                if (soundManager != null)
                {
                    soundManager.PlaySFX(
                        soundManager.FaxPrint
                    );
                }
            }

            return;
        }


        // -----------------------------------------------------
        // HIDE FAX
        // -----------------------------------------------------
        //
        // The Fax disappears when:
        //
        // - The ship starts travelling.
        // - The ship leaves the Resource Depot.
        // - The ship is at another location.
        // -----------------------------------------------------

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


        // -----------------------------------------------------
        // CONVERT 0-100 LEVER INTO 0-5 LEVELS
        // -----------------------------------------------------

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
        // Level 0 means completely stopped.
        if (currentSpeedLevel == 0)
        {
            return 0f;
        }


        // -----------------------------------------------------
        // SPEED LEVEL
        // -----------------------------------------------------

        float speedPercentage =
            currentSpeedLevel / 5f;


        // -----------------------------------------------------
        // CARGO WEIGHT
        // -----------------------------------------------------

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


        // -----------------------------------------------------
        // FINAL SPEED
        // -----------------------------------------------------

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