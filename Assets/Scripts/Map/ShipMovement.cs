using UnityEngine;

public class ShipMovement : MonoBehaviour
{
    [Header("Movement")]
    public float speed = 5f;

    [Header("Current Movement Stats")]
    public float currentSpeed;
    public float currentCargoWeight;

    [Header("Locations")]
    public Location currentLocation;
    public Location targetLocation;

    [Header("Route System")]
    public ShipRouteSystem shipRouteSystem;

    [Header("Resource Depot UI")]
    public GameObject resourceDepotPaper;

    [Header("Fuel")]
    public int fuelPerLocation = 5;

    [Header("Cargo Weight")]
    public float maximumCargoWeight = 750f;

    [Tooltip("Lowest speed multiplier when cargo is at maximum weight.")]
    [Range(0f, 1f)]
    public float minimumSpeedMultiplier = 0.25f;

    private ShipCargo shipCargo;

    private bool moving = false;


    // =========================================================
    // START
    // =========================================================

    private void Start()
    {
        shipCargo =
            FindObjectOfType<ShipCargo>();

        if (shipCargo == null)
        {
            Debug.LogError(
                "ShipMovement: ShipCargo not found!"
            );
        }

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

        if (shipRouteSystem == null)
        {
            Debug.LogError(
                "ShipMovement: ShipRouteSystem not found!"
            );
        }

        currentSpeed =
            speed;

        UpdateResourceDepotPaper();
    }


    // =========================================================
    // UPDATE
    // =========================================================

    private void Update()
    {
        if (!moving)
            return;

        if (targetLocation == null)
        {
            moving = false;
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
            return;
        }

        // Stop if there is no fuel.
        if (
            shipCargo.GetResourceAmount("fuel") <= 0
        )
        {
            moving = false;
            targetLocation = null;

            Debug.Log(
                "Ship has run out of fuel!"
            );

            return;
        }

        currentCargoWeight =
            shipCargo.GetTotalWeight();

        currentSpeed =
            CalculateCurrentSpeed();

        transform.position =
            Vector3.MoveTowards(
                transform.position,
                targetLocation.transform.position,
                currentSpeed *
                Time.deltaTime
            );

        // Check for arrival.
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
            return;
        }

        Location arrivedLocation =
            targetLocation;

        // Update current location.
        currentLocation =
            arrivedLocation;

        // Consume fuel once for this trip.
        ConsumeFuel();

        // Update Resource Depot visibility.
        UpdateResourceDepotPaper();

        // Deliver requests at this location.
        CompleteRequestsAtLocation(
            currentLocation
        );

        // Collect pending gold if this is Vanderbilt.
        CheckForVanderbiltPayment();

        // Update the route.
        if (shipRouteSystem != null)
        {
            shipRouteSystem.OnArrivedAtLocation(
                currentLocation
            );

            /*
             * Get the next location AFTER
             * the route has been updated.
             */
            targetLocation =
                shipRouteSystem.GetNextLocation();
        }
        else
        {
            targetLocation = null;
        }

        /*
         * Safety check.
         *
         * Prevents the ship from repeatedly
         * arriving at the same location.
         */
        if (
            targetLocation != null &&
            targetLocation == currentLocation
        )
        {
            Debug.LogWarning(
                "ShipMovement: Next location is the " +
                "same as the current location. " +
                "Stopping movement to prevent a loop."
            );

            targetLocation = null;
        }

        // Continue route if another destination exists.
        if (targetLocation != null)
        {
            moving = true;

            Debug.Log(
                "Continuing route to " +
                targetLocation.GetDisplayName()
            );
        }
        else
        {
            moving = false;

            Debug.Log(
                "Ship route complete."
            );
        }
    }


    // =========================================================
    // REQUEST DELIVERY
    // =========================================================

    private void CompleteRequestsAtLocation(
        Location location
    )
    {
        if (location == null)
            return;

        if (location.activeRequests == null)
            return;

        /*
         * Make a copy because completing a request
         * removes it from activeRequests.
         */
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

        /*
         * Only Vanderbilt locations can pay
         * pending request rewards.
         */
        if (
            currentLocation.locationType.LocationType !=
            "Vanderbilt"
        )
        {
            return;
        }

        if (shipCargo == null)
        {
            Debug.LogError(
                "ShipMovement: ShipCargo not found!"
            );

            return;
        }

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

            if (gold <= 0)
                continue;

            shipCargo.AddOrRemoveResource(
                "gold",
                gold
            );

            totalGoldCollected += gold;
        }

        if (totalGoldCollected > 0)
        {
            Debug.Log(
                "Arrived at Vanderbilt. " +
                "Collected " +
                totalGoldCollected +
                " gold from completed requests."
            );
        }
    }


    // =========================================================
    // FUEL
    // =========================================================

    private void ConsumeFuel()
    {
        if (shipCargo == null)
            return;

        shipCargo.AddOrRemoveResource(
            "fuel",
            -fuelPerLocation
        );

        Debug.Log(
            "Fuel consumed: " +
            fuelPerLocation
        );
    }


    // =========================================================
    // START ROUTE
    // =========================================================

    public void StartRoute()
    {
        Debug.Log(
            "========== START ROUTE =========="
        );

        if (shipRouteSystem == null)
        {
            Debug.LogError(
                "ShipMovement: ShipRouteSystem not found!"
            );

            return;
        }

        if (moving)
        {
            Debug.Log(
                "Ship is already moving."
            );

            return;
        }

        /*
         * Get the next location from the
         * planned route.
         */
        targetLocation =
            shipRouteSystem.GetNextLocation();

        if (targetLocation == null)
        {
            Debug.Log(
                "Ship route is empty."
            );

            moving = false;

            return;
        }

        /*
         * Prevent movement to the location
         * the ship is already at.
         */
        if (targetLocation == currentLocation)
        {
            Debug.LogWarning(
                "ShipMovement: Target location is " +
                "the same as current location."
            );

            targetLocation = null;
            moving = false;

            return;
        }

        moving = true;

        Debug.Log(
            "Ship starting route to " +
            targetLocation.GetDisplayName()
        );
    }


    // =========================================================
    // STOP MOVEMENT
    // =========================================================

    public void StopMovement()
    {
        moving = false;

        targetLocation = null;

        Debug.Log(
            "Ship movement stopped."
        );
    }


    // =========================================================
    // RESOURCE DEPOT
    // =========================================================

    private void UpdateResourceDepotPaper()
    {
        if (resourceDepotPaper == null)
            return;

        if (
            currentLocation != null &&
            currentLocation.locationType != null &&
            currentLocation.locationType.LocationType ==
            "ResourceDepot"
        )
        {
            resourceDepotPaper.SetActive(true);
        }
        else
        {
            resourceDepotPaper.SetActive(false);
        }
    }


    // =========================================================
    // CARGO WEIGHT / SPEED
    // =========================================================

    private float CalculateCurrentSpeed()
    {
        if (shipCargo == null)
            return speed;

        currentCargoWeight =
            shipCargo.GetTotalWeight();

        float weightPercentage =
            currentCargoWeight /
            maximumCargoWeight;

        weightPercentage =
            Mathf.Clamp01(
                weightPercentage
            );

        /*
         * Empty cargo:
         * 100% speed
         *
         * Maximum cargo:
         * minimumSpeedMultiplier
         */
        float speedMultiplier =
            Mathf.Lerp(
                1f,
                minimumSpeedMultiplier,
                weightPercentage
            );

        return speed *
               speedMultiplier;
    }


    // =========================================================
    // GETTERS
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
}