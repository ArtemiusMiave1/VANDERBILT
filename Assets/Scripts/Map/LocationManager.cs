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

    [Header("Location Height")]
    public float locationHeight = 0.5f;

    [Header("Map Height")]
    public Transform mapObject;

    [Header("Route Line Prefabs")]
    public GameObject normalRoutePrefab;
    public GameObject dangerousRoutePrefab;
    public GameObject veryDangerousRoutePrefab;

    [Header("Route Line Parent")]
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
            {
                locations.Add(location);

                // Raise location above the board
                Vector3 position =
                    location.transform.position;

                position.y = locationHeight;

                location.transform.position =
                    position;
            }
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


                if (distance <= connectionDistance)
                {
                    RouteConnection route =
                        new RouteConnection();


                    route.locationA =
                        locationA;

                    route.locationB =
                        locationB;

                    route.distance =
                        distance;


                    // =========================================
                    // RANDOM DANGER
                    // =========================================

                    float dangerRoll =
                        Random.value;


                    if (dangerRoll < 0.05f)
                    {
                        // 5% Very Dangerous
                        route.dangerLevel = 2;
                    }
                    else if (dangerRoll < 0.20f)
                    {
                        // 15% Dangerous
                        route.dangerLevel = 1;
                    }
                    else
                    {
                        // 80% Safe
                        route.dangerLevel = 0;
                    }


                    // =========================================
                    // FUEL
                    // =========================================

                    route.fuelCost =
                        Mathf.Max(
                            1f,
                            Mathf.Ceil(
                                distance / 10f
                            )
                        );


                    route.blocked = false;


                    // =========================================
                    // ADD ROUTE
                    // =========================================

                    routes.Add(route);

                    locationA.connections.Add(route);
                    locationB.connections.Add(route);


                    // =========================================
                    // CREATE VISUAL ROUTE
                    // =========================================

                    CreateRouteLine(route);
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


    // =========================================================
    // CREATE ROUTE LINE
    // =========================================================

    
    private void CreateRouteLine(RouteConnection route)
{
    if (route == null)
        return;

    GameObject selectedPrefab = null;

    switch (route.dangerLevel)
    {
        case 0:
            selectedPrefab = normalRoutePrefab;
            break;

        case 1:
            selectedPrefab = dangerousRoutePrefab;
            break;

        case 2:
            selectedPrefab = veryDangerousRoutePrefab;
            break;
    }

    if (selectedPrefab == null)
    {
        Debug.LogWarning(
            "No route prefab assigned for danger level " +
            route.dangerLevel
        );

        return;
    }

    GameObject lineObject = Instantiate(
        selectedPrefab,
        Vector3.zero,
        Quaternion.identity
    );

    if (routeLineParent != null)
    {
        lineObject.transform.SetParent(
            routeLineParent,
            false
        );
    }

    RouteLine routeLine =
        lineObject.GetComponent<RouteLine>();

    if (routeLine == null)
    {
        Debug.LogError(
            "Route prefab is missing RouteLine!"
        );

        Destroy(lineObject);
        return;
    }

    // Use the map object's height
    if (mapObject != null)
    {
    }

    routeLine.Setup(
        route.locationA,
        route.locationB
    );
}
}