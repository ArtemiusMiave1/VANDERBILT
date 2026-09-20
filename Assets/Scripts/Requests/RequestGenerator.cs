using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RequestGenerator : MonoBehaviour
{
    [Header("Request Paper")]
    public GameObject requestPaperPrefab;

    [Header("Spawn Settings")]
    public Transform spawnLocation;

    [Tooltip("How many requests to try spawning in one group.")]
    public int requestsToSpawn = 3;

    [Tooltip("Time between each individual request spawning.")]
    public float spawnInterval = 5f;

    [Header("Request Line")]
    public RequestLineManager requestLineManager;

    [Header("Automatic Spawning")]
    public bool automaticSpawning = true;

    [Tooltip("Time between automatic request groups.")]
    public float automaticSpawnInterval = 45f;

    [Header("Manual Spawn")]
    public bool spawn = false;

    private bool currentlySpawning = false;


    private void Start()
    {
        if (requestLineManager == null)
        {
            requestLineManager =
                FindObjectOfType<RequestLineManager>();
        }

        // Spawn the first group when the game starts.
        StartCoroutine(SpawnRequestGroup());

        // Start automatic spawning.
        if (automaticSpawning)
        {
            InvokeRepeating(
                nameof(StartAutomaticSpawn),
                automaticSpawnInterval,
                automaticSpawnInterval
            );
        }
    }


    private void Update()
    {
        // Manual spawning from the Inspector.
        if (spawn)
        {
            spawn = false;

            if (!currentlySpawning)
            {
                StartCoroutine(SpawnRequestGroup());
            }
        }
    }


    private void OnDestroy()
    {
        CancelInvoke(
            nameof(StartAutomaticSpawn)
        );
    }


    private void StartAutomaticSpawn()
    {
        if (!currentlySpawning)
        {
            StartCoroutine(SpawnRequestGroup());
        }
    }


    private IEnumerator SpawnRequestGroup()
    {
        if (currentlySpawning)
            yield break;

        currentlySpawning = true;

        for (int i = 0; i < requestsToSpawn; i++)
        {
            if (requestLineManager == null)
            {
                Debug.LogError(
                    "RequestGenerator: RequestLineManager not found!"
                );

                break;
            }

            // Stop spawning if the top request line is full.
            if (!requestLineManager.HasSpace())
            {
                Debug.Log(
                    "Request line is full. Stopping request generation."
                );

                break;
            }

            Vector3 spawnPoint =
                spawnLocation.position;

            SpawnAvailableRequest(
                spawnPoint
            );

            // Wait before spawning the next request.
            if (i < requestsToSpawn - 1)
            {
                yield return new WaitForSeconds(
                    spawnInterval
                );
            }
        }

        currentlySpawning = false;
    }


    public void GenerateRequests()
    {
        if (!currentlySpawning)
        {
            StartCoroutine(
                SpawnRequestGroup()
            );
        }
    }


    private GameObject SpawnAvailableRequest(
        Vector3 spawnPoint
    )
    {
        // -----------------------------
        // Check GameDatabase
        // -----------------------------

        if (GameDatabase.Instance == null)
        {
            Debug.LogError(
                "RequestGenerator: GameDatabase not found!"
            );

            return null;
        }

        if (GameDatabase.Instance.Requests.Count == 0)
        {
            Debug.LogWarning(
                "RequestGenerator: No requests loaded!"
            );

            return null;
        }


        // -----------------------------
        // Check LocationManager
        // -----------------------------

        if (LocationManager.Instance == null)
        {
            Debug.LogError(
                "RequestGenerator: LocationManager not found!"
            );

            return null;
        }

        if (LocationManager.Instance.locations.Count == 0)
        {
            Debug.LogWarning(
                "RequestGenerator: No locations found!"
            );

            return null;
        }


        // -----------------------------
        // Check Spawn Location
        // -----------------------------

        if (spawnLocation == null)
        {
            Debug.LogError(
                "RequestGenerator: Spawn Location has not been assigned!"
            );

            return null;
        }


        // -----------------------------
        // Check Request Prefab
        // -----------------------------

        if (requestPaperPrefab == null)
        {
            Debug.LogError(
                "RequestGenerator: Request Paper Prefab has not been assigned!"
            );

            return null;
        }


        // -----------------------------
        // Check Request Line
        // -----------------------------

        if (requestLineManager == null)
        {
            Debug.LogError(
                "RequestGenerator: Request Line Manager has not been assigned!"
            );

            return null;
        }


        // -----------------------------
        // Find Valid Requests
        // -----------------------------

        List<RequestData> validRequests =
            new List<RequestData>();

        foreach (
            RequestData request
            in GameDatabase.Instance.Requests
        )
        {
            if (request == null)
                continue;

            List<Location> matchingLocations =
                FindLocationsForRequest(
                    request
                );

            if (matchingLocations.Count > 0)
            {
                validRequests.Add(
                    request
                );
            }
        }


        if (validRequests.Count == 0)
        {
            Debug.LogWarning(
                "RequestGenerator: No requests have a matching LocationType!"
            );

            return null;
        }


        // -----------------------------
        // Select Random Request
        // -----------------------------

        RequestData selectedRequest =
            validRequests[
                Random.Range(
                    0,
                    validRequests.Count
                )
            ];


        // -----------------------------
        // Find Target Location
        // -----------------------------

        List<Location> matchingLocationsForRequest =
            FindLocationsForRequest(
                selectedRequest
            );

        if (matchingLocationsForRequest.Count == 0)
        {
            Debug.LogWarning(
                "Could not find location for request: " +
                selectedRequest.Title
            );

            return null;
        }


        Location targetLocation =
            matchingLocationsForRequest[
                Random.Range(
                    0,
                    matchingLocationsForRequest.Count
                )
            ];


        // -----------------------------
        // Create Request Paper
        // -----------------------------

        GameObject paper =
            Instantiate(
                requestPaperPrefab,
                spawnPoint,
                Quaternion.identity,
                spawnLocation
            );


        RequestPaper requestPaper =
            paper.GetComponent<RequestPaper>();

        if (requestPaper == null)
        {
            Debug.LogError(
                "Request Paper prefab does not have a RequestPaper component!"
            );

            Destroy(paper);

            return null;
        }


        // -----------------------------
        // Display Request Information
        // -----------------------------

        requestPaper.DisplayRequest(
            selectedRequest
        );

        requestPaper.AssignLocation(
            targetLocation
        );


        // -----------------------------
        // Set Estimated Arrival
        // -----------------------------

        requestPaper.SetEstimatedArrival();


        // -----------------------------
        // Add Request To Location
        // -----------------------------

        if (
            !targetLocation.activeRequests.Contains(
                requestPaper
            )
        )
        {
            targetLocation.activeRequests.Add(
                requestPaper
            );
        }


        // -----------------------------
        // Add Request To Top Line
        // -----------------------------

        bool added =
            requestLineManager.AddNewRequest(
                paper
            );

        if (!added)
        {
            Destroy(paper);

            return null;
        }


        // -----------------------------
        // Debug
        // -----------------------------

        string locationName =
            targetLocation.locationType != null
                ? targetLocation.locationType.Name
                : "Unknown";

        Debug.Log(
            "Generated Request: " +
            selectedRequest.Title +
            " → " +
            targetLocation.name +
            " (" +
            locationName +
            ")"
        );

        return paper;
    }


    private List<Location> FindLocationsForRequest(
        RequestData request
    )
    {
        List<Location> matchingLocations =
            new List<Location>();

        if (request == null)
            return matchingLocations;

        if (LocationManager.Instance == null)
            return matchingLocations;


        foreach (
            Location location
            in LocationManager.Instance.locations
        )
        {
            if (location == null)
                continue;

            if (location.locationType == null)
                continue;


            if (
                location.locationType.LocationType ==
                request.LocationType
            )
            {
                matchingLocations.Add(
                    location
                );
            }
        }

        return matchingLocations;
    }
}