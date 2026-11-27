using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(-100)]
public class LocationManager : MonoBehaviour
{
    public static LocationManager Instance;
    
    [Header("Custom Route Dangers")]
    public bool useCustomRouteDangers = true;

    public bool randomiseUnassignedRoutes = false;

    public List<CustomRouteDanger> customRouteDangers =
        new List<CustomRouteDanger>();

    [Header("Locations")]
    public List<Location> locations =
        new List<Location>();

    [Header("Connections")]
    [Tooltip("Maximum distance between locations for a connection.")]
    public float connectionDistance = 20f;

    [Tooltip("Automatically generate connections when requested.")]
    public bool generateConnectionsOnStart = false;

    [Header("Route Danger")]
    [Range(0f, 1f)]
    [Tooltip("Chance of a route being dangerous.")]
    public float dangerousRouteChance = 0.15f;

    [Range(0f, 1f)]
    [Tooltip("Chance of a route being very dangerous.")]
    public float veryDangerousRouteChance = 0.05f;

    [Header("Custom Map")]
    public bool useCustomMap = false;

    public Transform customLocationsParent;

    // --------------------------------------------------
    // AWAKE
    // --------------------------------------------------

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }


    // --------------------------------------------------
    // START
    // --------------------------------------------------

    private void Start()
    {
        if (useCustomMap)
        {
            LoadCustomLocations();
        }
        else if (generateConnectionsOnStart)
        {
            GenerateConnections();
        }
    }


    // --------------------------------------------------
    // GENERATE CONNECTIONS
    // --------------------------------------------------

    public void GenerateConnections()
    {
        ClearConnections();

        if (locations == null || locations.Count == 0)
        {
            Debug.LogWarning(
                "LocationManager: No locations found."
            );

            return;
        }

        int connectionCount = 0;

        for (int i = 0; i < locations.Count; i++)
        {
            Location locationA =
                locations[i];

            if (locationA == null)
                continue;


            for (int j = i + 1; j < locations.Count; j++)
            {
                Location locationB =
                    locations[j];

                if (locationB == null)
                    continue;


                // ------------------------------------------
                // CALCULATE X/Z DISTANCE
                // ------------------------------------------

                Vector2 positionA =
                    new Vector2(
                        locationA.transform.position.x,
                        locationA.transform.position.z
                    );

                Vector2 positionB =
                    new Vector2(
                        locationB.transform.position.x,
                        locationB.transform.position.z
                    );

                float distance =
                    Vector2.Distance(
                        positionA,
                        positionB
                    );


                // ------------------------------------------
                // CHECK CONNECTION DISTANCE
                // ------------------------------------------

                if (distance > connectionDistance)
                    continue;


                // ------------------------------------------
                // CREATE ROUTE
                // ------------------------------------------

                RouteConnection route =
                    new RouteConnection();

                route.locationA =
                    locationA;

                route.locationB =
                    locationB;

                route.distance =
                    distance;

                route.dangerLevel = GetRouteDangerLevel(
                    locationA,
                    locationB
                    );

                route.fuelCost =
                    CalculateFuelCost(
                        distance
                    );

                route.blocked =
                    false;


                // ------------------------------------------
                // ADD TO BOTH LOCATIONS
                // ------------------------------------------

                locationA.connections.Add(
                    route
                );

                locationB.connections.Add(
                    route
                );

                connectionCount++;
            }
        }


        Debug.Log(
            "LocationManager: Generated " +
            connectionCount +
            " routes between " +
            locations.Count +
            " locations."
        );
    }

    private int GetRouteDangerLevel(
    Location locationA,
    Location locationB)
    {
        if (useCustomMap && useCustomRouteDangers)
        {
            foreach (CustomRouteDanger customRoute
                in customRouteDangers)
            {
                if (customRoute == null)
                    continue;

                if (customRoute.Matches(locationA, locationB))
                {
                    return (int)customRoute.dangerLevel;
                }
            }

            // Unassigned custom routes default to normal.
            if (!randomiseUnassignedRoutes)
            {
                return 0;
            }
        }

        // Preserve the existing random danger system.
        return GenerateDangerLevel();
    }

    // --------------------------------------------------
    // GENERATE DANGER LEVEL
    // --------------------------------------------------
    public void LoadCustomLocations()
    {
        if (customLocationsParent == null)
        {
            Debug.LogWarning(
                "LocationManager: Custom locations parent not assigned."
            );

            return;
        }

        locations.Clear();

        Location[] customLocations =
            customLocationsParent.GetComponentsInChildren<Location>();

        foreach (Location location in customLocations)
        {
            if (location == null)
                continue;

            // Initialise the location's data and visuals.
            if (location.locationType != null)
            {
                location.SetLocationType(location.locationType);
            }

            AddLocation(location);
        }

        Debug.Log(
            "LocationManager: Loaded " +
            locations.Count +
            " custom locations."
        );

        GenerateConnections();
    }
    private int GenerateDangerLevel()
    {
        float roll =
            Random.value;


        // Very dangerous.
        if (
            roll <
            veryDangerousRouteChance
        )
        {
            return 2;
        }


        // Dangerous.
        if (
            roll <
            veryDangerousRouteChance +
            dangerousRouteChance
        )
        {
            return 1;
        }


        // Safe.
        return 0;
    }


    // --------------------------------------------------
    // CALCULATE FUEL COST
    // --------------------------------------------------

    private float CalculateFuelCost(
        float distance)
    {
        return distance;
    }


    // --------------------------------------------------
    // CLEAR CONNECTIONS
    // --------------------------------------------------

    public void ClearConnections()
    {
        foreach (
            Location location
            in locations
        )
        {
            if (location == null)
                continue;

            if (location.connections == null)
            {
                location.connections =
                    new List<RouteConnection>();

                continue;
            }

            location.connections.Clear();
        }
    }


    // --------------------------------------------------
    // ADD LOCATION
    // --------------------------------------------------

    public void AddLocation(
        Location location)
    {
        if (location == null)
            return;

        if (locations.Contains(location))
            return;

        locations.Add(
            location
        );
    }


    // --------------------------------------------------
    // REMOVE LOCATION
    // --------------------------------------------------

    public void RemoveLocation(
        Location location)
    {
        if (location == null)
            return;

        locations.Remove(
            location
        );
    }


    // --------------------------------------------------
    // GET LOCATION BY ID
    // --------------------------------------------------

    public Location GetLocationByID(
        string locationID)
    {
        if (string.IsNullOrEmpty(locationID))
            return null;


        foreach (
            Location location
            in locations
        )
        {
            if (location == null)
                continue;

            if (
                location.locationID ==
                locationID
            )
            {
                return location;
            }
        }


        return null;
    }


    // --------------------------------------------------
    // GET LOCATIONS BY DISTRICT
    // --------------------------------------------------

    public List<Location> GetLocationsByDistrict(
        string districtType)
    {
        List<Location> results =
            new List<Location>();


        if (string.IsNullOrEmpty(districtType))
            return results;


        foreach (
            Location location
            in locations
        )
        {
            if (location == null)
                continue;

            if (location.locationType == null)
                continue;


            if (
                location.locationType.District &&
                location.locationType.DistrictType ==
                districtType
            )
            {
                results.Add(
                    location
                );
            }
        }


        return results;
    }


    // --------------------------------------------------
    // GET LOCATIONS BY TYPE
    // --------------------------------------------------

    public List<Location> GetLocationsByType(
        string locationType)
    {
        List<Location> results =
            new List<Location>();


        if (string.IsNullOrEmpty(locationType))
            return results;


        foreach (
            Location location
            in locations
        )
        {
            if (location == null)
                continue;

            if (location.locationType == null)
                continue;


            if (
                location.locationType.LocationType ==
                locationType
            )
            {
                results.Add(
                    location
                );
            }
        }


        return results;
    }


    // --------------------------------------------------
    // FIND ROUTE CONNECTION
    // --------------------------------------------------

    public RouteConnection GetConnection(
        Location locationA,
        Location locationB)
    {
        if (
            locationA == null ||
            locationB == null
        )
        {
            return null;
        }


        foreach (
            RouteConnection connection
            in locationA.connections
        )
        {
            if (connection == null)
                continue;


            if (
                (
                    connection.locationA ==
                    locationA &&
                    connection.locationB ==
                    locationB
                )
                ||
                (
                    connection.locationA ==
                    locationB &&
                    connection.locationB ==
                    locationA
                )
            )
            {
                return connection;
            }
        }


        return null;
    }


    // --------------------------------------------------
    // CHECK IF CONNECTED
    // --------------------------------------------------

    public bool AreLocationsConnected(
        Location locationA,
        Location locationB)
    {
        return GetConnection(
            locationA,
            locationB
        ) != null;
    }
}