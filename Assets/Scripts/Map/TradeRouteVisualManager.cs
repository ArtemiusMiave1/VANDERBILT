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

    [Tooltip("Optional parent. Place this underneath the moving map.")]
    public Transform routeLineParent;

    [Header("Visual Settings")]
    [Tooltip("Also used as the route parent when Route Line Parent is empty.")]
    public Transform mapObject;

    [Tooltip("Height above the map.")]
    public float heightOffset = 0.01f;

    [Header("Generation")]
    public bool generateOnStart = true;

    private readonly List<GameObject> spawnedRouteLines =
        new List<GameObject>();

    private void Start()
    {
        if (locationManager == null)
        {
            locationManager = FindObjectOfType<LocationManager>();
        }

        if (generateOnStart)
        {
            GenerateVisuals();
        }
    }

    public void GenerateVisuals()
    {
        ClearVisuals();

        if (!ValidateSetup())
        {
            return;
        }

        HashSet<RouteConnection> processedConnections =
            new HashSet<RouteConnection>();

        foreach (Location location in locationManager.locations)
        {
            if (location == null || location.connections == null)
            {
                continue;
            }

            foreach (RouteConnection connection in location.connections)
            {
                if (connection == null)
                {
                    continue;
                }

                // Avoid generating the same connection twice.
                if (!processedConnections.Add(connection))
                {
                    continue;
                }

                SpawnRouteLine(connection);
            }
        }

        Debug.Log(
            "TradeRouteVisualManager: Generated " +
            spawnedRouteLines.Count +
            " route visuals."
        );
    }

    private void SpawnRouteLine(RouteConnection connection)
    {
        if (connection == null)
        {
            return;
        }

        if (connection.locationA == null ||
            connection.locationB == null)
        {
            Debug.LogWarning(
                "TradeRouteVisualManager: " +
                "Route connection has a missing location."
            );

            return;
        }

        GameObject prefab = GetRoutePrefab(connection.dangerLevel);

        if (prefab == null)
        {
            Debug.LogWarning(
                "TradeRouteVisualManager: " +
                "No prefab assigned for danger level " +
                connection.dangerLevel
            );

            return;
        }

        Transform parent = routeLineParent != null
            ? routeLineParent
            : (mapObject != null ? mapObject : transform);

        // Instantiate relative to the parent.
        GameObject routeObject = Instantiate(prefab, parent, false);

        routeObject.transform.localPosition = Vector3.zero;
        routeObject.transform.localRotation = Quaternion.identity;
        routeObject.transform.localScale = Vector3.one;

        RouteLine routeLine = routeObject.GetComponent<RouteLine>();

        if (routeLine == null)
        {
            Debug.LogError(
                "TradeRouteVisualManager: " +
                prefab.name +
                " does not have a RouteLine component!"
            );

            Destroy(routeObject);
            return;
        }

        // Set local space before Setup calculates the positions.
        SetLocalSpace(routeObject);

        routeLine.mapObject = mapObject;
        routeLine.heightOffset = heightOffset;

        routeLine.Setup(
            connection.locationA,
            connection.locationB
        );

        routeObject.name =
            "Route_" +
            connection.locationA.GetDisplayName() +
            "_" +
            connection.locationB.GetDisplayName();

        spawnedRouteLines.Add(routeObject);
    }

    private void SetLocalSpace(GameObject routeObject)
    {
        LineRenderer[] lineRenderers =
            routeObject.GetComponentsInChildren<LineRenderer>(true);

        foreach (LineRenderer lineRenderer in lineRenderers)
        {
            lineRenderer.useWorldSpace = false;
        }
    }

    private GameObject GetRoutePrefab(int dangerLevel)
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
                    "TradeRouteVisualManager: Unknown danger level: " +
                    dangerLevel +
                    ". Using normal route."
                );

                return normalRoutePrefab;
        }
    }

    public void ClearVisuals()
    {
        foreach (GameObject routeLine in spawnedRouteLines)
        {
            if (routeLine != null)
            {
                // Hide immediately while Destroy is pending.
                routeLine.SetActive(false);
                Destroy(routeLine);
            }
        }

        spawnedRouteLines.Clear();
    }

    private bool ValidateSetup()
    {
        bool valid = true;

        if (locationManager == null)
        {
            Debug.LogError(
                "TradeRouteVisualManager: LocationManager not found!"
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

    public int GetRouteVisualCount()
    {
        return spawnedRouteLines.Count;
    }
}