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

        // Use the seed if requested.
        if (!useRandomSeed)
        {
            Random.InitState(randomSeed);
        }

        int attempts = 0;

        int maximumAttempts =
            locationAmount * 100;


        while (
            spawnedLocations.Count < locationAmount &&
            attempts < maximumAttempts
        )
        {
            attempts++;


            // ------------------------------------------
            // GET AVAILABLE LOCATION DATA
            // ------------------------------------------

            LocationData selectedData =
                GetAvailableLocationData();


            if (selectedData == null)
            {
                Debug.LogWarning(
                    "RandomLocationSpawner: " +
                    "No more valid LocationData available."
                );

                break;
            }


            // ------------------------------------------
            // GET RANDOM POSITION
            // ------------------------------------------

            Vector3 spawnPosition =
                GetRandomSpawnPosition();


            if (!IsPositionValid(spawnPosition))
            {
                continue;
            }


            // ------------------------------------------
            // CREATE LOCATION
            // ------------------------------------------

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

                Destroy(locationObject);

                return;
            }


            // ------------------------------------------
            // ASSIGN LOCATION DATA
            // ------------------------------------------

            location.SetLocationType(
                selectedData
            );


            // ------------------------------------------
            // GENERATE LOCATION ID
            // ------------------------------------------

            string generatedID =
                GenerateLocationID(
                    selectedData
                );


            location.SetLocationID(
                generatedID
            );


            // ------------------------------------------
            // ADD TO SPAWNED LIST
            // ------------------------------------------

            spawnedLocations.Add(
                location
            );


            // ------------------------------------------
            // ADD TO LOCATION MANAGER
            // ------------------------------------------

            if (
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
                selectedData.LocationType
            );
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
    // GET AVAILABLE LOCATION DATA
    // --------------------------------------------------

    private LocationData GetAvailableLocationData()
    {
        if (GameDatabase.Instance == null)
        {
            Debug.LogError(
                "RandomLocationSpawner: " +
                "GameDatabase not found!"
            );

            return null;
        }


        if (
            GameDatabase.Instance.LocationData == null ||
            GameDatabase.Instance.LocationData.Count == 0
        )
        {
            Debug.LogWarning(
                "RandomLocationSpawner: " +
                "GameDatabase contains no LocationData!"
            );

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


            // ------------------------------------------
            // DISTRICT LOCATIONS
            // ------------------------------------------

            // District locations can have any number.
            if (data.District)
            {
                available.Add(data);
                continue;
            }


            // ------------------------------------------
            // SPECIAL LOCATIONS
            // ------------------------------------------

            int currentCount =
                CountLocationType(data);


            if (
                data.Limit <= 0 ||
                currentCount < data.Limit
            )
            {
                available.Add(data);
            }
        }


        if (available.Count == 0)
        {
            return null;
        }


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
        // DISTRICT
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

        return data.LocationType;
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
    // COUNT LOCATION TYPE
    // --------------------------------------------------

    private int CountLocationType(
        LocationData data
    )
    {
        int count = 0;


        foreach (
            Location location
            in spawnedLocations
        )
        {
            if (location == null)
                continue;


            if (
                location.locationType ==
                data
            )
            {
                count++;
            }
        }


        return count;
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

    private bool IsPositionValid(
        Vector3 position
    )
    {
        foreach (
            Location location
            in spawnedLocations
        )
        {
            if (location == null)
                continue;


            Vector2 a =
                new Vector2(
                    position.x,
                    position.z
                );


            Vector2 b =
                new Vector2(
                    location.transform.position.x,
                    location.transform.position.z
                );


            float distance =
                Vector2.Distance(
                    a,
                    b
                );


            if (
                distance <
                minimumDistance
            )
            {
                return false;
            }
        }


        return true;
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


        // ----------------------------------------------
        // GAME DATABASE
        // ----------------------------------------------

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