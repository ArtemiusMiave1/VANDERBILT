using System.Collections.Generic;
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

    [Header("Route")]
    public List<Location> route = new List<Location>();

    [Header("Route String Height")]
    public float routeLineHeightOffset = 0.5f;

    [Header("Route Danger")]
    public RouteDangerSystem routeDangerSystem;

    [Header("Route Line")]
    public LineRenderer routeLine;

    [Header("Route Preview")]
    public Camera mapCamera;

    public LayerMask mapPlaneLayer;

    public float routeLineHeight = 0.2f;

    [Header("Resource Depot UI")]
    public GameObject resourceDepotPaper;

    [Header("Fuel")]
    public int fuelPerLocation = 5;

    [Header("Cargo Weight")]
    public float maximumCargoWeight = 2500f;
    public float minimumSpeedMultiplier = 0.25f;

    private ShipCargo shipCargo;

    private bool moving = false;


    private void Start()
    {
        shipCargo = FindObjectOfType<ShipCargo>();

        if (shipCargo == null)
        {
            Debug.LogError("ShipCargo not found!");
        }

        if (mapCamera == null)
        {
            mapCamera = Camera.main;
        }

        currentSpeed = speed;

        UpdateResourceDepotPaper();
        UpdateRouteLine();
    }


    private void Update()
    {
        // =====================================================
        // MOVEMENT
        // =====================================================

        if (moving && targetLocation != null)
        {
            UpdateMovement();
        }


        // =====================================================
        // ROUTE PREVIEW
        // =====================================================

        if (Input.GetMouseButton(1))
        {
            UpdateRoutePreview();
        }
        else
        {
            RemoveRoutePreview();
        }
    }


    // =========================================================
    // MOVEMENT
    // =========================================================

    private void UpdateMovement()
    {
        // Check fuel
        if (shipCargo == null ||
            shipCargo.GetResourceAmount("fuel") <= 0)
        {
            moving = false;
            targetLocation = null;

            Debug.Log("Ship has run out of fuel!");

            UpdateRouteLine();

            return;
        }


        // Update cargo weight and speed
        currentCargoWeight =
            shipCargo.GetTotalWeight();

        currentSpeed =
            GetCurrentSpeed();


        // Move towards target
        transform.position = Vector3.MoveTowards(
            transform.position,
            targetLocation.transform.position,
            currentSpeed * Time.deltaTime
        );


        // Check if arrived
        if (Vector3.Distance(
            transform.position,
            targetLocation.transform.position
        ) < 0.05f)
        {
            transform.position =
                targetLocation.transform.position;

            ArriveAtLocation();
        }
    }


    private void ArriveAtLocation()
    {
        Location previousLocation = currentLocation;

        currentLocation = targetLocation;

        //Debug.Log(
        //    "Arrived at " +
        //    currentLocation.name
        //);


        // CHECK ROUTE DANGER
        RouteConnection completedRoute =
            GetRouteConnection(
                previousLocation,
                currentLocation
            );

        if (routeDangerSystem != null)
        {
            routeDangerSystem.CheckRouteDanger(
                completedRoute
            );
        }


        // USE FUEL
        if (shipCargo != null)
        {
            shipCargo.AddOrRemoveResource(
                "fuel",
                -fuelPerLocation
            );

            //Debug.Log(
            //    "Used " +
            //    fuelPerLocation +
            //    " fuel. Remaining fuel: " +
            //    shipCargo.GetResourceAmount("fuel")
            //);
        }


        UpdateResourceDepotPaper();


        // CHECK REQUESTS AT THIS LOCATION
        if (currentLocation.activeRequests != null)
        {
            List<RequestPaper> requestsAtLocation =
                new List<RequestPaper>(
                    currentLocation.activeRequests
                );

            foreach (RequestPaper request in requestsAtLocation)
            {
                if (request != null)
                    request.OnShipArrived(currentLocation);
            }
        }


        // Remove location we just arrived at
        if (route.Count > 0)
            route.RemoveAt(0);


        UpdateRouteLine();


        // Continue to next location
        if (route.Count > 0)
        {
            targetLocation = route[0];

            //Debug.Log(
            //    "Travelling to " +
            //    targetLocation.name
            //);
        }
        else
        {
            targetLocation = null;
            moving = false;

            //Debug.Log("Route complete.");

            UpdateRouteLine();
        }
    }


    // =========================================================
    // RESOURCE DEPOT
    // =========================================================

    private void UpdateResourceDepotPaper()
    {
        if (resourceDepotPaper == null)
        {
            return;
        }


        if (currentLocation != null &&
            currentLocation.locationType != null &&
            currentLocation.locationType.LocationType ==
            "ResourceDepot")
        {
            resourceDepotPaper.SetActive(true);
        }
        else
        {
            resourceDepotPaper.SetActive(false);
        }
    }


    // =========================================================
    // ROUTE CONNECTION
    // =========================================================

    private RouteConnection GetRouteConnection(
        Location locationA,
        Location locationB)
    {
        if (locationA == null || locationB == null)
            return null;


        foreach (RouteConnection connection
                 in locationA.connections)
        {
            if (connection == null)
                continue;


            if (connection.locationA == locationA &&
                connection.locationB == locationB)
            {
                return connection;
            }


            if (connection.locationA == locationB &&
                connection.locationB == locationA)
            {
                return connection;
            }
        }


        return null;
    }


    // =========================================================
    // ADD LOCATION TO ROUTE
    // =========================================================

    public void AddToRoute(Location destination)
    {
        if (destination == null)
            return;


        if (destination == currentLocation)
        {
            //Debug.Log(
            //    "Already at " +
            //    destination.name
            //);

            return;
        }


        if (route.Contains(destination))
        {
            //Debug.Log(
            //    destination.name +
            //    " is already in the route."
            //);

            return;
        }


        Location previousLocation;


        if (route.Count > 0)
        {
            previousLocation =
                route[route.Count - 1];
        }
        else
        {
            previousLocation =
                currentLocation;
        }


        RouteConnection connection =
            GetRouteConnection(
                previousLocation,
                destination
            );


        if (connection == null)
        {
            Debug.Log(
                destination.name +
                " is not connected to " +
                previousLocation.name
            );

            return;
        }


        if (connection.blocked)
        {
            Debug.Log(
                "Route from " +
                previousLocation.name +
                " to " +
                destination.name +
                " is BLOCKED!"
            );

            return;
        }


        if (!HasEnoughFuel())
            return;


        route.Add(destination);


        //Debug.Log(
        //    "Added " +
        //    destination.name +
        //    " to route."
        //);


        Debug.Log(
            "Route danger level: " +
            connection.dangerLevel
        );


        UpdateRouteLine();


        if (!moving)
        {
            targetLocation = route[0];

            moving = true;


            //Debug.Log(
            //    "Ship travelling to " +
            //    targetLocation.name
            //);
        }
    }


    // =========================================================
    // FUEL
    // =========================================================

    private bool HasEnoughFuel()
    {
        if (shipCargo == null)
            return false;


        int currentFuel =
            shipCargo.GetResourceAmount("fuel");


        if (currentFuel < fuelPerLocation)
        {
            //Debug.Log(
            //    "Not enough fuel! " +
            //    "Required: " +
            //    fuelPerLocation +
            //    " | Have: " +
            //    currentFuel
            //);

            return false;
        }


        return true;
    }


    // =========================================================
    // CLEAR ROUTE
    // =========================================================

    public void ClearRoute()
    {
        route.Clear();


        if (!moving)
            targetLocation = null;


        UpdateRouteLine();


        //Debug.Log("Route cleared.");
    }


    // =========================================================
    // ROUTE LINE
    // =========================================================

    private void UpdateRouteLine()
    {
        if (routeLine == null)
            return;

        if (currentLocation == null)
            return;

        routeLine.useWorldSpace = true;

        routeLine.positionCount = route.Count + 1;

        // Start at current location
        Vector3 startPosition =
            currentLocation.transform.position;

        startPosition.y += routeLineHeightOffset;

        routeLine.SetPosition(
            0,
            startPosition
        );

        // Draw the route string above the map
        for (int i = 0; i < route.Count; i++)
        {
            Vector3 position =
                route[i].transform.position;

            position.y += routeLineHeightOffset;

            routeLine.SetPosition(
                i + 1,
                position
            );
        }
    }


    // =========================================================
    // ROUTE PREVIEW
    // =========================================================

    private void UpdateRoutePreview()
    {
        if (routeLine == null)
            return;


        if (mapCamera == null)
            return;


        // We need somewhere to start the preview from
        if (currentLocation == null)
            return;


        Vector3 startPosition;


        // If there are locations already selected,
        // start from the most recently selected one
        if (route.Count > 0)
        {
            startPosition =
                route[route.Count - 1].transform.position;
        }
        else
        {
            startPosition =
                currentLocation.transform.position;
        }


        // Raycast from the mouse
        Ray ray =
            mapCamera.ScreenPointToRay(
                Input.mousePosition
            );


        RaycastHit hit;


        if (Physics.Raycast(
            ray,
            out hit,
            1000f,
            mapPlaneLayer
        ))
        {
            Vector3 mousePosition =
                hit.point;


            // Raise line above the map
            mousePosition.y +=
                routeLineHeight;


            // Existing route + temporary mouse point
            routeLine.positionCount =
                route.Count + 2;


            // Current location
            routeLine.SetPosition(
                0,
                currentLocation.transform.position
            );


            // Existing route locations
            for (int i = 0; i < route.Count; i++)
            {
                routeLine.SetPosition(
                    i + 1,
                    route[i].transform.position
                );
            }


            // Mouse preview point
            routeLine.SetPosition(
                route.Count + 1,
                mousePosition
            );
        }
    }


    private void RemoveRoutePreview()
    {
        if (routeLine == null)
            return;


        // Return to normal route line
        UpdateRouteLine();
    }


    // =========================================================
    // CLICK LOCATION
    // =========================================================

    public void HandleRouteLocation(Location location)
    {
        if (location == null)
            return;


        // -----------------------------------------
        // REMOVE LAST LOCATION
        // -----------------------------------------

        if (route.Count > 0 &&
            route[route.Count - 1] == location)
        {
            route.RemoveAt(
                route.Count - 1
            );


            Debug.Log(
                "Removed " +
                location.name +
                " from route."
            );


            if (moving)
            {
                if (route.Count > 0)
                {
                    targetLocation =
                        route[0];
                }
                else
                {
                    targetLocation = null;
                    moving = false;
                }
            }


            UpdateRouteLine();

            return;
        }


        // -----------------------------------------
        // DON'T ADD DUPLICATES
        // -----------------------------------------

        if (route.Contains(location))
            return;


        // -----------------------------------------
        // FIND PREVIOUS LOCATION
        // -----------------------------------------

        Location previousLocation;


        if (route.Count > 0)
        {
            previousLocation =
                route[route.Count - 1];
        }
        else
        {
            previousLocation =
                currentLocation;
        }


        // -----------------------------------------
        // FIND CONNECTION
        // -----------------------------------------

        RouteConnection connection =
            GetRouteConnection(
                previousLocation,
                location
            );


        if (connection == null)
        {
            Debug.Log(
                location.name +
                " is not connected to " +
                previousLocation.name
            );

            return;
        }


        // -----------------------------------------
        // CHECK BLOCKED
        // -----------------------------------------

        if (connection.blocked)
        {
            Debug.Log(
                "This route is blocked!"
            );

            return;
        }


        // -----------------------------------------
        // ADD LOCATION
        // -----------------------------------------

        route.Add(location);


        //Debug.Log(
        //    "Added " +
        //    location.name +
        //    " to route."
        //);


        Debug.Log(
            "Danger Level: " +
            connection.dangerLevel
        );


        UpdateRouteLine();


        // -----------------------------------------
        // START MOVEMENT
        // -----------------------------------------

        if (!moving)
        {
            targetLocation =
                route[0];

            moving = true;


            //Debug.Log(
            //    "Ship travelling to " +
            //    targetLocation.name
            //);
        }
    }


    // =========================================================
    // CARGO WEIGHT / SPEED
    // =========================================================

    private float GetCurrentSpeed()
    {
        if (shipCargo == null)
            return speed;


        currentCargoWeight =
            shipCargo.GetTotalWeight();


        float weightPercentage =
            currentCargoWeight /
            maximumCargoWeight;


        weightPercentage =
            Mathf.Clamp01(weightPercentage);


        float speedMultiplier =
            Mathf.Lerp(
                1f,
                minimumSpeedMultiplier,
                weightPercentage
            );


        return speed * speedMultiplier;
    }
}