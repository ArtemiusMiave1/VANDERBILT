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
    public List<RouteConnection> routes =
        new List<RouteConnection>();

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

        // Clear old routes
        routes.Clear();

        foreach (Location location in locations)
        {
            if (location != null)
                location.connections.Clear();
        }

        // Create connections
        for (int i = 0; i < locations.Count; i++)
        {
            Location locationA = locations[i];

            for (int j = i + 1; j < locations.Count; j++)
            {
                Location locationB = locations[j];

                float distance = Vector3.Distance(
                    locationA.transform.position,
                    locationB.transform.position
                );

                if (distance <= connectionDistance)
                {
                    RouteConnection route =
                        new RouteConnection();

                    route.locationA = locationA;
                    route.locationB = locationB;

                    route.distance = distance;

                    route.dangerLevel = 0;

                    route.fuelCost =
                        Mathf.Max(
                            1f,
                            Mathf.Ceil(distance / 10f)
                        );

                    route.blocked = false;

                    routes.Add(route);

                    // Add route to both locations
                    locationA.connections.Add(route);
                    locationB.connections.Add(route);

                    // Create visual line
                    CreateRouteLine(route);
                }
            }
        }

        Debug.Log(
            "Created " +
            routes.Count +
            " routes."
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

        // Put all route lines under a parent
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
            route.locationB
        );
    }
}