using System.Collections.Generic;
using UnityEngine;

public class RandomLocationSpawner : MonoBehaviour
{
    [Header("Location")]
    public GameObject locationPrefab;

    [Header("Spawn Area")]
    public Transform spawnPlane;

    [Header("Spawn Settings")]
    public int locationAmount = 20;
    public float minimumDistance = 2f;

    [Header("Special Location Spacing")]
    [Tooltip("Minimum distance between locations of the same special type.")]
    public float specialLocationMinimumDistance = 1f;

    [Header("Parent")]
    public Transform locationParent;

    [Header("Location Manager")]
    public LocationManager locationManager;

    [Header("Generation")]
    public bool generateOnStart = false;

    [Header("Random Seed")]
    public bool useRandomSeed = true;
    public int randomSeed = 12345;

    private List<Location> spawnedLocations =
        new List<Location>();


    // --------------------------------------------------
    // START
    // --------------------------------------------------

    private void Start()
    {
        if (generateOnStart)
        {
            SpawnLocations();
        }
    }


    // --------------------------------------------------
    // SPAWN LOCATIONS
    // --------------------------------------------------

    public void SpawnLocations()
    {
        ClearExistingLocations();

        if (!ValidateSetup())
        {
            return;
        }


        // ----------------------------------------------
        // RANDOM SEED
        // ----------------------------------------------

        if (!useRandomSeed)
        {
            Random.InitState(
                randomSeed
            );
        }


        // ----------------------------------------------
        // SPAWN REQUIRED SPECIAL LOCATIONS FIRST
        // ----------------------------------------------

        SpawnSpecialLocations();


        // ----------------------------------------------
        // SPAWN NORMAL DISTRICT LOCATIONS
        // ----------------------------------------------

        int attempts = 0;

        int maximumAttempts =
            locationAmount * 200;


        while (
            spawnedLocations.Count < locationAmount &&
            attempts < maximumAttempts
        )
        {
            attempts++;


            LocationData selectedData =
                GetRandomDistrictLocationData();


            if (selectedData == null)
            {
                Debug.LogWarning(
                    "RandomLocationSpawner: " +
                    "No district LocationData available."
                );

                break;
            }


            if (
                TrySpawnLocation(
                    selectedData
                )
            )
            {
                attempts = 0;
            }
        }


        // ----------------------------------------------
        // GENERATE CONNECTIONS
        // ----------------------------------------------

        if (locationManager != null)
        {
            locationManager.GenerateConnections();
        }


        Debug.Log(
            "Location generation complete. " +
            "Spawned: " +
            spawnedLocations.Count +
            "/" +
            locationAmount
        );
    }


    // --------------------------------------------------
    // SPAWN SPECIAL LOCATIONS
    // --------------------------------------------------

    private void SpawnSpecialLocations()
    {
        if (GameDatabase.Instance == null)
            return;

        if (
            GameDatabase.Instance.LocationData ==
            null
        )
        {
            return;
        }


        foreach (
            LocationData data
            in GameDatabase.Instance.LocationData
        )
        {
            if (data == null)
                continue;


            // District locations are spawned later.
            if (data.District)
                continue;


            // Limit now determines how many
            // of this special location should exist.
            if (data.Limit <= 0)
                continue;


            for (
                int i = 0;
                i < data.Limit;
                i++
            )
            {
                bool spawned =
                    TrySpawnSpecialLocation(
                        data
                    );


                if (!spawned)
                {
                    Debug.LogWarning(
                        "RandomLocationSpawner: " +
                        "Could not spawn required special location: " +
                        data.LocationType +
                        " (" +
                        (i + 1) +
                        "/" +
                        data.Limit +
                        ")"
                    );
                }
            }
        }
    }


    // --------------------------------------------------
    // TRY SPAWN SPECIAL LOCATION
    // --------------------------------------------------

    private bool TrySpawnSpecialLocation(
        LocationData data
    )
    {
        const int maxAttempts = 500;


        for (
            int attempt = 0;
            attempt < maxAttempts;
            attempt++
        )
        {
            Vector3 spawnPosition =
                GetRandomSpawnPosition();


            if (
                !IsPositionValid(
                    spawnPosition,
                    data
                )
            )
            {
                continue;
            }


            return CreateLocation(
                data,
                spawnPosition
            );
        }


        return false;
    }


    // --------------------------------------------------
    // TRY SPAWN NORMAL LOCATION
    // --------------------------------------------------

    private bool TrySpawnLocation(
        LocationData data
    )
    {
        if (data == null)
            return false;


        Vector3 spawnPosition =
            GetRandomSpawnPosition();


        if (
            !IsPositionValid(
                spawnPosition,
                data
            )
        )
        {
            return false;
        }


        return CreateLocation(
            data,
            spawnPosition
        );
    }


    // --------------------------------------------------
    // CREATE LOCATION
    // --------------------------------------------------

    private bool CreateLocation(
        LocationData data,
        Vector3 spawnPosition
    )
    {
        if (data == null)
            return false;


        GameObject locationObject =
            Instantiate(
                locationPrefab,
                spawnPosition,
                Quaternion.identity,
                locationParent
            );


        Location location =
            locationObject.GetComponent<Location>();


        if (location == null)
        {
            Debug.LogError(
                "RandomLocationSpawner: " +
                "Location prefab does not have " +
                "a Location component!"
            );

            Destroy(
                locationObject
            );

            return false;
        }


        // ----------------------------------------------
        // LOCATION DATA
        // ----------------------------------------------

        location.SetLocationType(
            data
        );


        // ----------------------------------------------
        // LOCATION ID
        // ----------------------------------------------

        string generatedID =
            GenerateLocationID(
                data
            );


        location.SetLocationID(
            generatedID
        );


        // ----------------------------------------------
        // ADD TO LIST
        // ----------------------------------------------

        spawnedLocations.Add(
            location
        );


        // ----------------------------------------------
        // LOCATION MANAGER
        // ----------------------------------------------

        if (
            locationManager != null &&
            !locationManager.locations.Contains(
                location
            )
        )
        {
            locationManager.locations.Add(
                location
            );
        }


        Debug.Log(
            "Spawned Location: " +
            generatedID +
            " | " +
            data.LocationType
        );


        return true;
    }


    // --------------------------------------------------
    // GET RANDOM DISTRICT LOCATION
    // --------------------------------------------------

    private LocationData GetRandomDistrictLocationData()
    {
        if (GameDatabase.Instance == null)
            return null;


        if (
            GameDatabase.Instance.LocationData ==
            null
        )
        {
            return null;
        }


        List<LocationData> available =
            new List<LocationData>();


        foreach (
            LocationData data
            in GameDatabase.Instance.LocationData
        )
        {
            if (data == null)
                continue;


            // Only normal district locations
            // are randomly selected here.
            if (!data.District)
                continue;


            available.Add(
                data
            );
        }


        if (available.Count == 0)
            return null;


        return available[
            Random.Range(
                0,
                available.Count
            )
        ];
    }


    // --------------------------------------------------
    // GENERATE LOCATION ID
    // --------------------------------------------------

    private string GenerateLocationID(
        LocationData data
    )
    {
        if (data == null)
            return "Unknown";


        // ----------------------------------------------
        // DISTRICT LOCATION
        // ----------------------------------------------

        if (data.District)
        {
            string district =
                data.DistrictType;


            int number = 1;


            while (
                LocationIDExists(
                    district + number
                )
            )
            {
                number++;
            }


            return district + number;
        }


        // ----------------------------------------------
        // SPECIAL LOCATION
        // ----------------------------------------------

        // Vanderbilt only has one, so it can
        // simply be called Vanderbilt.
        if (
            data.LocationType ==
            "Vanderbilt"
        )
        {
            return "Vanderbilt";
        }


        // Resource Depot can have multiple locations,
        // so number them.
        if (
            data.LocationType ==
            "ResourceDepot"
        )
        {
            int number = 1;


            while (
                LocationIDExists(
                    "ResourceDepot" +
                    number
                )
            )
            {
                number++;
            }


            return
                "ResourceDepot" +
                number;
        }


        // Generic fallback for any future
        // special location type.
        int specialNumber = 1;

        string specialID =
            data.LocationType;


        while (
            LocationIDExists(
                specialID
            )
        )
        {
            specialNumber++;

            specialID =
                data.LocationType +
                specialNumber;
        }


        return specialID;
    }


    // --------------------------------------------------
    // CHECK LOCATION ID
    // --------------------------------------------------

    private bool LocationIDExists(
        string id
    )
    {
        foreach (
            Location location
            in spawnedLocations
        )
        {
            if (location == null)
                continue;


            if (
                location.locationID ==
                id
            )
            {
                return true;
            }
        }


        return false;
    }


    // --------------------------------------------------
    // RANDOM SPAWN POSITION
    // --------------------------------------------------

    private Vector3 GetRandomSpawnPosition()
    {
        Bounds bounds =
            GetSpawnBounds();


        float x =
            Random.Range(
                bounds.min.x,
                bounds.max.x
            );


        float z =
            Random.Range(
                bounds.min.z,
                bounds.max.z
            );


        float y =
            spawnPlane.position.y;


        return new Vector3(
            x,
            y,
            z
        );
    }


    // --------------------------------------------------
    // GET SPAWN BOUNDS
    // --------------------------------------------------

    private Bounds GetSpawnBounds()
    {
        Renderer renderer =
            spawnPlane.GetComponent<Renderer>();


        if (renderer != null)
        {
            return renderer.bounds;
        }


        Collider collider =
            spawnPlane.GetComponent<Collider>();


        if (collider != null)
        {
            return collider.bounds;
        }


        return new Bounds(
            spawnPlane.position,
            Vector3.one * 20f
        );
    }


    // --------------------------------------------------
    // CHECK POSITION
    // --------------------------------------------------

    // --------------------------------------------------
    // CHECK POSITION
    // --------------------------------------------------

    private bool IsPositionValid(
        Vector3 position,
        LocationData selectedData
    )
    {
        foreach (
            Location location
            in spawnedLocations
        )
        {
            if (location == null)
                continue;


            Vector2 newPosition =
                new Vector2(
                    position.x,
                    position.z
                );


            Vector2 existingPosition =
                new Vector2(
                    location.transform.position.x,
                    location.transform.position.z
                );


            float distance =
                Vector2.Distance(
                    newPosition,
                    existingPosition
                );


            // ------------------------------------------
            // DEFAULT DISTANCE
            // ------------------------------------------

            float requiredDistance =
                minimumDistance;


            // ------------------------------------------
            // SAME SPECIAL LOCATION TYPE
            // ------------------------------------------

            if (
                selectedData != null &&
                location.locationType != null
            )
            {
                bool sameLocationType =
                    selectedData.LocationType ==
                    location.locationType.LocationType;


                bool isResourceDepot =
                    selectedData.LocationType ==
                    "ResourceDepot";


                bool isVanderbilt =
                    selectedData.LocationType ==
                    "Vanderbilt";


                // Only apply the larger spacing when
                // the SAME special location type
                // is being placed near itself.
                if (
                    sameLocationType &&
                    (
                        isResourceDepot ||
                        isVanderbilt
                    )
                )
                {
                    requiredDistance =
                        specialLocationMinimumDistance;
                }
            }


            // ------------------------------------------
            // CHECK DISTANCE
            // ------------------------------------------

            if (
                distance <
                requiredDistance
            )
            {
                return false;
            }
        }


        return true;
    }


    // --------------------------------------------------
    // CHECK SPECIAL LOCATION
    // --------------------------------------------------

    private bool IsSpecialLocation(
        LocationData data
    )
    {
        if (data == null)
            return false;


        return
            data.LocationType ==
            "ResourceDepot" ||
            data.LocationType ==
            "Vanderbilt";
    }


    // --------------------------------------------------
    // CLEAR EXISTING LOCATIONS
    // --------------------------------------------------

    private void ClearExistingLocations()
    {
        if (locationManager != null)
        {
            locationManager.locations.Clear();
        }


        foreach (
            Location location
            in spawnedLocations
        )
        {
            if (location != null)
            {
                Destroy(
                    location.gameObject
                );
            }
        }


        spawnedLocations.Clear();
    }


    // --------------------------------------------------
    // VALIDATE SETUP
    // --------------------------------------------------

    private bool ValidateSetup()
    {
        if (locationPrefab == null)
        {
            Debug.LogError(
                "RandomLocationSpawner: " +
                "Location Prefab has not been assigned!"
            );

            return false;
        }


        if (spawnPlane == null)
        {
            Debug.LogError(
                "RandomLocationSpawner: " +
                "Spawn Plane has not been assigned!"
            );

            return false;
        }


        if (locationParent == null)
        {
            Debug.LogWarning(
                "RandomLocationSpawner: " +
                "Location Parent has not been assigned. " +
                "Locations will use this object as parent."
            );

            locationParent =
                transform;
        }


        if (locationManager == null)
        {
            locationManager =
                FindObjectOfType<LocationManager>();
        }


        if (locationManager == null)
        {
            Debug.LogError(
                "RandomLocationSpawner: " +
                "LocationManager not found!"
            );

            return false;
        }


        if (GameDatabase.Instance == null)
        {
            Debug.LogError(
                "RandomLocationSpawner: " +
                "GameDatabase not found!"
            );

            return false;
        }


        if (
            GameDatabase.Instance.LocationData == null ||
            GameDatabase.Instance.LocationData.Count == 0
        )
        {
            Debug.LogError(
                "RandomLocationSpawner: " +
                "GameDatabase has no LocationData!"
            );

            return false;
        }


        if (locationAmount <= 0)
        {
            Debug.LogError(
                "RandomLocationSpawner: " +
                "Location Amount must be greater than 0."
            );

            return false;
        }


        return true;
    }


    // --------------------------------------------------
    // GET SPAWNED LOCATIONS
    // --------------------------------------------------

    public List<Location> GetSpawnedLocations()
    {
        return spawnedLocations;
    }
}