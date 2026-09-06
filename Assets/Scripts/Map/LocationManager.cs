using System.Collections.Generic;
using UnityEngine;

public class LocationManager : MonoBehaviour
{
    public static LocationManager Instance;

    [Header("Connection Settings")]
    public float connectionDistance = 10f;

    [Header("Locations")]
    public List<Location> locations = new List<Location>();

    [Header("Routes")]
    public List<RouteConnection> routes = new List<RouteConnection>();

    [Header("Route Line")]
    public GameObject routeLinePrefab;
    public Transform routeLineParent;


    private void Awake()
    {
        Instance = this;
    }


    private void Start()
    {
        CreateConnections();
    }


    public void CreateConnections()
    {
        Location[] foundLocations =
            FindObjectsOfType<Location>();

        locations.Clear();

        foreach (Location location in foundLocations)
        {
            if (location != null)
                locations.Add(location);
        }


        routes.Clear();


        // Clear old connections
        foreach (Location location in locations)
        {
            if (location != null)
                location.connections.Clear();
        }


        // Create connections
        for (int i = 0; i < locations.Count; i++)
        {
            Location locationA =
                locations[i];


            for (int j = i + 1; j < locations.Count; j++)
            {
                Location locationB =
                    locations[j];


                float distance =
                    Vector3.Distance(
                        locationA.transform.position,
                        locationB.transform.position
                    );


                // Only connect nearby locations
                if (distance <= connectionDistance)
                {
                    RouteConnection route =
                        new RouteConnection();


                    // =========================================
                    // ROUTE INFORMATION
                    // =========================================

                    route.locationA = locationA;
                    route.locationB = locationB;

                    route.distance = distance;


                    // =========================================
                    // RANDOM DANGER
                    // =========================================

                    float dangerRoll =
                        Random.value;


                    if (dangerRoll < 0.05f)
                    {
                        // 5% chance
                        // VERY DANGEROUS

                        route.dangerLevel = 2;
                    }
                    else if (dangerRoll < 0.20f)
                    {
                        // 15% chance
                        // DANGEROUS

                        route.dangerLevel = 1;
                    }
                    else
                    {
                        // 80% chance
                        // SAFE

                        route.dangerLevel = 0;
                    }


                    // =========================================
                    // FUEL COST
                    // =========================================

                    route.fuelCost =
                        Mathf.Max(
                            1f,
                            Mathf.Ceil(
                                distance / 10f
                            )
                        );


                    // =========================================
                    // BLOCKED
                    // =========================================

                    route.blocked = false;


                    // =========================================
                    // ADD ROUTE
                    // =========================================

                    routes.Add(route);

                    locationA.connections.Add(route);
                    locationB.connections.Add(route);


                    // =========================================
                    // CREATE VISUAL LINE
                    // =========================================

                    CreateRouteLine(route);


                    // DEBUG
                    Debug.Log(
                        locationA.name +
                        " -> " +
                        locationB.name +
                        " | Danger Level: " +
                        route.dangerLevel
                    );
                }
            }
        }


        Debug.Log(
            "Created " +
            routes.Count +
            " routes between " +
            locations.Count +
            " locations."
        );
    }


    private void CreateRouteLine(
        RouteConnection route)
    {
        if (routeLinePrefab == null)
        {
            Debug.LogWarning(
                "No Route Line Prefab assigned!"
            );

            return;
        }


        GameObject lineObject =
            Instantiate(
                routeLinePrefab,
                Vector3.zero,
                Quaternion.identity
            );


        if (routeLineParent != null)
        {
            lineObject.transform.SetParent(
                routeLineParent
            );
        }


        RouteLine routeLine =
            lineObject.GetComponent<RouteLine>();


        if (routeLine == null)
        {
            Debug.LogError(
                "Route Line Prefab is missing " +
                "the RouteLine script!"
            );

            return;
        }


        routeLine.Setup(
            route.locationA,
            route.locationB,
            route
        );
    }
}