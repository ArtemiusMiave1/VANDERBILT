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
        }


        if (shipRouteSystem == null)
        {
            shipRouteSystem =
                FindObjectOfType<ShipRouteSystem>();
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
        // -----------------------------------------------
        // CHECK FUEL
        // -----------------------------------------------

        if (
            shipCargo == null ||
            shipCargo.GetResourceAmount("fuel") <= 0
        )
        {
            moving = false;
            targetLocation = null;

            //Debug.Log(
            //    "Ship has run out of fuel!"
            //);

            return;
        }


        // -----------------------------------------------
        // UPDATE CARGO WEIGHT
        // -----------------------------------------------

        currentCargoWeight =
            shipCargo.GetTotalWeight();


        // -----------------------------------------------
        // UPDATE SPEED
        // -----------------------------------------------

        currentSpeed =
            GetCurrentSpeed();


        // -----------------------------------------------
        // MOVE TOWARDS TARGET
        // -----------------------------------------------

        transform.position =
            Vector3.MoveTowards(
                transform.position,
                targetLocation.transform.position,
                currentSpeed *
                Time.deltaTime
            );


        // -----------------------------------------------
        // CHECK ARRIVAL
        // -----------------------------------------------

        if (
            Vector3.Distance(
                transform.position,
                targetLocation.transform.position
            ) < 0.05f
        )
        {
            transform.position =
                targetLocation.transform.position;
            print("Ship has arrived at " + targetLocation.GetDisplayName());
            ArriveAtLocation();
        }
    }


    // =========================================================
    // ARRIVE AT LOCATION
    // =========================================================

    private void ArriveAtLocation()
    {
        Location previousLocation =
            currentLocation;


        // -----------------------------------------------
        // UPDATE CURRENT LOCATION
        // -----------------------------------------------

        currentLocation =
            targetLocation;


        // -----------------------------------------------
        // CONSUME FUEL
        // -----------------------------------------------

        ConsumeFuel();


        // -----------------------------------------------
        // UPDATE RESOURCE DEPOT
        // -----------------------------------------------

        UpdateResourceDepotPaper();


        // -----------------------------------------------
        // COMPLETE REQUESTS
        // -----------------------------------------------

        if (
            currentLocation.activeRequests != null
        )
        {
            RequestPaper[] requestsAtLocation =
                currentLocation.activeRequests.ToArray();


            foreach (
                RequestPaper request
                in requestsAtLocation
            )
            {
                if (request != null)
                {
                    request.OnShipArrived(
                        currentLocation
                    );
                }
            }
        }


        // -----------------------------------------------
        // TELL ROUTE SYSTEM WE ARRIVED
        // -----------------------------------------------

        if (shipRouteSystem != null)
        {
            shipRouteSystem.OnArrivedAtLocation(
                currentLocation
            );


            // Get the next location in the route.

            targetLocation =
                shipRouteSystem.GetNextLocation();
        }
        else
        {
            targetLocation = null;
        }


        // -----------------------------------------------
        // CONTINUE OR FINISH ROUTE
        // -----------------------------------------------

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
    // CONSUME FUEL
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
    // RESOURCE DEPOT PAPER
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
    // CALCULATE CURRENT SPEED
    // =========================================================

    private float GetCurrentSpeed()
    {
        if (shipCargo == null)
            return speed;


        currentCargoWeight =
            shipCargo.GetTotalWeight();


        // Convert cargo weight into a value
        // between 0 and 1.

        float weightPercentage =
            currentCargoWeight /
            maximumCargoWeight;


        weightPercentage =
            Mathf.Clamp01(
                weightPercentage
            );


        // Empty cargo = 1x speed
        // Full cargo = minimumSpeedMultiplier

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
    // PUBLIC INFORMATION
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

    public float GetCurrentCargoWeight()
    {
        return currentCargoWeight;
    }
}