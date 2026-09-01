using System.Collections.Generic;
using UnityEngine;

public class ShipMovement : MonoBehaviour
{
    [Header("Movement")]
    public float speed = 5f;

    [Header("Locations")]
    public Location currentLocation;
    public Location targetLocation;

    [Header("Route")]
    public List<Location> route = new List<Location>();

    [Header("Route Line")]
    public LineRenderer routeLine;

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

        UpdateResourceDepotPaper();
    }

    private void Update()
    {
        if (!moving || targetLocation == null)
            return;

        // Stop the ship if it runs out of fuel
        if (shipCargo == null || shipCargo.GetResourceAmount("fuel") <= 0)
        {
            moving = false;
            targetLocation = null;

            Debug.Log("Ship has run out of fuel!");

            UpdateRouteLine();

            return;
        }

        float currentSpeed = GetCurrentSpeed();

        transform.position = Vector3.MoveTowards(
            transform.position,
            targetLocation.transform.position,
            currentSpeed * Time.deltaTime
        );

        if (Vector3.Distance(
            transform.position,
            targetLocation.transform.position
        ) < 0.05f)
        {
            transform.position = targetLocation.transform.position;

            ArriveAtLocation();
        }
    }


    private void ArriveAtLocation()
    {
        // Set the new current location
        currentLocation = targetLocation;

        Debug.Log("Arrived at " + currentLocation.name);


        // ========================================
        // USE FUEL
        // ========================================

        if (shipCargo != null)
        {
            shipCargo.AddOrRemoveResource(
                "fuel",
                -fuelPerLocation
            );

            Debug.Log(
                "Used " + fuelPerLocation +
                " fuel. Remaining fuel: " +
                shipCargo.GetResourceAmount("fuel")
            );
        }


        UpdateResourceDepotPaper();


        // ========================================
        // CHECK REQUESTS AT THIS LOCATION
        // ========================================

        if (currentLocation.activeRequests != null)
        {
            List<RequestPaper> requestsAtLocation =
                new List<RequestPaper>(
                    currentLocation.activeRequests
                );

            foreach (RequestPaper request in requestsAtLocation)
            {
                if (request != null)
                {
                    request.OnShipArrived(currentLocation);
                }
            }
        }

        if (route.Count > 0)
        {
            route.RemoveAt(0);
        }

        // Update visible route
        UpdateRouteLine();

        
        if (route.Count > 0)
        {
            targetLocation = route[0];

            Debug.Log(
                "Travelling to " +
                targetLocation.name
            );
        }
        else
        {
            targetLocation = null;
            moving = false;

            Debug.Log("Route complete.");

            UpdateRouteLine();
        }
    }

    private void UpdateResourceDepotPaper()
    {
        if (resourceDepotPaper == null) {
            Debug.Log("test" + resourceDepotPaper); return; }

        if (currentLocation != null &&
            currentLocation.locationType != null &&
            currentLocation.locationType.LocationType == "ResourceDepot")
        {
            resourceDepotPaper.SetActive(true);
            Debug.Log("true");
        }
        else
        {
            resourceDepotPaper.SetActive(false);
            Debug.Log("false");
        }
    }

    public void AddToRoute(Location destination)
    {
        if (destination == null)
            return;

        if (destination == currentLocation)
        {
            Debug.Log("Already at " + destination.name);
            return;
        }

        if (route.Contains(destination))
        {
            Debug.Log(
                destination.name +
                " is already in the route."
            );

            return;
        }


        Location previousLocation;

        if (route.Count > 0)
        {
            previousLocation = route[route.Count - 1];
        }
        else
        {
            previousLocation = currentLocation;
        }


        // Make sure the location is connected
        if (!previousLocation.connections.Contains(destination))
        {
            Debug.Log(
                destination.name +
                " is not connected to " +
                previousLocation.name
            );

            return;
        }


        // ========================================
        // CHECK FUEL
        // ========================================

        if (!HasEnoughFuel())
        {
            return;
        }


        // Add destination
        route.Add(destination);

        Debug.Log(
            "Added " +
            destination.name +
            " to route."
        );


        UpdateRouteLine();


        // Start moving automatically
        if (!moving)
        {
            targetLocation = route[0];
            moving = true;

            Debug.Log(
                "Ship travelling to " +
                targetLocation.name
            );
        }
    }

    private float GetCurrentSpeed()
    {
        if (shipCargo == null)
            return speed;

        float weight =
            shipCargo.GetCurrentCargoWeight();

        float weightPercentage =
            weight / maximumCargoWeight;

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

    private bool HasEnoughFuel()
    {
        if (shipCargo == null)
            return false;

        int currentFuel = shipCargo.GetResourceAmount("fuel");

        if (currentFuel < fuelPerLocation)
        {
            Debug.Log(
                "Not enough fuel! " +
                "Required: " + fuelPerLocation +
                " | Have: " + currentFuel
            );

            return false;
        }

        return true;
    }

    public void ClearRoute()
    {
        route.Clear();

        if (!moving)
        {
            targetLocation = null;
        }

        UpdateRouteLine();

        Debug.Log("Route cleared.");
    }


    private void UpdateRouteLine()
    {
        if (routeLine == null)
            return;

        if (currentLocation == null)
            return;


        routeLine.positionCount = route.Count + 1;


        // Start at current ship location
        routeLine.SetPosition(
            0,
            currentLocation.transform.position
        );


        // Add each destination
        for (int i = 0; i < route.Count; i++)
        {
            routeLine.SetPosition(
                i + 1,
                route[i].transform.position
            );
        }
    }


    public void HandleRouteLocation(Location location)
    {
        if (location == null)
            return;


        // ========================================
        // REMOVE LAST LOCATION
        // ========================================

        if (route.Count > 0 &&
            route[route.Count - 1] == location)
        {
            route.RemoveAt(route.Count - 1);

            Debug.Log(
                "Removed " +
                location.name +
                " from route."
            );

            UpdateRouteLine();

            return;
        }


        // ========================================
        // ADD LOCATION
        // ========================================

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


        // Make sure it is connected
        if (!previousLocation.connections.Contains(location))
        {
            Debug.Log(
                location.name +
                " is not connected to " +
                previousLocation.name
            );

            return;
        }


        // Don't add duplicate locations
        if (route.Contains(location))
        {
            return;
        }


        // Add to route
        route.Add(location);

        Debug.Log(
            "Added " +
            location.name +
            " to route."
        );


        // Update red line
        UpdateRouteLine();


        // Start moving
        if (!moving)
        {
            targetLocation = route[0];
            moving = true;

            Debug.Log(
                "Ship travelling to " +
                targetLocation.name
            );
        }
    }
}