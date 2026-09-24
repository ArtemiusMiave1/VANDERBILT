using System.Collections.Generic;
using UnityEngine;

public class TradeRouteVisualManager : MonoBehaviour
{
    [Header("Route Line Prefabs")]
    [Tooltip("Used for danger level 0.")]
    public GameObject normalRoutePrefab;

    [Tooltip("Used for danger level 1.")]
    public GameObject dangerousRoutePrefab;

    [Tooltip("Used for danger level 2.")]
    public GameObject veryDangerousRoutePrefab;


    [Header("References")]
    public LocationManager locationManager;

    [Tooltip("Optional parent for all spawned route lines.")]
    public Transform routeLineParent;


    [Header("Visual Settings")]
    [Tooltip("Map object used to determine the height of the route lines.")]
    public Transform mapObject;

    [Tooltip("Height above the map.")]
    public float heightOffset = 0.01f;


    [Header("Generation")]
    [Tooltip("Automatically generate route visuals on Start.")]
    public bool generateOnStart = true;


    private List<GameObject> spawnedRouteLines =
        new List<GameObject>();


    // =========================================================
    // START
    // =========================================================

    private void Start()
    {
        if (locationManager == null)
        {
            locationManager =
                FindObjectOfType<LocationManager>();
        }

        if (generateOnStart)
        {
            GenerateVisuals();
        }
    }


    // =========================================================
    // GENERATE VISUALS
    // =========================================================

    public void GenerateVisuals()
    {
        ClearVisuals();

        if (!ValidateSetup())
        {
            return;
        }

        HashSet<RouteConnection> processedConnections =
            new HashSet<RouteConnection>();


        foreach (
            Location location
            in locationManager.locations
        )
        {
            if (location == null)
                continue;

            if (location.connections == null)
                continue;


            foreach (
                RouteConnection connection
                in location.connections
            )
            {
                if (connection == null)
                    continue;


                // Each connection exists in both locations'
                // connection lists, so only create the visual once.

                if (
                    processedConnections.Contains(
                        connection
                    )
                )
                {
                    continue;
                }


                processedConnections.Add(
                    connection
                );


                SpawnRouteLine(
                    connection
                );
            }
        }


        Debug.Log(
            "TradeRouteVisualManager: Generated " +
            spawnedRouteLines.Count +
            " route visuals."
        );
    }


    // =========================================================
    // SPAWN ROUTE LINE
    // =========================================================

    private void SpawnRouteLine(
        RouteConnection connection
    )
    {
        if (connection == null)
            return;


        if (
            connection.locationA == null ||
            connection.locationB == null
        )
        {
            Debug.LogWarning(
                "TradeRouteVisualManager: " +
                "Route connection has a missing location."
            );

            return;
        }


        GameObject prefab =
            GetRoutePrefab(
                connection.dangerLevel
            );


        if (prefab == null)
        {
            Debug.LogWarning(
                "TradeRouteVisualManager: " +
                "No prefab assigned for danger level " +
                connection.dangerLevel
            );

            return;
        }


        GameObject routeObject =
            Instantiate(
                prefab,
                Vector3.zero,
                Quaternion.identity,
                routeLineParent
            );


        RouteLine routeLine =
            routeObject.GetComponent<RouteLine>();


        if (routeLine == null)
        {
            Debug.LogError(
                "TradeRouteVisualManager: " +
                prefab.name +
                " does not have a RouteLine component!"
            );

            Destroy(
                routeObject
            );

            return;
        }


        routeLine.mapObject =
            mapObject;


        routeLine.heightOffset =
            heightOffset;


        routeLine.Setup(
            connection.locationA,
            connection.locationB
        );


        routeObject.name =
            "Route_" +
            connection.locationA.GetDisplayName() +
            "_" +
            connection.locationB.GetDisplayName();


        spawnedRouteLines.Add(
            routeObject
        );
    }


    // =========================================================
    // GET ROUTE PREFAB
    // =========================================================

    private GameObject GetRoutePrefab(
        int dangerLevel
    )
    {
        switch (dangerLevel)
        {
            case 0:
                return normalRoutePrefab;


            case 1:
                return dangerousRoutePrefab;


            case 2:
                return veryDangerousRoutePrefab;


            default:

                Debug.LogWarning(
                    "TradeRouteVisualManager: " +
                    "Unknown danger level: " +
                    dangerLevel +
                    ". Using normal route."
                );

                return normalRoutePrefab;
        }
    }


    // =========================================================
    // CLEAR VISUALS
    // =========================================================

    public void ClearVisuals()
    {
        foreach (
            GameObject routeLine
            in spawnedRouteLines
        )
        {
            if (routeLine != null)
            {
                Destroy(
                    routeLine
                );
            }
        }


        spawnedRouteLines.Clear();
    }


    // =========================================================
    // VALIDATE SETUP
    // =========================================================

    private bool ValidateSetup()
    {
        bool valid = true;


        if (locationManager == null)
        {
            Debug.LogError(
                "TradeRouteVisualManager: " +
                "LocationManager not found!"
            );

            valid = false;
        }


        if (normalRoutePrefab == null)
        {
            Debug.LogError(
                "TradeRouteVisualManager: " +
                "Normal Route Prefab has not been assigned!"
            );

            valid = false;
        }


        if (dangerousRoutePrefab == null)
        {
            Debug.LogError(
                "TradeRouteVisualManager: " +
                "Dangerous Route Prefab has not been assigned!"
            );

            valid = false;
        }


        if (veryDangerousRoutePrefab == null)
        {
            Debug.LogError(
                "TradeRouteVisualManager: " +
                "Very Dangerous Route Prefab has not been assigned!"
            );

            valid = false;
        }


        return valid;
    }


    // =========================================================
    // PUBLIC INFORMATION
    // =========================================================

    public int GetRouteVisualCount()
    {
        return spawnedRouteLines.Count;
    }
}